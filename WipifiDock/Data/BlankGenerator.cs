using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Security.Permissions;
using Markdig;
using Markdig.Extensions;

namespace WipifiDock.Data
{
    public static class BlankGenerator
    {
        private const string BLANK_FILE = "Blanks\\MainBlank.html";

        public const string TITLE_LINE = "#TITLE#";
        public const string HEAD_LINE = "#HEAD#";
        public const string BODY_LINE = "#BODY#";
        public const string FOOTER_LINE = "#FOOTER#";
        public const string MD_LINE = "#MD#";

        private static string[] blankLines;
        private static string headText, bodyText, footerText;
        private static FileSystemWatcher fileWatcher;
        private static StringBuilder sb = new StringBuilder();

        private static MarkdownPipeline pipeline;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void InitBlankGenerator()
        {
            Log.Write("Init BlankGenerator");

            try
            {
                sb = new StringBuilder();
                headText = string.Empty;
                bodyText = string.Empty;
                footerText = string.Empty;

                Log.Write("Start FileSystemWatcher in \"" + FileManager.RootPath + "\"");

                fileWatcher = new FileSystemWatcher(FileManager.RootPath);
                fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                fileWatcher.Filter = "*.html";

                fileWatcher.Created += fileSystemWatcher_clearText;
                fileWatcher.Deleted += fileSystemWatcher_clearText;
                fileWatcher.Renamed += fileSystemWatcher_renamed;
                fileWatcher.Changed += fileSystemWatcher_readText;

                blankLines = File.ReadAllLines(BLANK_FILE);

                fileWatcher.EnableRaisingEvents = true;

                // read
                if (File.Exists(FileManager.RootPath + "\\" + FileManager.HEAD_FILE))
                    headText = File.ReadAllText(FileManager.RootPath + "\\" + FileManager.HEAD_FILE);

                if (File.Exists(FileManager.RootPath + "\\" + FileManager.BODY_FILE))
                    bodyText = File.ReadAllText(FileManager.RootPath + "\\" + FileManager.BODY_FILE);

                if (File.Exists(FileManager.RootPath + "\\" + FileManager.FOOTER_FILE))
                    footerText = File.ReadAllText(FileManager.RootPath + "\\" + FileManager.FOOTER_FILE);
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString(), Log.MessageType.ERROR);
            }
        }

        private static void fileSystemWatcher_readText(object sender, FileSystemEventArgs e)
        {
            switch (e.Name)
            {
            case FileManager.HEAD_FILE:
                headText = File.ReadAllText(e.FullPath);
                break;

            case FileManager.BODY_FILE:
                bodyText = File.ReadAllText(e.FullPath);
                break;

            case FileManager.FOOTER_FILE:
                footerText = File.ReadAllText(e.FullPath);
                break;

            default: return;
            }
#if DEBUG
            Log.Write("FileSystemWatcher: " + e.ChangeType + " " + e.Name);
#endif
            Log.Write("Read " + e.Name);
        }

        private static void fileSystemWatcher_renamed(object sender, RenamedEventArgs e)
        {
            switch (e.OldName)
            {
            case FileManager.HEAD_FILE:
                headText = string.Empty;
                break;

            case FileManager.BODY_FILE:
                bodyText = string.Empty;
                break;

            case FileManager.FOOTER_FILE:
                footerText = string.Empty;
                break;

            default: return;
            }
#if DEBUG
            Log.Write("FileSystemWatcher: Rename " + e.OldName + " to " + e.Name);
#endif
        }

        private static void fileSystemWatcher_clearText(object sender, FileSystemEventArgs e)
        {
            switch (e.Name)
            {
            case FileManager.HEAD_FILE:
                headText = string.Empty;
                break;

            case FileManager.BODY_FILE:
                bodyText = string.Empty;
                break;

            case FileManager.FOOTER_FILE:
                footerText = string.Empty;
                break;

            default: return;
            }
#if DEBUG
            Log.Write("FileSystemWatcher: " + e.ChangeType + " " + e.Name);
#endif
        }

        /// <summary> Сгенерировать HTML из шаблона с добавлением кода. </summary>
        /// <param name="title"> Текст заголовка страницы. </param>
        /// <param name="head"> Вставочный код блок HEAD. </param>
        /// <param name="body"> Вставочный код блок BODY. </param>
        private static void createHtml(string title, string[] head, string[] body)
        {
            //Log.Write("Create HTML " + title);

            sb.Clear();

            for (int i = 0; i < blankLines.Length; i++)
            {
                if (blankLines[i].Contains(TITLE_LINE))
                {
                    // TITLE
                    sb.AppendLine(blankLines[i].Replace(TITLE_LINE, title == null ? "Blank" : title));
                }
                else if (blankLines[i].Contains(HEAD_LINE))
                {
                    // HEAD
                    sb.AppendLine(headText);
                    if (head != null && head.Length > 0)
                    {
                        for (int j = 0; j < head.Length; j++)
                        {
                            sb.AppendLine(head[j]);
                        }
                    }
                }
                else if (blankLines[i].Contains(BODY_LINE))
                {
                    // BODY
                    sb.AppendLine(bodyText);
                    if (body != null && body.Length > 0)
                    {
                        for (int j = 0; j < body.Length; j++)
                        {
                            sb.AppendLine(body[j]);
                        }
                    }
                }
                else if (blankLines[i].Contains(FOOTER_LINE))
                {
                    // FOOTER (end of body)
                    sb.AppendLine(footerText);
                }
                else if (blankLines[i].Contains(MD_LINE))
                {
                }
                else
                {
                    sb.AppendLine(blankLines[i]);
                }
            }
        }

        /// <summary> Сгенерировать HTML cо своим набором кода. </summary>
        /// <param name="title"> Текст заголовка страницы. </param>
        /// <param name="head"> Вставочный код блок HEAD. </param>
        /// <param name="body"> Вставочный код блок BODY. </param>
        public static string HTML(string title, string[] head, string[] body)
        {
            createHtml(title, head, body);
            return sb.ToString();
        }

        /// <summary> Сгенерировать HTML из MarkDown. </summary>
        /// <param name="title"> Текст заголовка страницы. </param>
        /// <param name="markDownText"> MarkDown текст. </param>
        public static string MD(string title, string markDownText)
        {
            if (pipeline == null)
                pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            markDownText = Markdown.ToHtml(markDownText, pipeline);

            createHtml(title, null, new[] { markDownText });
            return sb.ToString();
        }

        public static string IMAGE(string file)
        {
            createHtml(
                Path.GetFileName(file),
                null,
                new[] { $"<img src=\"file:\\\\\\{file}\" alt=\"{Path.GetFileName(file)}\">" }
                );
            return sb.ToString();
        }

    }
}
