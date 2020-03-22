import httpService from "./../../../common/http/http-service";

export default {
    getPackagesList(projectId) {
        return httpService
            .post('api/project/packages/list-packages', { projectId });
    }
}