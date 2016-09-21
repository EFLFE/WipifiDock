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

            projectName.Background = Brushes.Transparent;
            projectDesc.Background = Brushes.Transparent;
            projectPathAndAuthor.Background = Brushes.Transparent;
        }

        private void ProjectListPage_Loaded(object sender, RoutedEventArgs e)
        {
            projectListBox.SelectionChanged += ProjectListBox_SelectionChanged;

            // загрузить и заполнить список проектов
            var p = ProjectDataManager.LoadProjectConfig();
            if (p != null)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    projectListBox.Items.Add(p[i].Name);
                }
            }
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
                projectName.Text = "Проект: ";
                projectDesc.Text = "Описание:\n";
                projectPathAndAuthor.Text = "Автор:\nКаталог:";
                openProject.IsEnabled = false;
                deleteSelectedProject.IsEnabled = false;
                return;
            }
            selectedProject = ProjectDataManager.GetProjectData(name);

            projectName.Text = "Проект: " + selectedProject.Name;
            projectDesc.Text = "Описание:\n" + selectedProject.Desc;
            projectPathAndAuthor.Text = $"Автор: {selectedProject.Author}\nКаталог: {selectedProject.Path}";

            openProject.IsEnabled = true;
            deleteSelectedProject.IsEnabled = true;
        }

        // после создания проекта перейти на CreateNewProjectPage
        private void createProject_Click(object sender, RoutedEventArgs e)
        {
            OwnerMainWindow.FrameNavigate(MainWipifiWindow.PageType.CreateNewProjectPage);
        }

        // открыть проект
        private void openProject_Click(object sender, RoutedEventArgs e)
        {
            var pi = projectListBox.SelectedItem as string;
            if (pi != null && ProjectDataManager.SelectProjectName(pi))
            {
                OwnerMainWindow.FrameNavigate(MainWipifiWindow.PageType.ProjectPage);
            }            
        }

        private void deleteSelectedProject_Click(object sender, RoutedEventArgs e)
        {
            if (projectListBox.SelectedIndex != -1 && ProjectDataManager.RemoveProjectData(selectedProject.Name))
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
