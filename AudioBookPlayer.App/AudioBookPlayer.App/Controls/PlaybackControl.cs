using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Controls
{
    public class PlaybackControl : TemplatedControl
    {
        private const string PlayButtonPartName = "PART_PlayButton";

        public static readonly BindableProperty PlayProperty;

        private ImageButton play;

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
                typeof(PlaybackControl),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: OnPlayPropertyChanged
            );
        }

        protected override void OnApplyTemplate()
        {
            play = GetTemplatePart<ImageButton>(PlayButtonPartName);

            base.OnApplyTemplate();
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
