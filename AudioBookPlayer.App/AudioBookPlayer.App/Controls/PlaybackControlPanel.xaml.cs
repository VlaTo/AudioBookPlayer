using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaybackControlPanel : ContentView
    {
        public static readonly BindableProperty PlayProperty;

        public ICommand Play
        {
            get => (ICommand)GetValue(PlayProperty);
            set => SetValue(PlayProperty, value);
        }

        public PlaybackControlPanel()
        {
            InitializeComponent();
        }

        static PlaybackControlPanel()
        {
            PlayProperty = BindableProperty.Create(
                nameof(Play),
                typeof(ICommand),
                typeof(PlaybackControlPanel),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: OnPlayPropertyChanged
            );
        }

        /*private void PlayButtonClicked(object sender, EventArgs e)
        {
            var command = Play;

            if (null == command)
            {
                return;
            }

            command.Execute(this);
        }*/

        private void OnPlayChanged(ICommand oldValue, ICommand newValue)
        {
            ;
        }

        private static void OnPlayPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var instance = (PlaybackControlPanel)bindable;
            instance.OnPlayChanged((ICommand)oldValue, (ICommand)newValue);
        }
    }
}