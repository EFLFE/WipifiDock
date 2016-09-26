using System;
using System.Linq;
using System.IO;
using System.Windows;

namespace WipifiDock.Data
{
    /// <summary> Менеджер файлов. </summary>
    public static class FileManager
    {
        public const string CONF_FILE = "projects.ini";
        public const string PROJECT_FILE = "wdproj.ini";

        public const string HEAD_FILE = "head.html";
        public const string BODY_FILE = "body.html";
        public const string FOOTER_FILE = "footer.html";

        public const string HEAD_FILE_WE = "head";
        public const string BODY_FILE_WE = "body";
        public const string FOOTER_FILE_WE = "footer";

        /// <summary> Корневой каталог проекта. </summary>
        public static string RootPath;

        public enum FileFormatType
        {
            Unknown,
            HTML,
            MD,
            CSS,
            TXT,
            IMAGE
        }

        /// <summary> Возвращает true, если файл конфликтует с шаблонными для вставки файлами. </summary>
        /// <param name="file"> полное имя файла. </param>
        /// <returns> конфликтует? </returns>
        public static bool CheckFileIsConflict(string file)
        {
            string mess = "Файл {0} является вставочным шаблоном и может быть открыт только в .html формате.";

            if (file.Contains(HEAD_FILE_WE) && file.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show( string.Format(mess, HEAD_FILE_WE), "Внимание",  MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            if (file.Contains(BODY_FILE_WE) && file.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(string.Format(mess, BODY_FILE_WE), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            if (file.Contains(FOOTER_FILE_WE) && file.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(string.Format(mess, FOOTER_FILE_WE), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }

        /// <summary> Получить тип проект-файла. </summary>
        /// <param name="fileNameOrPathToFileOrHalfPathToFile"> Имя файла или путь к нему. </param>
        public static FileFormatType DetectProjectFileFormatType(string fileNameOrPathToFileOrHalfPathToFile)
        {
            switch (getExt(fileNameOrPathToFileOrHalfPathToFile))
            {
            case ".html": return FileFormatType.HTML;

            case ".md": return FileFormatType.MD;

            case ".css": return FileFormatType.CSS;

            case ".txt": return FileFormatType.TXT;

            case ".png":
            case ".jpeg":
            case ".jpg":
            case ".tga":
            case ".gif":
                return FileFormatType.IMAGE;

            default: return FileFormatType.Unknown;
            }
        }

        // to .ext
        private static string getExt(string text)
        {
            if (text == null || text.Length < 2)
                return string.Empty;

            int index1 = text.LastIndexOf('\\');
            if (index1 != -1)
            {
                text = text.Remove(0, index1 + 1);
            }
            index1 = text.LastIndexOf('.');
            if (index1 != -1)
            {
                text = text.Remove(0, index1);
            }

            return text.ToLower();
        }

        /// <summary> Получить файлы проекта. </summary>
        /// <param name="path"> Поиска начинается с корня проекта. </param>
        public static string[] GetProjectFiles(string path = "\\")
        {
            return Directory.EnumerateFiles(RootPath + path, "*.*", SearchOption.TopDirectoryOnly).ToArray();
        }

        /// <summary> Получить каталоги проекта. </summary>
        /// <param name="path"> Поиска начинается с корня проекта. </param>
        public static string[] GetProjectDirs(string path = "\\")
        {
            return Directory.EnumerateDirectories(RootPath + path, "*", SearchOption.TopDirectoryOnly).ToArray();
        }

    }
}
