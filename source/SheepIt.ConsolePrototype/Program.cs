using System;
using CommandLine;

namespace SheepIt.ConsolePrototype
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine();

            Parser.Default
                .ParseArguments<
                    CreateReleaseOptions,
                    DeployReleaseOptions,
                    ShowDashboardOptions,
                    CreateProjectOptions,
                    ListReleasesOptions,
                    ListDeploymentsOptions,
                    ListProjectsOptions
                >(args)
                .WithParsed<CreateProjectOptions>(CreateProject.Run)
                .WithParsed<CreateReleaseOptions>(CreateRelease.Run)
                .WithParsed<DeployReleaseOptions>(DeployRelease.Run)
                .WithParsed<ShowDashboardOptions>(ShowDashboard.Run)
                .WithParsed<ListReleasesOptions>(ListReleases.Run)
                .WithParsed<ListDeploymentsOptions>(ListDeployments.Run)
                .WithParsed<ListProjectsOptions>(ListProjects.Run)
                .WithNotParsed(errors => {});
        }
    }
}
