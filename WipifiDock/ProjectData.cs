using System;

namespace WipifiDock
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

        /// <summary> Создать данные. </summary>
        /// <param name="name"> Имя проекта. </param>
        /// <param name="path"> Путь. </param>
        /// <param name="desc"> Описание. </param>
        /// <param name="author"> Автор. </param>
        public ProjectData(string name, string path, string desc, string author)
        {
            Name = name;
            Path = path;
            Desc = desc;
            Author = author;
        }
    }
}
