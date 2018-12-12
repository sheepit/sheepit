using SheepIt.Domain;

namespace SheepIt.ConsolePrototype.UseCases
{
    public class CreateProjectRequest
    {
        public string ProjectId { get; set; }
        public string RepositoryUrl { get; set; }
    }

    public static class CreateProjectHandler
    {
        public static void Handle(CreateProjectRequest request)
        {
            Projects.Add(new Project
            {
                Id = request.ProjectId,
                RepositoryUrl = request.RepositoryUrl
            });
        }
    }
}