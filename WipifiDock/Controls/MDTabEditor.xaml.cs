using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WipifiDock.Data;
using WipifiDock.Forms;

namespace WipifiDock.Controls
{
    /// <summary> Элемент с браузером и текстовый редактором для открытия md файлов. </summary>
    public sealed partial class MDTabEditor : UserControl
    {
        /// <summary> Ссылка на вкладки. </summary>
        public TabItem OwnerTab; // (не проще ли сюда перенести этот элемент?)

        private string workFileName, workFileFullPath;
        private bool textWasChanged;

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

        /*
        public delegate void DelOnNavigated(string title, object sender, NavigationEventArgs e);

        /// <summary> Событие после навигации. </summary>
        public event DelOnNavigated OnNavigated;
        */

        // ctor //
        public MDTabEditor(TabItem ownerTab)
        {
            InitializeComponent();
            grid.IsEnabled = false;
            styleMenu.IsEnabled = false;
            OwnerTab = ownerTab;

            webBrowser.Loaded += delegate { Log.Write("Chromium loaded", Log.MessageType.NOTE); };

            //!webBrowser.Navigated += WebBrowser_Navigated;
            webBrowser.FrameLoadEnd += WebBrowser_FrameLoadEnd;
            textBox.TextChanged += TextBox_TextChanged;

            // set-up text editor
            textBox.Margins[0].Width = 32; // line numbers
            textBox.AssignCmdKey(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S, ScintillaNET.Command.Null);
            textBox.KeyDown += textBox_KeyDown;

            ThreadPool.QueueUserWorkItem(pool, null);
        }

