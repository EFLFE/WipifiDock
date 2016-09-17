using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonMark;
using System.Windows.Media;
using System.Threading;
using System.Collections.Generic;

namespace WipifiDock
{
    public partial class ProjectsWindow : Window
    {
        private ProjectData projectData;
        private WebTab selectedWebTab;

        public ProjectsWindow()
        {
            InitializeComponent();
            treeView.MouseDoubleClick += TreeView_MouseDoubleClick;
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            object item = treeView.SelectedItem;
            if (item != null)
            {
                if (item is TreeViewData.TreeViewDataFile)
                {
                    var tt = item as TreeViewData.TreeViewDataFile;
                    var path = tt.Path + "\\" + tt.Name;

                    if (tabControl.Items.Count == 1)
                    {
                        addWebItem();
                    }
                    selectedWebTab.navigateFile(path, true);
                }
            }
        }

        // open new tab and load selected tree item (middle?)
        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            object item = treeView.SelectedItem;
            if (item != null)
            {
                if (item is TreeViewData.TreeViewDataFile)
                {
                    var tt = item as TreeViewData.TreeViewDataFile;
                    var path = tt.Path + "\\" + tt.Name;

                    addWebItem();
                    selectedWebTab.navigateFile(path, true);
                }
            }
        }

        private void exitmenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            projectData = ProjectDataManager.GetSelectedProjectData();
            //selectedWebTab.navigateFile("MainPage.html");

            // load tree
            int i, j;
            string[] rootFiles = Directory.GetFiles(projectData.Path,"*.*", SearchOption.TopDirectoryOnly);
            string[] dirs = Directory.GetDirectories(projectData.Path, "*", SearchOption.AllDirectories);

            // root files
            for (i = 0; i < rootFiles.Length; i++)
            {
                treeView.Items.Add(
                    new TreeViewData.TreeViewDataFile(Path.GetFileName(rootFiles[i]),
                        Environment.CurrentDirectory));
            }

            for (i = 0; i < dirs.Length; i++)
            {
                var indir = new TreeViewData.TreeViewDataFolder(dirs[i].Remove(0, projectData.Path.Length + 1));
                var dif = Directory.GetFiles(dirs[i], "*.*", SearchOption.TopDirectoryOnly);

                for (j = 0; j < dif.Length; j++)
                {
                    indir.Members.Add(
                        new TreeViewData.TreeViewDataFile(Path.GetFileName(dif[j]),
                            dirs[i]));
                }

                treeView.Items.Add(indir);
            }
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
                selectedWebTab.navigateFile(urlTextBox.Text, false);
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
            (tabControl.Items[tabControl.Items.Count - 1] as TabItem).IsEnabled = false;

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
