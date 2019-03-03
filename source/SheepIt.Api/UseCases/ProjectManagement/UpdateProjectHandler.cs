using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
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

    public class UpdateProjectHandler : IHandler<UpdateProjectRequest>
    {
        private readonly IProjectContext _projectContext;
        private readonly SheepItDatabase _database;

        public UpdateProjectHandler(
            IProjectContext projectContext,
            SheepItDatabase database)
        {
            _projectContext = projectContext;
            _database = database;
        }
        
        public async Task Handle(UpdateProjectRequest request)
        {
            _projectContext.Project.UpdateRepositoryUrl(request.RepositoryUrl);
            
            await _database.Projects
                .ReplaceOneById(_projectContext.Project);
        }
    }
    
    public class UpdateProjectModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdateProjectHandler>()
                .WithDefaultResponse()
                .InProjectContext()
                .RegisterIn(builder);
        }
    }
}