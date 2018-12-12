﻿using System;
using System.Linq;

namespace SheepIt.Domain
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
        public static int Add(Release release)
        {
            using (var liteDatabase = Database.Open())
            {
                var releases = liteDatabase.GetCollection<Release>();

                return releases.Insert(release);
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
    }
}