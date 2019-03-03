using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Environments
{
    public class AddEnvironmentRequest : IRequest, IProjectRequest
    {
        public string ProjectId { get; set; }
        public string DisplayName { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class AddEnvironmentController : MediatorController
    {
        [HttpPost]
        [Route("project/environment/add-environment")]
        public async Task AddEnvironment(AddEnvironmentRequest request)
        {
            await Handle(request);
        }
    }

    public class AddEnvironmentHandler : ISyncHandler<AddEnvironmentRequest>
    {
        private readonly EnvironmentsStorage _environmentsStorage;

        public AddEnvironmentHandler(EnvironmentsStorage environmentsStorage)
        {
            _environmentsStorage = environmentsStorage;
        }
        
        public void Handle(AddEnvironmentRequest request)
        {
            var environment = new Environment(request.ProjectId, request.DisplayName);
            
            _environmentsStorage.AddSync(environment);
        }
    }
    
    public class AddEnvironmentModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<AddEnvironmentHandler>()
                .WithDefaultResponse()
                .AsAsyncHandler()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}