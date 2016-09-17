using System;
using System.Collections.ObjectModel;

namespace WipifiDock
{
    namespace TreeViewData
    {
        public class TreeViewDataFolder
        {
            public string Name { get; set; }

            public ObservableCollection<TreeViewDataFile> Members { get; set; }

            public TreeViewDataFolder(string name)
            {
                Name = name;
                Members = new ObservableCollection<TreeViewDataFile>();
            }
        }

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
