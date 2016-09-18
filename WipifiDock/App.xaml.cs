using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace WipifiDock
{
    public partial class App : Application
    {
        public static string Version { get; private set; }

        public App()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            Version = fvi.FileVersion;
        }

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
