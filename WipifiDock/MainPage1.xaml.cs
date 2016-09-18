﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WipifiDock
{
    /// <summary> Project list. </summary>
    public partial class MainPage1 : Page
    {
        /// <summary> СОбытия для перехода на MainPage2. </summary>
        public event EventHandler NavigateToPage2;

        /// <summary> Событие создания нового проекта. </summary>
        public event EventHandler ProjectWasSelected;

        private ProjectData selectedProject;

        public MainPage1()
        {
            InitializeComponent();

            projectName.Background = Brushes.Transparent;
            projectDesc.Background = Brushes.Transparent;
            projectPathAndAuthor.Background = Brushes.Transparent;

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
            if (name == null)
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

        // после создания проекта перейти на MainPage2
        private void createProject_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage2?.Invoke(sender, e);
        }

        // открыть проект
        private void openProject_Click(object sender, RoutedEventArgs e)
        {
            var pi = projectListBox.SelectedItem as string;
            if (pi != null && ProjectDataManager.SelectProjectName(pi))
            {
                ProjectWasSelected(sender, e);
            }
        }

        private void deleteSelectedProject_Click(object sender, RoutedEventArgs e)
        {
            if (projectListBox.SelectedIndex != -1 && ProjectDataManager.RemoveProjectData(selectedProject.Name))
            {
                projectListBox.Items.RemoveAt(projectListBox.SelectedIndex);
            }
        }

#warning TODO: delete project
        private void openExistProject_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Не реализован.");
        }
    }
}
