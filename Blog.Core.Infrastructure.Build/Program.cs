using System.Collections.Generic;
using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV1s;

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

                Jobs = new Jobs
                {
                    Build = new BuildJob
                    {
                        RunsOn = BuildMachines.WindowsLatest,

                        Steps = new List<GithubTask>
                        {
                          new CheckoutTaskV2
                          {
                              Name = "Pulling Code"
                          },

                          new SetupDotNetTaskV1
                          {
                              Name = "Installing .NET",
                              TargetDotNetVersion = new TargetDotNetVersion
                              {
                                  DotNetVersion = "7.0.202"
                              }
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
            };

            adotNetClient.SerializeAndWriteToFile(
                githubPipeline,
                path: "../../../../.github/workflows/provision.yml"
                );
        }
    }
}