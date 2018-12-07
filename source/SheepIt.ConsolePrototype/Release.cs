using System;
using System.Collections.Generic;

namespace SheepIt.ConsolePrototype
{
    public class Release
    {
        public int Id { get; set; }
        public string CommitSha { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
