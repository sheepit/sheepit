using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Environments
{
    public class Environment : IDocumentWithId<int>, IDocumentInProject
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string DisplayName { get; set; }
        public int Rank { get; set; }

        public void SetRank(int rank)
        {
            Rank = rank;
        }

        public void UpdateDisplayName(string displayName)
        {
            DisplayName = displayName;
        }
    }
}