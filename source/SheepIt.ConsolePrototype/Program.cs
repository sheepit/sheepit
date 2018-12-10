using System;
using CommandLine;
using SheepIt.ConsolePrototype.Cli;

namespace SheepIt.ConsolePrototype
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine();

            Parser.Default
                .ParseArguments<
                    CreateProjectOptions,
                    CreateReleaseOptions,
                    DeployReleaseOptions,
                    ListDeploymentsOptions,
                    ListProjectsOptions,
                    ListReleasesOptions,
                    ShowDashboardOptions
                >(args)
                .WithParsed<CreateProjectOptions>(CreateProject.Run)
                .WithParsed<CreateReleaseOptions>(CreateRelease.Run)
                .WithParsed<DeployReleaseOptions>(DeployRelease.Run)
                .WithParsed<ListDeploymentsOptions>(ListDeployments.Run)
                .WithParsed<ListProjectsOptions>(ListProjects.Run)
                .WithParsed<ListReleasesOptions>(ListReleases.Run)
                .WithParsed<ShowDashboardOptions>(ShowDashboard.Run)
                .WithNotParsed(errors => {});
        }
    }
}
