using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WipifiDock.Controls;
using WipifiDock.Controls.TreeViewData;
using WipifiDock.Data;

namespace WipifiDock.Pages
{
    public sealed partial class ProjectPage : Page
    {
        private ProjectData projectData;
        private MDTabEditor selectedTab;

        public ProjectPage()
        {
            InitializeComponent();
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
            treeView.MouseDoubleClick += TreeView_MouseDoubleClick;
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
        }

        private void clear()
        {
            treeView.Items.Clear();
        }

        // load root files and dirs
        private void loadTree()
        {
            int i;
            string[] rootFiles = FileManager.GetProjectFiles("");
            string[] rootDirs = FileManager.GetProjectDirs("");

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
                string path = "\\" + item.FolderName;
                string[] files = FileManager.GetProjectFiles(path);
                string[] dirs = FileManager.GetProjectDirs(path);

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

        // двойной клик по TreeView (что бы выбрать выделенный элемент)
        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeView_SelectedItemChanged(sender, null);
        }

        // при выборе элемента в TreeView, открыть выбранный элемент
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem == null)
                return;

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
                    if (FileManager.CheckFileIsConflict(tt.FullFileName))
                        return;

                    addWebItem();
                    selectedTab?.OpenFile(path, tt.GetProjectFileFormatType);
                }
            }
        }

        /// <summary> Добавить tab в tabControl. </summary>
        /// <param name="suspedAddTabButton"> Заморозить кнопку + таба на короткое время. </param>
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

        // когда выбрана вкладка
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Log.Write("Tab selected: " + tabControl.SelectedIndex);

            if (!IsLoaded || tabControl.SelectedIndex == -1)
                return;

            selectedTab = ((tabControl.SelectedItem as TabItem).Content as Grid).Children[0] as MDTabEditor;
        }

        #region Click Events

        private void configProjectButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void openProjectFolderButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void projectToWebButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void renderButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void mdButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void htmlButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void addDockButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void addFolderButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void deleteDockButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void deleteFolderButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void collapseButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void expandButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void renameButton_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion

    }
}
