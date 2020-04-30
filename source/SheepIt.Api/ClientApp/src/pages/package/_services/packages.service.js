import httpService from "./../../../common/http/http-service";

export default {
    getPackagesList(projectId) {
        return httpService
            .post('frontendApi/project/packages/list-packages', { projectId });
    }
}