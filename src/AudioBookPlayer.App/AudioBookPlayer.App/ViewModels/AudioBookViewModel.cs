using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class AudioBookViewModel : ViewModelBase
    {
        private EntityId id;
        private string authors;
        private string title;
        private string synopsis;
        private TimeSpan duration;
        private Func<CancellationToken, Task<Stream>> imageSource;

        public EntityId Id
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

        public Func<CancellationToken, Task<Stream>> ImageSource
        {
            get => imageSource;
            set => SetProperty(ref imageSource, value);
        }
    }
}