using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WipifiDock.Pages
{
    public partial class MainPage : Page
    {
        private MainWipifiWindow ownerMainWindow;
        
        public MainPage(MainWipifiWindow _ownerMainWindow)
        {
            ownerMainWindow = _ownerMainWindow;
            InitializeComponent();
            Background = Brushes.Transparent; // в редакторе фон тёмный и нeфига не видно текст
            vText.Text += App.Version;
        }

        // click on link
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri); // абсолютли
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // такое возможно в XAML?
            // возможно, но тут проще, чем в xaml
            line1.X2 = WindowWidth - 30.0;
            line2.X2 = WindowWidth - 30.0;
        }

        private void buttonGotoPage1_Click(object sender, RoutedEventArgs e)
        {
            ownerMainWindow.FrameNavigate(MainWipifiWindow.PageType.ProjectListPage);
        }

        private async void buttonCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            await Update.Check();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            ownerMainWindow.Close();
        }

    }
}
