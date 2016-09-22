using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WipifiDock.Controls;
using WipifiDock.Controls.TreeViewData;
using WipifiDock.Data;

namespace WipifiDock.Pages
{
    public sealed partial class ProjectPage : Page
    {
        private ProjectData projectData;
        private WebTab selectedWebTab;

        public ProjectPage()
        {
            InitializeComponent();
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
        }

        // on load (navigate to this page)
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            projectData = ProjectDataManager.GetSelectedProjectData();
            Title = "Проект - " + projectData.Name;
            FileManager.RootPath = projectData.Path;

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

        // select item in TreeView
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        // когда выбрана вкладка
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
