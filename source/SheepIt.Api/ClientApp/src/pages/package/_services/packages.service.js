import httpService from "./../../../common/http/http-service";

export default {
    getPackagesList(projectId) {
        return httpService
            .post('frontendApi/project/packages/list-packages', { projectId });
    },

    getCurl(projectId, packageId) {
        return httpService
            .post('frontendApi/project/deployments/get-curl', { projectId, packageId });
    }
}