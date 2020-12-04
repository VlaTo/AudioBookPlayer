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
                    case FtypChunk mediaType:
                    {
                        VisitFtyp(mediaType);
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

        public virtual void VisitFtyp(FtypChunk chunk)
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
                    case MvhdChunk mvhd:
                    {
                        VisitMvhd(mvhd);
                        break;
                    }

                    case TrakChunk trak:
                    {
                        VisitTrak(trak);
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

        public virtual void VisitMvhd(MvhdChunk chunk)
        {
            ;
        }

        public virtual void VisitTrak(TrakChunk chunk)
        {
            foreach (var child in chunk.Chunks)
            {
                switch (child)
                {
                    case TkhdChunk tkhd:
                    {
                        VisitTkhd(tkhd);
                        break;
                    }

                    case MdiaChunk mdia:
                    {
                        VisitMdia(mdia);
                        break;
                    }

                    case UdtaChunk udta:
                    {
                        VisitUdta(udta);
                        break;
                    }

                    case TrefChunk tref:
                    {
                        VisitTref(tref);
                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
        }

        public virtual void VisitTkhd(TkhdChunk chunk)
        {
            ;
        }

        public virtual void VisitTref(TrefChunk chunk)
        {
            foreach (var child in chunk.Chunks)
            {
                switch (child)
                {
                    case ChapChunk chap:
                    {
                        VisitChap(chap);
                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
        }

        public virtual void VisitChap(ChapChunk chunk)
        {
            ;
        }

        public virtual void VisitMdia(MdiaChunk chunk)
        {
            foreach (var child in chunk.Chunks)
            {
                switch (child)
                {
                    case MdhdChunk mdhd:
                    {
                        VisitMdhd(mdhd);
                        break;
                    }

                    case HdlrChunk hdlr:
                    {
                        VisitHdlr(hdlr);
                        break;
                    }

                    case MinfChunk minf:
                    {
                        VisitMinf(minf);
                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
        }

        public virtual void VisitMdhd(MdhdChunk chunk)
        {
            ;
        }

        public virtual void VisitMinf(MinfChunk chunk)
        {
            foreach (var child in chunk.Chunks)
            {
                switch (child)
                {
                    case DinfChunk dinf:
                    {
                        VisitDinf(dinf);
                        break;
                    }

                    case StblChunk stbl:
                    {
                        VisitStbl(stbl);
                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
        }

        public virtual void VisitDinf(DinfChunk chunk)
        {
            ;
        }

        public virtual void VisitStbl(StblChunk chunk)
        {
            foreach (var child in chunk.Chunks)
            {
                switch (child)
                {
                    case StsdChunk stsd:
                    {
                        VisitStsd(stsd);
                        break;
                    }

                    case SttsChunk stts:
                    {
                        VisitStts(stts);
                        break;
                    }

                    case StszChunk stsz:
                    {
                        VisitStsz(stsz);
                        break;
                    }

                    case StscChunk stsc:
                    {
                        VisitStsc(stsc);
                        break;
                    }

                    case StcoChunk stco:
                    {
                        VisitStco(stco);
                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
        }

        public virtual void VisitStsd(StsdChunk chunk)
        {
            ;
        }

        public virtual void VisitStts(SttsChunk chunk)
        {
            ;
        }

        public virtual void VisitStsz(StszChunk chunk)
        {
            ;
        }

        public virtual void VisitStsc(StscChunk chunk)
        {
            ;
        }

        public virtual void VisitStco(StcoChunk chunk)
        {
            ;
        }
    }
}
