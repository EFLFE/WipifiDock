using System;
using System.IO;
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

        /// <summary> Путь к файлу, к которобу был последний переход. </summary>
        public string LastNavigatedPath { get; private set; }

        /// <summary> Формат, к которобу был последний переход. </summary>
        public BlankGenerator.FileFormatType LastFileFormatType { get; private set; }

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

        public void NavigateToFile(string filePath)
        {
            var ff = BlankGenerator.DetectFileFormatType(filePath);
            LastFileFormatType = ff;
            LastNavigatedPath = filePath;

            switch (ff)
            {
            case BlankGenerator.FileFormatType.None:
                webBrowser.NavigateToString(BlankGenerator.FileNotFouund(filePath));
                break;

            case BlankGenerator.FileFormatType.Unknown:
                webBrowser.NavigateToString(BlankGenerator.UnknownFileInfo(filePath));
                break;

            case BlankGenerator.FileFormatType.Text:
                webBrowser.NavigateToString(BlankGenerator.Text(filePath));
                break;

            case BlankGenerator.FileFormatType.MarkDown:
                //webBrowser.Navigate($"file:///{Environment.CurrentDirectory}/{filePath}");
                webBrowser.Navigate(BlankGenerator.Text(filePath));
                break;

            case BlankGenerator.FileFormatType.HTML:
                webBrowser.Navigate(filePath);
                break;

            case BlankGenerator.FileFormatType.Image:
                webBrowser.NavigateToString(BlankGenerator.ViewImage(filePath));
                break;

            //case BlankGenerator.FileFormatType.Project:
            //    break;

            default:
                webBrowser.NavigateToString(BlankGenerator.Error("Ошибка", "NavigateToFile: Формат файла " + ff + " не перечислен."));
                break;
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

        /// <summary> Включить режим редактирования и получить md код страницы. </summary>
        public void EnableEditMode()
        {
            try
            {
                if (LastFileFormatType == BlankGenerator.FileFormatType.HTML)
                {
                    // get MarkDown text
                    var mdFile = LastNavigatedPath.Replace(".html", ".md");
                    if (File.Exists(mdFile))
                    {
                        mdTextBox.Text = File.ReadAllText(mdFile);
                    }
                    else
                    {
                        // convert HTML to MD
                        //var htmlToMD = ??????????
                        //File.WriteAllText(mdFile, htmlToMD);
                        //mdTextBox.Text = htmlToMD;
                    }
                }

                EditMode = true;
                webBrowser.Visibility = Visibility.Hidden;
                mdTextBox.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                webBrowser.Visibility = Visibility.Visible;
                mdTextBox.Visibility = Visibility.Hidden;

                webBrowser.NavigateToString(BlankGenerator.Error(
                    "Ошибка",
                    $"Ошибка при переходе в режим редактирования!<br/>{ex.Message}<br/>{ex.ToString()}"));
            }
        }

        /// <summary> Выключить режим редактирования и получить сконвертированный html код. </summary>
        public void DisableEditMode()
        {
            try
            {
                if (LastFileFormatType == BlankGenerator.FileFormatType.HTML)
                {
                    var mdFile = LastNavigatedPath.Replace(".html", ".md");

                    // save md text
                    File.WriteAllText(mdFile, mdTextBox.Text);

                    // convert MarkDown to HTML
                    var htmlText = CommonMark.CommonMarkConverter.Convert(mdTextBox.Text);

                    // save html
                    File.WriteAllText(LastNavigatedPath, htmlText);
                }

                EditMode = false;
                webBrowser.Visibility = Visibility.Visible;
                mdTextBox.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                webBrowser.Visibility = Visibility.Visible;
                mdTextBox.Visibility = Visibility.Hidden;

                webBrowser.NavigateToString(BlankGenerator.Error(
                    "Ошибка",
                    $"Ошибка при переходе в режим просмотра HTML страницы!<br/>{ex.Message}<br/>{ex.ToString()}"));
            }
        }

    }
}
