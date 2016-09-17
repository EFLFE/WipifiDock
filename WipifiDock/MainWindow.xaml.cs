﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace WipifiDock
{
    public partial class MainWindow : Window
    {
        private MainPage1 page1;
        private MainPage2 page2;

        private bool                        _allowDirectNavigation = false;
        private NavigatingCancelEventArgs   _navArgs = null;
        private Duration                    _duration = new Duration(TimeSpan.FromSeconds(0.2));

        public MainWindow()
        {
            InitializeComponent();

            page1 = new MainPage1();
            page2 = new MainPage2();

            page1.NavigateToPage2 += Page1_NavigateToPage2;
            page1.ProjectWasSelected += Page1_ProjectWasSelected;

            page2.NavigateToPage1 += Page2_NavigateToPage1;
            page2.OnCreateNewProject += Page2_OnCreateNewProject;

            frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }

        private void Page1_ProjectWasSelected(object sender, EventArgs e)
        {
            Close();
        }

        private void Page2_OnCreateNewProject(string name)
        {
            page1.AddProfile(name);
        }

        private void Page2_NavigateToPage1(object sender, EventArgs e)
        {
            frame.Navigate(page1);
        }

        private void Page1_NavigateToPage2(object sender, EventArgs e)
        {
            frame.Navigate(page2);
        }

        private void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (Content != null && !_allowDirectNavigation)
            {
                e.Cancel = true;

                _navArgs = e;

                DoubleAnimation animation0 = new DoubleAnimation();
                animation0.From = frame.Opacity;
                animation0.To = 0;
                animation0.Duration = _duration;
                animation0.Completed += SlideCompleted;
                frame.BeginAnimation(OpacityProperty, animation0);
            }
            _allowDirectNavigation = false;
        }

        private void SlideCompleted(object sender, EventArgs e)
        {
            _allowDirectNavigation = true;
            switch (_navArgs.NavigationMode)
            {
            case NavigationMode.New:
                if (_navArgs.Uri == null)
                    frame.Navigate(_navArgs.Content);
                else
                    frame.Navigate(_navArgs.Uri);
                break;
            case NavigationMode.Back:
                frame.GoBack();
                break;
            case NavigationMode.Forward:
                frame.GoForward();
                break;
            case NavigationMode.Refresh:
                frame.Refresh();
                break;
            }

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)delegate ()
                {
                    DoubleAnimation animation0 = new DoubleAnimation();
                    animation0.From = 0;
                    animation0.To = 1.0;
                    animation0.Duration = _duration;
                    frame.BeginAnimation(OpacityProperty, animation0);
                });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            frame.Navigate(page1);
        }
    }
}
