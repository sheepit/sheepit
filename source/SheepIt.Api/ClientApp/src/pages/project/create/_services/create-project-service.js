import httpService from './../../../../common/http/http-service'

export default {
    createProject(
        projectId,
        environmentNames,
        componentNames) {

        return httpService.post('frontendApi/create-project', {
            projectId,
            environmentNames,
            componentNames
        }, false);
    }
}