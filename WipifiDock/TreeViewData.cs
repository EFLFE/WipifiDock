using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WipifiDock
{
    namespace TreeViewData
    {
        public enum FileFormatType
        {
            Unknown,
            MarkDown,
            HTML,
            Image,
            Project
        }

        /// <summary> TreeView каталог. </summary>
        public sealed class TreeViewDataFolder : TreeViewItem
        {
            private static Image folderImage;
            private int initial;

            public string FolderName { get; set; }

            public string Path { get; set; }

            public string GetItemCountString
            { get { return IsEmpty ? $" ({initial})" : $" ({Items.Count})"; } }

            public bool IsEmpty
            { get { return Items.Count == 0 || (Items.Count == 1 && Items[0] == null); } }

            public ImageSource GetImage
            { get { return folderImage.Source; } }

            public TreeViewDataFolder(string folderName, string path)
            {
                FolderName = folderName;
                Path = path;
                Items.Add(null); // для возможности развернуть
                initial = Directory.GetDirectories($"{path}\\{folderName}", "*", SearchOption.TopDirectoryOnly).Length
                        + Directory.GetFiles($"{path}\\{folderName}", "*.*", SearchOption.TopDirectoryOnly).Length;

                if (folderImage == null)
                {
                    folderImage = new Image();
                    folderImage.Source = FindResource("miniFolder") as BitmapImage;
                }
            }
        }

        /// <summary> TreeView файл. </summary>
        public sealed class TreeViewDataFile : TreeViewItem
        {
            private static Image htmlFile;
            private static Image imageFile;
            private static Image mdFile;
            private static Image miniProjFile;
            private static Image unknownFile;

            public string FileName { get; set; }

            public string Path { get; set; }

            private FileFormatType fileFormatType;

            public ImageSource GetImage
            {
                get
                {
                    switch (fileFormatType)
                    {
                    case FileFormatType.MarkDown:
                        if (mdFile == null)
                        {
                            mdFile = new Image();
                            mdFile.Source = FindResource("mdFile") as BitmapImage;
                        }
                        return mdFile.Source;

                    case FileFormatType.HTML:
                        if (htmlFile == null)
                        {
                            htmlFile = new Image();
                            htmlFile.Source = FindResource("htmlFile") as BitmapImage;
                        }
                        return htmlFile.Source;

                    case FileFormatType.Image:
                        if (imageFile == null)
                        {
                            imageFile = new Image();
                            imageFile.Source = FindResource("imageFile") as BitmapImage;
                        }
                        return imageFile.Source;

                    case FileFormatType.Project:
                        if (miniProjFile == null)
                        {
                            miniProjFile = new Image();
                            miniProjFile.Source = FindResource("miniProjFile") as BitmapImage;
                        }
                        return miniProjFile.Source;

                    case FileFormatType.Unknown:
                    default:
                        if (unknownFile == null)
                        {
                            unknownFile = new Image();
                            unknownFile.Source = FindResource("unknownFile") as BitmapImage;
                        }
                        return unknownFile.Source;
                    }
                }
                set { }
            }

            public TreeViewDataFile(string fileName, string path)
            {
                FileName = fileName;
                Path = path;

                fileFormatType = FileFormatType.Unknown;
                int lio = fileName.LastIndexOf('.');
                if (lio != -1)
                {
                    string rmr = fileName.Remove(0, lio + 1).ToLower();
                    switch (rmr)
                    {
                    case "bmp":
                    case "gif":
                    case "jpg":
                    case "jpeg":
                    case "jpe":
                    case "jpf":
                    case "jps":
                    case "flif":
                    case "png":
                    case "pbm":
                    case "tga":
                    case "tif":
                        fileFormatType = FileFormatType.Image;
                        break;

                    case "html":
                        fileFormatType = FileFormatType.HTML;
                        break;

                    case "md":
                        fileFormatType = FileFormatType.MarkDown;
                        break;
                    }
                }
            }
        }
    }
}
