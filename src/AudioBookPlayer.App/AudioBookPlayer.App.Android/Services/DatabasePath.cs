using Android.App;
using LibraProgramming.Xamarin.Core;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class DatabasePath : IPlatformDatabasePath
    {
        public string GetDatabasePath(string databaseName)
        {
            var file = Application.Context.GetDatabasePath(databaseName);
            //var file0 = Application.Context.GetFileStreamPath(databaseName);
            //var dirs = Application.Context.GetExternalCacheDirs();
            //var dir = Application.Context.GetExternalFilesDir(databaseName);

            if (false == file.Exists())
            {
                var created = file.CreateNewFile();
            }

            return file.AbsolutePath;
        }
    }
}