using Android.App;
using LibraProgramming.Xamarin.Core;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class DatabasePath : IPlatformDatabasePath
    {
        public string GetDatabasePath(string databaseName)
        {
            var file = Application.Context.GetDatabasePath(databaseName);

            if (false == file.Exists())
            {
                var created = file.CreateNewFile();
            }
            
            /*if (false == Directory.Exists(file.Parent))
            {
                var folder = Directory.CreateDirectory(file.Parent);
            }*/

            return file.AbsolutePath;
        }
    }
}