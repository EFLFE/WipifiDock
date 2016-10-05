using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Navigation;
using WipifiDock.Forms;

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

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // такое возможно в XAML?
            // возможно, но тут проще, чем в xaml
            ////line1.X2 = WindowWidth - 30.0;
            ////line2.X2 = WindowWidth - 30.0;
        }

        private void buttonGotoPage1_Click(object sender, RoutedEventArgs e)
        {
            ownerMainWindow.FrameNavigate(MainWipifiWindow.PageType.ProjectListPage);
        }

        private void buttonCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            new UpdateForm().ShowDialog();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            ownerMainWindow.Close();
        }

        private void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutForm().ShowDialog();
        }
    }
}
