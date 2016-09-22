using System;
using System.IO;
using System.Text;
using System.Windows;

namespace WipifiDock.Data
{
    public static class BlankGenerator
    {
        // TODO: позже сделать кэш что бы не генерировать заного одинаковый файл

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

        /// <summary> Сгенерировать HTML cо своим набором кода. </summary>
        /// <param name="title"> Текст заголовка страницы. </param>
        /// <param name="head"> Вставочный код блок HEAD. </param>
        /// <param name="body"> Вставочный код блок BODY. </param>
        public static string Custom(string title, string[] head, string[] body)
        {
            createBlank(title, head, body);
            return sb.ToString();
        }

        /// <summary> Сгенерировать HTML для просмотра изображения. </summary>
        /// <param name="imagePath"> Полный путь к файлу изображения. </param>
        /// <returns> HTML код. </returns>
        public static string ViewImage(string imagePath)
        {
            createBlank(
                title: Path.GetFileName(imagePath),
                head: null,
                body: new string[]
                {
                    $"<img src=\"{imagePath}\" alt=\"{Path.GetFileName(imagePath)}\" />"
                });
            return sb.ToString();
        }

        /// <summary> Сгенерировать HTML для неизвестного формата файла. </summary>
        /// <param name="imagePath"> Полный путь к файлу. </param>
        /// <returns> HTML код. </returns>
        public static string UnknownFileInfo(string path)
        {
            createBlank(
                title: Path.GetFileName(path),
                head: null,
                body: new string[]
                {
                    $"<h2>Неизветный формта файла {Path.GetFileName(path)}</h2>"
                });
            return sb.ToString();
        }

        /// <summary> Сгенерировать HTML для файла который не был найден. </summary>
        /// <param name="imagePath"> Полный путь к фейковому файлу. </param>
        /// <returns> HTML код. </returns>
        public static string FileNotFouund(string path)
        {
            createBlank(
                title: Path.GetFileName(path),
                head: null,
                body: new string[]
                {
                    $"<h2>Файл {Path.GetFileName(path)} не найден</h2>"
                });
            return sb.ToString();
        }

        /// <summary> Сгенерировать HTML для просмотра текстового файла. </summary>
        /// <param name="imagePath"> Полный путь к фейковому файлу. </param>
        /// <returns> HTML код. </returns>
        public static string Text(string path)
        {
            createBlank(
                title: Path.GetFileName(path),
                head: null,
                body: new string[]
                {
                    "<p>",
                    File.ReadAllText(path),
                    "</p>"
                });
            return sb.ToString();
        }

        /// <summary> Сгенерировать HTML для просмотра текстового файла. </summary>
        /// <param name="title"> Заголовок. </param>
        /// <param name="message"> Текст ошибки. </param>
        /// <returns> HTML </returns>
        public static string Error(string title, string message)
        {
            createBlank(
                title: title,
                head: null,
                body: new string[]
                {
                    "<p>",
                    "Ошибка! " + message,
                    "</p>"
                });
            return sb.ToString();
        }

    }
}
