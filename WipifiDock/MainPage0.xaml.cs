using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WipifiDock
{
    public partial class MainPage0 : Page
    {
        /// <summary> Событие для перехода на MainPage1. </summary>
        public event EventHandler GotoPage1;

        /// <summary> Событие по нажатии кнопки выхода. </summary>
        public event EventHandler OnExit;

        public MainPage0()
        {
            InitializeComponent();
            Background = Brushes.Transparent; // в редакторе фон тёмный и нeфига не видно текст
            vText.Text += App.VERSION;
        }

        // click on link
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri); // абсолютли
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // такое возможно в XAML?
            line1.X2 = WindowWidth - 30.0;
            line2.X2 = WindowWidth - 30.0;
        }

        private void buttonGotoPage1_Click(object sender, RoutedEventArgs e)
        {
            GotoPage1(sender, e);
        }

        private void buttonCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            OnExit(sender, e);
        }

    }
}
