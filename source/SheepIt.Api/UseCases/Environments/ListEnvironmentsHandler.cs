using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases.Environments
{
    public class ListEnvironmentsRequest
    {
        public string ProjectId { get; set; }
    }

    public class ListEnvironmentsResponse
    {
        public EnvironmentDto[] Environments { get; set; }

        public class EnvironmentDto
        {
            public int Id { get; set; }
            public string DisplayName { get; set; }
        }
    }
    
    [Route("api")]
    [ApiController]
    public class ListEnvironmentsController : ControllerBase
    {
        [HttpPost]
        [Route("list-environments")]
        public object ListEnvironments(ListEnvironmentsRequest request)
        {
            return ListEnvironmentsHandler.Handle(request);
        }
    }

    public static class ListEnvironmentsHandler
    {
        public static ListEnvironmentsResponse Handle(ListEnvironmentsRequest options)
        {
            using (var database = Database.Open())
            {
                var environmentCollection = database.GetCollection<Environment>();

                var items = environmentCollection
                    .Find(environment => environment.ProjectId == options.ProjectId)
                    .OrderBy(environment => environment.DisplayName)
                    .Select(Map)
                    .ToArray();

                return new ListEnvironmentsResponse
                {
                    Environments = items
                };
            }
        }
        
        private static ListEnvironmentsResponse.EnvironmentDto Map(Environment environment)
        {
            return new ListEnvironmentsResponse.EnvironmentDto
            {
                Id = environment.Id,
                DisplayName = environment.DisplayName
            };
        }
    }
}
