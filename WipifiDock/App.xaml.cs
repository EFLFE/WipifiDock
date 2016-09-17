using System;
using System.Windows;

namespace WipifiDock
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var main = new MainWindow();
            var proj = new ProjectsWindow();

            main.ShowDialog();

            if (ProjectDataManager.ProjectProfileWasSelected)
            {
                proj.ShowDialog();
            }
            else
            {
                // если не закрыть, то поток приложения не завершится
                proj.Close();
            }
        }
    }
}
