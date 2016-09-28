﻿using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WipifiDock.Data;

namespace WipifiDock.Controls.TreeViewData
{
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

        /// <summary> Получить иконфу папки. </summary>
        public ImageSource GetImage
        {
            get
            {
                if (folderImage == null)
                {
                    folderImage = new Image();
                    folderImage.Source = FindResource("miniFolder") as BitmapImage;
                }
                return folderImage.Source;
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
        private static Image webFile;
        private static Image styleFile;
        private static Image imageFile;
        private static Image mdFile;
        private static Image txtFile;
        private static Image unknownFile;

        private FileManager.FileFormatType fileFormatType;

        public FileManager.FileFormatType GetProjectFileFormatType => fileFormatType;

        public string FileName { get; set; }

        public string FullFileName { get; set; }

        public string Path { get; set; }

        public ImageSource GetImage
        {
            get
            {
                switch (fileFormatType)
                {
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

        public TreeViewDataFile(string fileName, string path)
        {
            FullFileName = fileName;
            FileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
            Path = path;
            fileFormatType = FileManager.DetectFileFormatType(fileName);
        }
    }
}
