using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Text;

namespace WipifiDock
{
    /// <summary> Наш личный менеджер по проектам. </summary>
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

        /// <summary> Имя проекта методом SelectProjectName был выбран. </summary>
        public static bool ProjectProfileWasSelected => selectedProjectName != null && selectedProjectName.Length > 0;

        // проекты [имя, данные]
        private static Dictionary<string, ProjectData> projects = new Dictionary<string, ProjectData>();

        /// <summary> Получить данные по проекту (без проверки). </summary>
        /// <param name="name"> Имя проекта. </param>
        /// <returns> Данные по проекту. </returns>
        /// <exception cref="KeyNotFoundException"> Возможно.. </exception>
        public static ProjectData GetProjectData(string name)
        {
            return projects[name];
        }

        /// <summary> Создать профиль. </summary>
        /// <param name="name"> Имя проекта. </param>
        /// <param name="path"> Путь. </param>
        /// <param name="desc"> Описание. </param>
        /// <param name="author"> Автор. </param>
        /// <returns> Профиль был создан. </returns>
        public static bool CreateProfile(string name, string path, string desc, string author, bool ignoreExistsDirectory)
        {
            try
            {
                if (Directory.Exists(path) && !ignoreExistsDirectory)
                {
                    var rere = MessageBox.Show("Каталог уже существует. Продолжить?",
                        "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (rere == MessageBoxResult.No || rere == MessageBoxResult.None)
                    {
                        return false;
                    }
                }

                if (!Directory.Exists(path))
                {
                    var dir = Directory.CreateDirectory(path);
                    if (dir == null || !dir.Exists)
                    {
                        throw new Exception("Не удалось создать каталог \"" + path + "\"!");
                    }
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
                    throw new Exception($"Проект \"{name}\" уже существует.");
                }
                projects.Add(name, new ProjectData(name, path, desc, author));

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка создания проекта.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary> Загрузить и полчить список проектов. </summary>
        /// <returns> ДАнные проектов. </returns>
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
                                    throw new Exception($"Проект \"{name}\" уже существует.");
                                }
                                projects.Add(name, new ProjectData(name, path, desc, author));
                            }
                        }
                    }

                    return projects.Values.ToArray();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при загрузке конфигурации.", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            return null;
        }

        /// <summary> Выбрать проект для дальнейшей работы. </summary>
        /// <param name="name"> Имя проекта. </param>
        /// <returns> Проект найден и выбран. </returns>
        public static bool SelectProjectName(string name)
        {
            if (projects.ContainsKey(name))
            {
                selectedProjectName = name;
                return true;
            }
            else
            {
                MessageBox.Show($"Проект \"{name}\" уже существует.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        /// <summary> Получить данные по выбранному проекту. </summary>
        /// <returns> Данные проекта. </returns>
        /// <exception cref="KeyNotFoundException"> Возможно.. </exception>
        public static ProjectData GetSelectedProjectData()
        {
            return projects[selectedProjectName];
        }

        /// <summary> Удалить проект. </summary>
        /// <param name="name"> Имя проекта. </param>
        /// <returns> Был удалён. </returns>
        public static bool RemoveProjectData(string name)
        {
            try
            {
                if (projects.ContainsKey(name))
                {
                    ProjectData selectedProject = projects[name];

                    if (Directory.Exists(selectedProject.Path))
                    {
                        var msr = MessageBox.Show(
                            $"После подтверждения данного сообщения, все данные проекта {selectedProject.Name} будут удалены.\n\n"
                            + "Все файлы и каталоги будут удалены из следующего каталога:\n" + selectedProject.Path
                            + "\n\nПродолжить удаление?",
                            $"Внимание! Вы собираетесь удалить проект {selectedProject.Name}",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (msr == MessageBoxResult.Yes)
                        {
                            // удалить каталог
                            //Directory.Delete(selectedProject.Path, true); // ОПАСНОСТЬ!
                            Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(
                                selectedProject.Path,
                                Microsoft.VisualBasic.FileIO.UIOption.AllDialogs,
                                Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);

                            if (Directory.Exists(selectedProject.Path))
                            {
                                MessageBox.Show($"Каталог \"{selectedProject.Path}\" не был удален.", "Сбой");
                                return false;
                            }

                            deleteProjectData(selectedProject.Name);
                            MessageBox.Show($"Все данные в каталоге \"{selectedProject.Path}\" были удалены.", "Готово");
                            return true;
                        }
                    }
                    else
                    {
                        deleteProjectData(selectedProject.Name);
                        MessageBox.Show($"Каталог \"{selectedProject.Path}\" не найден.\n\nИнформация о проекте была удалена.", "Готово");
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show($"Проект \"{name}\" не был найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        private static void deleteProjectData(string name)
        {
            // без расчёта на очень большой конфиг файл
            var txt = File.ReadAllLines(CONF_FILE);
            int i;

            for (i = 0; i < txt.Length; i++)
            {
                // 6 lines
                // PROJECT_LABEL
                // name
                // path
                // desc
                // author
                // \n

                if (txt[i].Equals(PROJECT_LABEL, StringComparison.OrdinalIgnoreCase))
                {
                    if (txt[i + 1].Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        // попался!
                        txt[i] = null;
                        txt[i + 1] = null;
                        txt[i + 2] = null;
                        txt[i + 3] = null;
                        txt[i + 4] = null;
                        txt[i + 5] = null;
                        break;
                    }
                }
            }
            // write
            var en = new UTF8Encoding(true);

            using (var stream = File.Open(CONF_FILE, FileMode.Create, FileAccess.Write))
            {
                for (i = 0; i < txt.Length; i++)
                {
                    if (txt[i] != null)
                    {
                        byte[] w = en.GetBytes(txt[i] + Environment.NewLine);
                        stream.Write(w, 0, w.Length);
                    }
                }
            }
        }

    }
}
