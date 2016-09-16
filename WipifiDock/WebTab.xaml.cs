using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using mshtml;

namespace WipifiDock
{
    public partial class WebTab : UserControl
    {
        public TabItem OwnerTab;

        public string Title { get; private set; }
        public bool EditMode { get; private set; }
        public bool Navigated { get; private set; }

        public delegate void DelOnNavigated(string title);
        public event DelOnNavigated OnNavigated;

        public WebTab(TabItem ownerTab)
        {
            InitializeComponent();
            OwnerTab = ownerTab;
            webBrowser.Navigated += WebBrowser_Navigated;
        }

        private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            Navigated = true;
            var doc = webBrowser.Document as HTMLDocument;
            var title = doc.title;

            Title = title;
            (OwnerTab.Header as TextBlock).Text = title;
            OnNavigated?.Invoke(title);
        }

        public void navigateFile(string fileName)
        {
            webBrowser.Navigate($"file:///{Environment.CurrentDirectory}/{fileName}");
        }

        public void GoBack()
        {
            if (webBrowser.CanGoBack)
                webBrowser.GoBack();
        }

        public void GoForward()
        {
            if (webBrowser.CanGoForward)
                webBrowser.GoForward();
        }

        public void Refresh()
        {
            webBrowser.Refresh();
        }

        public void EnableEditMode()
        {
            if (!Navigated)
                return;

            EditMode = true;
            webBrowser.Visibility = Visibility.Hidden;
            mdTextBox.Visibility = Visibility.Visible;
        }

        public void DisableEditMode()
        {
            if (!Navigated)
                return;

            EditMode = false;
            webBrowser.Visibility = Visibility.Visible;
            mdTextBox.Visibility = Visibility.Hidden;
        }

    }
}
