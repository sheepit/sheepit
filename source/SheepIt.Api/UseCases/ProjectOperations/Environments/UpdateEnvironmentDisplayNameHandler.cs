using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Infrastructure.Handlers;
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

    public class UpdateEnvironmentDisplayNameHandler : ISyncHandler<UpdateEnvironmentDisplayNameRequest>
    {
        private readonly EnvironmentsStorage _environmentsStorage;

        public UpdateEnvironmentDisplayNameHandler(EnvironmentsStorage environmentsStorage)
        {
            _environmentsStorage = environmentsStorage;
        }
        
        public void Handle(UpdateEnvironmentDisplayNameRequest request)
        {
            // todo: [rt] check project id or use IProjectContext.Environments
            var environment = _environmentsStorage.Get(request.EnvironmentId);

            environment.UpdateDisplayName(request.DisplayName);
            
            _environmentsStorage.Update(environment);
        }
    }
    
    public class UpdateEnvironmentDisplayNameModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdateEnvironmentDisplayNameHandler>()
                .WithDefaultResponse()
                .AsAsyncHandler()
                .InProjectContext()
                .RegisterIn(builder);
        }
    }
}