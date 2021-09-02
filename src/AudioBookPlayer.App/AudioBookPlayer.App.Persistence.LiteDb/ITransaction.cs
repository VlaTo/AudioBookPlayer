using System;

namespace AudioBookPlayer.App.Persistence.LiteDb
{
    public interface ITransaction : IDisposable
    {
        void Commit();
    }
}