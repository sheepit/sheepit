import httpService from './../../../common/http/http-service'

export default {
    getPackageDetails(projectId, packageId) {
        return httpService
            .post('api/project/package/get-package-details', { projectId, packageId });
    },

    getEnvironments(projectId) {
        return httpService
            .post('api/project/environment/list-environments', { projectId });
    },

    deploy(projectId, packageId, environmentId) {
        const request = {
            projectId: projectId,
            packageId: packageId,
            environmentId: environmentId
        };
        
        return httpService
            .post('api/project/deployment/deploy-package', request);
    }
}
