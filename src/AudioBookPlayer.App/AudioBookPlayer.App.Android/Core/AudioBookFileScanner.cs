using Android.Content;
using Android.Content.Res;
using Android.Database;
using Android.Provider;
using AudioBookPlayer.App.Android.Core.Attributes;
using AudioBookPlayer.App.Android.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xamarin.Forms.Internals;
using Exception = Java.Lang.Exception;
using String = System.String;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Android.Core
{
    internal sealed class AudioBookFile
    {
        [Column(Name = MediaStore.Audio.Media.InterfaceConsts.Id)]
        public long Id
        {
            get;
            set;
        }

        [Column(Name = MediaStore.Audio.IAudioColumns.Track)]
        public int TrackNumber
        {
            get;
            set;
        }

        [Column(Name = MediaStore.Audio.IAudioColumns.Album)]
        public string Album
        {
            get;
            set;
        }

        [Column(Name = MediaStore.Audio.IAudioColumns.Artist)]
        public string Artist
        {
            get;
            set;
        }
        
        [Column(Name = MediaStore.Audio.IAudioColumns.Composer)]
        public string Composer
        {
            get;
            set;
        }
        
        [Column(Name = MediaStore.Files.IFileColumns.Title)]
        public string Title
        {
            get;
            set;
        }

        [Column(Name = MediaStore.Audio.IAudioColumns.IsAudiobook)]
        public bool IsAudioBook
        {
            get;
            set;
        }

        [Column(Name = MediaStore.Audio.IAudioColumns.Duration)]
        public TimeSpan Duration
        {
            get;
            set;
        }

        [Column(Name = MediaStore.Audio.IAudioColumns.Year)]
        public short Year
        {
            get;
            set;
        }

        [Column(Name = MediaStore.IMediaColumns.DisplayName)]
        public string Name
        {
            get;
            set;
        }

        public Uri ContentUri => MediaStore.Audio.Media.GetContentUri(MediaStore.VolumeExternal, Id);
    }

    internal sealed class AudioBookFileScanner
    {
        private const string OpenRead = "r";

        private readonly ContentResolver contentResolver;
        private readonly Uri collection;

        public AudioBookFileScanner(ContentResolver contentResolver, Uri collection)
        {
            this.contentResolver = contentResolver;
            this.collection = collection;
        }

        public IEnumerable<AudioBookFile> QueryFiles()
        {
            const string sortOrder = MediaStore.Audio.Media.InterfaceConsts.Id + " ASC";
            var columns = CreateProjectionColumns();

            if (null == contentResolver)
            {
                yield break;
            }

            var projection = columns
                .Select(tuple => tuple.Name)
                .ToArray();
            var cursor = contentResolver.Query(
                collection,
                projection,
                $"{MediaStore.Audio.IAudioColumns.IsAudiobook} = ?",
                new []{ 1.ToString() },
                sortOrder
            );

            var success = null != cursor && cursor.MoveToFirst();

            while (success)
            {
                var audioBookFile = CreateAudioBookFile(cursor, columns);

                yield return audioBookFile;

                success = cursor.MoveToNext();
            }
        }

        public AssetFileDescriptor OpenFile(AudioBookFile file)
        {
            if (null == file)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return contentResolver.OpenAssetFileDescriptor(file.ContentUri, OpenRead);
        }

        public (string MimeType, ContentResolver.MimeTypeInfo MimeInfo) GetMimeInfo(AudioBookFile file)
        {
            if (null == file)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var mimeType = contentResolver.GetType(file.ContentUri);
            var mimeInfo = contentResolver.GetTypeInfo(mimeType);

            return (mimeType, mimeInfo);
        }

        private static AudioBookFile CreateAudioBookFile(ICursor cursor, (string Name, PropertyInfo Property)[] columns)
        {
            var instance = new AudioBookFile();

            for (var index = 0; index < cursor.ColumnCount; index++)
            {
                var columnName = cursor.GetColumnName(index);
                var position = columns.IndexOf(tuple => String.Equals(tuple.Name, columnName));

                if (0 > position)
                {
                    continue;
                }

                var property = columns[position].Property;
                var value = GetColumnValue(cursor, index, property);

                property.SetValue(instance, value);
            }

            return instance;
        }

        private static object GetColumnValue(ICursor cursor, int columnIndex, PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            if (propertyType.IsString())
            {
                return cursor.GetString(columnIndex);
            }

            if (propertyType.IsShort())
            {
                return cursor.GetShort(columnIndex);
            }

            if (propertyType.IsInt())
            {
                return cursor.GetInt(columnIndex);
            }

            if (propertyType.IsLong())
            {
                return cursor.GetLong(columnIndex);
            }

            if (propertyType.IsBoolean())
            {
                return 0 < cursor.GetShort(columnIndex);
            }

            if (propertyType.IsTimeSpan())
            {
                var value = cursor.GetLong(columnIndex);
                return TimeSpan.FromMilliseconds(value);
            }

            throw new Exception();
        }

        private static (string Name, PropertyInfo Property)[] CreateProjectionColumns()
        {
            var type = typeof(AudioBookFile);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var columns = new List<(string Name, PropertyInfo Property)>();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<ColumnAttribute>();

                if (null == attribute)
                {
                    continue;
                }

                var columnName = attribute.Name ?? property.Name;

                columns.Add((columnName, property));
            }

            return columns.ToArray();
        }
    }
}