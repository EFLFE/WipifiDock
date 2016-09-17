﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace WipifiDock
{
    /// <summary> Create new project. </summary>
    public partial class MainPage2 : Page
    {
        public delegate void DelOnCreateNewProject(string name);

        /// <summary> Событие для перехода на MainPage1. </summary>
        public event EventHandler NavigateToPage1;

        /// <summary> Событие создания нового проекта. </summary>
        public event DelOnCreateNewProject OnCreateNewProject;

        // костыли
        private bool autoAddNameToPath = true;
        private bool onlock;

        public MainPage2()
        {
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
                if (ProjectDataManager.CreateProfile(projectName.Text, projectDirPath.Text, projectDesc.Text, projectAuthor.Text))
                {
                    OnCreateNewProject.Invoke(projectName.Text);
                    NavigateToPage1?.Invoke(sender, e);
                }
            }
        }

        // отмена
        private void projectCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage1?.Invoke(sender, e);
        }
    }
}
