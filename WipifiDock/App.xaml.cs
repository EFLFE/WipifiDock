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
        public const string Version = "0.1.0";

        //public static string FullVersion { get; private set; }

        //public App()
        //{
        //    var assembly = Assembly.GetExecutingAssembly();
        //    var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        //    FullVersion = fvi.FileVersion;
        //}

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            bool _try_ =
#if DEBUG
                !Debugger.IsAttached;
#else
                true;
#endif
            if (_try_)
            {
                try
                {
                    var mainWindow = new MainWipifiWindow();
                    InsertMDTextForm.Instance = new InsertMDTextForm();

                    mainWindow.ShowDialog();

                    InsertMDTextForm.Instance.Close();
                }
                catch (Exception ex)
                {
                    Log.Write(ex.ToString(), Log.MessageType.ERROR);

                    var savelog = MessageBox.Show(
                        $"{ex.Message}\n\nСохранить лог?",
                        "Сбой программы!",
                        MessageBoxButton.YesNo, MessageBoxImage.Error);

                    if (savelog == MessageBoxResult.Yes)
                    {
                        Process.Start(Log.SaveLog("log"));
                    }
                }
            }
            else
            {
                Log.EnableTrace = true;

                var mainWindow = new MainWipifiWindow();
                InsertMDTextForm.Instance = new InsertMDTextForm();

                mainWindow.ShowDialog();

                InsertMDTextForm.Instance.Close();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ProjectManager.SaveProjects();
            base.OnExit(e);
        }

    }
}
