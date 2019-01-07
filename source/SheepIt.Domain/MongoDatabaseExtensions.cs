using MongoDB.Driver;

namespace SheepIt.Domain
{
    public static class MongoDatabaseExtensions
    {
        public static IMongoCollection<TDocument> GetCollection<TDocument>(this IMongoDatabase database)
        {
            // todo: this approach could get us into trouble
            var documentTypeName = typeof(TDocument).Name;

            return database.GetCollection<TDocument>(documentTypeName);
        }
    }
}