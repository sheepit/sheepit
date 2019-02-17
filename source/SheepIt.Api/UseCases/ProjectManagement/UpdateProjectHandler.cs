using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectManagement
{
    public class UpdateProjectRequest : IRequest, IProjectRequest
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
        private readonly IProjectContext _projectContext;

        public UpdateProjectHandler(ProjectsStorage projectsStorage, IProjectContext projectContext)
        {
            _projectsStorage = projectsStorage;
            _projectContext = projectContext;
        }
        
        public void Handle(UpdateProjectRequest request)
        {
            _projectContext.Project.UpdateRepositoryUrl(request.RepositoryUrl);
            
            _projectsStorage.Update(_projectContext.Project);
        }
    }
    
    public class UpdateProjectModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdateProjectHandler>()
                .WithDefaultResponse()
                .AsAsyncHandler()
                .InProjectContext()
                .RegisterIn(builder);
        }
    }
}