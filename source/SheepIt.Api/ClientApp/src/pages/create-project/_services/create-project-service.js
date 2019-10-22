import httpService from './../../../common/http/http-service'

export default {
    createProject(
        projectId,
        environmentNames,
        releaseDisplayName,
        zipFileData) {

        const formData = new FormData();
        formData.append('ZipFile', zipFileData);
        formData.append('ProjectId', projectId);
        formData.append('ReleaseDisplayName', releaseDisplayName);

        for(let i = 0; i < environmentNames.length; i++) {
            formData.append('EnvironmentNames[]', environmentNames[i]);
        }

        return httpService
            .postFormData('api/create-project', formData);
    }
}
