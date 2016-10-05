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
        public const string BASETAG_LINE = "#BASE#";
        public const string BODY_LINE = "#BODY#";
        public const string FOOTER_LINE = "#FOOTER#";
        //public const string MD_LINE = "#MD#";

        private static string[] blankLines;
        private static string headText, bodyText, footerText;
        private static FileSystemWatcher fileWatcher;
        private static StringBuilder sb = new StringBuilder();
        private static string projectPath;
        private static MarkdownPipeline pipeline;
        private static object _lock_ = new object();

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void InitBlankGenerator(string _projectPath)
        {
            Log.Write("Init BlankGenerator");
            projectPath = _projectPath;

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
                fileWatcher.Changed += fileSystemWatcher_readText; // todo: событие вызывается два раза при сохранении файла и я хз почему

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
            lock (_lock_)
            {
                Log.Write($"Read {e.Name} ({e.ChangeType})");
                try
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
                }
                catch (Exception ex)
                {
                    Log.Write(ex.Message, Log.MessageType.ERROR);
                }
            }
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
        private static void createHtml(string title, string[] head, string[] body, int pathlevel)
        {
            //Log.Write("Create HTML " + title);

            // тэг base
            string baseText = string.Empty;
            if (pathlevel > 0)
            {
                for (int i = 0; i < pathlevel; i++)
                {
                    baseText += "../";
                }
            }

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
                    sb.AppendLine(headText.Replace(BASETAG_LINE, $"<base href=\"{baseText}\" target=\"_blank\">\n"));

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
                //else if (blankLines[i].Contains(MD_LINE))
                //{
                //}
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
        public static string HTML(string title, string[] head, string[] body, int pathlevel)
        {
            createHtml(title, head, body, pathlevel);
            return sb.ToString();
        }

        /// <summary> Сгенерировать HTML из MarkDown. </summary>
        /// <param name="title"> Текст заголовка страницы. </param>
        /// <param name="markDownText"> MarkDown текст. </param>
        public static string MD(string title, string markDownText, int pathlevel)
        {
            if (pipeline == null)
                pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            markDownText = Markdown.ToHtml(markDownText, pipeline);

            createHtml(title, null, new[] { markDownText }, pathlevel);
            return sb.ToString();
        }

        public static string IMAGE(string file)
        {
            createHtml(
                Path.GetFileName(file),
                null,
                new[] { $"<img src=\"file:\\\\\\{file}\" alt=\"{Path.GetFileName(file)}\">" },
                0);
            return sb.ToString();
        }

        public static int GetPathLevel(string path)
        {
            //    0       1    2
            // ..\project\dir1\dir2
            path = path.Remove(0, projectPath.Length + 1);
            return path.Count(c => c == '\\');
        }

    }
}
