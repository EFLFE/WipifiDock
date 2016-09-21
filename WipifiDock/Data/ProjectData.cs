using System;

namespace WipifiDock.Data
{
    /// <summary> Данные по проекту. </summary>
    public struct ProjectData
    {

        /// <summary> Имя проекта. </summary>
        public string Name;

        /// <summary> Путь. </summary>
        public string Path;

        /// <summary> Описание. </summary>
        public string Desc;

        /// <summary> Автор. </summary>
        public string Author;

        /// <summary> Имя файла начальной страницы. </summary>
        public string MainHtmlFile;

        /// <summary> Имя стиля. </summary>
        public string StyleName;

        /// <summary> Создать данные. </summary>
        /// <param name="name"> Имя проекта. </param>
        /// <param name="path"> Путь. </param>
        public ProjectData(string name, string path)
        {
            Name = name;
            Path = path;
            Desc = string.Empty;
            Author = string.Empty;
            MainHtmlFile = string.Empty;
            StyleName = string.Empty;
        }

        /// <summary> Создать данные. </summary>
        /// <param name="name"> Имя проекта. </param>
        /// <param name="path"> Путь. </param>
        /// <param name="desc"> Описание. </param>
        /// <param name="author"> Автор. </param>
        /// <param name="mainHtmlFile"> Имя файла начальной страницы. </param>
        /// <param name="styleName"> Имя стиля. </param>
        public ProjectData(string name, string path, string desc, string author, string mainHtmlFile, string styleName)
        {
            Name = name;
            Path = path;
            Desc = desc;
            Author = author;
            MainHtmlFile = mainHtmlFile;
            StyleName = styleName;
        }
    }
}
