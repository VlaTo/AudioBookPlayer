namespace LibraProgramming.Media.QuickTime
{
    public static class AtomTypes
    {
        public const uint Root = 0x00000000;
        public const uint Empty = 0xFFFFFFFF;

        //
        public const uint Mvhd = 0x6D766864;
        public const uint Trak = 0x7472616B;
        public const uint Tkhd = 0x746B6864;
        public const uint Stco = 0x7374636F;
        public const uint Stbl = 0x7374626C;
        public const uint Stts = 0x73747473;
        public const uint Ftyp = 0x66747970;
        public const uint Moov = 0x6D6F6F76;
        public const uint Free = 0x66726565;
        public const uint Mdat = 0x6D646174;
        public const uint Mdia = 0x6D646961;
        public const uint Minf = 0x6D696E66;
        public const uint Mdhd = 0x6D646864;
        public const uint Tref = 0x74726566;
        public const uint Hdlr = 0x68646C72;

        public const uint Chap = 0x63686170;

        //
        public const uint Soun = 0x736F756E;
        public const uint Text = 0x74657874;
        public const uint Iods = 0x696F6473;
        public const uint Udta = 0x75647461;
        public const uint Smhd = 0x736D6864;

        public const uint Dinf = 0x64696E66;
        public const uint Dref = 0x64726566;
        public const uint Gmhd = 0x676D6864;
        public const uint Stsd = 0x73747364;
        public const uint Stsz = 0x7374737A;
        public const uint Stsc = 0x73747363;
        public const uint Ctts = 0x63747473;
        public const uint Name = 0x6E616D65;
        public const uint Meta = 0x6D657461;
        public const uint Edts = 0x65647473;
        public const uint Vmhd = 0x766D6864;
        public const uint Stss = 0x73747373;
        public const uint Hnti = 0x686E7469;
        public const uint Hinf = 0x68696E66;
        public const uint Wide = 0x77696465;
        public const uint Elst = 0x656C7374;
        public const uint Sgpd = 0x73677064;
        public const uint Sbgp = 0x73626770;
        public const uint Chpl = 0x6368706C;

        public const uint Url = 0x75726C20;

        // 
        public const uint Mp4a = 0x6D703461;
        public const uint Esds = 0x65736473;

        public const uint Ilst = 0x696C7374;

        // ilst
        public const uint Too = 0xA9746F6F;
        public const uint Art0 = 0xA9415254;
        public const uint Art1 = 0xA9617274;
        public const uint Aart = 0x61415254;
        public const uint Alb = 0xA9616C62;
        public const uint Day = 0xA9646179;
        public const uint Gen = 0xA967656E;
        public const uint Cmt = 0xA9636D74;
        public const uint Covr = 0x636F7672;
        public const uint Data = 0x64617461;
        public const uint Alis = 0x616C6973;
        public const uint Mp4v = 0x6D703476;
        public const uint Rtp = 0x72747020;
        public const uint Nam = 0xA96E616D;
        public const uint Stik = 0x7374696B;
        public const uint Wrt = 0xA9777274;
        public const uint Lyr = 0xA96C7972;
        public const uint Trkn = 0x74726B6E;
    }
}