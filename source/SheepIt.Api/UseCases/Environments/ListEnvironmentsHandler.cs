using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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
        private readonly ListEnvironmentsHandler _handler;

        public ListEnvironmentsController(ListEnvironmentsHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("list-environments")]
        public object ListEnvironments(ListEnvironmentsRequest request)
        {
            return _handler.Handle(request);
        }
    }

    public class ListEnvironmentsHandler
    {
        private readonly SheepItDatabase sheepItDatabase = new SheepItDatabase();
        
        public ListEnvironmentsResponse Handle(ListEnvironmentsRequest request)
        {
            var environments = sheepItDatabase.Environments
                .Find(filter => filter.FromProject(request.ProjectId))
                .SortBy(environment => environment.Rank)
                .ToEnumerable()
                .Select(Map)
                .ToArray();
            
            return new ListEnvironmentsResponse
            {
                Environments = environments
            };
        }

        private ListEnvironmentsResponse.EnvironmentDto Map(Environment environment)
        {
            return new ListEnvironmentsResponse.EnvironmentDto
            {
                Id = environment.Id,
                DisplayName = environment.DisplayName
            };
        }
    }
}
