using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SheepIt.Api.Core.DeploymentProcesses
{
    public class DeploymentProcess
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public byte[] File { get; set; }
    }
}