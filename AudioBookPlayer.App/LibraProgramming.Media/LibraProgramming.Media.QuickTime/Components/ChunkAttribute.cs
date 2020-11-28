using System;

namespace LibraProgramming.Media.QuickTime.Components
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ChunkAttribute : Attribute
    {
        public uint AtomType
        {
            get;
        }

        public ChunkAttribute(uint atomType)
        {
            AtomType = atomType;
        }
    }
}