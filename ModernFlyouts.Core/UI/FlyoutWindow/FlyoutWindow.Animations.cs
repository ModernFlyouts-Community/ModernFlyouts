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


            if (FlyoutAnimationEnabled)
            {
                c_translation = 40;
                PlayOpenAnimation();
            }
            else
            {
                c_translation = 0;
                RenderTransform = new TranslateTransform();
                BeginAnimation(VisibilityProperty, null);
                BeginAnimation(OpacityProperty, null);
                Visibility = Visibility.Visible;
                Opacity = 1.0;
            }

            timer?.Start();
        }

        private void CloseFlyout()
        {
            RoutedEventArgs args = new(ClosingEvent);
            RaiseEvent(args);

            if (FlyoutAnimationEnabled)
            {
                PlayCloseAnimation();
            }
            else
            {
                Opacity = 0.0;
                Visibility = Visibility.Hidden;
            }
        }

        #region Close Timer

        private void SetupCloseTimer()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Timeout) };

            timer.Tick += (_, __) =>
            {
                timer.Stop();

                if (!IsTimeoutEnabled || IsDragMoving)
                    return;

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
            PreviewTouchDown += (_, __) => timer.Stop();
            PreviewMouseUp += (_, __) => { timer.Stop(); timer.Start(); };
            PreviewStylusUp += (_, __) => { timer.Stop(); timer.Start(); };
            PreviewTouchUp += (_, __) => { timer.Stop(); timer.Start(); };
        }

        public void StartCloseTimer()
        {
            if (IsTimeoutEnabled && timer != null && !timer.IsEnabled) { timer?.Start(); }
        }

        public void StopCloseTimer()
        {
            timer?.Stop();
        }

        public void UpdateCloseTimerInterval(double timeout)
        {
            if (timer == null)
                return;

            var isRunning = timer.IsEnabled;

            timer.Stop();
            timer.Interval = TimeSpan.FromMilliseconds(timeout);

            if (IsTimeoutEnabled && isRunning) timer.Start();
        }

        #endregion

        #region Animations

        private bool hasAnimationsCreated;
        private Storyboard openingStoryboard;
        private Storyboard closingStoryboard;
        private DoubleKeyFrame fromHorizontalOffsetKeyFrameOpening;
        private DoubleKeyFrame fromVerticalOffsetKeyFrameOpening;
        private DoubleKeyFrame fromHorizontalOffsetKeyFrameClosing;
        private DoubleKeyFrame fromVerticalOffsetKeyFrameClosing;

        private double c_translation = 40;
        private static readonly TimeSpan translateDuration = TimeSpan.FromMilliseconds(367);

        private static readonly PropertyPath opacityPath = new(OpacityProperty);
        private static readonly PropertyPath visibilityPath = new(VisibilityProperty);
        private static readonly PropertyPath translateXPath = new("(UIElement.RenderTransform).(TranslateTransform.X)");
        private static readonly PropertyPath translateYPath = new("(UIElement.RenderTransform).(TranslateTransform.Y)");
        private static readonly KeySpline decelerateKeySplineOpening = new(0.1, 0.9, 0.2, 1);
        private static readonly KeySpline decelerateKeySplineClosing = new(1, 0.2, 0.9, 0.1);


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

            UpdateAnimations(ActualExpandDirection);
        }

        private void EnsureOpeningStoryboard()
        {
            if (openingStoryboard == null)
            {
                ObjectAnimationUsingKeyFrames visibilityAnim = new()
                {
                    KeyFrames =
                    {
                        new DiscreteObjectKeyFrame(Visibility.Visible, TimeSpan.Zero)
                    }
                    
                };
                Storyboard.SetTarget(visibilityAnim, this);
                Storyboard.SetTargetProperty(visibilityAnim, visibilityPath);
                

                DoubleAnimationUsingKeyFrames opacityAnim = new()
                {
                    KeyFrames =
                    {
                        new DiscreteDoubleKeyFrame(0, TimeSpan.Zero),
                        new DiscreteDoubleKeyFrame(0, TimeSpan.FromMilliseconds(83)),
                        new LinearDoubleKeyFrame(1, TimeSpan.FromMilliseconds(166))
                    }
                };
                Storyboard.SetTarget(opacityAnim, this);
                Storyboard.SetTargetProperty(opacityAnim, opacityPath);

                DoubleAnimationUsingKeyFrames xAnim = new()
                {
                    KeyFrames =
                    {
                        (fromHorizontalOffsetKeyFrameOpening = new DiscreteDoubleKeyFrame(0, TimeSpan.Zero)),
                        new SplineDoubleKeyFrame(0, translateDuration, decelerateKeySplineOpening)
                    }
                };
                Storyboard.SetTarget(xAnim, this);
                Storyboard.SetTargetProperty(xAnim, translateXPath);

                DoubleAnimationUsingKeyFrames yAnim = new()
                {
                    KeyFrames =
                    {
                        (fromVerticalOffsetKeyFrameOpening = new DiscreteDoubleKeyFrame(0, TimeSpan.Zero)),
                        new SplineDoubleKeyFrame(0, translateDuration, decelerateKeySplineOpening)
                    }
                };
                Storyboard.SetTarget(yAnim, this);
                Storyboard.SetTargetProperty(yAnim, translateYPath);

                openingStoryboard = new()
                {
                    Children = { visibilityAnim, opacityAnim, xAnim, yAnim },
                };
            }
        }

        private void EnsureClosingStoryboard()
        {
            if (closingStoryboard == null)
            {
                DoubleAnimationUsingKeyFrames opacityAnim = new()
                {
                    KeyFrames =
                    {
                        new DiscreteDoubleKeyFrame(1, TimeSpan.Zero),
                        new DiscreteDoubleKeyFrame(1, TimeSpan.FromMilliseconds(83)),
                        new LinearDoubleKeyFrame(0, TimeSpan.FromMilliseconds(166))
                    }
                };
                Storyboard.SetTarget(opacityAnim, this);
                Storyboard.SetTargetProperty(opacityAnim, opacityPath);

                DoubleAnimationUsingKeyFrames xAnim = new()
                {
                    KeyFrames =
                    {
                        new DiscreteDoubleKeyFrame(0, TimeSpan.Zero),
                        (fromHorizontalOffsetKeyFrameClosing = new SplineDoubleKeyFrame(
                            0, translateDuration, decelerateKeySplineClosing))
                    }
                };
                Storyboard.SetTarget(xAnim, this);
                Storyboard.SetTargetProperty(xAnim, translateXPath);

                DoubleAnimationUsingKeyFrames yAnim = new()
                {
                    KeyFrames =
                    {
                        new DiscreteDoubleKeyFrame(0, TimeSpan.Zero),
                        (fromVerticalOffsetKeyFrameClosing = new SplineDoubleKeyFrame(
                            0, translateDuration, decelerateKeySplineClosing))
                    }
                };
                Storyboard.SetTarget(yAnim, this);
                Storyboard.SetTargetProperty(yAnim, translateYPath);

                ObjectAnimationUsingKeyFrames visibilityAnim = new()
                {
                    KeyFrames =
                    {
                        new DiscreteObjectKeyFrame(Visibility.Hidden, TimeSpan.FromMilliseconds(250))
                    }
                };
                Storyboard.SetTarget(visibilityAnim, this);
                Storyboard.SetTargetProperty(visibilityAnim, visibilityPath);

                closingStoryboard = new()
                {
                    Children = { opacityAnim, xAnim, yAnim, visibilityAnim },
                };
            }
        }

        private void PlayOpenAnimation()
        {
            openingStoryboard.Begin(this, true);
        }

        private void PlayCloseAnimation()
        {
            closingStoryboard.Begin(this, true);
        }

        private void UpdateAnimations(FlyoutWindowExpandDirection expandDirection)
        {
            if (!hasAnimationsCreated)
                return;

            switch (expandDirection)
            {
                case FlyoutWindowExpandDirection.Auto:
                    fromHorizontalOffsetKeyFrameOpening.Value = 0;
                    fromHorizontalOffsetKeyFrameClosing.Value = 0;
                    fromVerticalOffsetKeyFrameOpening.Value = 0;
                    fromVerticalOffsetKeyFrameClosing.Value = 0;
                    break;

                case FlyoutWindowExpandDirection.Up:
                    fromHorizontalOffsetKeyFrameOpening.Value = 0;
                    fromHorizontalOffsetKeyFrameClosing.Value = 0;
                    fromVerticalOffsetKeyFrameOpening.Value = c_translation;
                    fromVerticalOffsetKeyFrameClosing.Value = c_translation;
                    break;

                case FlyoutWindowExpandDirection.Down:
                    fromHorizontalOffsetKeyFrameOpening.Value = 0;
                    fromHorizontalOffsetKeyFrameClosing.Value = 0;
                    fromVerticalOffsetKeyFrameOpening.Value = -c_translation;
                    fromVerticalOffsetKeyFrameClosing.Value = -c_translation;
                    break;

                case FlyoutWindowExpandDirection.Left:
                    fromHorizontalOffsetKeyFrameOpening.Value = c_translation;
                    fromHorizontalOffsetKeyFrameClosing.Value = c_translation;
                    fromVerticalOffsetKeyFrameOpening.Value = 0;
                    fromVerticalOffsetKeyFrameClosing.Value = 0;
                    break;

                case FlyoutWindowExpandDirection.Right:
                    fromHorizontalOffsetKeyFrameOpening.Value = -c_translation;
                    fromHorizontalOffsetKeyFrameClosing.Value = -c_translation;
                    fromVerticalOffsetKeyFrameOpening.Value = 0;
                    fromVerticalOffsetKeyFrameClosing.Value = 0;
                    break;
            }
        }

        #endregion
    }
}
