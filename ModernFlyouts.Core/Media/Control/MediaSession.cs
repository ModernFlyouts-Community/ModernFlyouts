using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernFlyouts.Core.Media.Control
{
    public abstract class MediaSession : ObservableObject
    {
        #region Properties

        private bool isCurrent;

        public bool IsCurrent
        {
            get => isCurrent;
            internal set => SetProperty(ref isCurrent, value);
        }

        #region Playback Features

        private bool isPlayEnabled;

        public bool IsPlayEnabled
        {
            get => isPlayEnabled;
            protected set => SetProperty(ref isPlayEnabled, value);
        }

        private bool isPauseEnabled;

        public bool IsPauseEnabled
        {
            get => isPauseEnabled;
            protected set => SetProperty(ref isPauseEnabled, value);
        }

        private bool isPlayOrPauseEnabled;

        public bool IsPlayOrPauseEnabled
        {
            get => isPlayOrPauseEnabled;
            protected set => SetProperty(ref isPlayOrPauseEnabled, value);
        }

        private bool isPreviousEnabled;

        public bool IsPreviousEnabled
        {
            get => isPreviousEnabled;
            protected set => SetProperty(ref isPreviousEnabled, value);
        }

        private bool isNextEnabled;

        public bool IsNextEnabled
        {
            get => isNextEnabled;
            protected set => SetProperty(ref isNextEnabled, value);
        }

        private bool isShuffleEnabled;

        public bool IsShuffleEnabled
        {
            get => isShuffleEnabled;
            protected set
            {
                if (SetProperty(ref isShuffleEnabled, value))
                {
                    CalculateMoreControlsButtonVisibility();
                }
            }
        }

        private bool isRepeatEnabled;

        public bool IsRepeatEnabled
        {
            get => isRepeatEnabled;
            protected set
            {
                if (SetProperty(ref isRepeatEnabled, value))
                {
                    CalculateMoreControlsButtonVisibility();
                }
            }
        }

        private bool isStopEnabled;

        public bool IsStopEnabled
        {
            get => isStopEnabled;
            protected set
            {
                if (SetProperty(ref isStopEnabled, value))
                {
                    CalculateMoreControlsButtonVisibility();
                }
            }
        }

        private bool isPlaybackPositionEnabled;

        public bool IsPlaybackPositionEnabled
        {
            get => isPlaybackPositionEnabled;
            protected set => SetProperty(ref isPlaybackPositionEnabled, value);
        }

        #endregion

        #region Playback Information

        private bool isPlaying;

        public bool IsPlaying
        {
            get => isPlaying;
            protected set => SetProperty(ref isPlaying, value);
        }

        private bool? isShuffleActive;

        public bool? IsShuffleActive
        {
            get => isShuffleActive;
            protected set => SetProperty(ref isShuffleActive, value);
        }

        private MediaPlaybackAutoRepeatMode autoRepeatMode = MediaPlaybackAutoRepeatMode.None;

        public MediaPlaybackAutoRepeatMode AutoRepeatMode
        {
            get => autoRepeatMode;
            protected set => SetProperty(ref autoRepeatMode, value);
        }

        private MediaPlaybackType playbackType = MediaPlaybackType.Unknown;

        public MediaPlaybackType PlaybackType
        {
            get => playbackType;
            protected set => SetProperty(ref playbackType, value);
        }

        private MediaPlaybackTrackChangeDirection trackChangeDirection = MediaPlaybackTrackChangeDirection.Unknown;

        public MediaPlaybackTrackChangeDirection TrackChangeDirection
        {
            get => trackChangeDirection;
            protected set => SetProperty(ref trackChangeDirection, value);
        }

        #endregion

        #region Playback Control

        private ICommand playCommand;

        public ICommand PlayCommand
        {
            get => playCommand;
            protected set => SetProperty(ref playCommand, value);
        }

        private ICommand pauseCommand;

        public ICommand PauseCommand
        {
            get => pauseCommand;
            protected set => SetProperty(ref pauseCommand, value);
        }

        private ICommand playOrPauseCommand;

        public ICommand PlayOrPauseCommand
        {
            get => playOrPauseCommand;
            protected set => SetProperty(ref playOrPauseCommand, value);
        }

        private ICommand previousTrackCommand;

        public ICommand PreviousTrackCommand
        {
            get => previousTrackCommand;
            protected set => SetProperty(ref previousTrackCommand, value);
        }

        private ICommand nextTrackCommand;

        public ICommand NextTrackCommand
        {
            get => nextTrackCommand;
            protected set => SetProperty(ref nextTrackCommand, value);
        }

        private ICommand changeShuffleActiveCommand;

        public ICommand ChangeShuffleActiveCommand
        {
            get => changeShuffleActiveCommand;
            protected set => SetProperty(ref changeShuffleActiveCommand, value);
        }

        private ICommand changeAutoRepeatModeCommand;

        public ICommand ChangeAutoRepeatModeCommand
        {
            get => changeAutoRepeatModeCommand;
            protected set => SetProperty(ref changeAutoRepeatModeCommand, value);
        }

        private ICommand stopCommand;

        public ICommand StopCommand
        {
            get => stopCommand;
            protected set => SetProperty(ref stopCommand, value);
        }

        #endregion

        #region Media Properties

        private string title = string.Empty;

        public string Title
        {
            get => title;
            protected set => SetProperty(ref title, value);
        }

        private string artist = string.Empty;

        public string Artist
        {
            get => artist;
            protected set => SetProperty(ref artist, value);
        }

        private ImageSource thumbnail;

        public ImageSource Thumbnail
        {
            get => thumbnail;
            protected set => SetProperty(ref thumbnail, value);
        }

        #endregion

        #region Timeline Properties

        private bool isTimelinePropertiesEnabled;

        public bool IsTimelinePropertiesEnabled
        {
            get => isTimelinePropertiesEnabled;
            protected set => SetProperty(ref isTimelinePropertiesEnabled, value);
        }

        private TimeSpan timelineStartTime = TimeSpan.Zero;

        public TimeSpan TimelineStartTime
        {
            get => timelineStartTime;
            protected set => SetProperty(ref timelineStartTime, value);
        }

        private TimeSpan timelineEndTime = TimeSpan.Zero;

        public TimeSpan TimelineEndTime
        {
            get => timelineEndTime;
            protected set => SetProperty(ref timelineEndTime, value);
        }

        private TimeSpan playbackPosition = TimeSpan.Zero;

        /// <summary>
        /// Gets or sets the position of the media playback.
        /// </summary>
        /// <remarks>
        /// Please use <see cref="SetPlaybackPosition"/> to set the playback position internally.
        /// This property's setter is only intended for binding purposes.
        /// </remarks>
        public TimeSpan PlaybackPosition
        {
            get => playbackPosition;
            set
            {
                if (SetProperty(ref playbackPosition, value))
                {
                    PlaybackPositionChanged(playbackPosition);
                }
            }
        }

        #endregion

        #region Media Source

        private ImageSource mediaSourceIcon;

        public ImageSource MediaSourceIcon
        {
            get => mediaSourceIcon;
            protected set => SetProperty(ref mediaSourceIcon, value);
        }

        private string mediaSourceName = string.Empty;

        public string MediaSourceName
        {
            get => mediaSourceName;
            protected set => SetProperty(ref mediaSourceName, value);
        }

        private ICommand activateMediaSourceCommand;

        public ICommand ActivateMediaSourceCommand
        {
            get => activateMediaSourceCommand;
            protected set => SetProperty(ref activateMediaSourceCommand, value);
        }

        #endregion

        #region Calculated Values

        private Visibility calculatedMoreControlsButtonVisibility = Visibility.Collapsed;

        public Visibility CalculatedMoreControlsButtonVisibility
        {
            get => calculatedMoreControlsButtonVisibility;
            private set => SetProperty(ref calculatedMoreControlsButtonVisibility, value);
        }

        #endregion

        #endregion

        public MediaSession()
        {
            PlayCommand = new RelayCommand(Play, () => IsPlayEnabled);
            PauseCommand = new RelayCommand(Pause, () => IsPauseEnabled);
            PlayOrPauseCommand = new RelayCommand(PlayOrPause, () => IsPlayOrPauseEnabled);
            PreviousTrackCommand = new RelayCommand(PreviousTrack, () => IsPreviousEnabled);
            NextTrackCommand = new RelayCommand(NextTrack, () => IsNextEnabled);
            ChangeShuffleActiveCommand = new RelayCommand(ChangeShuffleActive, () => IsShuffleEnabled);
            ChangeAutoRepeatModeCommand = new RelayCommand(ChangeAutoRepeatMode, () => IsRepeatEnabled);
            StopCommand = new RelayCommand(Stop, () => IsStopEnabled);
        }

        #region Private Methods

        private void CalculateMoreControlsButtonVisibility()
        {
            CalculatedMoreControlsButtonVisibility = (IsShuffleEnabled || IsRepeatEnabled || IsStopEnabled) ?
                Visibility.Visible : Visibility.Collapsed;
        }

        private void PlayOrPause()
        {
            if (isPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        #endregion

        #region Abstract Methods

        protected abstract void Play();

        protected abstract void Pause();

        protected abstract void PreviousTrack();

        protected abstract void NextTrack();

        protected abstract void ChangeShuffleActive();

        protected abstract void ChangeAutoRepeatMode();

        protected abstract void Stop();

        protected abstract void PlaybackPositionChanged(TimeSpan playbackPosition);

        #endregion

        /// <summary>
        /// Sets the <see cref="PlaybackPosition"/> property's value internally and updates it without causing a callback loop
        /// </summary>
        protected void SetPlaybackPosition(TimeSpan value)
        {
            SetProperty(ref playbackPosition, value, nameof(PlaybackPosition));
        }

        protected void RaiseMediaPropertiesChanging()
        {
            MediaPropertiesChanging?.Invoke(this, EventArgs.Empty);
        }

        protected void RaiseMediaPropertiesChanged()
        {
            MediaPropertiesChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Disconnect()
        {
        }

        public event EventHandler MediaPropertiesChanging;

        public event EventHandler MediaPropertiesChanged;
    }
}
