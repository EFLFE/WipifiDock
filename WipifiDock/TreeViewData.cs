using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace WipifiDock
{
    namespace TreeViewData
    {
        /// <summary> TreeView каталог. </summary>
        public sealed class TreeViewDataFolder
        {
            public string FolderName { get; set; }

            /// <summary> Файлы каталога. </summary>
            public ObservableCollection<TreeViewDataFile> Members { get; set; }

            public TreeViewDataFolder(string name) : base()
            {
                FolderName = name;
                Members = new ObservableCollection<TreeViewDataFile>();
            }
        }

        /// <summary> TreeView файл. </summary>
        public sealed class TreeViewDataFile
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
