﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SheepIt.Domain
{
    public class Project : IDocumentWithId<string>
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        
        public string Id { get; set; }
        public string RepositoryUrl { get; set; }
    }

    public static class Projects
    {
        public static void Add(Project project)
        {
            using (var database = Database.Open())
            {
                var projectCollection = database.GetCollection<Project>();

                projectCollection.Insert(project);
            }
        }

        public static Project Get(string projectId)
        {
            using (var database = Database.Open())
            {
                var projectCollection = database.GetCollection<Project>();

                return projectCollection.FindById(projectId);
            }
        }
    }
}