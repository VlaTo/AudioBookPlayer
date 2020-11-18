using LibraProgramming.Media.QuickTime.Chunks;

namespace LibraProgramming.Media.QuickTime.Visitors
{
    internal interface IContainerChunkVisitor
    {
        void Visit(ContainerChunk chunk);
    }
}
