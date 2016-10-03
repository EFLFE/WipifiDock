using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WipifiDock
{
    /// <summary> Логирование. </summary>
    public static class Log
    {
        private static readonly List<string> logs;
        private static int[] typeLogCount;

        public delegate void DelOnAddLog(string logText);
        public static event DelOnAddLog OnAddLog;

        static Log()
        {
            logs = new List<string>();
            typeLogCount = new int[4];
        }

        /// <summary> Последняя ошибка. </summary>
        public static string LastError { get; private set; }

        /// <summary> Выводить лог в консоль </summary>
        public static bool EnableOutWrite;

        /// <summary> Записывать сообщение в прослушиватели трассировки в коллекции <see cref="P:System.Diagnostics.Trace.Listeners"/>. </summary>
        public static bool EnableTrace;

        /// <summary> GetTexture full log </summary>
        public static string GetFullLog
        {
            get
            {
                if (logs == null)
                    return string.Empty;

                var sb = new StringBuilder( "Export at " + DateTime.Now.ToString( "yyyy MM dd  HH\n" ) );
                for (int i = 0; i < logs.Count; i++)
                {
                    sb.AppendLine(logs[i]);
                }

                return sb.ToString();
            }
        }

        /// <summary> Очистить лог. </summary>
        public static void ClearLog()
        {
            logs.Clear();
            typeLogCount = new int[4];
            if (EnableOutWrite)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /// <summary> Тип лог сообщения. </summary>
        public enum MessageType
        {
            LOG,
            WARNING,
            ERROR,
            NOTE,
        }

        /// <summary> Записать лог. </summary>
        /// <param name="text">Текст.</param>
        /// <param name="messType">Тип сообщения.</param>
        public static void Write(string text, MessageType messType = MessageType.LOG)
        {
            var s = string.Format( "{0:D4} {1:D2}:{2:D2}:{3:D2}:{4:D3} [{6}] {5}", logs.Count + 1, DateTime.Now.Hour,
                DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond, text, messType );

            switch (messType)
            {
            case MessageType.LOG:
                typeLogCount[0]++;
                if (EnableOutWrite && Console.ForegroundColor != ConsoleColor.Gray)
                    Console.ForegroundColor = ConsoleColor.Gray;

                if (EnableTrace)
                    Trace.WriteLine(s);
                break;

            case MessageType.WARNING:
                typeLogCount[1]++;
                if (EnableOutWrite && Console.ForegroundColor != ConsoleColor.Yellow)
                    Console.ForegroundColor = ConsoleColor.White;

                if (EnableTrace)
                    Trace.TraceWarning(s);
                break;

            case MessageType.ERROR:
                LastError = text;
                typeLogCount[2]++;
                if (EnableOutWrite && Console.ForegroundColor != ConsoleColor.Red)
                    Console.ForegroundColor = ConsoleColor.Red;

                if (EnableTrace)
                    Trace.TraceError(s);
                break;

            case MessageType.NOTE:
                typeLogCount[3]++;
                if (EnableOutWrite && Console.ForegroundColor != ConsoleColor.White)
                    Console.ForegroundColor = ConsoleColor.White;

                if (EnableTrace)
                    Trace.TraceInformation(s);
                break;
            }

            logs.Add(s);
            OnAddLog?.Invoke(s);

            if (EnableOutWrite)
                Console.WriteLine(s);
        }

        /// <summary> Export log in txt. Return full path. </summary>
        /// <param name="path"> Full path or dir name. (C:\\...) </param>
        /// <exception cref="IOException">Каталог, заданный параметром <paramref name="path" />, доступен только для чтения.</exception>
        /// <exception cref="UnauthorizedAccessException">Вызывающий оператор не имеет необходимого разрешения. </exception>
        /// <exception cref="DirectoryNotFoundException">Указанный путь недопустим (например, он соответствует неподключенному диску). </exception>
        public static string SaveLog(string path)
        {
            var fullPath = path + "\\" + DateTime.Now.ToString( "yyyy_MM_dd__HH_mm_ss" ) + ".log.txt";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (var sw = File.CreateText(fullPath))
            {
                sw.WriteLine($"Log: LOG({typeLogCount[0]}), WARNING({typeLogCount[1]}), ERROR({typeLogCount[2]}), NOTE({typeLogCount[3]}).");
                sw.WriteLine(GetFullLog);
            }

            return fullPath;
        }
    }
}