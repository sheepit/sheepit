using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Infrastructure.Utils;

namespace SheepIt.Api.UseCases.ProjectOperations.Environments
{
    public class UpdateEnvironmentsRankRequest : IRequest, IProjectRequest
    {
        public string ProjectId { get; set; }
        public int[] EnvironmentIds { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class UpdateEnvironmentsRankController : MediatorController
    {
        [HttpPost]
        [Route("project/environment/update-environments-rank")]
        public async Task UpdateEnvironmentsRank(UpdateEnvironmentsRankRequest request)
        {
            await Handle(request);
        }
    }

    public class UpdateEnvironmentsRankHandler : IHandler<UpdateEnvironmentsRankRequest>
    {
        private readonly SheepItDatabase _sheepItDatabase;
        private readonly Core.Environments.AddEnvironment _addEnvironment;

        public UpdateEnvironmentsRankHandler(SheepItDatabase sheepItDatabase, Core.Environments.AddEnvironment addEnvironment)
        {
            _sheepItDatabase = sheepItDatabase;
            _addEnvironment = addEnvironment;
        }

        public async Task Handle(UpdateEnvironmentsRankRequest request)
        {
            // todo: use project context
            var environments = await _sheepItDatabase.Environments
                .Find(filter => filter.FromProject(request.ProjectId))
                .ToArray();
            
            var environmentsById = environments
                .IndexBy(environment => environment.Id);

            var orderedEnvironments = request.EnvironmentIds
                .Select(environmentId => environmentsById[environmentId])
                .ToArray();

            await orderedEnvironments.ForEachAsync(async (environment, index) =>
            {
                environment.SetRank(index + 1);
                
                await _sheepItDatabase.Environments
                    .ReplaceOneById(environment);
            });
        }
    }
    
    public class UpdateEnvironmentsRankModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdateEnvironmentsRankHandler>()
                .WithDefaultResponse()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}
