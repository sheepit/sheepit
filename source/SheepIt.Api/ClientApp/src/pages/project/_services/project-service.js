import httpService from "./../../../common/http/http-service";

export default {
    getDashboard(projectId) {
        return httpService
            .post('frontendApi/project/dashboard', { projectId });
    }
}