using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SheepIt.Domain
{
    public class TempMongoDatabase : IDisposable
    {
        private const string Database = "sheep-it";
        
        private readonly IMongoDatabase _mongoDatabase;

        public TempMongoDatabase()
        {
            var mongoClient = new MongoClient();

            _mongoDatabase = mongoClient.GetDatabase(Database);
        }

        public TempCollection<TDocument> GetCollection<TDocument>()
            where TDocument : IDocument
        {
            var collectionName = typeof(TDocument).Name;

            var collection = _mongoDatabase.GetCollection<TDocument>(collectionName);

            return new TempCollection<TDocument>(collection); 
        }

        public void Dispose()
        {
            // todo: not necessary - get rid of it
        }
    }
    
    // todo: this class is meant to have same interface as LiteDb's collection object; it should be inlined
    public class TempCollection<TDocument>
        where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;

        public TempCollection(IMongoCollection<TDocument> collection)
        {
            _collection = collection;
        }

        public IEnumerable<TDocument> FindAll()
        {
            return Find(document => true);
        }

        public IEnumerable<TDocument> Find(Expression<Func<TDocument, bool>> predicate)
        {
            return _collection
                .Find(predicate)
                .ToList();
        }

        public void Insert(TDocument document)
        {
            _collection.InsertOne(document);
        }

        public void Update(TDocument document)
        {
            _collection.ReplaceOne(searchedDocument => searchedDocument.ObjectId == document.ObjectId, document);
        }
    }

    public static class TempMongoCollectionExtensions
    {
        public static TDocument FindById<TDocument, TId>(this TempCollection<TDocument> collection, TId id)
            where TDocument : IDocumentWithId<TId>
        {
            return collection.FindAll()
                .Single(document => document.Id.Equals(id));
        }
        
        public static int InsertWithIntId<TDocument>(this TempCollection<TDocument> collection, TDocument document)
            where TDocument : IDocumentWithId<int>
        {
            var lastId = collection
                .FindAll()
                .Select(searchedDocument => searchedDocument.Id)
                .OrderByDescending(id => id)
                .FirstOrDefault();

            var nextId = lastId + 1;
            
            document.Id = nextId;

            collection.Insert(document);

            return nextId;
        } 
    }

    // todo: temp, remove later
    public interface IDocumentWithId<TId> : IDocument
    {
        TId Id { get; set; }
    }

    public interface IDocument
    {
        ObjectId ObjectId { get; set; }
    }
}