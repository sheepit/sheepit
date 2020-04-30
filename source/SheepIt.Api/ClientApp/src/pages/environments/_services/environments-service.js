import httpService from "../../../common/http/http-service";

export default {
    getEnvironmentsList(projectId) {
        return httpService
            .post('frontendApi/project/environments/list-environments', { projectId });
    }
}