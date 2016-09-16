using System;

namespace WipifiDock
{
    public struct ProjectData
    {
        public string Name, Path, Desc, Author;

        public ProjectData(string name, string path, string desc, string author)
        {
            Name = name;
            Path = path;
            Desc = desc;
            Author = author;
        }
    }
}
