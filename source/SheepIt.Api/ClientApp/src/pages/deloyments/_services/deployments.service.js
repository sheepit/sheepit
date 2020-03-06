import httpService from "./../../../common/http/http-service";

export default {
    getDeploymentsList(projectId) {
        return httpService
            .post('api/project/deployments/list-deployments', { projectId });
    }
}