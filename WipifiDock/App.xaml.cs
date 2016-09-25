using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using WipifiDock.Data;
using WipifiDock.Forms;

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

#if DEBUG
            InsertTextForm.Instance = new InsertTextForm();

            var mainWindow = new MainWipifiWindow();
            mainWindow.ShowDialog();

            InsertTextForm.Instance.Close();
#else
#error TRY RELEASE!!!!!!!
#endif

        }

        protected override void OnExit(ExitEventArgs e)
        {
            ProjectDataManager.SaveProjects();
            base.OnExit(e);
        }

    }
}
