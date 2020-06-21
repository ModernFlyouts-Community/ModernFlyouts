using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ModernFlyouts.Navigation
{
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void MarkdownTextBlock_LinkClicked(object sender, BUSK.Markdown.Controls.LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri _))
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = e.Link,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        private void MarkdownTextBlock_ImageClicked(object sender, BUSK.Markdown.Controls.LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri _))
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = e.Link,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        private void MarkdownTextBlock_ImageResolving(object sender, BUSK.Markdown.Controls.ImageResolvingEventArgs e)
        {
            if (Uri.TryCreate(e.Url, UriKind.Absolute, out Uri result))
            {
                e.Image = new BitmapImage(result);
                e.Handled = true;
                return;
            }
            else
            {
                try
                {
                    e.Image = new BitmapImage(PackUriHelper.GetAbsoluteUri(e.Url));
                    e.Handled = true;
                    return;
                } catch { return; }
            }
        }
    }
}
