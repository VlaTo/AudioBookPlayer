namespace LibraProgramming.QuickTime.Container.Chunks
{
    /*
    [Chunk(AtomTypes.Tkhd)]
    public sealed class TkhdChunk : Chunk
    {
        public bool Poster
        {
            get;
        }

        public bool Preview
        {
            get;
        }

        public bool Movie
        {
            get;
        }

        public bool Enabled
        {
            get;
        }

        public DateTime Created
        {
            get;
        }

        public DateTime Modified
        {
            get;
        }

        public TimeSpan Duration
        {
            get;
        }

        public int TrackId
        {
            get;
        }

        private TkhdChunk(
            bool poster,
            bool preview,
            bool movie,
            bool enabled,
            DateTime created,
            DateTime modified,
            TimeSpan duration,
            int trackId)
            : base(AtomTypes.Tkhd)
        {
            Poster = poster;
            Preview = preview;
            Movie = movie;
            Enabled = enabled;
            Created = created;
            Modified = modified;
            Duration = duration;
            TrackId = trackId;
        }

        public static TkhdChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            //var bytes = StreamHelper.ReadBytes(atom.Stream, (uint) atom.Stream.Length);
            //Print.WriteDump(bytes);

            var bits = StreamHelper.ReadUInt32(atom.Stream);
            var version = (byte) ((bits & Flag.VersionMask) >> 24);
            var poster = (bits & Flag.Poster) == Flag.Poster;
            var preview = (bits & Flag.Preview) == Flag.Preview;
            var movie = (bits & Flag.Movie) == Flag.Movie;
            var enabled = (bits & Flag.Enabled) == Flag.Enabled;
            DateTime created;
            DateTime modified;
            TimeSpan duration;

            switch (version)
            {
                case 0:
                {
                    var temp1 = StreamHelper.ReadUInt32(atom.Stream);
                    var temp2 = StreamHelper.ReadUInt32(atom.Stream);

                    created = DateTime.UtcNow;
                    modified = DateTime.UtcNow;

                    break;
                }

                case 1:
                {
                    var temp1 = StreamHelper.ReadUInt64(atom.Stream);
                    var temp2 = StreamHelper.ReadUInt64(atom.Stream);

                    created = DateTime.UtcNow;
                    modified = DateTime.UtcNow;

                    break;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }

            var trackId = StreamHelper.ReadInt32(atom.Stream);
            var reserved0 = StreamHelper.ReadUInt64(atom.Stream);

            switch (version)
            {
                case 0:
                {
                    var temp1 = StreamHelper.ReadUInt32(atom.Stream);

                    duration = TimeSpan.Zero;

                    break;
                }

                case 1:
                {
                    var temp1 = StreamHelper.ReadUInt64(atom.Stream);

                    duration = TimeSpan.Zero;

                    break;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }

            var reserved1 = StreamHelper.ReadUInt32(atom.Stream);
            var videoLayer = StreamHelper.ReadUInt16(atom.Stream);

            return new TkhdChunk(poster, preview, movie, enabled, created, modified, duration, trackId);
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Console.WriteLine($"{tabs}{type} track ID: '{TrackId}'");
        }

        private static class Flag
        {
            public const uint VersionMask = 0xFF00_0000;
            public const uint Poster = 0x0000_0008;
            public const uint Preview = 0x0000_0004;
            public const uint Movie = 0x0000_0002;
            public const uint Enabled = 0x0000_0001;
        }
    }
    */
}