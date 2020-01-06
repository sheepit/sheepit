using System.Linq;

namespace SheepIt.Api.Core.Projects
{
    public static class ProjectQueryExtensions
    {
        public static IQueryable<Project> WithId(
            this IQueryable<Project> query,
            string projectId)
        {
            return query.Where(project => project.Id == projectId);
        }
    }
}