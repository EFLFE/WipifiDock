using System;
using System.IO;
using System.Text;

namespace WipifiDock
{
    public static class BlankGenerator
    {
        private const string BLANK_FILE = "Blanks\\MainBlank.html";

        private static StringBuilder sb = new StringBuilder();

        private static void loadBlank(string title, string[] head, string[] body)
        {
            sb.Clear();
            string[] txt = File.ReadAllLines(BLANK_FILE);
            for (int i = 0; i < txt.Length; i++)
            {
                if (txt[i].Contains("#TITLE#"))
                {
                    sb.AppendLine(txt[i].Replace("#TITLE#", title == null ? "Blank" : title));
                }
                else if (txt[i].Contains("#HEAD#"))
                {
                    if (head != null && head.Length > 0)
                    {
                        for (int j = 0; j < head.Length; j++)
                        {
                            sb.AppendLine(head[j]);
                        }
                    }
                }
                else if (txt[i].Contains("#BODY#"))
                {
                    if (body != null && body.Length > 0)
                    {
                        for (int j = 0; j < body.Length; j++)
                        {
                            sb.AppendLine(body[j]);
                        }
                    }
                }
                else
                {
                    sb.AppendLine(txt[i]);
                }
            }
        }

        public static string ViewImage(string imagePath)
        {
            loadBlank(
                Path.GetFileName(imagePath),
                null,
                new string[]
                {
                    $"<img src=\"{imagePath}\" alt=\"{Path.GetFileName(imagePath)}\" />"
                });
            return sb.ToString();
        }

    }
}
