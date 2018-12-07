using System;
using System.Linq;
using LiteDB;

namespace SheepIt.ConsolePrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var liteDatabase = new LiteDatabase(@"C:\sheep-it\poc-database.db"))
            {
                var releases = liteDatabase.GetCollection<Release>();

                var allReleases = releases.FindAll().ToArray();

                foreach (var release in allReleases)
                {
                    Console.WriteLine($"release {release.Id}: createdAt: {release.CreatedAt}, commitHash: {release.CommitHash}");
                }

                //releases.Insert(new Release
                //{
                //    CommitHash = "123",
                //    CreatedAt = DateTime.UtcNow
                //});
            }

            Console.ReadLine();
        }
    }
}
