using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WipifiDock.Data;

namespace WipifiDock.Controls
{
    /// <summary> TreeViewData. </summary>
    [DebuggerDisplay("Name = {FullName}, Type = {fileFormatType}, Items = {Items.Count}")]
    public sealed class TreeViewData : TreeViewItem
    {
        private static Image folderFile;
        private static Image webFile;
        private static Image styleFile;
        private static Image imageFile;
        private static Image mdFile;
        private static Image txtFile;
        private static Image binaryFile;
        private static Image unknownFile;

        private FileManager.FileFormatType fileFormatType;

        public FileManager.FileFormatType GetFileFormatType => fileFormatType;

        /// <summary> Уникальный номер для поиска. </summary>
        public int ID { get; private set; }

        /// <summary> Это дикертория? </summary>
        public bool IsFolder { get; set; }

        /// <summary> Имя файла/каталога. </summary>
        public string FileName { get; set; }

        /// <summary> Имя файла с расширением. </summary>
        public string FullName { get; set; }

        //public string GetItemCountString
        //{ get { return IsEmpty ? $" ({initial})" : $" ({Items.Count})"; } }

        public bool IsEmpty
        {
            get
            {
                return Items.Count == 0 || (Items.Count == 1 && Items[0] == null);
            }
        }

        /// <summary> Полный путь к файлу/каталогу. </summary>
        public string Path { get; set; }

        //public new ItemCollection Items { get { return base.Items; } }

        public ImageSource GetImage
        {
            get
            {
                switch (fileFormatType)
                {
                case FileManager.FileFormatType.Folder:
                    if (folderFile == null)
                    {
                        folderFile = new Image();
                        folderFile.Source = FindResource("miniFolder") as BitmapImage;
                    }
                    return folderFile.Source;

                case FileManager.FileFormatType.HTML:
                    if (webFile == null)
                    {
                        webFile = new Image();
                        webFile.Source = FindResource("htmlFile") as BitmapImage;
                    }
                    return webFile.Source;

                case FileManager.FileFormatType.CSS:
                    if (styleFile == null)
                    {
                        styleFile = new Image();
                        styleFile.Source = FindResource("styleFile") as BitmapImage;
                    }
                    return styleFile.Source;

                case FileManager.FileFormatType.IMAGE:
                    if (imageFile == null)
                    {
                        imageFile = new Image();
                        imageFile.Source = FindResource("imageFile") as BitmapImage;
                    }
                    return imageFile.Source;

                case FileManager.FileFormatType.MD:
                    if (mdFile == null)
                    {
                        mdFile = new Image();
                        mdFile.Source = FindResource("mdFile") as BitmapImage;
                    }
                    return mdFile.Source;

                case FileManager.FileFormatType.TXT:
                    if (txtFile == null)
                    {
                        txtFile = new Image();
                        txtFile.Source = FindResource("textFile") as BitmapImage;
                    }
                    return txtFile.Source;

                case FileManager.FileFormatType.Binary:
                    if (binaryFile == null)
                    {
                        binaryFile = new Image();
                        binaryFile.Source = FindResource("binaryFile") as BitmapImage;
                    }
                    return binaryFile.Source;

                case FileManager.FileFormatType.Unknown:
                default:
                    if (unknownFile == null)
                    {
                        unknownFile = new Image();
                        unknownFile.Source = FindResource("unknownFile") as BitmapImage;
                    }
                    return unknownFile.Source;
                }
            }
        }

        public TreeViewData(string fileOrDirName, string path, bool isFolder)
        {
            ID = $"{path}\\{fileOrDirName}".ToID();

            FullName = fileOrDirName;
            FileName = System.IO.Path.GetFileNameWithoutExtension(fileOrDirName);
            Path = path;

            if (isFolder)
            {
                fileFormatType = FileManager.FileFormatType.Folder;
                IsFolder = true;
                Items.Add(null); // для возможности развернуть
                //initial = Directory.GetDirectories($"{path}\\{folderName}", "*", SearchOption.TopDirectoryOnly).Length
                //        + Directory.GetFiles($"{path}\\{folderName}", "*.*", SearchOption.TopDirectoryOnly).Length;
            }
            else
            {
                fileFormatType = FileManager.DetectFileFormatType(fileOrDirName);
            }
        }
    }
}
