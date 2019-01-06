using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SheepIt.Domain
{
    public class Release : IDocumentWithId<int>
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

    public static class ReleasesStorage
    {
        public static int Add(Release release)
        {
            using (var liteDatabase = Database.Open())
            {
                var releases = liteDatabase.GetCollection<Release>();

                return releases.InsertWithIntId(release);
            }
        }

        public static Release Get(string projectId, int releaseId)
        {
            using (var database = Database.Open())
            {
                return database
                    .GetCollection<Release>()
                    .Find(release => release.ProjectId == projectId && release.Id == releaseId)
                    .Single();
            }
        }

        public static Release GetNewest(string projectId)
        {
            using (var database = Database.Open())
            {
                return database
                    .GetCollection<Release>()
                    .Find(release => release.ProjectId == projectId)
                    .OrderByDescending(release => release.Id)
                    .First();
            }
        }
    }
}
