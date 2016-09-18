using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonMark;

namespace WipifiDock
{
    /// <summary> Окно самого проекта. </summary>
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

        // при выборе элемента в TreeView, открыть выбранный элемент
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
                    selectedWebTab.NavigateToFile(path, true);
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
                    selectedWebTab.NavigateToFile(path, true);
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
            Title += projectData.Name;
            //selectedWebTab.navigateFile("MainPage.html");
            loadTree();
        }

        private void loadTree()
        {
            int i;
            string[] rootFiles = Directory.GetFiles(projectData.Path,"*.*", SearchOption.TopDirectoryOnly);
            string[] rootDirs = Directory.GetDirectories(projectData.Path, "*", SearchOption.TopDirectoryOnly);

            // root files
            for (i = 0; i < rootFiles.Length; i++)
            {
                var treeFile = new TreeViewData.TreeViewDataFile(
                    Path.GetFileName(rootFiles[i]),
                    projectData.Path);

                treeView.Items.Add(treeFile);
            }
            // root dirs
            for (i = 0; i < rootDirs.Length; i++)
            {
                var treeFolder = new TreeViewData.TreeViewDataFolder(
                    Path.GetFileName(rootDirs[i]),
                    projectData.Path);

                treeFolder.Expanded += TreeFolder_Expanded;

                treeView.Items.Add(treeFolder);
            }
        }

        // load expanded tree
        private void TreeFolder_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewData.TreeViewDataFolder)sender;
            if (item.Items.Count == 0 || (item.Items.Count == 1 && item.Items[0] == null))
            {
                item.Items.Clear();
                // == ~COPY~ ==
                int i;
                string path = $"{item.Path}\\{item.FolderName}";
                string[] files = Directory.GetFiles(path,"*.*", SearchOption.TopDirectoryOnly);
                string[] dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

                for (i = 0; i < files.Length; i++)
                {
                    var treeFile = new TreeViewData.TreeViewDataFile(
                        Path.GetFileName(files[i]),
                        path);

                    item.Items.Add(treeFile);
                }
                for (i = 0; i < dirs.Length; i++)
                {
                    var treeFolder = new TreeViewData.TreeViewDataFolder(
                        Path.GetFileName(dirs[i]),
                        path);

                    treeFolder.Expanded += TreeFolder_Expanded;

                    item.Items.Add(treeFolder);
                }
            }
        }

        // collapse/expand treeView
        private void collapse_Click(object sender, RoutedEventArgs e)
        {
            setTreeViewExpand(treeView.Items, false);
            if (safeRecursion != -2) safeRecursion = 0;
        }

        private void expand_Click(object sender, RoutedEventArgs e)
        {
            setTreeViewExpand(treeView.Items, true);
            if (safeRecursion != -2) safeRecursion = 0;
        }

        // по теории, этого быть не должно, но я всё же перестрахуюсь
        // (-1 стоп, -2 игнорировать)
        private static int safeRecursion;

        private void setTreeViewExpand(ItemCollection treeViewItems, bool isExpanded)
        {
            if (safeRecursion != -2)
            {
                if (safeRecursion == -1)
                {
                    return;
                }
                if (safeRecursion++ > 399)
                {
                    var kek = MessageBox.Show(
                        "Обнаружен предел рекурсии, или слишком много каталогов для открытия. Игнорировать данное сообщение и продолжить?",
                        "Предел рекурсии/каталогов",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    safeRecursion = kek == MessageBoxResult.Yes ? -2 : -1;
                    if (kek == MessageBoxResult.No)
                        return;
                }
            }

            foreach (var item in treeViewItems)
            {
                var _folder = item as TreeViewData.TreeViewDataFolder;
                if (_folder != null)
                {
                    if (isExpanded)
                    {
                        // IsExpanded не меняется в ручную в форме
                        //if (!_folder.IsExpanded)
                        {
                            _folder.IsExpanded = true;
                            if (!_folder.IsEmpty)
                            {
                                // рекурсия
                                setTreeViewExpand(_folder.Items, true);
                            }
                        }
                    }
                    else //if (_folder.IsExpanded)
                    {
                        _folder.IsExpanded = false;
                        if (!_folder.IsEmpty)
                        {
                            // рекурсия
                            setTreeViewExpand(_folder.Items, false);
                        }
                    }
                }
            }
            safeRecursion--;
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
                selectedWebTab.NavigateToFile(urlTextBox.Text, false);
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

#warning TODO: addFolder_Click
        private void addFolder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Не реализован.");
        }

#warning TODO: addDock_Click
        private void addDock_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Не реализован.");
        }

#warning TODO: deleteDock_Click
        private void deleteDock_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Не реализован.");
        }

#warning TODO: deleteFolder_Click
        private void deleteFolder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Не реализован.");
        }

#warning TODO: saveProject_Click
        private void saveProject_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Не реализован.");
        }

        // open project dir
        private void openProjectFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(projectData.Path);
        }
    }
}
