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
        [Route("update-release-process")]
        public object CreateProject(UpdateReleaseProcessRequest processRequest)
        {
            return UpdateReleaseProcessHandler.Handle(processRequest);
        }

        [HttpPost]
        [Route("deploy-release")]
        public object DeployRelease(DeployReleaseRequest request)
        {
            return DeployReleaseHandler.Handle(request);
        }

        [HttpGet]
        [Route("list-projects")]
        public object ListProjects()
        {
            return ListProjectsHandler.Handle(new ListProjectsRequest());
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

        [HttpPost]
        [Route("show-dashboard")]
        public object ShowDashboard(ShowDashboardRequest request)
        {
            return ShowDashboardHandler.Handle(request);
        }
    }
}
