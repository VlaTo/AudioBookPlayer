using System;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class AudioBookViewModel : ViewModelBase
    {
        private string authors;
        private string title;
        private string synopsis;
        private TimeSpan duration;

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
    }
}