using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace SheepIt.ConsolePrototype
{
    public class Release
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string CommitSha { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public static class Releases
    {
        public static Release GetRelease(string projectId, int releaseId)
        {
            using (var database = Database.Open())
            {
                return database
                    .GetCollection<Release>()
                    .Find(release => release.ProjectId == projectId && release.Id == releaseId)
                    .Single();
            }
        }
    }
}
