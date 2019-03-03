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

    public class AddEnvironmentHandler : IHandler<AddEnvironmentRequest>
    {
        private readonly AddEnvironment _addEnvironment;

        public AddEnvironmentHandler(AddEnvironment addEnvironment)
        {
            _addEnvironment = addEnvironment;
        }
        
        public async Task Handle(AddEnvironmentRequest request)
        {
            await _addEnvironment.Add(request.ProjectId, request.DisplayName);
        }
    }
    
    public class AddEnvironmentModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<AddEnvironmentHandler>()
                .WithDefaultResponse()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}