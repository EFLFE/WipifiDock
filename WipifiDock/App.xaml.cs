using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
                proj.Close();
            }
        }
    }
}
