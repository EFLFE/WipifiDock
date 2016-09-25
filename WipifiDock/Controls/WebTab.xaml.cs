using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using mshtml;
using WipifiDock.Data;
using WipifiDock.Forms;

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

        private string workFileName;
        private string workPath;

        private bool poolEnable;
        private int timeToUpdate = 10;

        public delegate void DelOnNavigated(string title, object sender, NavigationEventArgs e);

        /// <summary> Событие после навигации. </summary>
        public event DelOnNavigated OnNavigated;

        public WebTab(TabItem ownerTab)
        {
            InitializeComponent();
            grid.IsEnabled = false;
            OwnerTab = ownerTab;

            webBrowser.Navigated += WebBrowser_Navigated;
            textBox.TextChanged += TextBox_TextChanged;

            ThreadPool.QueueUserWorkItem(poolToHtml, null);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // start time to update
            timeToUpdate = 05;
        }

        // после успешной навигации
        private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            var doc = webBrowser.Document as HTMLDocument;
            var title = doc.title;

            Title = title;
            setTitle(title);
            OnNavigated?.Invoke(title, sender, e);

            Navigated = true;
        }

        private void setTitle(string title)
        {
            ((OwnerTab.Header as StackPanel).Children[0] as TextBlock).Text = title;
        }

        private void poolToHtml(object _)
        {
            while (true)
            {
                Thread.Sleep(100);
                if (poolEnable && timeToUpdate > 0 && Navigated)
                {
                    if (--timeToUpdate == 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            navigateToHtml();
                        });
                    }
                }
            }
        }

        private void navigateToHtml()
        {
            var html = Markdig.Markdown.ToHtml(textBox.Text);
            html = BlankGenerator.HTML(Title, null, new[] { html });

            string htmlPath = workFileName.Replace(FileManager.WEB_EXT, ".html");

            File.WriteAllText(htmlPath, html, Encoding.UTF8);
            webBrowser.Navigate(new Uri(htmlPath));
        }

        /// <summary> Открыть файл проекта. </summary>
        /// <param name="file"> Имя файла. Путь строится от корневого. </param>
        /// <param name="projectFileFormatType"> Тип файла. </param>
        public void OpenFile(string file, FileManager.ProjectFileFormatType projectFileFormatType)
        {
            workFileName = file;

            switch (projectFileFormatType)
            {
            case FileManager.ProjectFileFormatType.Web:
                textBox.Text = FileManager.GetTextFromMagnetProjectFile(file, ".md");

                //Title = Path.GetFileNameWithoutExtension(file);
                //(OwnerTab.Header as TextBlock).Text = Title;

                navigateToHtml();
                poolEnable = true;
                grid.IsEnabled = true;
                break;

            case FileManager.ProjectFileFormatType.Style:
            case FileManager.ProjectFileFormatType.Content:
            case FileManager.ProjectFileFormatType.Unknown:
            default:
                poolEnable = false;
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

        private void insertMD(string text, int offset = 0)
        {
            var newCaret = textBox.CaretIndex + text.Length + offset;
            textBox.Text = textBox.Text.Insert(textBox.CaretIndex, text);
            textBox.CaretIndex = newCaret;
        }

        #region MD INSERT MENU
        private void mdInsertH1(object sender, RoutedEventArgs e)
        {
            insertMD("# ");
        }

        private void mdInsertH2(object sender, RoutedEventArgs e)
        {
            insertMD("## ");
        }

        private void mdInsertH3(object sender, RoutedEventArgs e)
        {
            insertMD("### ");
        }

        private void mdInsertH4(object sender, RoutedEventArgs e)
        {
            insertMD("#### ");
        }

        private void mdInsertH5(object sender, RoutedEventArgs e)
        {
            insertMD("##### ");
        }

        private void mdInsertH6(object sender, RoutedEventArgs e)
        {
            insertMD("###### ");
        }

        private void mdInsertItalic1(object sender, RoutedEventArgs e)
        {
            insertMD("**", -1);
        }

        private void mdInsertItalic2(object sender, RoutedEventArgs e)
        {
            insertMD("__", -1);
        }

        private void mdInsertBold1(object sender, RoutedEventArgs e)
        {
            insertMD("****", -2);
        }

        private void mdInsertBold2(object sender, RoutedEventArgs e)
        {
            insertMD("____", -2);
        }

        private void mdInsertList1(object sender, RoutedEventArgs e)
        {
            insertMD("* ");
        }

        private void mdInsertList2(object sender, RoutedEventArgs e)
        {
            insertMD("1. ");
        }

        private void mdInsertImage(object sender, RoutedEventArgs e)
        {
            InsertTextForm.Instance.ShowDialog();
            insertMD(InsertTextForm.GetInsertText);
        }

        private void mdInsertUrl(object sender, RoutedEventArgs e)
        {
            InsertTextForm.Instance.ShowDialog();
            insertMD(InsertTextForm.GetInsertText);
        }

        private void mdInsertQuote(object sender, RoutedEventArgs e)
        {
            insertMD("> ");
        }

        private void mdInsertCode(object sender, RoutedEventArgs e)
        {
            insertMD("``", -1);
        }

        private void mdInsertCodeBlock(object sender, RoutedEventArgs e)
        {
            insertMD("``````", -3);
        }
        #endregion
    }
}
