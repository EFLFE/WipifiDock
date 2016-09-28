using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WipifiDock.Controls;
using WipifiDock.Controls.TreeViewData;
using WipifiDock.Data;
using System.Windows.Media;

namespace WipifiDock.Pages
{
    public sealed partial class ProjectPage : Page
    {
        private ProjectData projectData;
        private MDTabEditor selectedTab;
        private FileSystemWatcher treeWatcher;

        public ProjectPage()
        {
            InitializeComponent();
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
        }

        // on load (navigate to this page)
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            projectData = ProjectManager.GetSelectedProjectData();
            Title = "Проект - " + projectData.Name;
            FileManager.RootPath = projectData.Path;
            BlankGenerator.InitBlankGenerator();
            clear();
            loadTree();

            treeWatcher = new FileSystemWatcher(FileManager.RootPath)
            {
                IncludeSubdirectories = true,
                Filter = "*",
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess
            };
            treeWatcher.Created += TreeWatcher_Created;
            treeWatcher.Deleted += TreeWatcher_Deleted;
            treeWatcher.Renamed += TreeWatcher_Renamed;
            treeWatcher.EnableRaisingEvents = true;
        }

        #region Tree view watcher

        private void TreeWatcher_Renamed(object sender, RenamedEventArgs e)
        {
        }

        private void TreeWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                findTreePatern(e.FullPath.ToID());
            });
        }

        private void TreeWatcher_Created(object sender, FileSystemEventArgs e)
        {
        }

        #endregion

        private void findTreePatern(int id)
        {
            TreeViewItem node = null;

            for (int i = 0; i < treeView.Items.Count; i++)
            {
                var iti = treeView.Items[i] as ITreeViewData;
                if (iti.IsFolder)
                {
                    node = findSubTreeItem(iti, id);
                }
                else
                {
                    if (iti.ID == id)
                    {
                        node = treeView.Items[i] as TreeViewItem;
                        break;
                    }
                }
            }

            if (node == null)
                return;

            DependencyObject parent = VisualTreeHelper.GetParent(node);

            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            // delete
            if (parent is ITreeViewData)
            {
                (parent as ITreeViewData).Items.Remove(node);
            }
        }

        private TreeViewItem findSubTreeItem(ITreeViewData itree, int id)
        {
            for (int i = 0; i < itree.Items.Count; i++)
            {
                ITreeViewData isubtree = itree.Items[i] as ITreeViewData;
                if (isubtree == null)
                    continue;

                if (isubtree.IsFolder)
                {
                    return findSubTreeItem(isubtree, id);
                }
                else if (isubtree.ID == id)
                {
                    return itree.Items[i] as TreeViewItem;
                }
            }
            return null;
        }

        private void clear()
        {
            treeView.Items.Clear();
            tabControl.Items.Clear();
        }

        // load root files and dirs
        private void loadTree()
        {
            int i;
            string[] rootFiles = Directory.GetFiles(FileManager.RootPath, "*", SearchOption.TopDirectoryOnly);
            string[] rootDirs = Directory.GetDirectories(FileManager.RootPath, "*", SearchOption.TopDirectoryOnly);

            // root files
            for (i = 0; i < rootFiles.Length; i++)
            {
                var treeFile = new TreeViewDataFile(
                    Path.GetFileName(rootFiles[i]),
                    projectData.Path);

                treeView.Items.Add(treeFile);
            }
            // root dirs
            for (i = 0; i < rootDirs.Length; i++)
            {
                var treeFolder = new TreeViewDataFolder(
                    Path.GetFileName(rootDirs[i]),
                    projectData.Path);

                treeFolder.Expanded += TreeFolder_Expanded;
                treeView.Items.Add(treeFolder);
            }
        }

        // load expanded tree
        private void TreeFolder_Expanded(object sender, RoutedEventArgs e)
        {
            var item = sender as TreeViewDataFolder;
            if (item.Items.Count == 0 || (item.Items.Count == 1 && item.Items[0] == null))
            {
                item.Items.Clear();

                int i;
                string path = $"{item.Path}\\{item.FolderName}";
                string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
                string[] dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

                for (i = 0; i < files.Length; i++)
                {
                    var treeFile = new TreeViewDataFile(
                        Path.GetFileName(files[i]),
                        path);

                    item.Items.Add(treeFile);
                }
                for (i = 0; i < dirs.Length; i++)
                {
                    var treeFolder = new TreeViewDataFolder(
                        Path.GetFileName(dirs[i]),
                        path);

                    treeFolder.Expanded += TreeFolder_Expanded;
                    item.Items.Add(treeFolder);
                }
            }
        }

        // при выборе элемента в TreeView, открыть выбранный элемент
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem == null)
                return;
            // TODO: проверить наличия файла
            object item = treeView.SelectedItem;
            if (item != null)
            {
                if (item is TreeViewDataFile)
                {
                    var tt = item as TreeViewDataFile;
                    var path = tt.Path + "\\" + tt.FullFileName;

                    // проверить наличие открытой вкладки для файла
                    for (int i = 0; i < tabControl.Items.Count; i++)
                    {
                        var mdTab = ((tabControl.Items[i] as TabItem).Content as Grid).Children[0] as MDTabEditor;

                        if (path.Equals(mdTab.GetWorkFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            tabControl.SelectedIndex = i;
                            return;
                        }
                    }

                    // запрет на открытия файлов шаблонных в .md формате
                    if (FileManager.CheckFileIsConflict(tt.FullFileName, true))
                        return;

                    addWebItem();
                    selectedTab?.OpenFile(path, tt.GetProjectFileFormatType);
                }
            }
        }

        // Добавить tab в tabControl.
        private void addWebItem()
        {
            Log.Write("Add tab");

            var _tab = new TabItem();
            // header
            var sp = new StackPanel() { Orientation = Orientation.Horizontal };
            var headerText = new TextBlock() { Text = "blank" };
            var headerImage = new Image()
            {
                Source = FindResource("X") as BitmapImage,
                Margin = new Thickness(5.0, 0.0, 0.0, 0.0),
                Width = 12,
                Height = 12
            };
            headerImage.MouseUp += delegate (object s, MouseButtonEventArgs e)
            {
                // close tab
                Log.Write("Close tab");
                tabControl.Items.Remove(_tab); // вызывается tabControl_SelectionChanged
            };

            sp.Children.Add(headerText);
            sp.Children.Add(headerImage);

            _tab.Header = sp;

            // content
            var _grid = new Grid();
            var _mdtab = new MDTabEditor(_tab);

            _grid.Children.Add(_mdtab);
            _tab.Content = _grid;

            tabControl.Items.Add(_tab);
            tabControl.SelectedIndex = tabControl.Items.Count - 1;
        }

        // когда была выбрана вкладка
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Log.Write("Tab selected: " + tabControl.SelectedIndex);

            if (!IsLoaded || tabControl.SelectedIndex == -1)
                return;

            selectedTab = ((tabControl.SelectedItem as TabItem).Content as Grid).Children[0] as MDTabEditor;
        }

        #region TreeView ContextMenu

        private void treeCreateFile(object sender, RoutedEventArgs e)
        {
            var tf = treeView.SelectedItem;
            if (tf == null)
            {
                // root folder
            }
            else
            {
            }
        }

        private void treeCreateDir(object sender, RoutedEventArgs e)
        {
        }

        private void treeRenameNode(object sender, RoutedEventArgs e)
        {
        }

        private void treeDeleteNode(object sender, RoutedEventArgs e)
        {
        }

        #endregion
    }
}
