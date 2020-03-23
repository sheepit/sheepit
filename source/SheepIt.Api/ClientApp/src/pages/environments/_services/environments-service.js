import httpService from "../../../common/http/http-service";

export default {
    getEnvironmentsList(projectId) {
        return httpService
            .post('api/project/environments/list-environments', { projectId });
    }
}