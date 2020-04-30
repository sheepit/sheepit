import httpService from "../../../common/http/http-service";

export default {
    getComponentsList(projectId) {
        return httpService
            .post('frontendApi/project/components', { projectId });
    }
}