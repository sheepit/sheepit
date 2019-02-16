using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectManagement
{
    public class UpdateProjectRequest : IRequest
    {
        public string ProjectId { get; set; }
        public string RepositoryUrl { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class UpdateProjectController : MediatorController
    {
        [HttpPost]
        [Route("update-project")]
        public async Task UpdateProject(UpdateProjectRequest request)
        {
            await Handle(request);
        }
    }

    public class UpdateProjectHandler : ISyncHandler<UpdateProjectRequest>
    {
        private readonly ProjectsStorage _projectsStorage;

        public UpdateProjectHandler(ProjectsStorage projectsStorage)
        {
            _projectsStorage = projectsStorage;
        }
        
        public void Handle(UpdateProjectRequest request)
        {
            var project = _projectsStorage.Get(request.ProjectId);

            project.UpdateRepositoryUrl(request.RepositoryUrl);
            
            _projectsStorage.Update(project);
        }
    }
    
    public class UpdateProjectModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdateProjectHandler>()
                .WithDefaultResponse()
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}