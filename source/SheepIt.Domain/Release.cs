using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace SheepIt.Domain
{
    public class Release : IDocumentWithId<int>, IDocumentInProject
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string CommitSha { get; set; }
        public DateTime CreatedAt { get; set; }
        public VariableCollection Variables { get; set; } = new VariableCollection();

        public VariableForEnvironment[] GetVariablesForEnvironment(int environmentId)
        {
            return Variables.GetForEnvironment(environmentId);
        }

        public Release WithUpdatedCommitSha(string newCommitSha)
        {
            return new Release
            {
                Id = 0,
                ProjectId = ProjectId,
                CommitSha = newCommitSha,
                CreatedAt = DateTime.UtcNow,
                Variables = Variables.Clone()
            };
        }

        public Release WithUpdatedVariables(VariableValues[] newVariables)
        {
            return new Release
            {
                Id = 0,
                ProjectId = ProjectId,
                CommitSha = CommitSha,
                CreatedAt = DateTime.UtcNow,
                Variables = Variables.WithUpdatedVariables(newVariables)
            };
        }

        public Release WithNewVariables(IEnumerable<VariableValues> newVariables)
        {
            return new Release
            {
                Id = 0,
                ProjectId = ProjectId,
                CommitSha = CommitSha,
                CreatedAt = DateTime.UtcNow,
                Variables = new VariableCollection()
                {
                    Variables = newVariables.ToArray()
                }
            };
        }
    }

    public class ReleasesStorage
    {
        private readonly SheepItDatabase _database = new SheepItDatabase();
        
        public int Add(Release release)
        {
            var nextId = _database.Releases.GetNextId();
            
            release.Id = nextId;

            _database.Releases.InsertOne(release);

            return nextId;
        }

        public Release Get(string projectId, int releaseId)
        {
            return _database.Releases
                .FindByProjectAndId(projectId, releaseId);
        }

        public Release GetNewest(string projectId)
        {
            return _database.Releases
                .Find(filter => filter.FromProject(projectId))
                .Sort(sort => sort.Descending(release => release.Id))
                .First();
        }
    }
}
