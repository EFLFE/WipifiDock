using System;
using System.Windows;
using System.Windows.Controls;
using WipifiDock.Data;

namespace WipifiDock.Pages
{
    /// <summary> Create new project. </summary>
    public partial class CreateNewProjectPage : Page
    {
        private MainWipifiWindow ownerMainWindow;
        
        // костыли
        private bool autoAddNameToPath = true;
        private bool onlock;
        private bool ignoreExistsDirectory;

        public CreateNewProjectPage(MainWipifiWindow _ownerMainWindow)
        {
            ownerMainWindow = _ownerMainWindow;
            InitializeComponent();
            projectDirPath.Text = Environment.CurrentDirectory + "\\";

            projectDirPath.TextChanged += ProjectDirPath_TextChanged;
            projectName.TextChanged += ProjectName_TextChanged;
        }

        // автозаполнение к директории проекта
        private void ProjectDirPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            autoAddNameToPath = onlock;
        }

        private void ProjectName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (autoAddNameToPath)
            {
                onlock = true;
                projectDirPath.Text = Environment.CurrentDirectory + "\\" + projectName.Text;
                onlock = false;
            }
        }

        // создать проект
        private void projectCreate_Click(object sender, RoutedEventArgs e)
        {
            if (projectName.Text.Length > 0 && projectDirPath.Text.Length > 3)
            {
                if (ProjectDataManager.CreateProfile(
                    projectName.Text, projectDirPath.Text, projectDesc.Text, projectAuthor.Text, ignoreExistsDirectory))
                {
                    ownerMainWindow.projectListPage.AddProfile(projectName.Text);
                    reset();
                    ownerMainWindow.FrameNavigate(MainWipifiWindow.PageType.ProjectListPage);
                }
            }
        }

        // отмена
        private void projectCancel_Click(object sender, RoutedEventArgs e)
        {
            reset();
            ownerMainWindow.FrameNavigate(MainWipifiWindow.PageType.ProjectListPage);
        }

        /// <summary> Задать начальные значения. </summary>
        /// <param name="name"> Имени. </param>
        /// <param name="path"> Каталога. </param>
        public void SetMainData(string name, string path)
        {
            projectName.Text = name;
            projectDirPath.Text = path;
            ignoreExistsDirectory = true;
            projectDirPath.IsEnabled = false;
        }

        private void reset()
        {
            projectName.Text = string.Empty;
            projectDesc.Text = string.Empty;
            projectDirPath.Text = Environment.CurrentDirectory + "\\";
            projectAuthor.Text = string.Empty;
            onlock = false;
            ignoreExistsDirectory = false;
            projectDirPath.IsEnabled = true;
        }
    }
}
