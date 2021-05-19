using System;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class AudioBookViewModel : ViewModelBase
    {
        private long id;
        private string authors;
        private string title;
        private string synopsis;
        private TimeSpan duration;
        private ImageSource imageSource;

        public long Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public string Authors
        {
            get => authors;
            set => SetProperty(ref authors, value);
        }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public string Synopsis
        {
            get => synopsis;
            set => SetProperty(ref synopsis, value);
        }

        public TimeSpan Duration
        {
            get => duration;
            set => SetProperty(ref duration, value);
        }

        public ImageSource ImageSource
        {
            get => imageSource;
            set => SetProperty(ref imageSource, value);
        }
    }
}