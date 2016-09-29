using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WipifiDock.Forms
{
    public partial class LoggerForm : Window
    {
        private static int threadID;

        public bool CanClosed;

        public LoggerForm()
        {
            InitializeComponent();
            Loaded += LoggerForm_Loaded;
            threadID = Thread.CurrentThread.ManagedThreadId;
            Closing += LoggerForm_Closing;
        }

        private void LoggerForm_Closing(object sender, CancelEventArgs e)
        {
            if (!CanClosed)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void LoggerForm_Loaded(object sender, RoutedEventArgs e)
        {
            Log.OnAddLog += Log_OnAddLog;
            Log.Write("LoggerForm monitor started in Thread ID: " + threadID);
        }

        private void Log_OnAddLog(string logText)
        {
            logText = logText.Insert(logText.IndexOf(']'), "|" + Thread.CurrentThread.ManagedThreadId);

            if (threadID == Thread.CurrentThread.ManagedThreadId)
            {
                textBox.Text += logText + Environment.NewLine;
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    textBox.Text += logText + Environment.NewLine;
                });
            }
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Topmost = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Log.OnAddLog -= Log_OnAddLog;
            Log.Write("LoggerForm monitor stoped.");
            base.OnClosing(e);
        }

    }
}
