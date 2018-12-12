using Microsoft.AspNetCore.Mvc;
using SheepIt.ConsolePrototype.UseCases;

namespace SheepIt.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class SheepItController : ControllerBase
    {
        [HttpPost]
        [Route("create-project")]
        public void CreateProject(CreateProjectRequest request)
        {
            CreateProjectHandler.Handle(request);
        }

        [HttpPost]
        [Route("create-release")]
        public object CreateProject(CreateReleaseRequest request)
        {
            return CreateReleaseHandler.Handle(request);
        }

        [HttpGet]
        [Route("list-releases")]
        public object ListReleases(ListReleasesRequest request)
        {
            return ListReleasesHandler.Handle(request);
        }

        [HttpGet]
        [Route("list-deployments")]
        public object ListDeployments(ListDeploymentsRequest request)
        {
            return ListDeploymentsHandler.Handle(request);
        }
    }
}
