using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime.Chunks;
using LibraProgramming.Media.QuickTime.Lists;
using System.Diagnostics;
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
                        if (DataType.Binary == meta.DataChunk.DataType)
                        {
                            var stream = new MemoryStream(meta.DataChunk.Data);
                            information.Add(MetaInformationItem.FromStream(WellKnownMetaItemNames.Cover, stream));
                        }

                        break;
                    }

                    case AtomTypes.Alb:
                    {
                        if (DataType.Text == meta.DataChunk.DataType)
                        {
                            var info = MetaInformationItem.FromText(WellKnownMetaItemNames.Title, meta.DataChunk.Text);
                            information.Add(info);
                        }

                        break;
                    }

                    case AtomTypes.Art:
                    {
                        if (DataType.Text == meta.DataChunk.DataType)
                        {
                            var info = MetaInformationItem.FromText(WellKnownMetaItemNames.Subtitle, meta.DataChunk.Text);
                            information.Add(info);
                        }

                        break;
                    }

                    default:
                    {
                        if (DataType.Binary == meta.DataChunk.DataType)
                        {
                            //Debug.WriteLine($"");
                        }
                        else if (DataType.Text == meta.DataChunk.DataType)
                        {
                            //meta.Debug(0);

                            //Debug.WriteLine($"Text: '{meta.DataChunk.Text}'");
                        }

                        break;
                    }
                }
            }
        }
    }
}
