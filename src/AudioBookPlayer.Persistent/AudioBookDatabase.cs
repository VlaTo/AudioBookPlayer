using Android.Content;
using AndroidX.Room;
using AudioBookPlayer.Persistent.DataAccess;
using AudioBookPlayer.Persistent.Entities;
using AudioBookPlayer.Persistent.Migrations;
using Java.Lang;

namespace AudioBookPlayer.Persistent
{
    //[Database(Entities = new []{ Class.FromType(typeof(BookEntity)) }, ExportSchema = true, Version = 1)]
    public abstract class AudioBookDatabase : RoomDatabase
    {
        private const string DbName = "audiobooks.db";

        private static AudioBookDatabase instance;

        public static AudioBookDatabase GetDatabase(Context context)
        {
            if (null == instance)
            {
                instance = (AudioBookDatabase)Room
                    .DatabaseBuilder(context, Class.FromType(typeof(AudioBookDatabase)), DbName)
                    .AddMigrations(new InitialMigration())
                    .Build();
            }

            return instance;
        }

        public abstract BookEntityDao Books();
    }
}
