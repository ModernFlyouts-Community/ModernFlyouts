using ModernFlyouts.Core.UI;
using System.Windows;
using System.Windows.Controls;

namespace ModernFlyouts.Controls
{
    public partial class FlyoutAlignmentPicker : UserControl
    {
        #region Properties

        public static readonly DependencyProperty AlignmentProperty =
            DependencyProperty.Register(
                nameof(Alignment),
                typeof(FlyoutWindowAlignments),
                typeof(FlyoutAlignmentPicker),
                new FrameworkPropertyMetadata(FlyoutWindowAlignments.Top | FlyoutWindowAlignments.Left,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnAlignmentPropertyChanged));

        public FlyoutWindowAlignments Alignment
        {
            get => (FlyoutWindowAlignments)GetValue(AlignmentProperty);
            set => SetValue(AlignmentProperty, value);
        }

        private static void OnAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutAlignmentPicker flyoutAlignmentPicker)
            {
                flyoutAlignmentPicker.UpdateControls((FlyoutWindowAlignments)e.NewValue);
            }
        }

        #endregion

        public FlyoutAlignmentPicker()
        {
            InitializeComponent();
            Loaded += FlyoutAlignmentPicker_Loaded;
            Unloaded += FlyoutAlignmentPicker_Unloaded;

            CmbHorizontal.ItemsSource = new FlyoutWindowAlignments[]
            {
                FlyoutWindowAlignments.Left,
                FlyoutWindowAlignments.Center,
                FlyoutWindowAlignments.Right
            };
            CmbVertical.ItemsSource = new FlyoutWindowAlignments[]
            {
                FlyoutWindowAlignments.Top,
                FlyoutWindowAlignments.Center,
                FlyoutWindowAlignments.Bottom
            };

            UpdateControls(Alignment);
        }

        private void FlyoutAlignmentPicker_Loaded(object sender, RoutedEventArgs e)
        {
            TgCenter.Checked += Tg_Checked;
            TgLeft.Checked += Tg_Checked;
            TgRight.Checked += Tg_Checked;
            TgTop.Checked += Tg_Checked;
            TgBottom.Checked += Tg_Checked;

            TgCenter.Unchecked += Tg_Unchecked;
            TgLeft.Unchecked += Tg_Unchecked;
            TgRight.Unchecked += Tg_Unchecked;
            TgTop.Unchecked += Tg_Unchecked;
            TgBottom.Unchecked += Tg_Unchecked;

            CmbHorizontal.SelectionChanged += Cmb_SelectionChanged;
            CmbVertical.SelectionChanged += Cmb_SelectionChanged;
        }

        private void FlyoutAlignmentPicker_Unloaded(object sender, RoutedEventArgs e)
        {
            TgCenter.Checked -= Tg_Checked;
            TgLeft.Checked -= Tg_Checked;
            TgRight.Checked -= Tg_Checked;
            TgTop.Checked -= Tg_Checked;
            TgBottom.Checked -= Tg_Checked;

            TgCenter.Unchecked -= Tg_Unchecked;
            TgLeft.Unchecked -= Tg_Unchecked;
            TgRight.Unchecked -= Tg_Unchecked;
            TgTop.Unchecked -= Tg_Unchecked;
            TgBottom.Unchecked -= Tg_Unchecked;

            CmbHorizontal.SelectionChanged -= Cmb_SelectionChanged;
            CmbVertical.SelectionChanged -= Cmb_SelectionChanged;
        }

        private void Tg_Checked(object sender, RoutedEventArgs e)
        {
            if (_isUpdating)
                return;

            var alignment = Alignment;

            if (sender == TgCenter)
            {
                alignment = FlyoutWindowAlignments.Center;
            }
            else if (sender == TgLeft)
            {
                alignment &= ~FlyoutWindowAlignments.Right;
                alignment |= FlyoutWindowAlignments.Left;
            }
            else if (sender == TgRight)
            {
                alignment &= ~FlyoutWindowAlignments.Left;
                alignment |= FlyoutWindowAlignments.Right;
            }
            else if (sender == TgTop)
            {
                alignment &= ~FlyoutWindowAlignments.Bottom;
                alignment |= FlyoutWindowAlignments.Top;
            }
            else if (sender == TgBottom)
            {
                alignment &= ~FlyoutWindowAlignments.Top;
                alignment |= FlyoutWindowAlignments.Bottom;
            }

            Alignment = alignment;
        }

        private void Tg_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isUpdating)
                return;

            var alignment = Alignment;

            if (sender == TgLeft)
            {
                alignment &= ~FlyoutWindowAlignments.Left;
            }
            else if (sender == TgRight)
            {
                alignment &= ~FlyoutWindowAlignments.Right;
            }
            else if (sender == TgTop)
            {
                alignment &= ~FlyoutWindowAlignments.Top;
            }
            else if (sender == TgBottom)
            {
                alignment &= ~FlyoutWindowAlignments.Bottom;
            }

            Alignment = alignment;
        }

        private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdating)
                return;

            var alignment = Alignment;

            if (sender == CmbHorizontal)
            {
                alignment &= ~FlyoutWindowAlignments.Left;
                alignment &= ~FlyoutWindowAlignments.Right;

                if (CmbHorizontal.SelectedItem is FlyoutWindowAlignments alignments)
                {
                    alignment |= alignments;
                }
            }
            else if (sender == CmbVertical)
            {
                alignment &= ~FlyoutWindowAlignments.Top;
                alignment &= ~FlyoutWindowAlignments.Bottom;

                if (CmbVertical.SelectedItem is FlyoutWindowAlignments alignments)
                {
                    alignment |= alignments;
                }
            }

            Alignment = alignment;
        }

        private bool _isUpdating;

        private void UpdateControls(FlyoutWindowAlignments alignment)
        {
            _isUpdating = true;

            TgLeft.IsChecked = alignment.HasFlag(FlyoutWindowAlignments.Left);
            TgRight.IsChecked = alignment.HasFlag(FlyoutWindowAlignments.Right);
            TgTop.IsChecked = alignment.HasFlag(FlyoutWindowAlignments.Top);
            TgBottom.IsChecked = alignment.HasFlag(FlyoutWindowAlignments.Bottom);

            TgCenter.IsChecked = alignment == FlyoutWindowAlignments.Center;

            CmbHorizontal.SelectedItem = GetHorizontalAlignment(alignment);
            CmbVertical.SelectedItem = GetVerticalAlignment(alignment);

            _isUpdating = false;

            TbAlignment.Text = alignment.ToString();
        }

        private FlyoutWindowAlignments GetHorizontalAlignment(FlyoutWindowAlignments alignment)
        {
            if (alignment.HasFlag(FlyoutWindowAlignments.Left))
            {
                return FlyoutWindowAlignments.Left;
            }
            else if (alignment.HasFlag(FlyoutWindowAlignments.Right))
            {
                return FlyoutWindowAlignments.Right;
            }
            else
            {
                return FlyoutWindowAlignments.Center;
            }
        }

        private FlyoutWindowAlignments GetVerticalAlignment(FlyoutWindowAlignments alignment)
        {
            if (alignment.HasFlag(FlyoutWindowAlignments.Top))
            {
                return FlyoutWindowAlignments.Top;
            }
            else if (alignment.HasFlag(FlyoutWindowAlignments.Bottom))
            {
                return FlyoutWindowAlignments.Bottom;
            }
            else
            {
                return FlyoutWindowAlignments.Center;
            }
        }
    }
}
