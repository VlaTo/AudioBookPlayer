using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Controls
{
    public class PlaybackControl : TemplatedView
    {
        public static readonly BindableProperty PlayProperty;

        public ICommand Play
        {
            get => (ICommand)GetValue(PlayProperty);
            set => SetValue(PlayProperty, value);
        }

        static PlaybackControl()
        {
            PlayProperty = BindableProperty.Create(
                nameof(Play),
                typeof(ICommand),
                typeof(PlaybackControlPanel),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: OnPlayPropertyChanged
            );
        }

        private void OnPlayChanged(ICommand oldValue, ICommand newValue)
        {
            ;
        }

        private static void OnPlayPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var instance = (PlaybackControl)bindable;
            instance.OnPlayChanged((ICommand)oldValue, (ICommand)newValue);
        }
    }
}
