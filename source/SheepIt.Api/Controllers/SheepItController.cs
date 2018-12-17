using Microsoft.AspNetCore.Mvc;
using SheepIt.ConsolePrototype.UseCases;
using SheepIt.ConsolePrototype.UseCases.Releases;

namespace SheepIt.Api.Controllers
{
    // todo: there should be a clear distinction between api used in web app and public api
    
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

        // currently used for editing variables
        [HttpPost]
        [Route("get-last-release")]
        public object GetLastRelease(GetLastReleaseRequest request)
        {
            return GetLastReleaseHandler.Handle(request);
        }
        
        [HttpPost]
        [Route("edit-release-variables")]
        public object EditReleaseVariables(EditReleaseVariablesRequest request)
        {
            return EditReleaseVariablesHandler.Handle(request);
        }
        
        [HttpPost]
        [Route("update-release-process")]
        public object UpdateReleaseProcess(UpdateReleaseProcessRequest request)
        {
            return UpdateReleaseProcessHandler.Handle(request);
        }
        
        // meant to be used programatically via public API, e. g. when you want to update single variable, like service version 
        [HttpPost]
        [Route("update-release-variables")]
        public object UpdateReleaseVariables(UpdateReleaseVariablesRequest request)
        {
            return UpdateReleaseVariablesHandler.Handle(request);
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

        [HttpPost]
        [Route("list-releases")]
        public object ListReleases(ListReleasesRequest request)
        {
            return ListReleasesHandler.Handle(request);
        }

        [HttpPost]
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
