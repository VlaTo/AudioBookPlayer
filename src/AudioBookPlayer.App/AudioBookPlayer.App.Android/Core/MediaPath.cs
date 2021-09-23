using System;
using System.Collections.Generic;
using AudioBookPlayer.App.Domain.Models;
using Java.Lang;
using Exception = System.Exception;
using String = System.String;

namespace AudioBookPlayer.App.Android.Core
{
    internal class MediaPath
    {
        public const char PathDelimiter = '/';
        private const string root = "/";

        private readonly PathSegment[] segments;

        public static readonly MediaPath Empty;
        public static readonly MediaPath Root;

        public bool IsEmpty { get; }

        public bool IsRoot { get; }

        public string this[int index]
        {
            get
            {
                var segment = segments[index];
                return segment.ToString();
            }
        }

        private MediaPath(PathSegment[] segments, bool isRoot)
        {
            this.segments = segments;

            IsEmpty = 0 == segments.Length && false == isRoot;
            IsRoot = isRoot;
        }

        static MediaPath()
        {
            Empty = new MediaPath(Array.Empty<PathSegment>(), false);
            Root = new MediaPath(new[] { new PathSegment(root, 0, root.Length) }, true);
        }

        public MediaPath ToAbsolute()
        {
            return Empty;
        }

        public override string ToString()
        {
            var sb = new global::System.Text.StringBuilder();

            for (var index = 0; index < segments.Length; index++)
            {
                var segment = segments[index];

                if (0 < sb.Length)
                {
                    sb.Append(PathDelimiter);
                }

                sb.Append(segment.ToString());
            }

            return sb.ToString();
        }

        public static bool TryParse(string s, out MediaPath mediaPath)
        {
            if (String.IsNullOrEmpty(s))
            {
                mediaPath = null;
                return false;
            }

            if (s.StartsWith(root))
            {
                var parts = Split(s);
                mediaPath = 0 == parts.Length
                    ? new MediaPath(Array.Empty<PathSegment>(), true)
                    : new MediaPath(parts, true);
            }
            else
            {
                mediaPath = Empty;
            }

            return true;
        }

        public static MediaPath Parse(string s)
        {
            if (false == TryParse(s, out var instance))
            {
                throw new Exception();
            }

            return instance;
        }

        public static MediaPath Combine(MediaPath basePath, EntityId id)
        {
            return basePath;
        }

        private static PathSegment[] Split(string s)
        {
            if (false == s.StartsWith(root))
            {
                return Array.Empty<PathSegment>();
            }

            var offset = 0;
            var segments = new List<PathSegment>();

            for (var index = 0; index < s.Length; index++)
            {
                if (PathDelimiter != s[index])
                {
                    continue;
                }

                var segment = new PathSegment(s, offset, index);

                segments.Add(segment);
                offset = index;
            }

            return segments.ToArray();
        }

        // String segment
        private readonly struct PathSegment
        {
            private readonly string source;
            private readonly int offset;

            public int Length
            {
                get;
            }

            public char this[int index]
            {
                get
                {
                    if (index >= Length)
                    {
                        throw new Exception();
                    }

                    return source[offset + index];
                }
            }

            public PathSegment(string source, int offset, int length)
            {
                this.source = source;
                this.offset = offset;
                Length = length;
            }

            public override string ToString()
            {
                return source.Substring(offset, Length);
            }
        }
    }
}