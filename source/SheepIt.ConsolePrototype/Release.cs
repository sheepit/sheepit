using System;
using System.Collections.Generic;

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
        public static Release GetReleaseById(int releaseId)
        {
            using (var database = Database.Open())
            {
                var releaseCollection = database.GetCollection<Release>();

                return releaseCollection.FindById(releaseId);
            }
        }
    }
}
