using System.Collections.Generic;
using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV3s;

namespace Blog.Core.Infrastructure.Build
{
    public class Program
    {
        static void Main(string[] args)
        {
            var adotNetClient = new ADotNetClient();

            var githubPipeline = new GithubPipeline
            {
                Name = "Blog.Core build",

                OnEvents = new Events
                {
                    Push = new PushEvent
                    {
                        Branches = new string[] { "master" }
                    },

                    PullRequest = new PullRequestEvent
                    {
                        Branches = new string[] { "master" }
                    }
                },

                Jobs = new Dictionary<string, Job>
                {
                    {
                        "build",
                        new Job
                        {
                            RunsOn = BuildMachines.WindowsLatest,

                            Steps = new List<GithubTask>
                            {
                                new CheckoutTaskV3
                                {
                                    Name = "Check out"
                                },

                                new SetupDotNetTaskV3
                                {
                                    Name = "Installing .NET",
                                    With = {DotNetVersion = "7.0.202" }
                                },

                                new RestoreTask
                                {
                                    Name = "Restoring .NET Packages"
                                },

                                new DotNetBuildTask
                                {
                                    Name = "Building Solution"
                                },

                                new TestTask
                                {
                                    Name = "Running Tests"
                                }
                            }
                        }
                    }
                }
            };

            adotNetClient.SerializeAndWriteToFile(
                githubPipeline,
                path: "../../../../.github/workflows/provision.yml"
                );
        }
    }
}