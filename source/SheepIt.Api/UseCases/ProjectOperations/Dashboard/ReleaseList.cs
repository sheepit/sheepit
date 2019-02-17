using System.Linq;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public class ReleaseList
    {
        public static GetDashboardResponse.ReleaseDto[] GetReleases(Release[] releases)
        {
            return releases
                .OrderByDescending(release => release.CreatedAt)
                .Select(MapRelease)
                .ToArray();
        }

        private static GetDashboardResponse.ReleaseDto MapRelease(Release release)
        {
            return new GetDashboardResponse.ReleaseDto
            {
                Id = release.Id,
                CommitSha = release.CommitSha,
                CreatedAt = release.CreatedAt
            };
        }
    }
}