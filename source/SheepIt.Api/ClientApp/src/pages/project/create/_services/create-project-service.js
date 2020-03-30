import httpService from './../../../../common/http/http-service'

export default {
    createProject(
        projectId,
        environmentNames,
        componentNames) {

        // todo: since package is not uploaded anymore, this doesn't have to be form data
        const formData = new FormData();
        
        formData.append('ProjectId', projectId);

        for (let environmentIndex = 0; environmentIndex < environmentNames.length; environmentIndex++) {
            formData.append('EnvironmentNames[]', environmentNames[environmentIndex]);
        }

        for (let componentIndex = 0; componentIndex < componentNames.length; componentIndex++) {
            formData.append('ComponentNames[]', componentNames[componentIndex]);
        }

        return httpService
            .postFormData('api/create-project', formData);
    }
}
