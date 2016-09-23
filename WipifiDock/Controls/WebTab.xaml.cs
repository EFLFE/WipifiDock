using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using mshtml;
using WipifiDock.Data;

namespace WipifiDock.Controls
{
    /// <summary> Элемент с браузером и текстовый редактором. </summary>
    public sealed partial class WebTab : UserControl
    {
        /// <summary> Ссылка на вкладки. </summary>
        public TabItem OwnerTab; // (не проще ли сюда перенести этот элемент?)

        /// <summary> Заголовок страницы. </summary>
        public string Title { get; private set; }

        /// <summary> Страница загружена. </summary>
        public bool Navigated { get; private set; }

        public delegate void DelOnNavigated(string title, object sender, NavigationEventArgs e);

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
            OnNavigated?.Invoke(title, sender, e);
        }

        /// <summary> Открыть файл проекта. </summary>
        /// <param name="file"> Имя файла. Путь строится от корневого. </param>
        /// <param name="projectFileFormatType"> Тип файла. </param>
        public void OpenFile(string file, FileManager.ProjectFileFormatType projectFileFormatType)
        {
            switch (projectFileFormatType)
            {
            case FileManager.ProjectFileFormatType.Web:
                string html = FileManager.GetTextFromMagnetProjectFile(file, ".html");

                if (html.Length > 0)
                {
                    webBrowser.NavigateToString(html);
                }
                else
                {
                    Title = Path.GetFileNameWithoutExtension(file);
                    (OwnerTab.Header as TextBlock).Text = Title;
                }

                textBox.Text = FileManager.GetTextFromMagnetProjectFile(file, ".md");
                break;

            case FileManager.ProjectFileFormatType.Style:
            case FileManager.ProjectFileFormatType.Content:
            case FileManager.ProjectFileFormatType.Unknown:
            default:
                webBrowser.NavigateToString(
                    BlankGenerator.Error("Ошибка", "Формат файла \"" + Path.GetFileName(file) + "\" не поддерживается."));
                break;
            }
        }

        /// <summary> Обновить страницу. </summary>
        public void Refresh()
        {
            webBrowser.Refresh(true);
        }

    }
}
