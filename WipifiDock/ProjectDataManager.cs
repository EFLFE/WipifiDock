using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Text;

namespace WipifiDock
{
    public static class ProjectDataManager
    {
        /* [project]
         * name
         * path
         * desc
         * author
         * {NewLine}
         */

        private const string CONF_FILE = "wipifi.cfg";
        private const string PROJECT_LABEL = "[project]";

        private static string selectedProjectName;

        public static bool ProjectProfileWasSelected => selectedProjectName != null && selectedProjectName.Length > 0;

        private static Dictionary<string, ProjectData> projects = new Dictionary<string, ProjectData>();

        public static ProjectData GetProjectData(string name)
        {
            return projects[name];
        }

        public static bool CreateProfile(string name, string path, string desc, string author)
        {
            try
            {
                var dir = Directory.CreateDirectory(path);
                if (dir == null || !dir.Exists)
                {
                    throw new Exception("Failed to create a new directory \"" + path + "\"!");
                }

                // write
                using (var stream = File.Open(CONF_FILE, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    // я знаю, что есть класс, который записывает текст одним методом
                    // но я забыл как он называется
                    var en = new UTF8Encoding(true);
                    byte[] w1 = en.GetBytes(PROJECT_LABEL + Environment.NewLine);
                    byte[] w2 = en.GetBytes(name + Environment.NewLine);
                    byte[] w3 = en.GetBytes(path + Environment.NewLine);
                    byte[] w4 = en.GetBytes(desc + Environment.NewLine);
                    byte[] w5 = en.GetBytes(author + Environment.NewLine);
                    byte[] w6 = en.GetBytes(Environment.NewLine);

                    stream.Seek(0, SeekOrigin.End);
                    stream.Write(w1, 0, w1.Length);
                    stream.Write(w2, 0, w2.Length);
                    stream.Write(w3, 0, w3.Length);
                    stream.Write(w4, 0, w4.Length);
                    stream.Write(w5, 0, w5.Length);
                    stream.Write(w6, 0, w6.Length);
                }

                if (projects.ContainsKey(name))
                {
                    throw new Exception($"Project by name \"{name}\" are contains.");
                }
                projects.Add(name, new ProjectData(name, path, desc, author));

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add new project error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static ProjectData[] LoadProjectConfig()
        {
            if (File.Exists(CONF_FILE))
            {
                try
                {
                    projects.Clear();
                    string[] txt = File.ReadAllLines(CONF_FILE);

                    for (int i = 0; i < txt.Length; i++)
                    {
                        if (txt[i].Length > 0)
                        {
                            if (txt[i].Equals(PROJECT_LABEL, StringComparison.OrdinalIgnoreCase))
                            {
                                string name = txt[++i];
                                string path = txt[++i];
                                string desc = txt[++i];
                                string author = txt[++i];

                                if (projects.ContainsKey(name))
                                {
                                    throw new Exception($"Project by name \"{name}\" are contains.");
                                }
                                projects.Add(name, new ProjectData(name, path, desc, author));
                            }
                        }
                    }

                    return projects.Values.ToArray();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Load config file error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            return null;
        }

        public static bool SelectProjectName(string name)
        {
            if (projects.ContainsKey(name))
            {
                selectedProjectName = name;
                return true;
            }
            else
            {
                MessageBox.Show($"Project by name \"{name}\" are contains.", "Config warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        public static ProjectData GetSelectedProjectData()
        {
            return projects[selectedProjectName];
        }

    }
}
