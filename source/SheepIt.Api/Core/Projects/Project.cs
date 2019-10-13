using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Projects
{
    public class Project : IDocumentWithId<string>
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        
        public string Id { get; set; }
        public string RepositoryUrl { get; set; }
    }
}