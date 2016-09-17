using System;
using System.Collections.ObjectModel;

namespace WipifiDock
{
    namespace TreeViewData
    {
        /// <summary> TreeView каталог. </summary>
        public class TreeViewDataFolder
        {
            public string Name { get; set; }

            /// <summary> Файлы каталога. </summary>
            public ObservableCollection<TreeViewDataFile> Members { get; set; }

            public TreeViewDataFolder(string name)
            {
                Name = name;
                Members = new ObservableCollection<TreeViewDataFile>();
            }
        }

        /// <summary> TreeView файл. </summary>
        public class TreeViewDataFile
        {
            public string Name { get; set; }

            public string Path;

            public TreeViewDataFile(string name, string path)
            {
                Name = name;
                Path = path;
            }
        }
    }
}
