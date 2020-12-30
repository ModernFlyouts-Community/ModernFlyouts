using ModernFlyouts.Helpers;
using ModernFlyouts.UI;
using ModernFlyouts.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ModernFlyouts.Controls
{
    public partial class FlyoutTopBar : UserControl
    {
        private bool _topBarOverlay;
        private bool _topBarVisible = true;
        private TopBarVisibility _topBarVisibility = TopBarVisibility.Visible;

        #region Properties

        public static readonly DependencyProperty TopBarVisibilityProperty =
            DependencyProperty.Register(
                nameof(TopBarVisibility),
                typeof(TopBarVisibility),
                typeof(FlyoutTopBar),
                new PropertyMetadata(DefaultValuesStore.DefaultTopBarVisibility, OnTopBarVisibilityPropertyChanged));

        public TopBarVisibility TopBarVisibility
        {
            get => (TopBarVisibility)GetValue(TopBarVisibilityProperty);
            set => SetValue(TopBarVisibilityProperty, value);
        }

        private static void OnTopBarVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutTopBar topBar)
            {
                topBar.OnTopBarVisibilityChanged((TopBarVisibility)e.NewValue);
            }
        }

        #endregion

        public FlyoutTopBar()
        {
            InitializeComponent();

            PrepareAnimations();

            MouseEnter += (_, e) =>
            {
                if (_topBarVisibility == TopBarVisibility.AutoHide && !_topBarOverlay)
                {
                    _topBarOverlay = true;
                    UpdateTopBar(true);
                }
            };
            MouseLeave += (_, e) =>
            {
                if (_topBarVisibility == TopBarVisibility.AutoHide && _topBarOverlay)
                {
                    _topBarOverlay = false;
                    UpdateTopBar(false);
                }
            };

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) { return; }

            BindingOperations.SetBinding(this, TopBarVisibilityProperty,
                new Binding(nameof(UIManager.TopBarVisibility)) { Source = FlyoutHandler.Instance.UIManager, Mode = BindingMode.OneWay });
        }

        private void OnTopBarVisibilityChanged(TopBarVisibility value)
        {
            _topBarVisibility = value;
            _topBarVisible = value == TopBarVisibility.Visible;

            TopBarPinButtonIcon.Glyph = _topBarVisible ? CommonGlyphs.UnPin : CommonGlyphs.Pin;
            TopBarPinButton.ToolTip = _topBarVisible ? Properties.Strings.UnpinTopBar : Properties.Strings.PinTopBar;
            _topBarOverlay = false;
            var pos = Mouse.GetPosition(TopBarGrid);
            if (pos.Y > 0 && pos.Y < 32)
            {
                _topBarOverlay = true;
                _topBarVisible = true;
            }

            UpdateTopBar(_topBarVisible);
        }

        private static TimeSpan s_duration = TimeSpan.FromMilliseconds(167);
        private static IEasingFunction s_easingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
        private static PropertyPath s_heightPath = new(HeightProperty);
        private static PropertyPath s_marginPath = new(MarginProperty);
        private static PropertyPath s_opacityPath = new("Background.Opacity");

        private Storyboard m_expandStoryboard;
        private Storyboard m_collapseStoryboard;

        private void UpdateTopBar(bool showTopBar)
        {
            PrepareAnimations();

            DragBorder.Visibility = _topBarVisibility != TopBarVisibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

            if (showTopBar)
            {
                m_expandStoryboard?.Begin();
            }
            else
            {
                m_collapseStoryboard?.Begin();
            }
        }

        private void PrepareAnimations()
        {
            if (m_expandStoryboard == null)
            {
                DoubleAnimation heightAnim = new()
                {
                    To = 32.0,
                    Duration = s_duration,
                    EasingFunction = s_easingFunction
                };
                Storyboard.SetTarget(heightAnim, TopBarGrid);
                Storyboard.SetTargetProperty(heightAnim, s_heightPath);

                ThicknessAnimation tbMarginAnim = new()
                {
                    To = new(0),
                    Duration = s_duration,
                    EasingFunction = s_easingFunction
                };
                Storyboard.SetTarget(tbMarginAnim, TopBarGrid);
                Storyboard.SetTargetProperty(tbMarginAnim, s_marginPath);

                ThicknessAnimation g1MarginAnim = new()
                {
                    To = new(0),
                    Duration = s_duration,
                    EasingFunction = s_easingFunction
                };
                Storyboard.SetTarget(g1MarginAnim, G1);
                Storyboard.SetTargetProperty(g1MarginAnim, s_marginPath);

                ThicknessAnimation g2MarginAnim = new()
                {
                    To = new(0),
                    Duration = s_duration,
                    EasingFunction = s_easingFunction
                };
                Storyboard.SetTarget(g2MarginAnim, G2);
                Storyboard.SetTargetProperty(g2MarginAnim, s_marginPath);

                DoubleAnimation opacityAnim = new()
                {
                    To = 1.0,
                    Duration = s_duration,
                    EasingFunction = s_easingFunction
                };
                Storyboard.SetTarget(opacityAnim, DragBorder);
                Storyboard.SetTargetProperty(opacityAnim, s_opacityPath);

                m_expandStoryboard = new()
                {
                    Children = { heightAnim, tbMarginAnim, g1MarginAnim, g2MarginAnim, opacityAnim }
                };
            }
            if (m_collapseStoryboard == null)
            {
                DoubleAnimation heightAnim = new()
                {
                    To = 10.0,
                    Duration = s_duration,
                    EasingFunction = s_easingFunction
                };
                Storyboard.SetTarget(heightAnim, TopBarGrid);
                Storyboard.SetTargetProperty(heightAnim, s_heightPath);

                ThicknessAnimation tbMarginAnim = new()
                {
                    To = new(0, 0, 0, -10),
                    Duration = s_duration,
                    EasingFunction = s_easingFunction
                };
                Storyboard.SetTarget(tbMarginAnim, TopBarGrid);
                Storyboard.SetTargetProperty(tbMarginAnim, s_marginPath);

                ThicknessAnimation g1MarginAnim = new()
                {
                    To = new(-68, 0, 0, 0),
                    Duration = s_duration,
                    EasingFunction = s_easingFunction
                };
                Storyboard.SetTarget(g1MarginAnim, G1);
                Storyboard.SetTargetProperty(g1MarginAnim, s_marginPath);

                ThicknessAnimation g2MarginAnim = new()
                {
                    To = new(0, 0, -68, 0),
                    Duration = s_duration,
                    EasingFunction = s_easingFunction
                };
                Storyboard.SetTarget(g2MarginAnim, G2);
                Storyboard.SetTargetProperty(g2MarginAnim, s_marginPath);

                DoubleAnimation opacityAnim = new()
                {
                    To = 0.0,
                    Duration = s_duration,
                    EasingFunction = s_easingFunction
                };
                Storyboard.SetTarget(opacityAnim, DragBorder);
                Storyboard.SetTargetProperty(opacityAnim, s_opacityPath);

                m_collapseStoryboard = new()
                {
                    Children = { heightAnim, tbMarginAnim, g1MarginAnim, g2MarginAnim, opacityAnim }
                };
            }
        }
    }
}
