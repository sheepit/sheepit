namespace SheepIt.Api.Infrastructure.Logger.CloudWatch
{
    internal class AwsConfigSection
    {
        public string Profile { get; set; }
        public string Region { get; set; }
        
        public AwsCloudWatchConfigSection CloudWatch { get; set; } = new AwsCloudWatchConfigSection();
    }
    
    internal class AwsCloudWatchConfigSection
    {
        public bool Enabled { get; set; }
    }
}