        private void setUpLexerStyles(ScintillaNET.Lexer lerex)
        {
            // По умолчанию нет никакой подцветки (или я что то не так делаю)
            if (lerex == ScintillaNET.Lexer.Html)
            {
                // HTML
                textBox.Lexer = lerex;
                textBox.Styles[ScintillaNET.Style.Html.Default].Font = "Consolas";
                textBox.Styles[ScintillaNET.Style.Html.Tag].ForeColor = System.Drawing.Color.Purple;
                textBox.Styles[ScintillaNET.Style.Html.TagUnknown].ForeColor = System.Drawing.Color.FromArgb(-4194304);
                textBox.Styles[ScintillaNET.Style.Html.Attribute].ForeColor = System.Drawing.Color.Olive;
                textBox.Styles[ScintillaNET.Style.Html.AttributeUnknown].ForeColor = System.Drawing.Color.FromArgb(-4194304);
                textBox.Styles[ScintillaNET.Style.Html.Number].ForeColor = System.Drawing.Color.Teal;
                textBox.Styles[ScintillaNET.Style.Html.DoubleString].ForeColor = System.Drawing.Color.FromArgb(-4194304);
                textBox.Styles[ScintillaNET.Style.Html.SingleString].ForeColor = System.Drawing.Color.Red;
                textBox.Styles[ScintillaNET.Style.Html.Other].ForeColor = System.Drawing.Color.FromArgb(-12566464);
                textBox.Styles[ScintillaNET.Style.Html.Comment].ForeColor = System.Drawing.Color.Green;
                textBox.Styles[ScintillaNET.Style.Html.Entity].ForeColor = System.Drawing.Color.Navy;
                textBox.Styles[ScintillaNET.Style.Html.Script].ForeColor = System.Drawing.Color.FromArgb(-4177920);
                textBox.Styles[ScintillaNET.Style.Html.CData].ForeColor = System.Drawing.Color.RosyBrown;
                textBox.Styles[ScintillaNET.Style.Html.Question].ForeColor = System.Drawing.Color.FromArgb(-16728064);
                textBox.Styles[ScintillaNET.Style.Html.Value].ForeColor = System.Drawing.Color.Purple;
                textBox.Styles[ScintillaNET.Style.Html.XcComment].ForeColor = System.Drawing.Color.Green;
            }
            else if (lerex == ScintillaNET.Lexer.Markdown)
            {
                // MarkDown
                textBox.Lexer = lerex;
                textBox.Styles[ScintillaNET.Style.Markdown.Default].Font = "Consolas";
                textBox.Styles[ScintillaNET.Style.Markdown.Strong1].Bold = true;
                textBox.Styles[ScintillaNET.Style.Markdown.Strong1].Weight = 700;
                textBox.Styles[ScintillaNET.Style.Markdown.Strong2].Bold = true;
                textBox.Styles[ScintillaNET.Style.Markdown.Strong2].Weight = 700;
                textBox.Styles[ScintillaNET.Style.Markdown.Em1].Italic = true;
                textBox.Styles[ScintillaNET.Style.Markdown.Em2].Italic = true;
                textBox.Styles[ScintillaNET.Style.Markdown.Header1].Bold = true;
                textBox.Styles[ScintillaNET.Style.Markdown.Header1].Weight = 700;
                textBox.Styles[ScintillaNET.Style.Markdown.Header2].Bold = true;
                textBox.Styles[ScintillaNET.Style.Markdown.Header2].Weight = 700;
                textBox.Styles[ScintillaNET.Style.Markdown.Header3].Bold = true;
                textBox.Styles[ScintillaNET.Style.Markdown.Header3].Weight = 700;
                textBox.Styles[ScintillaNET.Style.Markdown.Header4].Bold = true;
                textBox.Styles[ScintillaNET.Style.Markdown.Header4].Weight = 700;
                textBox.Styles[ScintillaNET.Style.Markdown.Header5].Bold = true;
                textBox.Styles[ScintillaNET.Style.Markdown.Header5].Weight = 700;
                textBox.Styles[ScintillaNET.Style.Markdown.Header6].Bold = true;
                textBox.Styles[ScintillaNET.Style.Markdown.Header6].Weight = 700;
                textBox.Styles[ScintillaNET.Style.Markdown.PreChar].ForeColor = System.Drawing.Color.Purple;
                textBox.Styles[ScintillaNET.Style.Markdown.OListItem].Bold = true;
                textBox.Styles[ScintillaNET.Style.Markdown.OListItem].Weight = 700;
                textBox.Styles[ScintillaNET.Style.Markdown.BlockQuote].BackColor = System.Drawing.Color.FromArgb(-986896);
                textBox.Styles[ScintillaNET.Style.Markdown.Strikeout].ForeColor = System.Drawing.Color.Maroon;
                textBox.Styles[ScintillaNET.Style.Markdown.HRule].Bold = true;
                textBox.Styles[ScintillaNET.Style.Markdown.HRule].Weight = 700;
                textBox.Styles[ScintillaNET.Style.Markdown.Link].ForeColor = System.Drawing.Color.Navy;
                textBox.Styles[ScintillaNET.Style.Markdown.Code].BackColor = System.Drawing.Color.WhiteSmoke;
                textBox.Styles[ScintillaNET.Style.Markdown.Code].ForeColor = System.Drawing.Color.FromArgb(-4177920);
                textBox.Styles[ScintillaNET.Style.Markdown.Code2].ForeColor = System.Drawing.Color.Maroon;
                textBox.Styles[ScintillaNET.Style.Markdown.CodeBk].ForeColor = System.Drawing.Color.Olive;
            }
            else if (lerex == ScintillaNET.Lexer.Css)
            {
                // CSS
                textBox.Lexer = lerex;
                textBox.Styles[ScintillaNET.Style.Css.Default].Font = "Consolas";
                textBox.Styles[ScintillaNET.Style.Css.Tag].Bold = true;
                textBox.Styles[ScintillaNET.Style.Css.Tag].ForeColor = System.Drawing.Color.FromArgb(-8372224);
                textBox.Styles[ScintillaNET.Style.Css.Tag].Weight = 700;
                textBox.Styles[ScintillaNET.Style.Css.Class].Bold = true;
                textBox.Styles[ScintillaNET.Style.Css.Class].ForeColor = System.Drawing.Color.FromArgb(-8372160);
                textBox.Styles[ScintillaNET.Style.Css.Class].Weight = 700;
                textBox.Styles[ScintillaNET.Style.Css.PseudoClass].ForeColor = System.Drawing.Color.FromArgb(-4177920);
                textBox.Styles[ScintillaNET.Style.Css.UnknownPseudoClass].ForeColor = System.Drawing.Color.FromArgb(-4194304);
                textBox.Styles[ScintillaNET.Style.Css.Operator].ForeColor = System.Drawing.Color.DarkBlue;
                textBox.Styles[ScintillaNET.Style.Css.Identifier].ForeColor = System.Drawing.Color.FromArgb(-12566528);
                textBox.Styles[ScintillaNET.Style.Css.UnknownIdentifier].ForeColor = System.Drawing.Color.FromArgb(-12566528);
                textBox.Styles[ScintillaNET.Style.Css.Value].ForeColor = System.Drawing.Color.Maroon;
                textBox.Styles[ScintillaNET.Style.Css.Comment].ForeColor = System.Drawing.Color.Green;
                textBox.Styles[ScintillaNET.Style.Css.Comment].Italic = true;
                textBox.Styles[ScintillaNET.Style.Css.Id].ForeColor = System.Drawing.Color.FromArgb(-16760768);
                textBox.Styles[ScintillaNET.Style.Css.Important].ForeColor = System.Drawing.Color.Red;
                textBox.Styles[ScintillaNET.Style.Css.Directive].ForeColor = System.Drawing.Color.Blue;
                textBox.Styles[ScintillaNET.Style.Css.DoubleString].ForeColor = System.Drawing.Color.Maroon;
                textBox.Styles[ScintillaNET.Style.Css.SingleString].ForeColor = System.Drawing.Color.FromArgb(-4194304);
                textBox.Styles[ScintillaNET.Style.Css.Identifier2].ForeColor = System.Drawing.Color.FromArgb(-16777024);
                textBox.Styles[ScintillaNET.Style.Css.Attribute].ForeColor = System.Drawing.Color.Teal;
                textBox.Styles[ScintillaNET.Style.Css.Identifier3].ForeColor = System.Drawing.Color.Navy;
                textBox.Styles[ScintillaNET.Style.Css.ExtendedIdentifier].ForeColor = System.Drawing.Color.Olive;
                textBox.Styles[ScintillaNET.Style.Css.ExtendedPseudoClass].ForeColor = System.Drawing.Color.FromArgb(-8372160);
                textBox.Styles[ScintillaNET.Style.Css.ExtendedPseudoElement].ForeColor = System.Drawing.Color.FromArgb(-12582848);
                textBox.Styles[ScintillaNET.Style.Css.Media].ForeColor = System.Drawing.Color.Purple;
                textBox.Styles[ScintillaNET.Style.Css.Variable].Bold = true;
                textBox.Styles[ScintillaNET.Style.Css.Variable].Weight = 700;
            }
        }

