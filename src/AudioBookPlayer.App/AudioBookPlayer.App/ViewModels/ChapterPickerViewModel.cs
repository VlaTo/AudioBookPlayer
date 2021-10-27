﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Services;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class ChapterPickerViewModel : ViewModelBase, IInitialize
    {
        private readonly IMediaBrowserServiceConnector connector;
        private readonly TaskExecutionMonitor chaptersLoader;
        private ChapterViewModel selectedChapter;
        private bool isInitializing;

        public ObservableCollection<IChapterViewModel> Sections
        {
            get;
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
                        connector.SetActiveQueueItemId(value.QueueId);
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

        public ChapterPickerViewModel(IMediaBrowserServiceConnector connector)
        {
            this.connector = connector;

            chaptersLoader = new TaskExecutionMonitor(LoadChaptersAsync);

            Close = new Command(DoCancel);
            CloseRequest = new InteractionRequest<ClosePopupRequestContext>();
            Sections = new ObservableCollection<IChapterViewModel>();
            
            // chapterIndex = -1;
            selectedChapter = null;
        }

        public void OnInitialize()
        {
            chaptersLoader.Start();
        }

        private int FindChapterIndex(ChapterViewModel model)
        {

            for (var index = 0; index < Sections.Count; index++)
            {
                var current = Sections[index];

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

                SectionViewModel GetOrCreateSection(ISectionMetadata sectionMetadata)
                {
                    for (int index = 0; index < Sections.Count; index++)
                    {
                        if (Sections[index] is SectionViewModel svm)
                        {
                            if (svm.Index == sectionMetadata.Index)
                            {
                                return svm;
                            }
                        }
                    }

                    var section = new SectionViewModel
                    {
                        Title = sectionMetadata.Name,
                        Index = sectionMetadata.Index
                    };

                    Sections.Add(section);

                    return section;
                }

                for (var chapterIndex = 0; chapterIndex < connector.Chapters.Count; chapterIndex++)
                {
                    var chapter = connector.Chapters[chapterIndex];
                    var sectionViewModel = GetOrCreateSection(chapter.Section);
                    var chapterViewModel = new ChapterViewModel(chapterIndex, chapter.QueueId)
                    {
                        Title = chapter.Title
                    };
                    
                    sectionViewModel.Entries.Add(chapterViewModel);

                    if (connector.ActiveQueueItemId == chapter.QueueId)
                    {
                        selectedChapter = chapterViewModel;
                    }
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