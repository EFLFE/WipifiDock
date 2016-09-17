using System;
using System.IO;
using System.Text;

namespace WipifiDock
{
    public static class BlankGenerator
    {
        private const string BLANK_FILE = "Blanks\\MainBlank.html";

        private static StringBuilder sb = new StringBuilder();

        /// <summary> Сгенерировать HTML из шаблона с добавлением кода. </summary>
        /// <param name="title"> Текст заголовка страницы. </param>
        /// <param name="head"> Вставочный код блок HEAD. </param>
        /// <param name="body"> Вставочный код блок BODY. </param>
        private static void createBlank(string title, string[] head, string[] body)
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

        /// <summary> Сгенерировать HTML для просмотра изображения. </summary>
        /// <param name="imagePath"> Полный путь к файлу изображения. </param>
        /// <returns> HTML код. </returns>
        public static string ViewImage(string imagePath)
        {
            createBlank(
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
