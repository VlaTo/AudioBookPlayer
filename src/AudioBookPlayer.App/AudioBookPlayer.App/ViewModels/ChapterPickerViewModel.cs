using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Services;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class ChapterPickerViewModel : ViewModelBase, IInitialize
    {
        private readonly IPlaybackService playbackService;
        private readonly TaskExecutionMonitor chaptersLoader;
        private int chapterIndex;
        private ChapterViewModel selectedChapter;
        private bool isInitializing;

        public ObservableCollection<ChapterViewModel> Chapters
        {
            get;
        }

        public int ChapterIndex
        {
            get => chapterIndex;
            set => SetProperty(ref chapterIndex, value);
        }

        public ChapterViewModel SelectedChapter
        {
            get => selectedChapter;
            set
            {
                if (SetProperty(ref selectedChapter, value))
                {
                    if (isInitializing)
                    {
                        return;
                    }

                    playbackService.ChapterIndex = FindChapterIndex(selectedChapter);

                    DoClose(false);
                }
            }
        }

        public Command Close
        {
            get;
        }

        public InteractionRequest<ClosePopupRequestContext> CloseRequest
        {
            get;
        }

        public ChapterPickerViewModel(IPlaybackService playbackService)
        {
            this.playbackService = playbackService;
            chaptersLoader = new TaskExecutionMonitor(LoadChaptersAsync);

            Close = new Command(DoCancel);
            CloseRequest = new InteractionRequest<ClosePopupRequestContext>();
            Chapters = new ObservableCollection<ChapterViewModel>();
            ChapterIndex = -1;
            SelectedChapter = null;
        }

        public void OnInitialize()
        {
            chaptersLoader.Start();
        }

        private int FindChapterIndex(ChapterViewModel model)
        {
            for (var index = 0; index < Chapters.Count; index++)
            {
                var current = Chapters[index];

                if (ReferenceEquals(current, model))
                {
                    return index;
                }
            }

            return -1;
        }

        private Task LoadChaptersAsync()
        {
            try
            {
                isInitializing = true;

                var chapters = playbackService.AudioBook.Chapters;
                ChapterViewModel selection = null;

                for (var index = 0; index < chapters.Count; index++)
                {
                    var chapter = chapters[index];
                    var model = new ChapterViewModel
                    {
                        Title = chapter.Title
                    };

                    Chapters.Add(model);

                    if (index == playbackService.ChapterIndex)
                    {
                        selection = model;
                    }
                }

                if (null != selection)
                {
                    SelectedChapter = selection;
                }
            }
            finally
            {
                isInitializing = false;
            }

            return Task.CompletedTask;
        }

        private void DoCancel()
        {
            DoClose(true);
        }

        private void DoClose(bool animated)
        {
            CloseRequest.Raise(new ClosePopupRequestContext(animated));
        }
    }
}