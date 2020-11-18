using LibraProgramming.Media.QuickTime.Chunks;

namespace LibraProgramming.Media.QuickTime.Visitors
{
    internal class QuickTimeMediaVisitor
    {
        protected QuickTimeMediaVisitor()
        {
        }

        public virtual void Visit(RootChunk chunk)
        {
            foreach(var child in chunk.Chunks)
            {
                switch (child)
                {
                    case MediaTypeChunk mediaType:
                    {
                        VisitMediaType(mediaType);
                        break;
                    }

                    case FreeChunk free:
                    {
                        VisitFree(free);
                        break;
                    }

                    case MdatChunk mdat:
                    {
                        VisitMdat(mdat);
                        break;
                    }

                    case MoovChunk moov:
                    {
                        VisitMoov(moov);
                        break;
                    }
                }
            }
        }

        public virtual void VisitMediaType(MediaTypeChunk chunk)
        {
            ;
        }

        public virtual void VisitFree(FreeChunk chunk)
        {
            ;
        }

        public virtual void VisitMdat(MdatChunk chunk)
        {
            ;
        }

        public virtual void VisitMoov(MoovChunk chunk)
        {
            foreach (var child in chunk.Chunks)
            {
                switch (child)
                {
                    case UdtaChunk udta:
                    {
                        VisitUdta(udta);
                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
        }

        public virtual void VisitUdta(UdtaChunk chunk)
        {
            foreach (var child in chunk.Chunks)
            {
                switch (child)
                {
                    case MetaChunk meta:
                    {
                        VisitMeta(meta);
                        break;
                    }

                    case UdtaChunk udta:
                    {
                        VisitUdta(udta);
                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
        }

        public virtual void VisitMeta(MetaChunk chunk)
        {
            foreach (var child in chunk.Chunks)
            {
                switch (child)
                {
                    case HdlrChunk hdlr:
                    {
                        VisitHdlr(hdlr);
                        break;
                    }

                    case IlstChunk ilst:
                    {
                        VisitIlst(ilst);
                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
        }

        public virtual void VisitHdlr(HdlrChunk chunk)
        {
            ;
        }

        public virtual void VisitIlst(IlstChunk chunk)
        {
            ;
        }
    }
}
