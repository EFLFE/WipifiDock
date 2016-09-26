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

        /// <summary> Создать данные. </summary>
        /// <param name="name"> Имя проекта. </param>
        /// <param name="path"> Путь. </param>
        public ProjectData(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
