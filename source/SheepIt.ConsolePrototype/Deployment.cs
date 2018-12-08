using System;

namespace SheepIt.ConsolePrototype
{
    public class Deployment
    {
        public int Id { get; set; }
        public string ProjectIt { get; set; }
        public int ReleaseId { get; set; }
        public DateTime DeployedAt { get; set; }
        public string EnvironmentId { get; set; }
    }

    public static class Deployments
    {

    }
}