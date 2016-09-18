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
                    return IsEmpty ? $" ({initial})" : $" ({Items.Count})";
                }
            }

            public bool IsEmpty
            {
                get
                {
                    return Items.Count == 0 || (Items.Count == 1 && Items[0] == null);
                }
            }

            public TreeViewDataFolder(string folderName, string path)
            {
                FolderName = folderName;
                Path = path;
                Items.Add(null); // для возможности развернуть
                initial = Directory.GetDirectories($"{path}\\{folderName}", "*", SearchOption.TopDirectoryOnly).Length
                        + Directory.GetFiles($"{path}\\{folderName}", "*.*", SearchOption.TopDirectoryOnly).Length;
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
