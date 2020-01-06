using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.Core.Projects;
using Environment = SheepIt.Api.Core.Environments.Environment;

namespace SheepIt.Api.Core.Deployments
{
    public class Deployment
    {
        // identity
        
        public int Id { get; set; }
        
        // relations
        
        public string ProjectId { get; set; }
        public virtual Project Project { get; set; }
        
        public int PackageId { get; set; }
        public virtual Package Package { get; set; }
        
        public int EnvironmentId { get; set; }
        public virtual Environment Environment { get; set; }
        
        // data
        
        public DateTime StartedAt { get; set; }
        public DeploymentStatus Status { get; set; }
        public ProcessOutput ProcessOutput { get; set; }

        public void MarkFinished(ProcessOutput processOutput)
        {
            ProcessOutput = processOutput;

            Status = processOutput.Steps.All(result => result.Successful)
                ? DeploymentStatus.Succeeded
                : DeploymentStatus.ProcessFailed;
        }

        public void MarkExecutionFailed()
        {
            Status = DeploymentStatus.ExecutionFailed;
        }

    }
    
    public enum DeploymentStatus
    {
        InProgress,
        Succeeded,
        ProcessFailed,
        ExecutionFailed
    }
    
    public class ProcessOutput
    {
        public ProcessStepResult[] Steps{ get; set; }
    }

    public class ProcessStepResult
    {
        public bool Successful { get; set; }
        public string Command { get; set; }
        public string[] Output { get; set; }
    }
}