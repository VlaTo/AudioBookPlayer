namespace LibraProgramming.QuickTime.Container.Chunks
{
    /*
    [Chunk(AtomTypes.Mvhd)]
    public sealed class MvhdChunk : Chunk
    {
        public uint Version
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

        private MvhdChunk(uint version, DateTime created, DateTime modified, TimeSpan duration)
            : base(AtomTypes.Mvhd)
        {
            Version = version;
            Created = created;
            Modified = modified;
            Duration = duration;
        }

        public static MvhdChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var bits = StreamHelper.ReadFlags32(atom.Stream);
            var version = (bits & 0xFF00_0000) >> 24;
            var flags = bits & 0x00FF_FFFF;

            DateTime created;
            DateTime modified;
            TimeSpan duration;

            switch (version)
            {
                case 0:
                {
                    var value = StreamHelper.ReadUInt32(atom.Stream);
                    created = DateTimeOffset.FromUnixTimeSeconds(value).ToUniversalTime().DateTime;

                    value = StreamHelper.ReadUInt32(atom.Stream);
                    modified = DateTime.UtcNow;

                    break;
                }

                case 1:
                {
                    var value = StreamHelper.ReadInt64(atom.Stream);
                    created = DateTimeOffset.FromUnixTimeMilliseconds(value).ToUniversalTime().DateTime;

                    value = StreamHelper.ReadInt64(atom.Stream);
                    modified = DateTime.UtcNow;

                    break;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }

            var timeScale= StreamHelper.ReadUInt32(atom.Stream);

            switch (version)
            {
                case 0:
                {
                    var value = StreamHelper.ReadUInt32(atom.Stream);

                    duration = TimeSpan.FromSeconds(value);

                    break;
                }

                case 1:
                {
                    var value = StreamHelper.ReadUInt64(atom.Stream);

                    duration = TimeSpan.FromSeconds(value);

                    break;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }

            var playbackSpeed = StreamHelper.ReadUInt32(atom.Stream);
            var volume = StreamHelper.ReadUInt16(atom.Stream);
            var reserved1 = StreamHelper.ReadUInt16(atom.Stream);
            var wgm = Wgm.Read(atom.Stream);
            var preview = StreamHelper.ReadUInt64(atom.Stream);
            var poster = StreamHelper.ReadUInt32(atom.Stream);
            var selectionTime = StreamHelper.ReadInt64(atom.Stream);
            var currentTime = StreamHelper.ReadInt64(atom.Stream);
            var nextId = StreamHelper.ReadInt32(atom.Stream);

            return new MvhdChunk(version, created, modified, duration);
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Console.WriteLine($"{tabs}{type} duration: '{Duration:g}'");
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class Wgm
        {
            public uint A
            {
                get;
            }

            public uint B
            {
                get;
            }

            public uint U
            {
                get;
            }

            public uint C
            {
                get;
            }

            public uint D
            {
                get;
            }

            public uint V
            {
                get;
            }

            public uint X
            {
                get;
            }

            public uint Y
            {
                get;
            }

            public uint W
            {
                get;
            }

            private Wgm(uint a, uint b, uint u, uint c, uint d, uint v, uint x, uint y, uint w)
            {
                A = a;
                B = b;
                U = u;
                C = c;
                D = d;
                V = v;
                X = x;
                Y = y;
                W = w;
            }

            public static Wgm Read(Stream stream)
            {
                var a = StreamHelper.ReadUInt32(stream);
                var b = StreamHelper.ReadUInt32(stream);
                var u = StreamHelper.ReadUInt32(stream);
                var c = StreamHelper.ReadUInt32(stream);
                var d = StreamHelper.ReadUInt32(stream);
                var v = StreamHelper.ReadUInt32(stream);
                var x = StreamHelper.ReadUInt32(stream);
                var y = StreamHelper.ReadUInt32(stream);
                var w = StreamHelper.ReadUInt32(stream);

                return new Wgm(a, b, u, c, d, v, x, y, w);
            }
        }
    }
    */
}