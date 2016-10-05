using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace WipifiDock.Forms
{
    public partial class AboutForm : Window
    {
        public AboutForm()
        {
            InitializeComponent();
            Loaded += AboutForm_Loaded;
            nameTextBlock.Text += App.Version;
        }

        private async void AboutForm_Loaded(object sender, RoutedEventArgs e)
        {
            asms.Text = await getVesrs();
        }

        private async Task<string> getVesrs()
        {
            return await Task.Run(() =>
            {
                var sb = new StringBuilder();
                sb.Append("WpfAnimatedGif: ");
                sb.AppendLine(typeof(WpfAnimatedGif.ImageBehavior).Assembly.GetName().Version.ToString());
                sb.Append("Octokit:        ");
                sb.AppendLine(typeof(Octokit.Account).Assembly.GetName().Version.ToString());
                sb.Append("Markdig:        ");
                sb.AppendLine(typeof(Markdig.Markdown).Assembly.GetName().Version.ToString());
                sb.Append("ScintillaNET:   ");
                sb.AppendLine(typeof(ScintillaNET.Scintilla).Assembly.GetName().Version.ToString());
                sb.Append("CefSharp:       ");
                sb.AppendLine(typeof(CefSharp.Wpf.ChromiumWebBrowser).Assembly.GetName().Version.ToString());
                return sb.ToString();
            });
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }
    }
}
