import httpService from "./../../../common/http/http-service.js";

export default {
    createProject(projectId, repositoryUrl, environmentNames) {
        return httpService
            .post('api/create-project', {
                projectId: projectId,
                repositoryUrl: repositoryUrl,
                environmentNames: environmentNames
            });
    }
}