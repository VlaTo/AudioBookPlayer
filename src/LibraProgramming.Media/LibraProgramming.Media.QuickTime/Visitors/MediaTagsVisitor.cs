using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime.Chunks;
using LibraProgramming.Media.QuickTime.Extensions;
using LibraProgramming.Media.QuickTime.Lists;
using System;
using System.IO;

namespace LibraProgramming.Media.QuickTime.Visitors
{
    internal sealed class MediaTagsVisitor : QuickTimeMediaVisitor
    {
        private readonly MediaTags information;

        public MediaTagsVisitor(MediaTags information)
        {
            this.information = information;
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
                            information.Add(WellKnownMediaTags.Cover, TagValue.FromStream(stream));
                        }

                        break;
                    }

                    case AtomTypes.Alb:
                    case AtomTypes.Nam:
                    {
                        if (DataType.Text == meta.DataChunk.DataType)
                        {
                            var info = TagValue.FromText(meta.DataChunk.Text);
                            information.Add(WellKnownMediaTags.Title, info);
                        }

                        break;
                    }

                    case AtomTypes.Art0:
                    case AtomTypes.Art1:
                    {
                        if (DataType.Text == meta.DataChunk.DataType)
                        {
                            var info = TagValue.FromText(meta.DataChunk.Text);
                            information.Add(WellKnownMediaTags.Author, info);
                        }

                        break;
                    }

                    case AtomTypes.Lyr:
                    {
                        if (DataType.Text == meta.DataChunk.DataType)
                        {
                            var info = TagValue.FromText(meta.DataChunk.Text);
                            information.Add(WellKnownMediaTags.Subtitle, info);
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
                            var info = TagValue.FromText( meta.DataChunk.Text);
                            information.Add(key, info);
                        }

                        break;
                    }
                }
            }
        }
    }
}
