using System;
using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime.Chunks;
using LibraProgramming.Media.QuickTime.Lists;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using LibraProgramming.Media.QuickTime.Extensions;

namespace LibraProgramming.Media.QuickTime.Visitors
{
    internal sealed class MetaInformationVisitor : QuickTimeMediaVisitor
    {
        private readonly IList<MetaInformationItem> items;

        public MetaInformationVisitor(IList<MetaInformationItem> items)
        {
            this.items = items;
        }

        public override void VisitIlst(IlstChunk chunk)
        {
            foreach (var meta in chunk.MetaInfoChunks)
            {
                /*{
                    var bytes = BitConverter.GetBytes(meta.Type).ToBigEndian();
                    var key = bytes.ToChunkKey();
                    Debug.WriteLine($"[MetaInformation] Key: '{key}'");
                }*/

                switch (meta.Type)
                {
                    case AtomTypes.Covr:
                    {
                        if (DataType.Binary == meta.DataChunk.DataType)
                        {
                            var stream = new MemoryStream(meta.DataChunk.Data);
                            items.Add(MetaInformationItem.FromStream(WellKnownMetaItemNames.Cover, stream));
                        }

                        break;
                    }

                    case AtomTypes.Alb:
                    case AtomTypes.Nam:
                    {
                        if (DataType.Text == meta.DataChunk.DataType)
                        {
                            var info = MetaInformationItem.FromText(WellKnownMetaItemNames.Title, meta.DataChunk.Text);
                            items.Add(info);
                        }

                        break;
                    }

                    case AtomTypes.Art0:
                    case AtomTypes.Art1:
                    {
                        if (DataType.Text == meta.DataChunk.DataType)
                        {
                            var info = MetaInformationItem.FromText(WellKnownMetaItemNames.Author, meta.DataChunk.Text);
                            items.Add(info);
                        }

                        break;
                    }

                    case AtomTypes.Lyr:
                    {
                        if (DataType.Text == meta.DataChunk.DataType)
                        {
                            var info = MetaInformationItem.FromText(WellKnownMetaItemNames.Subtitle, meta.DataChunk.Text);
                            items.Add(info);
                        }

                        break;
                    }

                    default:
                    {
                        var bytes = BitConverter.GetBytes(meta.Type).ToBigEndian();
                        var key = bytes.ToChunkKey();

                        if (DataType.Binary == meta.DataChunk.DataType)
                        {
                            //Debug.WriteLine($"");
                        }
                        else if (DataType.Text == meta.DataChunk.DataType)
                        {
                            var info = MetaInformationItem.FromText(key, meta.DataChunk.Text);
                            items.Add(info);
                        }

                        break;
                    }
                }
            }
        }
    }
}
