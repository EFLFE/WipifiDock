﻿using System;
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
    /// <summary> Элемент с браузером и текстовый редактором для открытия md файлов. </summary>
    public sealed partial class MDTabEditor : UserControl
    {
        /// <summary> Ссылка на вкладки. </summary>
        public TabItem OwnerTab; // (не проще ли сюда перенести этот элемент?)

        private string workFileName;

        private object _lock_ = new object();
        private bool poolEnable;
        private int timeToUpdate = -1;

        public string GetWorkFileName
        {
            get { return workFileName; }
        }

        /// <summary> Заголовок страницы. </summary>
        public string Title { get; private set; }

        /// <summary> Страница загружена. </summary>
        public bool Navigated { get; private set; }

        public delegate void DelOnNavigated(string title, object sender, NavigationEventArgs e);

        /// <summary> Событие после навигации. </summary>
        public event DelOnNavigated OnNavigated;

        public MDTabEditor(TabItem ownerTab)
        {
            InitializeComponent();
            grid.IsEnabled = false;
            menu.IsEnabled = false;
            OwnerTab = ownerTab;

            webBrowser.Navigated += WebBrowser_Navigated;
            textBox.TextChanged += TextBox_TextChanged;

            ThreadPool.QueueUserWorkItem(pool, null);
        }

        // задаёт таймер обновления страницы после изменения текста (только .md)
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
            //setTitle(title);
            OnNavigated?.Invoke(title, sender, e);

            Navigated = true;
        }

        private void setTitle(string title)
        {
            ((OwnerTab.Header as StackPanel).Children[0] as TextBlock).Text = title;
        }

        // пул поток для автообновления страницы
        private void pool(object _)
        {
            while (true)
            {
                Thread.Sleep(100);
                if (poolEnable)
                {
                    lock (_lock_)
                    {
                        if (timeToUpdate > 0 && Navigated)
                        {
                            if (--timeToUpdate == 0)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    File.WriteAllText(workFileName, textBox.Text);
                                    navigateToHtml();
                                });
                            }
                        }
                    }
                }
            }
        }

        // конвертировать md в html и открыть
        private void navigateToHtml()
        {
            var html = Markdig.Markdown.ToHtml(textBox.Text);
            html = BlankGenerator.HTML(Title, null, new[] { html });

            string htmlPath = workFileName.Replace(".md", ".html");
            File.WriteAllText(htmlPath, html, Encoding.UTF8);
            Thread.Sleep(0);

            webBrowser.Navigate(new Uri(htmlPath));
        }

        /// <summary> Открыть файл проекта. </summary>
        /// <param name="file"> Имя файла. Путь строится от корневого. </param>
        /// <param name="projectFileFormatType"> Тип файла. </param>
        public void OpenFile(string file, FileManager.FileFormatType fileType)
        {
            setTitle(Path.GetFileName(file));

            switch (fileType)
            {
            case FileManager.FileFormatType.MD:
                openMD(file);
                break;

            case FileManager.FileFormatType.HTML:
            case FileManager.FileFormatType.CSS:
            case FileManager.FileFormatType.TXT:
                openText(file);
                break;

            case FileManager.FileFormatType.IMAGE:
            case FileManager.FileFormatType.Unknown:
            default:
                break;
            }
        }

        private void openMD(string file)
        {
            showWebBrowser();
            workFileName = file;
            textBox.Text = File.ReadAllText(file);
            menu.IsEnabled = true;
            timeToUpdate = -1;

            navigateToHtml();

            poolEnable = true;
            grid.IsEnabled = true;
            mdMenuItem.Visibility = Visibility.Visible;
        }

        private void openText(string file)
        {
            hideWebBrowser();
            workFileName = file;
            textBox.Text = File.ReadAllText(file);
            grid.IsEnabled = true;
            mdMenuItem.Visibility = Visibility.Hidden;
        }

        /// <summary> Закрыть файл. </summary>
        public void Close()
        {
            grid.IsEnabled = false;
            lock (_lock_)
            {
                poolEnable = false;
                timeToUpdate = -1;
            }
            webBrowser.Navigate("");
            textBox.Text = string.Empty;
            workFileName = string.Empty;
            menu.IsEnabled = false;
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

        private void showWebBrowser()
        {
            gridColumnDefinition.Width = new GridLength(1.0, GridUnitType.Star);
            //gridColumnDefinition.Width = new GridLength(contentGrid.ActualWidth / 2.0, GridUnitType.Pixel);
            webBrowser.Visibility = Visibility.Visible;
            gridSplitter.Visibility = Visibility.Visible;
        }

        private void hideWebBrowser()
        {
            gridColumnDefinition.Width = new GridLength(0.0, GridUnitType.Pixel);
            webBrowser.Visibility = Visibility.Hidden;
            gridSplitter.Visibility = Visibility.Hidden;
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
            InsertTextForm.Instance.SelectTab(0);
            InsertTextForm.Instance.ShowDialog();
            insertMD(InsertTextForm.GetInsertText);
        }

        private void mdInsertUrl(object sender, RoutedEventArgs e)
        {
            InsertTextForm.Instance.SelectTab(1);
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
