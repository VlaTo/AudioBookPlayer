#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Java.IO;

namespace AudioBookPlayer.App.Android.Core
{
	public class PackageValidator
    {
        private readonly Context context;
		private readonly Dictionary<string, List<CallerInfo>> certificates;

		public PackageValidator(Context context)
        {
            this.context = context;

			using (var reader = context.Resources?.GetXml(Resource.Xml.allowed_media_browser_callers))
			{
				certificates = ReadValidCertificates(reader);
			}
		}

		private static Dictionary<string, List<CallerInfo>> ReadValidCertificates(XmlReader parser)
		{
			var validCertificates = new Dictionary<string, List<CallerInfo>>();
			try
			{
				while (parser.Read())
				{
					if (parser.IsStartElement() && parser.Name == "signing_certificate" && parser.HasAttributes)
                    {
                        var name = parser[0];
                        var packageName = parser[1];
                        var isRelease = Convert.ToBoolean(parser[2]);
						var certificate = parser.ReadString();

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

                        if (false == validCertificates.TryGetValue(certificate, out var infos))
                        {
                            infos = new List<CallerInfo>();
                            validCertificates.Add(certificate, infos);
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

			return validCertificates;
		}

        public bool IsCallerAllowed([NotNull] string callingPackageName, int callingUid)
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
            catch (PackageManager.NameNotFoundException e)
            {
                //LogHelper.Warn(Tag, e, "Package manager can't find package: ", callingPackage);
                return false;
            }

            var signatures = packageInfo.SigningInfo.GetSigningCertificateHistory();

            for (int index = 0; index < signatures.Length; index++)
            {
                var packageSignature = Base64.EncodeToString(signatures[index].ToByteArray(), Base64Flags.NoWrap);
                // var signature = Base64.EncodeToString(packageInfo.SigningInfo.ToByteArray(), Base64Flags.NoWrap);

                if (false == certificates.TryGetValue(packageSignature, out var validCallers))
                {
                    return false;
                }

                // List<CallerInfo> validCallers = certificates[packageSignature];

                if (validCallers == null)
                {
                    //LogHelper.Verbose(Tag, "Signature for caller ", callingPackage, " is not valid: \n", signature);
                    if (certificates.Count == 0)
                    {
                        /*LogHelper.Warn(Tag, "The list of valid certificates is empty. Either your file ",
                            "res/xml/allowed_media_browser_callers.xml is empty or there was an error ",
                            "while reading it. Check previous log messages.");*/
                    }

                    return false;
                }

                // Check if the package name is valid for the certificate:
                var expectedPackages = new StringBuilder();

                foreach (var info in validCallers)
                {
                    if (callingPackageName == info.PackageName)
                    {
                        //LogHelper.Verbose(Tag, "Valid caller: ", info.Name, " package=", info.PackageName, " release=", info.Release);
                        return true;
                    }

                    expectedPackages.Append(info.PackageName).Append(' ');
                }
            }

            /*LogHelper.Info(Tag, "Caller has a valid certificate, but its package doesn't match any ",
                "expected package for the given certificate. Caller's package is ", callingPackage,
                ". Expected packages as defined in res/xml/allowed_media_browser_callers.xml are (",
                expectedPackages, "). This caller's certificate is: \n", signature);*/

            return false;
        }

        private sealed class CallerInfo
		{
			public string Name { get; set; }
			public string PackageName { get; set; }
			public bool Release { get; set; }
			public string? SigningCertificate { get; set; }

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