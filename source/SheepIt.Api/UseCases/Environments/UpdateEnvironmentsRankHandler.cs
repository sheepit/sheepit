using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
        public static UpdateEnvironmentsRankResponse Handle(UpdateEnvironmentsRankRequest request)
        {
            using (var database = Database.Open())
            {
                // todo: we should filter it by project id, as in the future environments in different projects might have same ids
                var environmentsById = database
                    .GetCollection<Environment>()
                    .Find(environment => environment.ProjectId == request.ProjectId)
                    .IndexBy(environment => environment.Id);

                var orderedEnvironments = request.EnvironmentIds
                    .Select(environmentId => environmentsById[environmentId])
                    .ToArray();

                orderedEnvironments.ForEach((environment, index) =>
                {
                    environment.SetRank(index + 1);
                    Domain.Environments.Update(environment);
                });
            }
            
            return new UpdateEnvironmentsRankResponse();
        }
    }
}
