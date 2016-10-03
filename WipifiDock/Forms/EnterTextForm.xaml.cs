using System;
using System.Windows;

namespace WipifiDock.Forms
{
    /// <summary> Форма для ввода текста. </summary>
    public partial class EnterTextForm : Window
    {
        public string GetEnteredText { get; private set; }

        public EnterTextForm(string initialText = "")
        {
            InitializeComponent();
            textBox.Text = initialText;
            GetEnteredText = null;

            okButton.Click += OkButton_Click;
            cancelButton.Click += CancelButton_Click;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            GetEnteredText = textBox.Text;
            Close();
        }
    }
}
