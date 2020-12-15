using ModernFlyouts.Core.Media;
using ModernFlyouts.Properties;
using ModernFlyouts.Utilities;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ModernFlyouts.Converters
{
    #region Tooltip converters

    internal class PlayPauseButtonTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPlaying)
            {
                return isPlaying ? Strings.SessionControl_Pause : Strings.SessionControl_Play;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class RepeatButtonTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MediaPlaybackAutoRepeatMode autoRepeatMode)
            {
                return autoRepeatMode switch
                {
                    MediaPlaybackAutoRepeatMode.None => Strings.SessionControl_RepeatOff,
                    MediaPlaybackAutoRepeatMode.Track => Strings.SessionControl_RepeatOne,
                    MediaPlaybackAutoRepeatMode.List => Strings.SessionControl_RepeatAll,
                    _ => throw new NotImplementedException(),
                };
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class ShuffleButtonTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? isShuffleActive = value as bool?;

            if (isShuffleActive != null)
            {
                if (isShuffleActive.HasValue)
                {
                    return isShuffleActive.Value ? Strings.SessionControl_ShuffleOn : Strings.SessionControl_ShuffleOff;
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Glyph converters

    internal class PlayPauseButtonGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPlaying)
            {
                return isPlaying ? CommonGlyphs.Pause : CommonGlyphs.Play;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class RepeatButtonGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MediaPlaybackAutoRepeatMode autoRepeatMode)
            {
                return autoRepeatMode switch
                {
                    MediaPlaybackAutoRepeatMode.None => CommonGlyphs.RepeatOff,
                    MediaPlaybackAutoRepeatMode.Track => CommonGlyphs.RepeatOne,
                    MediaPlaybackAutoRepeatMode.List => CommonGlyphs.RepeatAll,
                    _ => throw new NotImplementedException(),
                };
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region IsChecked converters

    internal class RepeatButtonIsCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MediaPlaybackAutoRepeatMode autoRepeatMode)
            {
                return autoRepeatMode switch
                {
                    MediaPlaybackAutoRepeatMode.None => false,
                    MediaPlaybackAutoRepeatMode.Track => true,
                    MediaPlaybackAutoRepeatMode.List => true,
                    _ => throw new NotImplementedException(),
                };
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    internal class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan.TotalMilliseconds;
            }

            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double milliSeconds)
            {
                return TimeSpan.FromMilliseconds(milliSeconds);
            }

            return TimeSpan.Zero;
        }
    }
}
