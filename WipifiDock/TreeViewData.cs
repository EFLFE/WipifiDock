using System;
using System.IO;
using System.Windows.Controls;

namespace WipifiDock
{
    namespace TreeViewData
    {
        /// <summary> TreeView каталог. </summary>
        public sealed class TreeViewDataFolder : TreeViewItem
        {
            private int initial;

            public string FolderName { get; set; }

            public string Path { get; set; }

            public string GetItemCountString
            {
                get
                {
                    return Items.Count < 2 && Items[0] == null ? $" ({initial})" : $" ({Items.Count})";
                }
            }

            public TreeViewDataFolder(string folderName, string path)
            {
                FolderName = folderName;
                Path = path;
                Items.Add(null); // для возможности развернуть
                initial = Directory.GetDirectories($"{path}\\{folderName}", "*", SearchOption.TopDirectoryOnly).Length;
                initial += Directory.GetFiles($"{path}\\{folderName}", "*.*", SearchOption.TopDirectoryOnly).Length;
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
