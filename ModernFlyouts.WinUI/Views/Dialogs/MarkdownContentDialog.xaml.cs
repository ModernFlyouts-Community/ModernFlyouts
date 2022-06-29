#nullable enable

using System;
using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.Views
{
    public sealed partial class MarkdownContentDialog : UserControl
    {
        public MarkdownContentDialog(string markdown)
        {
            InitializeComponent();

            MarkdownTextBlock.Text = markdown;
        }

        private async void MarkdownTextBlock_LinkClicked(object sender, CommunityToolkit.WinUI.UI.Controls.LinkClickedEventArgs e)
        {
            try
            {
                string? uriToLaunch = e.Link;
                var uri = new Uri(uriToLaunch);

                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
            catch { }
        }
    }
}
