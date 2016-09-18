using System;
using System.Windows.Controls;

namespace WipifiDock
{
    namespace TreeViewData
    {
        /// <summary> TreeView каталог. </summary>
        public sealed class TreeViewDataFolder : TreeViewItem
        {
            public string FolderName { get; set; }

            public string Path { get; set; }

            public TreeViewDataFolder(string folderName, string path)
            {
                FolderName = folderName;
                Path = path;
            }
        }

        /// <summary> TreeView файл. </summary>
        public sealed class TreeViewDataFile : TreeViewItem
        {
            public string FileName { get; set; }

            public string Path { get; set; }

            public TreeViewDataFile(string filename, string path)
            {
                FileName = filename;
                Path = path;
            }
        }
    }
}
