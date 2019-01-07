using MongoDB.Bson;
using MongoDB.Driver;

namespace SheepIt.Domain
{
    public static class DocumentFilters
    {
        public static FilterDefinition<TDocument> WithId<TDocument, TId>(this FilterDefinitionBuilder<TDocument> filter, TId id)
            where TDocument : IDocumentWithId<TId>
        {
            return filter.Eq(document => document.Id, id);
        }
        
        public static FilterDefinition<TDocument> WithObjectId<TDocument>(this FilterDefinitionBuilder<TDocument> filter, ObjectId objectId)
            where TDocument : IDocument
        {
            return filter.Eq(document => document.ObjectId, objectId);
        }
        
        public static FilterDefinition<TDocument> FromProject<TDocument>(this FilterDefinitionBuilder<TDocument> filter, string projectId)
            where TDocument : IDocumentInProject
        {
            return filter.Eq(document => document.ProjectId, projectId);
        }
    }
}