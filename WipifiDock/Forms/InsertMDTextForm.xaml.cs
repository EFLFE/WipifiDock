﻿using System;
using System.Windows;

namespace WipifiDock.Forms
{
    /// <summary> Окно генерации вставки текстового шаблона. </summary>
    public sealed partial class InsertMDTextForm : Window
    {
        /// <summary> Статический this. </summary>
        public static InsertMDTextForm Instance;

        /// <summary> Готовый текст для вставки. </summary>
        public static string GetInsertText;

        public InsertMDTextForm()
        {
            InitializeComponent();
        }

        public void SelectTab(int index)
        {
            if (index < tab.Items.Count && index > -2)
            {
                tab.SelectedIndex = index;
            }
        }

        private void imageUrlButton_Click(object sender, RoutedEventArgs e)
        {
            if (tab.SelectedIndex == 0)
            {
                GetInsertText = $"![{altUrlTextBox.Text}]({imageUrlTextBox.Text})";
            }
            else if (tab.SelectedIndex == 1)
            {
                GetInsertText = $"[{urlTextTextBox.Text}]({urlTextBox.Text})";
            }
            else
            {
                cancelButton_Click(null, null);
                return;
            }
            clear();
            Hide();
        }

        private void clear()
        {
            altUrlTextBox.Text = string.Empty;
            imageUrlTextBox.Text = string.Empty;
            urlTextTextBox.Text = string.Empty;
            urlTextBox.Text = string.Empty;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            GetInsertText = string.Empty;
            clear();
            Hide();
        }
    }
}
