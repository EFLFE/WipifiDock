using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using WipifiDock.Forms;
using WipifiDock.Pages;

namespace WipifiDock
{
    public partial class MainWipifiWindow : Window
    {
        public enum PageType
        {
            MainPage,
            ProjectListPage,
            CreateNewProjectPage,
            ProjectPage
        }

        public MainPage mainPage;
        public ProjectListPage projectListPage;
        public CreateNewProjectPage createNewProjectPage;
        public ProjectPage projectPage;

        private LoggerForm loggerForm;

        // animation
        private bool                        _allowDirectNavigation = false;
        private NavigatingCancelEventArgs   _navArgs = null;
        private Duration                    _duration = new Duration(TimeSpan.FromSeconds(0.1));

        public MainWipifiWindow()
        {
            InitializeComponent();

            loggerForm = new LoggerForm();
            mainPage = new MainPage(this);
            projectListPage = new ProjectListPage(this);
            createNewProjectPage = new CreateNewProjectPage(this);
            projectPage = new ProjectPage();

            frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            frame.Navigating += frame_Navigating;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            frame.Navigate(mainPage);
            loggerForm.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            loggerForm.Close();
            base.OnClosed(e);
        }

        public void FrameNavigate(PageType pageType)
        {
            Log.Write("Frame navigate to " + pageType);
            switch (pageType)
            {
            case PageType.MainPage:
                frame.Navigate(mainPage);
                break;

            case PageType.ProjectListPage:
                frame.Navigate(projectListPage);
                break;

            case PageType.CreateNewProjectPage:
                frame.Navigate(createNewProjectPage);
                break;

            case PageType.ProjectPage:
                frame.Navigate(projectPage);
                break;
            }
        }

        // page animation
        private void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri != null)
            {
                e.Cancel = true;
                return;
            }

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
    }
}
