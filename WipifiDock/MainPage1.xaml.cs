using System;
using System.Windows;
using System.Windows.Controls;

namespace WipifiDock
{
    /// <summary> Project list. </summary>
    public partial class MainPage1 : Page
    {
        public event EventHandler NavigateToPage2;

        public MainPage1()
        {
            InitializeComponent();
            projectListBox.SelectionChanged += ProjectListBox_SelectionChanged;

            var p = ProjectDataManager.LoadProjectConfig();
            if (p != null)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    projectListBox.Items.Add(p[i].Name);
                }
            }
        }

        public void AddProfile(string name)
        {
            projectListBox.Items.Add(name);
        }

        private void ProjectListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = projectListBox.SelectedItem as string;
            var proj = ProjectDataManager.GetProjectData(name);

            desc.Text = $"Name: {proj.Name}\nDescription: {proj.Desc}\nPath: {proj.Path}\nAuthor: {proj.Author}";
        }

        private void createProject_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage2?.Invoke(sender, e);
        }

        private void openProject_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
