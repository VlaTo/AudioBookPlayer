using AndroidX.Room.Migration;
using AndroidX.Sqlite.Db;

namespace AudioBookPlayer.Persistent.Migrations
{
    internal class InitialMigration : Migration
    {
        public InitialMigration()
            : base(0, 1)
        {
        }

        public override void Migrate(ISupportSQLiteDatabase database)
        {
            throw new System.NotImplementedException();
        }
    }
}