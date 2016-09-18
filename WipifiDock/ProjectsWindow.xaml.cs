﻿using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonMark;
using System.Windows.Media;
using System.Threading;
using System.Collections.Generic;

namespace WipifiDock
{
    /// <summary> Окно самого проекта. </summary>
    public partial class ProjectsWindow : Window
    {
        private ProjectData projectData;
        private WebTab selectedWebTab;

        public ProjectsWindow()
        {
            InitializeComponent();
            treeView.MouseDoubleClick += TreeView_MouseDoubleClick;
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
        }

        // при выборе элемента в TreeView, открыть выбранный элемент
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            object item = treeView.SelectedItem;
            if (item != null)
            {
                if (item is TreeViewData.TreeViewDataFile)
                {
                    var tt = item as TreeViewData.TreeViewDataFile;
                    var path = tt.Path + "\\" + tt.Name;

                    if (tabControl.Items.Count == 1)
                    {
                        addWebItem();
                    }
                    selectedWebTab.NavigateToFile(path, true);
                }
            }
        }

        // open new tab and load selected tree item (middle?)
        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            object item = treeView.SelectedItem;
            if (item != null)
            {
                if (item is TreeViewData.TreeViewDataFile)
                {
                    var tt = item as TreeViewData.TreeViewDataFile;
                    var path = tt.Path + "\\" + tt.Name;

                    addWebItem();
                    selectedWebTab.NavigateToFile(path, true);
                }
            }
        }

        private void exitmenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            projectData = ProjectDataManager.GetSelectedProjectData();
            Title += projectData.Name;
            //selectedWebTab.navigateFile("MainPage.html");
            loadTree();
        }

        private void loadTree()
        {
            int i;
            string[] rootFiles = Directory.GetFiles(projectData.Path,"*.*", SearchOption.TopDirectoryOnly);
            string[] rootDirs = Directory.GetDirectories(projectData.Path, "*", SearchOption.TopDirectoryOnly);

            // root files
            for (i = 0; i < rootFiles.Length; i++)
            {
                treeView.Items.Add(new TreeViewData.TreeViewDataFile(
                    Path.GetFileName(rootFiles[i]),
                    projectData.Path));
            }
            for (i = 0; i < rootDirs.Length; i++)
            {
                treeView.Items.Add(new TreeViewData.TreeViewDataFolder(
                    Path.GetFileName(rootDirs[i]),
                    projectData.Path));
            }
        }

        private void convertMDtoHTML()
        {
            using (var reader = new StreamReader("source.md"))
            {
                using (var writer = new StreamWriter("result.html"))
                {
                    CommonMarkConverter.Convert(reader, writer);
                }
            }
        }

        private void urlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                selectedWebTab.NavigateToFile(urlTextBox.Text, false);
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            selectedWebTab?.GoBack();
        }

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            selectedWebTab?.GoForward();
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            selectedWebTab?.Refresh();
            updateEditButtons();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            selectedWebTab?.EnableEditMode();
            updateEditButtons();
        }

        private void toHtmlButton_Click(object sender, RoutedEventArgs e)
        {
            selectedWebTab?.DisableEditMode();
            updateEditButtons();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            if (tabControl.SelectedIndex == tabControl.Items.Count - 1)
            {
                addWebItem();
                return;
            }

            selectedWebTab = ((tabControl.SelectedItem as TabItem).Content as Grid).Children[0] as WebTab;
            updateEditButtons();
        }

        private void addWebItem()
        {
            (tabControl.Items[tabControl.Items.Count - 1] as TabItem).IsEnabled = false;

            var _tab = new TabItem();
            var _grid = new Grid();
            var _webTab = new WebTab(_tab);
            _webTab.OnNavigated += _webTab_OnNavigated;

            _grid.Children.Add(_webTab);
            _tab.Content = _grid;

            _tab.Header = new TextBlock()
            {
                Text = "blank"
            };

            tabControl.Items.Insert(tabControl.Items.Count - 1, _tab);
            tabControl.SelectedIndex = tabControl.Items.Count - 2;

            ThreadPool.QueueUserWorkItem(new WaitCallback((_) =>
            {
                // костыль, ибо я без понятия
                // почему при первом добавлении таба
                // он добавляет сразу два
                Thread.Sleep(99);
                Dispatcher.Invoke(() => { (tabControl.Items[tabControl.Items.Count - 1] as TabItem).IsEnabled = true; });
            }
            ));
        }

        private void _webTab_OnNavigated(string title)
        {
            updateEditButtons();
        }

        private void updateEditButtons()
        {
            if (selectedWebTab == null || !selectedWebTab.Navigated)
            {
                editButton.IsEnabled = false;
                toHtmlButton.IsEnabled = false;
            }
            else
            {
                if (selectedWebTab.EditMode)
                {
                    editButton.IsEnabled = false;
                    toHtmlButton.IsEnabled = true;
                }
                else
                {
                    editButton.IsEnabled = true;
                    toHtmlButton.IsEnabled = false;
                }
            }
        }

        private void addFolder_Click(object sender, RoutedEventArgs e)
        {
        }

        private void addDock_Click(object sender, RoutedEventArgs e)
        {
        }

        private void deleteDock_Click(object sender, RoutedEventArgs e)
        {
        }

        private void deleteFolder_Click(object sender, RoutedEventArgs e)
        {
        }

        // collapse/expand treeView
        private void collapse_Click(object sender, RoutedEventArgs e)
        {
            setTreeViewExpand(false);
        }

        private void expand_Click(object sender, RoutedEventArgs e)
        {
            setTreeViewExpand(true);
        }

        private void setTreeViewExpand(bool isExpanded)
        {
            foreach (var item in treeView.Items)
            {
                if (item is TreeViewData.TreeViewDataFolder)
                {
                    (item as TreeViewData.TreeViewDataFolder).IsExpanded = isExpanded;
                }
            }
        }

        // save
        private void saveProject_Click(object sender, RoutedEventArgs e)
        {
        }

        // open project dir
        private void openProjectFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(projectData.Path);
        }
    }
}