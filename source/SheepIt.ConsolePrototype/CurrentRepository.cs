using LibGit2Sharp;

namespace SheepIt.ConsolePrototype
{
    public class CurrentRepository
    {
        public static Repository Open()
        {
            return new Repository(".");
        }
    }
}