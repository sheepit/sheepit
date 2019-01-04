using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases.Environments
{
    public class UpdateEnvironmentsRankRequest
    {
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
        public static UpdateEnvironmentsRankResponse Handle(UpdateEnvironmentsRankRequest options)
        {
            using (var database = Database.Open())
            {
                var environmentCollection = database.GetCollection<Environment>().FindAll();

                for (var i = 0; i < options.EnvironmentIds.Length; i++)
                {
                    var environmentId = options.EnvironmentIds[i];
                    var environment = environmentCollection.Single(e => e.Id == environmentId);

                    environment.SetRank(i + 1);
                    
                    Domain.Environments.Update(environment);
                }
            }
            
            return new UpdateEnvironmentsRankResponse();
        }
    }
}
