namespace LibraProgramming.Media.QuickTime
{
    public interface IStreamChunk
    {
        AtomType Type
        {
            get;
        }

        long Offset
        {
            get;
        }

        uint Length
        {
            get;
        }

        void Debug();
    }
}