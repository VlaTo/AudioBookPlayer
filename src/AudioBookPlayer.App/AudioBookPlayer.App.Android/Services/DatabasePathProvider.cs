using Android.App;
using LibraProgramming.Xamarin.Core;

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// Android platform Database path provider implementation.
    /// </summary>
    internal sealed class DatabasePathProvider : IDatabasePathProvider
    {
        /// <inheritdoc cref="IDatabasePathProvider.GetDatabasePath" />
        public string GetDatabasePath(string databaseName)
        {
            var databasePath = Application.Context.GetDatabasePath(databaseName);

            if (null == databasePath)
            {
                System.Diagnostics.Debug.WriteLine("[IPlatformDatabasePath] Cannot get database file path");
                return null;
            }

            if (false == databasePath.Exists())
            {
                var created = databasePath.CreateNewFile();

                if (created)
                {
                    System.Diagnostics.Debug.WriteLine($"[IPlatformDatabasePath] Creating new database file: \"{databasePath}\"");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[IPlatformDatabasePath] Failed to create database file: \"{databasePath}\"");
                    return null;
                }
            }

            return databasePath.AbsolutePath;
        }
    }
}