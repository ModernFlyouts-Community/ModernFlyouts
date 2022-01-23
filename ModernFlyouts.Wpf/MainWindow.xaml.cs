using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ModernFlyouts.Wpf
{
    public partial class MainWindow : Window
    {
        /* TO DO:
         * REMOVE
        */
        private NotifyIcon _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
  }
}