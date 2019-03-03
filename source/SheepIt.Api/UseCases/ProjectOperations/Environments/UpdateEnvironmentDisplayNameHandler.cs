using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Environments
{
    public class UpdateEnvironmentDisplayNameRequest : IRequest, IProjectRequest
    {
        public string ProjectId { get; set; }
        public int EnvironmentId { get; set; }
        public string DisplayName { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class UpdateEnvironmentDisplayNameController : MediatorController
    {
        [HttpPost]
        [Route("project/environment/update-environment-display-name")]
        public async Task UpdateEnvironmentDisplayName(UpdateEnvironmentDisplayNameRequest request)
        {
            await Handle(request);
        }
    }

    public class UpdateEnvironmentDisplayNameHandler : IHandler<UpdateEnvironmentDisplayNameRequest>
    {
        private readonly SheepItDatabase _database;

        public UpdateEnvironmentDisplayNameHandler(SheepItDatabase database)
        {
            _database = database;
        }
        
        public async Task Handle(UpdateEnvironmentDisplayNameRequest request)
        {
            // todo: [rt] use IProjectContext.Environments, add method GetEnvironmentById or sth
            var environment = await _database.Environments
                .FindByProjectAndId(request.ProjectId, request.EnvironmentId);

            environment.UpdateDisplayName(request.DisplayName);
            
            await _database.Environments
                .ReplaceOneById(environment);
        }
    }
    
    public class UpdateEnvironmentDisplayNameModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdateEnvironmentDisplayNameHandler>()
                .WithDefaultResponse()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}