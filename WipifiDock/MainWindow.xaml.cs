using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonMark;
using System.Windows.Media;
using System.Threading;

namespace WipifiDock
{
    public partial class MainWindow : Window
    {
        private WebBrowser webBrowser;
        private TextBox mdTextBox;
        private DataLabel dataLabel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void exitmenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            navigateFile("index");
        }

        private void navigateFile(string fileName)
        {
            webBrowser?.Navigate($"file:///{Environment.CurrentDirectory}/{fileName}.html");
        }

        private void convertMDtoHTML()
        {
            using (var reader = new StreamReader("source.md"))
            {
                using (var writer = new StreamWriter("result.html"))
                {
                    CommonMarkConverter.Convert(reader, writer);
                }
            }
        }

        private void urlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // navigate
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (webBrowser != null && webBrowser.CanGoBack)
            {
                webBrowser.GoBack();
            }
        }

        private void frontButton_Click(object sender, RoutedEventArgs e)
        {
            if (webBrowser != null && webBrowser.CanGoForward)
            {
                webBrowser.GoForward();
            }
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (webBrowser != null)
            {
                webBrowser.Refresh();
            }
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void toHtmlButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == tabControl.Items.Count - 1)
            {
                // add tab
                (tabControl.SelectedItem as TabItem).IsEnabled = false;

                var _tab = new TabItem();
                var _grid = new Grid();

                var _dataLabel = new Label();
                _dataLabel.Content = new DataLabel();

                _grid.Children.Add(new WebBrowser());
                _grid.Children.Add(new TextBox());
                _grid.Children.Add(_dataLabel);

                _tab.Header = new TextBlock()
                {
                    Text = "New item " + tabControl.Items.Count
                };

                _tab.Content = _grid;

                tabControl.Items.Insert(tabControl.Items.Count - 1, _tab);
                tabControl.SelectedIndex = tabControl.Items.Count - 2;

                ThreadPool.QueueUserWorkItem(new WaitCallback((_) =>
                {
                    // костыль, ибо я без понятия
                    // почему при первом добавлении таба
                    // он добавляет сразу два
                    Thread.Sleep(99);
                    Dispatcher.Invoke(() => { (tabControl.Items[tabControl.Items.Count - 1] as TabItem).IsEnabled = true; });
                }
                ));
                return;
            }

            // get
            var item = tabControl.SelectedItem as TabItem;
            var grid = item.Content as Grid;

            webBrowser = grid.Children[0] as WebBrowser;
            mdTextBox = grid.Children[1] as TextBox;
            dataLabel = grid.Children[2] as DataLabel;

            if (webBrowser == null)
            {
                throw new Exception("Not found WebBrowser and TextBox control in tab.");
            }
            if (mdTextBox == null)
            {
                throw new Exception("Not found TextBox control in tab.");
            }
            if (dataLabel == null)
            {
                throw new Exception("Not found DataLabel control in tab.");
            }

            // set
            urlTextBox.Text = dataLabel.Uri;
        }
    }
}
