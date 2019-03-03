using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace SheepIt.Api.Infrastructure.Mongo
{
    public static class MongoCollectionExtensions
    {
        public static IFindFluent<TDocument, TDocument> FindAll<TDocument>(
            this IMongoCollection<TDocument> mongoCollection)
        {
            return mongoCollection.Find(filter => filter.Empty);
        }
        
        public static IFindFluent<TDocument, TDocument> Find<TDocument>(
            this IMongoCollection<TDocument> mongoCollection,
            Func<FilterDefinitionBuilder<TDocument>, FilterDefinition<TDocument>> buildFilter)
        {
            var filterDefinitionBuilder = Builders<TDocument>.Filter;
            var filterDefinition = buildFilter(filterDefinitionBuilder);

            return mongoCollection.Find(filterDefinition);
        }
        
        public static void ReplaceOneById<TDocument>(
            this IMongoCollection<TDocument> mongoCollection,
            TDocument replacement)
            where TDocument : IDocument
        {
            mongoCollection.ReplaceOne(
                filter => filter.WithObjectId(replacement.ObjectId),
                replacement
            );
        }
        
        public static void ReplaceOne<TDocument>(
            this IMongoCollection<TDocument> mongoCollection,
            Func<FilterDefinitionBuilder<TDocument>, FilterDefinition<TDocument>> buildFilter,
            TDocument replacement)
            where TDocument : IDocument
        {
            var filterDefinitionBuilder = Builders<TDocument>.Filter;
            var filterDefinition = buildFilter(filterDefinitionBuilder);
            
            mongoCollection.ReplaceOne(filterDefinition, replacement);
        }

        public static TDocument FindById<TDocument, TId>(this IMongoCollection<TDocument> mongoCollection, TId id)
            where TDocument : IDocumentWithId<TId>
        {
            var foundDocumentOrNull = mongoCollection
                .Find(filter => filter.WithId(id))
                .SingleOrDefault();

            if (foundDocumentOrNull == null)
            {
                throw new InvalidOperationException($"Document of type {typeof(TDocument).Name} with id {id} could not be found.");
            }
            
            return foundDocumentOrNull;
        }
        
        public static TDocument FindByProjectAndId<TDocument, TId>(this IMongoCollection<TDocument> mongoCollection, string projectId, TId id)
            where TDocument : IDocumentWithId<TId>, IDocumentInProject
        {
            var foundDocumentOrNull = mongoCollection
                .Find(filter => filter.FromProject(projectId) & filter.WithId(id))
                .SingleOrDefault();
            
            if (foundDocumentOrNull == null)
            {
                throw new InvalidOperationException($"Document of type {typeof(TDocument).Name} with id {id} in project {projectId} could not be found.");
            }
            
            return foundDocumentOrNull;
        }

        [Obsolete("use async version")]
        public static int GetNextIdSync<TDocument>(this IMongoCollection<TDocument> databaseEnvironments)
            where TDocument : IDocumentWithId<int>
        {
            return GetNextId(databaseEnvironments).Result;
        }
        
        public static async Task<int> GetNextId<TDocument>(this IMongoCollection<TDocument> databaseEnvironments)
            where TDocument : IDocumentWithId<int>
        {
            var lastDocumentOrNull = await databaseEnvironments
                .FindAll()
                .Sort(sort => sort.Descending(document => document.Id))
                .FirstOrDefaultAsync();

            var currentId = lastDocumentOrNull?.Id ?? 0;

            return currentId + 1;
        }
    }
}