        // после успешной навигации
        private void WebBrowser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            Navigated = true;
        }

        // задаёт таймер обновления страницы после изменения текста (только .md)
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            // start time to update
            timeToUpdate = 05;
            if (!textWasChanged && grid.IsEnabled)
            {
                setTitle(Title + "*");
                textWasChanged = true;
            }
        }

        /*
        private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            var doc = webBrowser.Document as HTMLDocument;
            var title = doc.title;

            //Title = title;
            //setTitle(title);
            OnNavigated?.Invoke(title, sender, e);

            Navigated = true;
        }
        */

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
            //var html = Markdig.Markdown.ToHtml(textBox.Text);
            var html = BlankGenerator.MD(Title, textBox.Text, BlankGenerator.GetPathLevel(workFileFullPath));

            string htmlPath = workFileName.Replace(".md", ".html");
            File.WriteAllText(htmlPath, html, Encoding.UTF8);
            Thread.Sleep(0);

            //webBrowser.Navigate(new Uri(htmlPath));
            webBrowser.Address = htmlPath;
        }

        /// <summary> Открыть файл проекта. </summary>
        /// <param name="file"> Имя файла. Путь строится от корневого. </param>
        /// <param name="projectFileFormatType"> Тип файла. </param>
        public void OpenFile(string file, string fileFullPath, FileManager.FileFormatType fileType)
        {
            workFileName = file;
            workFileFullPath = fileFullPath;
            Title = Path.GetFileName(file);
            setTitle(Path.GetFileName(file));

            switch (fileType)
            {
            case FileManager.FileFormatType.MD:
                setUpLexerStyles(ScintillaNET.Lexer.Markdown);
                openMD(file);
                break;

            case FileManager.FileFormatType.HTML:
                setUpLexerStyles(ScintillaNET.Lexer.Html);
                openText(file);
                break;

            case FileManager.FileFormatType.CSS:
                setUpLexerStyles(ScintillaNET.Lexer.Css);
                openText(file);
                break;

            case FileManager.FileFormatType.TXT:
                openText(file);
                break;

            case FileManager.FileFormatType.IMAGE:
                openImage(file);
                break;

            case FileManager.FileFormatType.Unknown:
            default:
                break;
            }
        }

        private void openMD(string file)
        {
            showTextBoxAndWebBrowser();
            textBox.Text = File.ReadAllText(file);
            styleMenu.IsEnabled = true;
            fileMenu.IsEnabled = true;
            timeToUpdate = -1;

            navigateToHtml();

            poolEnable = true;
            grid.IsEnabled = true;
            mdMenuItem.Visibility = Visibility.Visible;
        }

        private void openText(string file)
        {
            showOnlyTextBox();
            textBox.Text = File.ReadAllText(file);
            grid.IsEnabled = true;
            mdMenuItem.Visibility = Visibility.Hidden;
            fileMenu.IsEnabled = true;
        }

        [System.Diagnostics.Conditional("DEBUG")] // todo: разве CefSharp может загрузить прямой html код?
        private void openImage(string file)
        {
            showOnlyWebBrowser();
            grid.IsEnabled = true;
            mdMenuItem.Visibility = Visibility.Hidden;
            webBrowser.Address = (BlankGenerator.IMAGE(file));
        }

        /// <summary> Закрыть файл. </summary>
        public void Close()
        {
            gridColumnDefinitionLeft.Width = new GridLength(1.0, GridUnitType.Star);
            gridColumnDefinition.Width = new GridLength(1.0, GridUnitType.Star);

            grid.IsEnabled = false;
            lock (_lock_)
            {
                poolEnable = false;
                timeToUpdate = -1;
            }
            //webBrowser.NavigateToString(string.Empty);
            webBrowser.Address = "about:blank";
            textBox.Text = string.Empty;
            workFileName = string.Empty;
            styleMenu.IsEnabled = false;
            fileMenu.IsEnabled = false;
        }

        /// <summary> Обновить страницу. </summary>
        public void Refresh()
        {
            //webBrowser.Refresh(true);
            webBrowser.Address = webBrowser.Address;
        }

        private void insertMD(string text, int offset = 0)
        {
            //var newCaret = textBox.CaretIndex + text.Length + offset;
            //textBox.Text = textBox.Text.Insert(textBox.CaretIndex, text);
            //textBox.CaretIndex = newCaret;
            textBox.InsertText(textBox.CurrentPosition + offset, text);
        }

        private void showTextBoxAndWebBrowser()
        {
            gridColumnDefinition.Width = new GridLength(1.0, GridUnitType.Star);
            //gridColumnDefinition.Width = new GridLength(contentGrid.ActualWidth / 2.0, GridUnitType.Pixel);
            webBrowser.Visibility = Visibility.Visible;
            gridSplitter.Visibility = Visibility.Visible;
            textBox.Visible = true;
        }

        private void showOnlyTextBox()
        {
            gridColumnDefinition.Width = new GridLength(0.0, GridUnitType.Pixel);
            webBrowser.Visibility = Visibility.Hidden;
            gridSplitter.Visibility = Visibility.Hidden;
            textBox.Visible = true;
        }

        private void showOnlyWebBrowser()
        {
            gridColumnDefinitionLeft.Width = new GridLength(0.0, GridUnitType.Star);
            webBrowser.Visibility = Visibility.Visible;
            gridSplitter.Visibility = Visibility.Hidden;
            textBox.Visible = false;
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
            InsertMDTextForm.Instance.SelectTab(0);
            InsertMDTextForm.Instance.ShowDialog();
            insertMD(InsertMDTextForm.GetInsertText);
        }

        private void mdInsertUrl(object sender, RoutedEventArgs e)
        {
            InsertMDTextForm.Instance.SelectTab(1);
            InsertMDTextForm.Instance.ShowDialog();
            insertMD(InsertMDTextForm.GetInsertText);
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

        // from Menu->Save
        private void saveText_Click(object sender, RoutedEventArgs e)
        {
            saveText();
        }

        // custom hotkeys
        private void textBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                saveText();
            }
        }

        private void saveText()
        {
            if (textWasChanged)
            {
                File.WriteAllText(workFileName, textBox.Text);
                setTitle(Title);
                textWasChanged = false;
            }
        }
    }
}
