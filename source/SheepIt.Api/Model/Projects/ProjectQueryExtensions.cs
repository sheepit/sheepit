using System.Linq;

namespace SheepIt.Api.Model.Projects
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