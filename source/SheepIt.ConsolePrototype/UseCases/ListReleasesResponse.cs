using System;

namespace SheepIt.ConsolePrototype.UseCases
{
    static internal class ListReleasesResponse
    {
        public class ReleaseDto
        {
            public int Id { get; set; }
            public string CommitSha { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}