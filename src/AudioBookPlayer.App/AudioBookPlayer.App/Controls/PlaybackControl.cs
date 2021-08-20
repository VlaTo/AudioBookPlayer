using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Controls
{
    public class PlaybackControl : TemplatedControl
    {
        private const string PlayButtonPartName = "PART_PlayButton";
        private const string FastForwardButtonPartName = "PART_FastForwardButton";
        private const string RewindButtonPartName = "PART_RewindButton";

        private const string PlayStateName = "Play";
        public const string PauseStateName = "Pause";

        public static readonly BindableProperty PlayProperty;
        public static readonly BindableProperty RewindProperty;
        public static readonly BindableProperty FastForwardProperty;
        public static readonly BindableProperty CanPlayProperty;
        public static readonly BindableProperty IsPlayingProperty;

        private ImageButton playButton;
        private ImageButton fastForwardButton;
        private ImageButton rewindButton;

        public bool CanPlay
        {
            get => (bool) GetValue(CanPlayProperty);
            set => SetValue(CanPlayProperty, value);
        }

        public bool IsPlaying
        {
            get => (bool) GetValue(IsPlayingProperty);
            set => SetValue(IsPlayingProperty, value);
        }

        public ICommand FastForward
        {
            get => (ICommand) GetValue(FastForwardProperty);
            set => SetValue(FastForwardProperty, value);
        }

        public ICommand Play
        {
            get => (ICommand) GetValue(PlayProperty);
            set => SetValue(PlayProperty, value);
        }

        public ICommand Rewind
        {
            get => (ICommand) GetValue(RewindProperty);
            set => SetValue(RewindProperty, value);
        }

        static PlaybackControl()
        {
            IsPlayingProperty = BindableProperty.Create(
                nameof(IsPlaying),
                typeof(bool),
                typeof(PlaybackControl),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: OnIsPlayingPropertyChanged,
                defaultValue: false
            );
            CanPlayProperty = BindableProperty.Create(
                nameof(CanPlay),
                typeof(bool),
                typeof(PlaybackControl),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: OnCanPlayPropertyChanged,
                defaultValue: true
            );
            FastForwardProperty = BindableProperty.Create(
                nameof(FastForward),
                typeof(ICommand),
                typeof(PlaybackControl),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: OnFastForwardPropertyChanged
            );
            PlayProperty = BindableProperty.Create(
                nameof(Play),
                typeof(ICommand),
                typeof(PlaybackControl),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: OnPlayPropertyChanged
            );
            RewindProperty = BindableProperty.Create(
                nameof(Rewind),
                typeof(ICommand),
                typeof(PlaybackControl),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: OnRewindPropertyChanged
            );
        }

        protected override void OnApplyTemplate()
        {
            rewindButton = GetTemplatePart<ImageButton>(RewindButtonPartName);
            playButton = GetTemplatePart<ImageButton>(PlayButtonPartName);
            fastForwardButton = GetTemplatePart<ImageButton>(FastForwardButtonPartName);

            playButton.Clicked += OnPlayButtonClicked;

            base.OnApplyTemplate();


            UpdatePlayButtonState();
        }

        private void UpdatePlayButtonState()
        {
            var stateName = IsPlaying ? PauseStateName : PlayStateName;
            VisualStateManager.GoToState(playButton, stateName);
        }

        private void OnPlayButtonClicked(object sender, EventArgs e)
        {
            ;
        }

        private void OnCanPlayPropertyChanged(bool oldValue, bool newValue)
        {
            //VisualStateManager.GoToState(this,);
        }

        private void OnIsPlayingPropertyChanged(bool oldValue, bool newValue)
        {
            UpdatePlayButtonState();
        }

        private void OnFastForwardChanged(ICommand oldValue, ICommand newValue)
        {
            ;
        }
        
        private void OnPlayChanged(ICommand oldValue, ICommand newValue)
        {
            ;
        }
        
        private void OnRewindChanged(ICommand oldValue, ICommand newValue)
        {
            ;
        }

        private static void OnIsPlayingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var instance = (PlaybackControl) bindable;
            instance.OnIsPlayingPropertyChanged((bool) oldValue, (bool) newValue);
        }

        private static void OnCanPlayPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var instance = (PlaybackControl) bindable;
            instance.OnCanPlayPropertyChanged((bool) oldValue, (bool) newValue);
        }

        private static void OnFastForwardPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var instance = (PlaybackControl) bindable;
            instance.OnFastForwardChanged((ICommand) oldValue, (ICommand) newValue);
        }

        private static void OnPlayPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var instance = (PlaybackControl) bindable;
            instance.OnPlayChanged((ICommand) oldValue, (ICommand) newValue);
        }

        private static void OnRewindPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var instance = (PlaybackControl) bindable;
            instance.OnRewindChanged((ICommand) oldValue, (ICommand) newValue);
        }
    }
}