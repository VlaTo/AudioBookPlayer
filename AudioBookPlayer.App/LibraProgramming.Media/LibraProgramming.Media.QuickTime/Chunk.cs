namespace LibraProgramming.Media.QuickTime
{
    public abstract class Chunk
    {
        public uint Type
        {
            get;
        }

        protected Chunk(uint type)
        {
            Type = type;
        }

        public abstract void Debug(int level);
    }
}