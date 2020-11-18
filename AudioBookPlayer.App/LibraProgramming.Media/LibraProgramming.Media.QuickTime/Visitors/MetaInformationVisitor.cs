using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime.Chunks;
using System.IO;

namespace LibraProgramming.Media.QuickTime.Visitors
{
    internal sealed class MetaInformationVisitor : QuickTimeMediaVisitor
    {
        private readonly MetaInformation information;

        public MetaInformationVisitor(MetaInformation information)
        {
            this.information = information;
        }

        public override void VisitIlst(IlstChunk chunk)
        {
            foreach (var meta in chunk.MetaInfoChunks)
            {
                switch (meta.Type)
                {
                    case AtomTypes.Covr:
                    {
                        var stream = new MemoryStream(meta.DataChunk.Data);

                        information.Add(MetaInformationItem.FromStream(WellKnownMetaItemNames.Cover, stream));

                        break;
                    }
                }
            }
        }
    }
}
