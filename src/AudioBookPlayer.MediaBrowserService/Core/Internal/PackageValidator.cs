#nullable enable

using Android.Content;
using Android.Content.PM;
using Android.Util;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Xml;
using static Android.Content.PM.PackageManager;
using Process = Android.OS.Process;
using String = System.String;

namespace AudioBookPlayer.MediaBrowserService.Core.Internal
{
    internal sealed class PackageValidator
    {
        private const string SigningCertificateElementName = "signing_certificate";
        private const string PrefixLocal = "local:";
        private const string NameAttributeName = "name";
        private const string ReleaseAttributeName = "release";
        private const string PackageAttributeName = "package";

        private readonly Context context;
        private readonly Dictionary<string, List<CallerInfo>> certificates;

        private PackageValidator(Context context, Dictionary<string, List<CallerInfo>> certificates)
        {
            this.context = context;
            this.certificates = certificates;
        }

        public static PackageValidator Create(Context context)
        {
            using var reader = context.Resources?.GetXml( Resource.Xml.allowed_media_browser_callers);
            var certificates = ReadCertificates(reader);
            return new PackageValidator(context, certificates);
        }

        public bool IsCallerAllowed(string callingPackageName, int callingUid)
        {
            if (Process.SystemUid == callingUid || Process.MyUid() == callingUid)
            {
                return true;
            }

            var packageManager = context.PackageManager;

            if (null == packageManager)
            {
                return false;
            }

            PackageInfo? packageInfo;
            try
            {
                packageInfo = packageManager.GetPackageInfo(callingPackageName, PackageInfoFlags.SigningCertificates);

                if (null == packageInfo)
                {
                    return false;
                }
            }
            catch (NameNotFoundException)
            {
                //LogHelper.Warn(Tag, e, "Package manager can't find package: ", callingPackage);
                return false;
            }

            var signatures = packageInfo.SigningInfo?.GetSigningCertificateHistory();

            for (var index = 0; null != signatures && index < signatures.Length; index++)
            {
                var packageSignature = Base64.EncodeToString(signatures[index].ToByteArray(), Base64Flags.NoWrap);
                // var signature = Base64.EncodeToString(packageInfo.SigningInfo.ToByteArray(), Base64Flags.NoWrap);

                if (null == packageSignature || false == certificates.TryGetValue(packageSignature, out var validCallers))
                {
                    return false;
                }

                if (null == validCallers)
                {
                    //LogHelper.Verbose(Tag, "Signature for caller ", callingPackage, " is not valid: \n", signature);
                    if (0 == certificates.Count)
                    {
                        /*LogHelper.Warn(Tag, "The list of valid certificates is empty. Either your file ",
                            "res/xml/allowed_media_browser_callers.xml is empty or there was an error ",
                            "while reading it. Check previous log messages.");*/
                    }

                    return false;
                }

                // Check if the package name is valid for the certificate:
                //var expectedPackages = new StringBuilder();

                foreach (var info in validCallers)
                {
                    if (callingPackageName == info.PackageName)
                    {
                        //LogHelper.Verbose(Tag, "Valid caller: ", info.Name, " package=", info.PackageName, " release=", info.Release);
                        return true;
                    }

                    //expectedPackages.Append(info.PackageName).Append(' ');
                }
            }

            /*LogHelper.Info(Tag, "Caller has a valid certificate, but its package doesn't match any ",
                "expected package for the given certificate. Caller's package is ", callingPackage,
                ". Expected packages as defined in res/xml/allowed_media_browser_callers.xml are (",
                expectedPackages, "). This caller's certificate is: \n", signature);*/

            return false;
        }

        private static Dictionary<string, List<CallerInfo>> ReadCertificates(XmlReader? reader)
        {
            var certificates = new Dictionary<string, List<CallerInfo>>();

            if (null != reader)
            {
                try
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement() && String.Equals(reader.Name, SigningCertificateElementName)  && reader.HasAttributes)
                        {
                            var name = reader[PrefixLocal + NameAttributeName];
                            var packageName = reader[PrefixLocal + PackageAttributeName];
                            var isRelease = Convert.ToBoolean(reader[PrefixLocal + ReleaseAttributeName]);
                            var certificate = reader.ReadString();

                            if (null != certificate)
                            {
                                var certData = Base64.Decode(certificate, Base64Flags.Crlf);
                                certificate = Base64.EncodeToString(certData, Base64Flags.NoWrap);
                            }

                            if (null == certificate || null == packageName || null == name)
                            {
                                continue;
                            }

                            var info = new CallerInfo(name, packageName, isRelease, certificate);

                            if (false == certificates.TryGetValue(certificate, out var infos))
                            {
                                infos = new List<CallerInfo>();
                                certificates.Add(certificate, infos);
                            }

                            infos.Add(info);
                        }
                    }
                }
                catch (XmlException e)
                {
                    // LogHelper.Error(Tag, e, "Could not read allowed callers from XML.");
                }
                catch (IOException e)
                {
                    // LogHelper.Error(Tag, e, "Could not read allowed callers from XML.");
                }
            }

            return certificates;
        }

        //
        private readonly struct CallerInfo
        {
            public string Name
            {
                get;
            }

            public string PackageName
            {
                get;
            }

            public bool Release
            {
                get;
            }

            public string? SigningCertificate
            {
                get;
            }

            public CallerInfo(string name, string packageName, bool release, string? signingCertificate)
            {
                Name = name;
                PackageName = packageName;
                Release = release;
                SigningCertificate = signingCertificate;
            }
        }
    }
}