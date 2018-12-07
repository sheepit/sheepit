using CommandLine;

namespace SheepIt.ConsolePrototype
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<CreateReleaseOptions, DeployReleaseOptions, ShowDashboardOptions>(args)
                .WithParsed<CreateReleaseOptions>(CreateRelease.Run)
                .WithParsed<DeployReleaseOptions>(DeployRelease.Run)
                .WithParsed<ShowDashboardOptions>(ShowDashboard.Run)
                .WithNotParsed(errors => {});
        }
    }
}
