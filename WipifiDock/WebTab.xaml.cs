using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using mshtml;

namespace WipifiDock
{
    /// <summary> Форма с браузером и текстовый редактором. </summary>
    public partial class WebTab : UserControl
    {
        /// <summary> Ссылка на вкладки. </summary>
        public TabItem OwnerTab; // (не проще ли сюда перенести этот элемент?)

        /// <summary> Заголовок страницы. </summary>
        public string Title { get; private set; }

        /// <summary> Получить статус режима редактирования. </summary>
        public bool EditMode { get; private set; }

        /// <summary> Страница загружена. </summary>
        public bool Navigated { get; private set; }
        
        public delegate void DelOnNavigated(string title);

        /// <summary> Событие после навигации. </summary>
        public event DelOnNavigated OnNavigated;

        public WebTab(TabItem ownerTab)
        {
            InitializeComponent();
            OwnerTab = ownerTab;
            webBrowser.Navigated += WebBrowser_Navigated;
        }

        // после успешной навигации
        private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            Navigated = true;
            var doc = webBrowser.Document as HTMLDocument;
            var title = doc.title;

            Title = title;
            (OwnerTab.Header as TextBlock).Text = title;
            OnNavigated?.Invoke(title);
        }

        public void NavigateToFile(string filePath, bool isImage)
        {
            if (isImage)
            {
                webBrowser.NavigateToString(BlankGenerator.ViewImage(filePath));
            }
            else
            {
                webBrowser.Navigate($"file:///{Environment.CurrentDirectory}/{filePath}");
            }
        }

        /// <summary> Назад в прошлое. </summary>
        public void GoBack()
        {
            if (webBrowser.CanGoBack)
                webBrowser.GoBack();
        }

        /// <summary> Назад в будущее. </summary>
        public void GoForward()
        {
            if (webBrowser.CanGoForward)
                webBrowser.GoForward();
        }

        /// <summary> Обновить страницу. </summary>
        public void Refresh()
        {
            webBrowser.Refresh(true);
        }

        /// <summary> Включить режим редактирования. </summary>
        public void EnableEditMode()
        {
            if (!Navigated)
                return;

            EditMode = true;
            webBrowser.Visibility = Visibility.Hidden;
            mdTextBox.Visibility = Visibility.Visible;
        }

        /// <summary> Выключить режим редактирования. </summary>
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
