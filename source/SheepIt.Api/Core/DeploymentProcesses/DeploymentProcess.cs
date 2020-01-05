using System;

namespace SheepIt.Api.Core.DeploymentProcesses
{
    public class DeploymentProcess
    {
        public Guid ObjectId { get; set; }
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public byte[] File { get; set; }
    }
}