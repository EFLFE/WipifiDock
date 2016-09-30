using System;
using System.Threading;
using System.Threading.Tasks;
using Octokit;

namespace WipifiDock
{
    public static class Update
    {
        public static async Task<Tuple<bool, string>> Check()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var github = new GitHubClient(new ProductHeaderValue("WipifiDock"));
                    Task<Release> taskRelease = github.Repository.Release.GetLatest("EFLFE", "WipifiDock");

                    bool off = taskRelease.Wait(5000);
                    Release release = taskRelease.Result;

                    return new Tuple<bool, string>(false, null);
                }
                catch (Exception ex)
                {
                    Log.Write(ex.InnerException.ToString(), Log.MessageType.ERROR);
                    return new Tuple<bool, string>(false, null);
                }
            });
        }
    }
}
