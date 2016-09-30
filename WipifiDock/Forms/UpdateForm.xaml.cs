﻿using System;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Octokit;
using System.Diagnostics;

namespace WipifiDock.Forms
{
    public partial class UpdateForm : Window
    {
        private string download_assetUri = null;
        private string download_zipUri = null;
        private string download_tarUri = null;

        public UpdateForm()
        {
            InitializeComponent();
            contentGrid.Opacity = 0.0;
            loadImage.Opacity = 0.0;
            Loaded += UpdateForm_Loaded;
        }

        private async void UpdateForm_Loaded(object sender, RoutedEventArgs e)
        {
            var da = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.5)));
            loadImage.BeginAnimation(OpacityProperty, da);

            Tuple<string, Release> release = await getLatestRelease();

            da = new DoubleAnimation(loadImage.Opacity, 0.0, new Duration(TimeSpan.FromSeconds(0.2)));
            loadImage.BeginAnimation(OpacityProperty, da);

            if (release.Item1 == null)
            {
                // was found
                if (!isNewVersion(release.Item2.TagName))
                {
                    headText.Text = "У вас последняя версия программы.";
                    return;
                }

                Log.Write("Found the latest version");
                showContentGrid();

                headText.Text = "О боже мой! Новая версия! " + release.Item2.TagName;
                titleText.Text = release.Item2.Name;
                descText.Text = release.Item2.Body;

                if (release.Item2.Assets.Count > 0)
                {
                    exeUrl.Text = release.Item2.Assets[0].Name;
                }

                download_assetUri = release.Item2.Assets[0].BrowserDownloadUrl;
                download_zipUri = release.Item2.ZipballUrl;
                download_tarUri = release.Item2.TarballUrl;
            }
            else
            {
                // error
                headText.Text = release.Item1;
                reCheckButton.IsEnabled = true;
                download_assetUri = null;
                download_zipUri = null;
                download_tarUri = null;
            }
        }

        private void showContentGrid()
        {
            const double TIME = 0.2;

            var gridAnim = new DoubleAnimation(0.0, 1.0,new Duration(TimeSpan.FromSeconds(TIME)));
            gridAnim.BeginTime = TimeSpan.FromSeconds(TIME);
            contentGrid.BeginAnimation(OpacityProperty, gridAnim);
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

        private void exeUrl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (download_assetUri != null)
                Process.Start(download_assetUri);
        }

        private void zipUrl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (download_zipUri != null)
                Process.Start(download_zipUri);
        }

        private void tarUrl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (download_tarUri != null)
                Process.Start(download_tarUri);
        }

        private void reCheckButton_Click(object sender, RoutedEventArgs e)
        {
            reCheckButton.IsEnabled = false;
            UpdateForm_Loaded(null, null);
        }
    }
}