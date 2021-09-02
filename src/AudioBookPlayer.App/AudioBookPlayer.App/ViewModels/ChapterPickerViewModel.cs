using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core;
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

        public ObservableCollection<IChapterViewModel> ChapterGroups
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

                    if (null != selectedChapter)
                    {
                        playbackService.ChapterIndex = selectedChapter.Index;
                    }

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
            ChapterGroups = new ObservableCollection<IChapterViewModel>();
            
            chapterIndex = -1;
            selectedChapter = null;
        }

        public void OnInitialize()
        {
            chaptersLoader.Start();
        }

        private int FindChapterIndex(ChapterViewModel model)
        {

            for (var index = 0; index < ChapterGroups.Count; index++)
            {
                var current = ChapterGroups[index];

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

                ChapterViewModel selection = null;

                var localIndex = 0;

                for (var partIndex = 0; partIndex < playbackService.AudioBook.Sections.Count; partIndex++)
                {
                    var part = playbackService.AudioBook.Sections[partIndex];
                    var groupEntry = new ChapterGroupViewModel
                    {
                        Title = part.Title
                    };

                    ChapterGroups.Add(groupEntry);

                    for (var index = 0; index < part.Chapters.Count; index++)
                    {
                        var chapter = part.Chapters[index];
                        var chapterViewModel = new ChapterViewModel(localIndex)
                        {
                            Title = chapter.Title
                        };

                        groupEntry.Entries.Add(chapterViewModel);

                        if (localIndex++ == playbackService.ChapterIndex)
                        {
                            selection = chapterViewModel;
                        }
                    }
                }


                /*else
                {
                    HasGroups = false;

                    var chapters = playbackService.AudioBook.Chapters;

                    for (var index = 0; index < chapters.Count; index++)
                    {
                        var chapter = chapters[index];
                        var model = new ChapterViewModel
                        {
                            Title = chapter.Title
                        };

                        ViewModels.Add(model);

                        if (index == playbackService.ChapterIndex)
                        {
                            selection = model;
                        }
                    }
                }*/

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