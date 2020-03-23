using System.Linq;

namespace SheepIt.Api.Model.Components
{
    public static class ComponentQueryExtensions
    {
        public static IQueryable<Component> FromProject(this IQueryable<Component> query, string projectId)
        {
            return query.Where(component => component.ProjectId == projectId);
        }
    }
}