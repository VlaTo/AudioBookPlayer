using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Services
{
    public sealed partial class PlaybackService
    {
        private IPlayerState state;
        
        internal IPlayerState State
        {
            get => state;
            set
            {
                if (ReferenceEquals(state, value))
                {
                    return;
                }

                if (null != state)
                {
                    state.Leave();
                }

                state = value;

                if (null != state)
                {
                    state.Enter();
                }
            }
        }

        /// <summary>
        /// Player state abstraction
        /// </summary>
        internal interface IPlayerState
        {
            void Enter();

            void Leave();

            #region Player methods

            void SetAudioBook(AudioBook audioBook);

            void SetChapterIndex(int chapterIndex);

            void Play();

            void Pause();

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        private abstract class PlayerState : IPlayerState
        {
            protected PlaybackService Service
            {
                get;
            }

            protected PlayerState(PlaybackService service)
            {
                Service = service;
            }

            public virtual void Enter()
            {
                System.Diagnostics.Debug.WriteLine($"[PlayerState] Entering state: {GetType().Name}");
            }

            public virtual void Leave()
            {
                System.Diagnostics.Debug.WriteLine($"[PlayerState] Leaving state: {GetType().Name}");
            }

            public abstract void SetAudioBook(AudioBook audioBook);

            public abstract void SetChapterIndex(int chapterIndex);

            public abstract void Play();

            public abstract void Pause();
        }
    }
}