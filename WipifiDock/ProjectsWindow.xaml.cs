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
    public partial class ProjectsWindow : Window
    {
        private WebTab selectedWebTab;

        public ProjectsWindow()
        {
            InitializeComponent();
        }

        private void exitmenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            addWebItem();
            selectedWebTab.navigateFile("MainPage.html");
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
                selectedWebTab.navigateFile(urlTextBox.Text);
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            selectedWebTab?.GoBack();
        }

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            selectedWebTab?.GoForward();
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            selectedWebTab?.Refresh();
            updateEditButtons();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            selectedWebTab?.EnableEditMode();
            updateEditButtons();
        }

        private void toHtmlButton_Click(object sender, RoutedEventArgs e)
        {
            selectedWebTab?.DisableEditMode();
            updateEditButtons();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            if (tabControl.SelectedIndex == tabControl.Items.Count - 1)
            {
                addWebItem();
                return;
            }

            selectedWebTab = ((tabControl.SelectedItem as TabItem).Content as Grid).Children[0] as WebTab;
            updateEditButtons();
        }

        private void addWebItem()
        {
            (tabControl.SelectedItem as TabItem).IsEnabled = false;

            var _tab = new TabItem();
            var _grid = new Grid();
            var _webTab = new WebTab(_tab);
            _webTab.OnNavigated += _webTab_OnNavigated;

            _grid.Children.Add(_webTab);
            _tab.Content = _grid;

            _tab.Header = new TextBlock()
            {
                Text = "blank"
            };

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
        }

        private void _webTab_OnNavigated(string title)
        {
            updateEditButtons();
        }

        private void updateEditButtons()
        {
            if (selectedWebTab == null || !selectedWebTab.Navigated)
            {
                editButton.IsEnabled = false;
                toHtmlButton.IsEnabled = false;
            }
            else
            {
                if (selectedWebTab.EditMode)
                {
                    editButton.IsEnabled = false;
                    toHtmlButton.IsEnabled = true;
                }
                else
                {
                    editButton.IsEnabled = true;
                    toHtmlButton.IsEnabled = false;
                }
            }
        }

    }
}
