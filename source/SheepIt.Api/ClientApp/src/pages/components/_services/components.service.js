import httpService from "../../../common/http/http-service";

export default {
    getComponentsList(projectId) {
        return httpService
            .post('api/project/components', { projectId });
    }
}