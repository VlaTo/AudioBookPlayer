using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class BookPreviewViewModel : ViewModelBase
    {
        private string authors;
        private string title;
        private TimeSpan duration;
        private double position;
        private bool completed;
        private Func<CancellationToken, Task<Stream>> imageSource;

        public BookId Id
        {
            get;
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

        public TimeSpan Duration
        {
            get => duration;
            set => SetProperty(ref duration, value);
        }

        public double Position
        {
            get => position;
            set => SetProperty(ref position, value);
        }

        public bool Completed
        {
            get => completed;
            set => SetProperty(ref completed, value);
        }

        public Func<CancellationToken, Task<Stream>> ImageSource
        {
            get => imageSource;
            set => SetProperty(ref imageSource, value);
        }

        public BookPreviewViewModel(BookId id)
        {
            Id = id;
        }
    }
}