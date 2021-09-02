using System;

namespace LibraProgramming.Media.QuickTime.Components
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class ChunkCreatorAttribute : Attribute
    {
        public ChunkCreatorAttribute()
        {
        }
    }
}
