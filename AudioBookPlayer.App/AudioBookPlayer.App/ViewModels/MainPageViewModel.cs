using AudioBookPlayer.App.Core.Services;
using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly ISourceStreamProvider streamProvider;
        private ImageSource imageSource;

        public ImageSource ImageSource
        {
            get => imageSource;
            private set => SetProperty(ref imageSource, value);
        }

        public ICommand Play
        {
            get;
        }

        public MainPageViewModel(INavigationService navigationService, ISourceStreamProvider streamProvider)
            : base(navigationService)
        {
            this.streamProvider = streamProvider;

            Title = "Main Page";
            Play = new DelegateCommand(DoPlayCommand);
            //ImageSource = new ImageSource();
        }

        private async void DoPlayCommand()
        {
            //var image = await FileSystem.OpenAppPackageFileAsync("sample-200.png");

            //ImageSource = ImageSource.FromStream(() => image);
            // 0F1A-3D0D:Audio/Books/book.m4b
            var drives = Directory.GetLogicalDrives();

            foreach(var drive in drives)
            {
                Debug.WriteLine($"Drive: '{drive}'");
            }

            /*var directories = Directory.EnumerateDirectories("/storage/0F1A-3D0D");
            
            foreach (var directory in directories)
            {
                Debug.WriteLine($"Directory: '{directory}'");
            }*/

            // /storage/emulated/0/Download/book.m4b
            // /storage/0F1A-3D0D/Audio/Books/book.m4b
            // FileSystem.CacheDirectory
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (var stream = File.OpenRead("Music/book.m4b"))
            {
                using (var extractor = QuickTimeMediaExtractor.CreateFrom(stream))
                {
                    var meta = extractor.GetMeta();

                    foreach (var item in meta.Items)
                    {
                        switch (item)
                        {
                            case MetaInformationStreamItem streamItem:
                            {
                                if (WellKnownMetaItemNames.Cover.Equals(item.Key))
                                {
                                    ImageSource = ImageSource.FromStream(() => streamItem.Stream);
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
