using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Utils;

namespace SheepIt.Api.UseCases.Environments
{
    public class UpdateEnvironmentsRankRequest : IRequest
    {
        public string ProjectId { get; set; }
        public int[] EnvironmentIds { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class UpdateEnvironmentsRankController : MediatorController
    {
        [HttpPost]
        [Route("update-environments-rank")]
        public async Task UpdateEnvironmentsRank(UpdateEnvironmentsRankRequest request)
        {
            await Handle(request);
        }
    }

    public class UpdateEnvironmentsRankHandler : ISyncHandler<UpdateEnvironmentsRankRequest>
    {
        private readonly SheepItDatabase _sheepItDatabase;
        private readonly Core.Environments.EnvironmentsStorage _environmentsStorage;

        public UpdateEnvironmentsRankHandler(SheepItDatabase sheepItDatabase, Core.Environments.EnvironmentsStorage environmentsStorage)
        {
            _sheepItDatabase = sheepItDatabase;
            _environmentsStorage = environmentsStorage;
        }

        public void Handle(UpdateEnvironmentsRankRequest request)
        {
            var environmentsById = _sheepItDatabase.Environments
                .Find(filter => filter.FromProject(request.ProjectId))
                .ToEnumerable()
                .IndexBy(environment => environment.Id);

            var orderedEnvironments = request.EnvironmentIds
                .Select(environmentId => environmentsById[environmentId])
                .ToArray();

            orderedEnvironments.ForEach((environment, index) =>
            {
                environment.SetRank(index + 1);
                _environmentsStorage.Update(environment);
            });
        }
    }
}
