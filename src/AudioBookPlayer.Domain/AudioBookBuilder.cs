using System;
using System.Collections.Generic;
using AudioBookPlayer.Domain.Models;

namespace AudioBookPlayer.Domain
{
    /*public class AudioBookBuilder
    {
        private readonly List<string> authors;
        private readonly List<string> description;
        private readonly List<AudioBookSectionBuilder> sections;
        private TimeSpan duration;

        public string Title
        {
            get;
            private set;
        }

        public long MediaId
        {
            get;
            private set;
        }

        public TimeSpan Duration
        {
            get;
            private set;
        }

        public AudioBookBuilder()
        {
            authors = new List<string>();
            description = new List<string>();
            sections = new List<AudioBookSectionBuilder>();
            duration = TimeSpan.Zero;
        }

        public AudioBook Build()
        {

        }

        public AudioBookBuilder SetTitle(string value)
        {
            Title = value;
            return this;
        }

        public AudioBookBuilder SetMediaId(long value)
        {
            MediaId = value;
            return this;
        }

        public AudioBookBuilder SetAuthors(IEnumerable<string> value)
        {
            authors.Clear();
            return AddAuthors(value);
        }

        public AudioBookBuilder AddAuthors(IEnumerable<string> value)
        {
            authors.AddRange(value);
            return this;
        }

        public AudioBookBuilder AddAuthor(string value)
        {
            authors.Add(value);
            return this;
        }

        public AudioBookBuilder SetDescription(string value)
        {
            description.Clear();
            return AddDescription(value);
        }

        public AudioBookBuilder AddDescription(string value)
        {
            description.Add(value);
            return this;
        }

        public AudioBookBuilder AddDescriptions(IEnumerable<string> value)
        {
            description.AddRange(value);
            return this;
        }

        public AudioBookSectionBuilder NewSection()
        {
            var builder = new AudioBookSectionBuilder(this);
            
            sections.Add(builder);

            return builder;
        }
    }

    public sealed class AudioBookSectionBuilder
    {
        private readonly AudioBookBuilder bookBuilder;
        private string title;
        private string sourceFileUri;

        public AudioBookSectionBuilder(AudioBookBuilder bookBuilder)
        {
            this.bookBuilder = bookBuilder;
        }

        public AudioBookSectionBuilder SetTitle(string value)
        {
            title = value;
            return this;
        }

        public AudioBookSectionBuilder SetSourceFileUri(string value)
        {
            sourceFileUri = value;
            return this;
        }


    }*/
}