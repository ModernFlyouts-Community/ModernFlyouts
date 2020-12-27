using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ModernFlyouts.Core.UI
{
    public partial class FlyoutWindow
    {
        private DispatcherTimer timer;

        private void OpenFlyout()
        {
            Show();
            timer?.Stop();

            RoutedEventArgs args = new(OpenedEvent);
            RaiseEvent(args);

            PlayOpenAnimation();

            timer?.Start();
        }

        private void CloseFlyout()
        {
            RoutedEventArgs args = new(ClosingEvent);
            RaiseEvent(args);

            PlayCloseAnimation();
        }

        #region Close Timer

        private void SetupCloseTimer()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Timeout) };

            timer.Tick += (_, __) =>
            {
                timer.Stop();

                RoutedEventArgs args = new(ClosingEvent);
                RaiseEvent(args);

                if (!IsMouseOver && !args.Handled)
                {
                    IsOpen = false;
                }
            };

            MouseEnter += (_, e) =>
            {
                timer.Stop();
            };
            MouseLeave += (_, e) =>
            {
                timer.Start();
            };

            PreviewMouseDown += (_, __) => timer.Stop();
            PreviewStylusDown += (_, __) => timer.Stop();
            PreviewMouseUp += (_, __) => { timer.Stop(); timer.Start(); };
            PreviewStylusUp += (_, __) => { timer.Stop(); timer.Start(); };
        }

        public void StartCloseTimer()
        {
            if (!timer.IsEnabled) { timer.Start(); }
        }

        public void StopCloseTimer()
        {
            timer.Stop();
        }

        public void UpdateCloseTimerInterval(double timeout)
        {
            if (timer == null)
                return;

            var isRunning = timer.IsEnabled;

            timer.Stop();
            timer.Interval = TimeSpan.FromMilliseconds(timeout);

            if (isRunning) timer.Start();
        }

        #endregion

        #region Animations

        private bool hasAnimationsCreated;
        private Storyboard m_openingStoryboard;
        private Storyboard m_closingStoryboard;
        private DoubleKeyFrame m_fromHorizontalOffsetKeyFrameOpening;
        private DoubleKeyFrame m_fromVerticalOffsetKeyFrameOpening;
        private DoubleKeyFrame m_fromHorizontalOffsetKeyFrameClosing;
        private DoubleKeyFrame m_fromVerticalOffsetKeyFrameClosing;

        private const double c_translation = 40;
        private static readonly TimeSpan s_translateDuration = TimeSpan.FromMilliseconds(367);

        private static readonly PropertyPath s_opacityPath = new(OpacityProperty);
        private static readonly PropertyPath s_visibilityPath = new(VisibilityProperty);
        private static readonly PropertyPath s_translateXPath = new("(UIElement.RenderTransform).(TranslateTransform.X)");
        private static readonly PropertyPath s_translateYPath = new("(UIElement.RenderTransform).(TranslateTransform.Y)");
        private static readonly KeySpline s_decelerateKeySplineOpening = new(0.1, 0.9, 0.2, 1);
        private static readonly KeySpline s_decelerateKeySplineClosing = new(1, 0.2, 0.9, 0.1);

        private void PrepareAnimations()
        {
            EnsureClosingStoryboard();
            EnsureOpeningStoryboard();

            Opacity = 0.0;
            if (!(RenderTransform is TranslateTransform))
            {
                RenderTransform = new TranslateTransform();
            }

            hasAnimationsCreated = true;
        }

        private void EnsureOpeningStoryboard()
        {
            if (m_openingStoryboard == null)
            {
                var visibilityAnim = new ObjectAnimationUsingKeyFrames
                {
                    KeyFrames =
                    {
                        new DiscreteObjectKeyFrame(Visibility.Visible, TimeSpan.Zero)
                    }
                };
                Storyboard.SetTarget(visibilityAnim, this);
                Storyboard.SetTargetProperty(visibilityAnim, s_visibilityPath);

                var opacityAnim = new DoubleAnimationUsingKeyFrames
                {
                    KeyFrames =
                    {
                        new DiscreteDoubleKeyFrame(0, TimeSpan.Zero),
                        new DiscreteDoubleKeyFrame(0, TimeSpan.FromMilliseconds(83)),
                        new LinearDoubleKeyFrame(1, TimeSpan.FromMilliseconds(166))
                    }
                };
                Storyboard.SetTarget(opacityAnim, this);
                Storyboard.SetTargetProperty(opacityAnim, s_opacityPath);

                var xAnim = new DoubleAnimationUsingKeyFrames
                {
                    KeyFrames =
                    {
                        (m_fromHorizontalOffsetKeyFrameOpening = new DiscreteDoubleKeyFrame(0, TimeSpan.Zero)),
                        new SplineDoubleKeyFrame(0, s_translateDuration, s_decelerateKeySplineOpening)
                    }
                };
                Storyboard.SetTarget(xAnim, this);
                Storyboard.SetTargetProperty(xAnim, s_translateXPath);

                var yAnim = new DoubleAnimationUsingKeyFrames
                {
                    KeyFrames =
                    {
                        (m_fromVerticalOffsetKeyFrameOpening = new DiscreteDoubleKeyFrame(0, TimeSpan.Zero)),
                        new SplineDoubleKeyFrame(0, s_translateDuration, s_decelerateKeySplineOpening)
                    }
                };
                Storyboard.SetTarget(yAnim, this);
                Storyboard.SetTargetProperty(yAnim, s_translateYPath);

                m_openingStoryboard = new Storyboard
                {
                    Children = { visibilityAnim, opacityAnim, xAnim, yAnim },
                };
            }
        }

        private void EnsureClosingStoryboard()
        {
            if (m_closingStoryboard == null)
            {
                var opacityAnim = new DoubleAnimationUsingKeyFrames
                {
                    KeyFrames =
                    {
                        new DiscreteDoubleKeyFrame(1, TimeSpan.Zero),
                        new DiscreteDoubleKeyFrame(1, TimeSpan.FromMilliseconds(83)),
                        new LinearDoubleKeyFrame(0, TimeSpan.FromMilliseconds(166))
                    }
                };
                Storyboard.SetTarget(opacityAnim, this);
                Storyboard.SetTargetProperty(opacityAnim, s_opacityPath);

                var xAnim = new DoubleAnimationUsingKeyFrames
                {
                    KeyFrames =
                    {
                        new DiscreteDoubleKeyFrame(0, TimeSpan.Zero),
                        (m_fromHorizontalOffsetKeyFrameClosing = new SplineDoubleKeyFrame(
                            0, s_translateDuration, s_decelerateKeySplineClosing))
                    }
                };
                Storyboard.SetTarget(xAnim, this);
                Storyboard.SetTargetProperty(xAnim, s_translateXPath);

                var yAnim = new DoubleAnimationUsingKeyFrames
                {
                    KeyFrames =
                    {
                        new DiscreteDoubleKeyFrame(0, TimeSpan.Zero),
                        (m_fromVerticalOffsetKeyFrameClosing = new SplineDoubleKeyFrame(
                            0, s_translateDuration, s_decelerateKeySplineClosing))
                    }
                };
                Storyboard.SetTarget(yAnim, this);
                Storyboard.SetTargetProperty(yAnim, s_translateYPath);

                var visibilityAnim = new ObjectAnimationUsingKeyFrames
                {
                    KeyFrames =
                    {
                        new DiscreteObjectKeyFrame(Visibility.Collapsed, TimeSpan.FromMilliseconds(250))
                    }
                };
                Storyboard.SetTarget(visibilityAnim, this);
                Storyboard.SetTargetProperty(visibilityAnim, s_visibilityPath);

                m_closingStoryboard = new Storyboard
                {
                    Children = { opacityAnim, xAnim, yAnim, visibilityAnim },
                };
            }
        }

        private void PlayOpenAnimation()
        {
            m_openingStoryboard.Begin(this, true);
        }

        private void PlayCloseAnimation()
        {
            m_closingStoryboard.Begin(this, true);
        }

        private void UpdateAnimations(FlyoutWindowExpandDirection expandDirection)
        {
            if (!hasAnimationsCreated)
                return;

            switch (expandDirection)
            {
                case FlyoutWindowExpandDirection.Auto:
                    m_fromHorizontalOffsetKeyFrameOpening.Value = 0;
                    m_fromHorizontalOffsetKeyFrameClosing.Value = 0;
                    m_fromVerticalOffsetKeyFrameOpening.Value = 0;
                    m_fromVerticalOffsetKeyFrameClosing.Value = 0;
                    break;

                case FlyoutWindowExpandDirection.Up:
                    m_fromHorizontalOffsetKeyFrameOpening.Value = 0;
                    m_fromHorizontalOffsetKeyFrameClosing.Value = 0;
                    m_fromVerticalOffsetKeyFrameOpening.Value = c_translation;
                    m_fromVerticalOffsetKeyFrameClosing.Value = c_translation;
                    break;

                case FlyoutWindowExpandDirection.Down:
                    m_fromHorizontalOffsetKeyFrameOpening.Value = 0;
                    m_fromHorizontalOffsetKeyFrameClosing.Value = 0;
                    m_fromVerticalOffsetKeyFrameOpening.Value = -c_translation;
                    m_fromVerticalOffsetKeyFrameClosing.Value = -c_translation;
                    break;

                case FlyoutWindowExpandDirection.Left:
                    m_fromHorizontalOffsetKeyFrameOpening.Value = c_translation;
                    m_fromHorizontalOffsetKeyFrameClosing.Value = c_translation;
                    m_fromVerticalOffsetKeyFrameOpening.Value = 0;
                    m_fromVerticalOffsetKeyFrameClosing.Value = 0;
                    break;

                case FlyoutWindowExpandDirection.Right:
                    m_fromHorizontalOffsetKeyFrameOpening.Value = -c_translation;
                    m_fromHorizontalOffsetKeyFrameClosing.Value = -c_translation;
                    m_fromVerticalOffsetKeyFrameOpening.Value = 0;
                    m_fromVerticalOffsetKeyFrameClosing.Value = 0;
                    break;
            }
        }

        #endregion
    }
}
