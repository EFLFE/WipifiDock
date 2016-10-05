using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Octokit;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace WipifiDock.Forms
{
    public partial class UpdateForm : Window
    {
        public UpdateForm()
        {
            InitializeComponent();
            contentGrid.Opacity = 0.0;
            loadImage.Opacity = 0.0;
            successImage.Opacity = 0.0;
            Loaded += UpdateForm_Loaded;
        }

        private async void UpdateForm_Loaded(object sender, RoutedEventArgs e)
        {
            // show loadImage
            var da = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.5)));
            loadImage.BeginAnimation(OpacityProperty, da);

            Tuple<string, Release> release = await getLatestRelease();

            // hide loadImage
            var da2 = new DoubleAnimation(loadImage.Opacity, 0.0, new Duration(TimeSpan.FromSeconds(0.2)));
            loadImage.BeginAnimation(OpacityProperty, da2);

            if (release.Item1 == null)
            {
                // was found
                if (!isNewVersion(release.Item2.TagName))
                {
                    headText.Text = "У вас последняя версия программы.";

                    // show successImage
                    var da3 = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.2)));
                    successImage.BeginAnimation(OpacityProperty, da3);

                    return;
                }

                Log.Write("Found the latest version");

                successImage.Visibility = Visibility.Hidden;

                var gridAnim = new DoubleAnimation(0.0, 1.0,new Duration(TimeSpan.FromSeconds(0.2)));
                gridAnim.BeginTime = TimeSpan.FromSeconds(0.2);
                contentGrid.BeginAnimation(OpacityProperty, gridAnim);

                headText.Text = "Найдена новая версия! " + release.Item2.TagName;
                titleText.Text = release.Item2.Name;
                descText.Text = release.Item2.Body;

                // add url's
                for (int i = 0; i < release.Item2.Assets.Count; i++)
                {
                    urlList.Children.Add(createHyperTextBlock(release.Item2.Assets[i].Name, release.Item2.Assets[i].BrowserDownloadUrl));
                }
                urlList.Children.Add(createHyperTextBlock("Source code (zip)", release.Item2.ZipballUrl));
                urlList.Children.Add(createHyperTextBlock("Source code (tar.gz)", release.Item2.TarballUrl));
            }
            else
            {
                // error
                headText.Text = release.Item1;
                reCheckButton.IsEnabled = true;
            }
        }

        private TextBlock createHyperTextBlock(string text, string url)
        {
            var textBlock = new TextBlock();
            var hyperlink = new Hyperlink();

            hyperlink.NavigateUri = new Uri(url);
            hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
            hyperlink.Inlines.Add(text);

            textBlock.Inlines.Add(hyperlink);

            return textBlock;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private bool isNewVersion(string ver)
        {
            var thisV = new Version(App.Version);
            var newVer = new Version(ver);
            return newVer > thisV;
        }

        /// <summary> Проверить обновление. Возвращает Tuple[string: текст ошибки или null, [Release или null в случае ошибки]] </summary>
        private static async Task<Tuple<string, Release>> getLatestRelease()
        {
            Log.Write("Search last version");
            return await Task.Run(() =>
            {
                try
                {
                    var github = new GitHubClient(new ProductHeaderValue("WipifiDock"));
                    Task<Release> taskRelease = github.Repository.Release.GetLatest("EFLFE", "WipifiDock");

                    bool until = taskRelease.Wait(5000);
                    if (!until)
                        return new Tuple<string, Release>("Ошибка - Время ожидания истекло.", null);

                    return new Tuple<string, Release>(null, taskRelease.Result);
                }
                catch (Exception ex)
                {
                    Log.Write(ex.InnerException.ToString(), Log.MessageType.ERROR);
                    return new Tuple<string, Release>("Ошибка - " + ex.InnerException.Message, null);
                }
            });
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void reCheckButton_Click(object sender, RoutedEventArgs e)
        {
            reCheckButton.IsEnabled = false;
            UpdateForm_Loaded(null, null);
        }
    }
}
