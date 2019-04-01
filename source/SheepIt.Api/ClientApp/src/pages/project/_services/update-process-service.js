import httpService from "./../../../common/http/http-service";

export default {
    updateProcess(projectId) {
        return httpService
            .post('api/project/release/update-release-process', { projectId });
    }
}