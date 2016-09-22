using System;
using System.Linq;
using System.IO;

namespace WipifiDock.Data
{
    /// <summary> Менеджер файлов. </summary>
    public static class FileManager
    {
        public const string CONF_FILE = "projects.ini";
        public const string PROJECT_FILE = "wdproj.ini";

        public const string WEB_EXT = ".wdweb";
        public const string STYLE_EXT = ".wdstyle";
        public const string CONTENT_EXT = ".wdcontent";

        /// <summary> Корневой каталог проекта. </summary>
        public static string RootPath;

        /// <summary> Тип контент-файла. </summary>
        public enum ContentFormatType
        {
            Unknown = 0,
            Image,
            TextFile,
            HTMLFile,
            MarkDownFile
        }

        /// <summary> Тип проект-файла (↑ расширение ↑). </summary>
        public enum ProjectFileFormatType
        {
            Unknown = 0,
            Web,
            Style,
            Content
        }

        /// <summary> Получить тип проект-файла. </summary>
        /// <param name="fileNameOrPathToFileOrHalfPathToFile"> Имя файла или путь к нему. </param>
        public static ProjectFileFormatType DetectProjectFileFormatType(string fileNameOrPathToFileOrHalfPathToFile)
        {
            switch (getExt(fileNameOrPathToFileOrHalfPathToFile))
            {
            case WEB_EXT:
                return ProjectFileFormatType.Web;

            case STYLE_EXT:
                return ProjectFileFormatType.Style;

            case CONTENT_EXT:
                return ProjectFileFormatType.Content;

            default:
                return ProjectFileFormatType.Unknown;
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
            return Directory.EnumerateFiles(RootPath + path, "*.*", SearchOption.TopDirectoryOnly)
                   .Where(f => (f.EndsWith(WEB_EXT) || f.EndsWith(STYLE_EXT) || f.EndsWith(CONTENT_EXT)))
                   .ToArray();
        }

        /// <summary> Получить каталоги проекта. </summary>
        /// <param name="path"> Поиска начинается с корня проекта. </param>
        public static string[] GetProjectDirs(string path = "\\")
        {
            return Directory.EnumerateDirectories(RootPath + path, "*", SearchOption.TopDirectoryOnly)
                   .Where(f => (f.EndsWith(WEB_EXT) || f.EndsWith(STYLE_EXT) || f.EndsWith(CONTENT_EXT)))
                   .ToArray();
        }

    }
}
