using System;
using System.Windows;

namespace WipifiDock
{
    public partial class App : Application
    {
        public const string VERSION = "1.0.0";

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
