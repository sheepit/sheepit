import httpService from './../../../../common/http/http-service'

export default {
    createProject(
        projectId,
        environmentNames,
        componentNames) {

        return httpService.post('api/create-project', {
            projectId,
            environmentNames,
            componentNames
        }, false);
    }
}
