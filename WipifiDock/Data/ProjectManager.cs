using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Text;

namespace WipifiDock.Data
{
    /// <summary> Мой личный менеджер по проектам. </summary>
    public static class ProjectManager
    {
        private static string selectedProjectName;

        /// <summary> Имя проекта методом SelectProjectName был выбран. </summary>
        public static bool ProjectProfileWasSelected =>
            selectedProjectName != null
            && selectedProjectName.Length > 0
            && projects.ContainsKey(selectedProjectName);

        // проекты [имя, данные]
        private static Dictionary<string, ProjectData> projects = new Dictionary<string, ProjectData>();

        /// <summary> Получить данные по проекту (без проверки). </summary>
        /// <param name="name"> Имя проекта. </param>
        /// <returns> Данные по проекту. </returns>
        /// <exception cref="KeyNotFoundException"> Возможно.. </exception>
        public static ProjectData GetProjectData(string name) => projects[name];

        /// <summary> Получить выбранный проект. </summary>
        public static ProjectData GetSelectedProjectData() => projects[selectedProjectName];

        /// <summary> Создать профиль. </summary>
        /// <param name="name"> Имя проекта. </param>
        /// <param name="path"> Путь. </param>
        /// <param name="ignoreExistsDirectory"> Игнорировать наличия одноимённого каталога. </param>
        /// <returns> Профиль был создан. </returns>
        public static bool CreateProfile(string name, string path, bool ignoreExistsDirectory)
        {
            Log.Write($"Create profile {name} in \"{path}\"");
            try
            {
                // проверка наличия каталога
                if (Directory.Exists(path) && !ignoreExistsDirectory)
                {
                    var rere = MessageBox.Show("Каталог уже существует. Некоторые данные будут перезаписаны Продолжить?",
                        "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (rere == MessageBoxResult.No || rere == MessageBoxResult.None)
                    {
                        return false;
                    }
                }

                // проверка наличия проекта в списке
                if (projects.ContainsKey(name))
                {
                    throw new Exception($"Проект \"{name}\" уже существует.");
                }

                // базовый набор проекта
                if (!Directory.Exists(path))
                {
                    var dir = Directory.CreateDirectory(path);
                    if (dir == null || !dir.Exists)
                    {
                        throw new Exception("Не удалось создать каталог \"" + path + "\"!");
                    }
                }

                // добавить ProjectData, после создание папки ProjectData
                projects.Add(name, new ProjectData(name, path));

                string cssFileName = "style.css";

                // index* md
                File.WriteAllText(
                    $"{path}\\index.md",
                    $"# Проект {name}");

                File.WriteAllText(
                    $"{path}\\{FileManager.HEAD_FILE}",
                    "<!-- Вставочный шаблон в тег head -->\n" +
                    "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + cssFileName + "\">\n" +
                    "<META charset=\"utf - 8\">\n");

                File.WriteAllText(
                    $"{path}\\{FileManager.BODY_FILE}",
                    "<!-- Вставочный шаблон в начало тега body -->\n");

                File.WriteAllText(
                    $"{path}\\{FileManager.FOOTER_FILE}",
                    "<!-- Вставочный шаблон в конец тега head -->\n");

                File.WriteAllText(path + "\\" + cssFileName, "/* Стиль для страниц */");

                return true;
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString(), Log.MessageType.ERROR);
                MessageBox.Show(ex.ToString(), "Ошибка создания проекта.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary> Загрузить и полчить список проектов. </summary>
        /// <returns> ДАнные проектов. </returns>
        public static ProjectData[] LoadProjectConfig()
        {
            Log.Write("Load Project Config");
            if (File.Exists(FileManager.CONF_FILE))
            {
                try
                {
                    projects.Clear();

                    if (File.Exists(FileManager.CONF_FILE))
                    {
                        var txt = File.ReadAllLines(FileManager.CONF_FILE);
                        for (int i = 0; i < txt.Length;)
                        {
                            if (txt[i].Length == 0)
                                continue;

                            string name = txt[i++];
                            string path = txt[i++];

                            projects.Add(name, new ProjectData(name, path));
                        }
                    }

                    return projects.Values.ToArray();
                }
                catch (Exception ex)
                {
                    Log.Write(ex.ToString(), Log.MessageType.ERROR);
                    MessageBox.Show(ex.ToString(), "Ошибка при загрузке конфигурации.", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            return null;
        }

        /// <summary> Выбрать проект для дальнейшей работы, получив все данные. </summary>
        /// <param name="name"> Имя проекта. </param>
        /// <returns> Получить данные по выбранному проекту. </returns>
        /// <exception cref="KeyNotFoundException"> Проект не был найден. </exception>
        public static ProjectData SelectAndLoadProject(string name)
        {
            Log.Write("Select And Load Project - " + name);
            if (!projects.ContainsKey(name))
            {
                throw new KeyNotFoundException($"Проект \"{name}\" не был найден.");
            }

            selectedProjectName = name;
            return projects[selectedProjectName];
        }

        /// <summary> Удалить проект. </summary>
        /// <param name="name"> Имя проекта. </param>
        /// <returns> Был удалён. </returns>
        public static bool RemoveProjectData(string name)
        {
            Log.Write("Remove Project Data");
            try
            {
                if (projects.ContainsKey(name))
                {
                    ProjectData selectedProject = projects[name];

                    if (Directory.Exists(selectedProject.Path))
                    {
                        var msr = MessageBox.Show(
                            "Удалить данные проекта?", "Удалить все данные?",
                            MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (msr == MessageBoxResult.Yes)
                        {
                            msr = MessageBox.Show(
                                $"После подтверждения данного сообщения, все данные проекта {selectedProject.Name} будут удалены.\n\n"
                                + "Все файлы и каталоги будут удалены из следующего каталога:\n" + selectedProject.Path
                                + "\n\nПродолжить удаление?",
                                $"Внимание! Вы собираетесь удалить проект {selectedProject.Name}",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning);

                            if (msr == MessageBoxResult.Yes)
                            {
                                // удалить каталог
                                //Directory.Delete(seleсtedProject.Pаth, true); #error ОПАСНО!
                                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(
                                    selectedProject.Path,
                                    Microsoft.VisualBasic.FileIO.UIOption.AllDialogs,
                                    Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);

                                if (Directory.Exists(selectedProject.Path))
                                {
                                    MessageBox.Show($"Каталог \"{selectedProject.Path}\" не был удален.", "Сбой");
                                    return false;
                                }

                                projects.Remove(name);
                                MessageBox.Show($"Все данные в каталоге \"{selectedProject.Path}\" были удалены.", "Готово");
                                return true;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        projects.Remove(name);
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
                Log.Write(ex.ToString(), Log.MessageType.ERROR);
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        /// <summary> Сохранить проекты в конфиг файл. </summary>
        public static void SaveProjects()
        {
            Log.Write("Save Projects");
            if (projects.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var p in projects.Values)
                {
                    sb.AppendLine(p.Name);
                    sb.AppendLine(p.Path);
                }
                File.WriteAllText(FileManager.CONF_FILE, sb.ToString());
            }
            else if (File.Exists(FileManager.CONF_FILE))
            {
                File.Delete(FileManager.CONF_FILE);
            }
        }

    }
}
