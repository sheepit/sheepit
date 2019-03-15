import httpService from "./../../../common/http/http-service";

export default {
    getDashboard(projectId) {
        return httpService
            .post('api/project/dashboard/get-dashboard', { projectId });
    }
}