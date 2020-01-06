using System.Linq;

namespace SheepIt.Api.Model.Environments
{
    public static class EnvironmentQueryExtensions
    {
        public static IQueryable<Environment> FromProject(
            this IQueryable<Environment> query,
            string projectId)
        {
            return query.Where(environment => environment.ProjectId == projectId);
        }
        
        public static IQueryable<Environment> OrderByRank(
            this IQueryable<Environment> query)
        {
            return query.OrderBy(environment => environment.Rank);
        } 
    }
}