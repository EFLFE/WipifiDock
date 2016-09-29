using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WipifiDock.Data;

namespace WipifiDock.Pages
{
    /// <summary> Project list. </summary>
    public partial class ProjectListPage : Page
    {
        private MainWipifiWindow OwnerMainWindow;
        private ProjectData selectedProject;

        public ProjectListPage(MainWipifiWindow ownerMainWindow)
        {
            OwnerMainWindow = ownerMainWindow;
            Loaded += ProjectListPage_Loaded;

            InitializeComponent();
            grid.Background = Brushes.Transparent;
            projectListBox.SelectionChanged += ProjectListBox_SelectionChanged;

            // загрузить и заполнить список проектов
            var p = ProjectManager.LoadProjectConfig();
            if (p != null)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    projectListBox.Items.Add(p[i].Name);
                }
            }
        }

        private void ProjectListPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary> Добавить имя проекта в ListBox. </summary>
        /// <param name="name"> Имя проекта. </param>
        public void AddProfile(string name)
        {
            projectListBox.Items.Add(name);
        }

        // при выборе имени проекта, вывести о нём инфу
        private void ProjectListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = projectListBox.SelectedItem as string;
            if (name == null || name.Length == 0)
            {
                clearInfo();
                openProject.IsEnabled = false;
                deleteSelectedProject.IsEnabled = false;
                return;
            }
            try
            {
                selectedProject = ProjectManager.SelectAndLoadProject(name);

                if (!Directory.Exists(selectedProject.Path))
                {
                    var mes = MessageBox.Show(
                        "Каталог проекта \"" + selectedProject.Path + "\" не найден.\nУдалить \"" + selectedProject.Name + "\" из списка?",
                        "Ошибка",
                        MessageBoxButton.YesNo, MessageBoxImage.Error);

                    openProject.IsEnabled = false;
                    deleteSelectedProject.IsEnabled = false;

                    if (mes == MessageBoxResult.Yes)
                        deleteSelectedProject_Click(null, null);

                    return;
                }

                nameText.Text = selectedProject.Name;
                dirText.Text = selectedProject.Path;

                var di = new DirectoryInfo(selectedProject.Path);
                lastDateText.Text = di.LastWriteTime.ToString("dd.MM.yyyy - HH:mm:ss");

                openProject.IsEnabled = true;
                deleteSelectedProject.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString(), Log.MessageType.ERROR);
                MessageBox.Show(ex.ToString(), "Ошибка получения данных проекта", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void clearInfo()
        {
            nameText.Text = string.Empty;
            dirText.Text = string.Empty;
            lastDateText.Text = string.Empty;
        }

        // после создания проекта перейти на CreateNewProjectPage
        private void createProject_Click(object sender, RoutedEventArgs e)
        {
            OwnerMainWindow.FrameNavigate(MainWipifiWindow.PageType.CreateNewProjectPage);
        }

        // открыть проект
        private void openProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectManager.ProjectProfileWasSelected)
            {
                OwnerMainWindow.FrameNavigate(MainWipifiWindow.PageType.ProjectPage);
            }
        }

        private void deleteSelectedProject_Click(object sender, RoutedEventArgs e)
        {
            if (projectListBox.SelectedIndex != -1 && ProjectManager.RemoveProjectData(selectedProject.Name))
            {
                projectListBox.Items.RemoveAt(projectListBox.SelectedIndex);
            }
        }

        private void openExistProject_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description = "Выберите каталог с проектом"
            };
            var re = ofd.ShowDialog();
            if (re == System.Windows.Forms.DialogResult.OK)
            {
                var path = ofd.SelectedPath;
                var name = path.Remove(0, path.LastIndexOf('\\') + 1);

                OwnerMainWindow.createNewProjectPage.SetMainData(name, path);
                OwnerMainWindow.FrameNavigate(MainWipifiWindow.PageType.CreateNewProjectPage);
            }
        }
    }
}
