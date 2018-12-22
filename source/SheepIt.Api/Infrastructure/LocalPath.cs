using System.IO;

namespace SheepIt.Api.Infrastructure
{
    public class LocalPath
    {
        private readonly string _path;

        public LocalPath(string path)
        {
            _path = path;
        }

        public LocalPath AddSegment(string pathSegment)
        {
            var newPath = Path.Combine(_path, pathSegment);

            return new LocalPath(newPath);
        }

        public override string ToString()
        {
            return _path;
        }
    }
}