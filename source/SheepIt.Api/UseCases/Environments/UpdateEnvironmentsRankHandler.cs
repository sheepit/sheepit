using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Domain;
using SheepIt.Utils.Extensions;

namespace SheepIt.Api.UseCases.Environments
{
    public class UpdateEnvironmentsRankRequest
    {
        public string ProjectId { get; set; }
        public int[] EnvironmentIds { get; set; }
    }

    public class UpdateEnvironmentsRankResponse
    {
    }
    
    [Route("api")]
    [ApiController]
    public class UpdateEnvironmentsRankController : ControllerBase
    {
        private readonly UpdateEnvironmentsRankHandler _handler;

        public UpdateEnvironmentsRankController(UpdateEnvironmentsRankHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("update-environments-rank")]
        public object UpdateEnvironmentsRank(UpdateEnvironmentsRankRequest request)
        {
            return _handler.Handle(request);
        }
    }

    public class UpdateEnvironmentsRankHandler
    {
        private readonly SheepItDatabase sheepItDatabase;
        private readonly Domain.Environments _environments;

        public UpdateEnvironmentsRankHandler(SheepItDatabase sheepItDatabase, Domain.Environments environments)
        {
            this.sheepItDatabase = sheepItDatabase;
            _environments = environments;
        }

        public UpdateEnvironmentsRankResponse Handle(UpdateEnvironmentsRankRequest request)
        {
            var environmentsById = sheepItDatabase.Environments
                .Find(filter => filter.FromProject(request.ProjectId))
                .ToEnumerable()
                .IndexBy(environment => environment.Id);

            var orderedEnvironments = request.EnvironmentIds
                .Select(environmentId => environmentsById[environmentId])
                .ToArray();

            orderedEnvironments.ForEach((environment, index) =>
            {
                environment.SetRank(index + 1);
                _environments.Update(environment);
            });

            return new UpdateEnvironmentsRankResponse();
        }
    }
}
