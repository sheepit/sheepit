using System.Linq;

namespace SheepIt.Api.Model.Components
{
    public static class ComponentQueryExtensions
    {
        public static IQueryable<Component> WithId(this IQueryable<Component> query, int id)
        {
            return query.Where(component => component.Id == id);
        }

        public static IQueryable<Component> FromProject(this IQueryable<Component> query, string projectId)
        {
            return query.Where(component => component.ProjectId == projectId);
        }

        public static IQueryable<Component> OrderByRank(this IQueryable<Component> query)
        {
            return query.OrderBy(component => component.Rank);
        }
    }
}