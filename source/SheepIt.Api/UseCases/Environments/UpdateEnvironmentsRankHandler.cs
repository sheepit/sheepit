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
        [HttpPost]
        [Route("update-environments-rank")]
        public object UpdateEnvironmentsRank(UpdateEnvironmentsRankRequest request)
        {
            return UpdateEnvironmentsRankHandler.Handle(request);
        }
    }

    public static class UpdateEnvironmentsRankHandler
    {
        private static readonly SheepItDatabase sheepItDatabase = new SheepItDatabase();
        
        public static UpdateEnvironmentsRankResponse Handle(UpdateEnvironmentsRankRequest request)
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
                Domain.Environments.Update(environment);
            });

            return new UpdateEnvironmentsRankResponse();
        }
    }
}
