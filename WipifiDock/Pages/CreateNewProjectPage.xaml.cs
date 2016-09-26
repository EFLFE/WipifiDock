using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

            grid.Background = Brushes.Transparent;
            projectDirPath.Text = Environment.CurrentDirectory + "\\";

            projectDirPath.TextChanged += ProjectDirPath_TextChanged;
            projectName.TextChanged += ProjectName_TextChanged;

            Loaded += CreateNewProjectPage_Loaded;
        }

        private void CreateNewProjectPage_Loaded(object sender, RoutedEventArgs e)
        {
            reset();
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
            if (projectName.Text.Length == 0)
            {
                MessageBox.Show("Введите название проекта.", "Введите название проекта",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (projectDirPath.Text.Length < 3)
            {
                MessageBox.Show("Введите полный путь в каталогу проекта.", "Введите полный путь в каталогу проекта",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // correct //
            var ipc = Path.GetInvalidPathChars();
            var ifc = Path.GetInvalidFileNameChars();
            if (projectName.Text.IndexOfAny(ifc) != -1)
            {
                MessageBox.Show("Введите корректное имя проекта.", "Введите корректное имя проекта",
                       MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (projectDirPath.Text.IndexOfAny(ipc) != -1)
            {
                MessageBox.Show("Введите корректный путь к проекту.", "Введите корректный путь к проекту",
                          MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // create
            if (ProjectManager.CreateProfile(projectName.Text, projectDirPath.Text, ignoreExistsDirectory))
            {
                ProjectManager.SaveProjects();
                ownerMainWindow.projectListPage.AddProfile(projectName.Text);
                ownerMainWindow.FrameNavigate(MainWipifiWindow.PageType.ProjectListPage);
            }
        }

        // отмена
        private void projectCancel_Click(object sender, RoutedEventArgs e)
        {
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
            projectDirPath.Text = Environment.CurrentDirectory + "\\";
            onlock = false;
            ignoreExistsDirectory = false;
            projectDirPath.IsEnabled = true;
        }
    }
}
