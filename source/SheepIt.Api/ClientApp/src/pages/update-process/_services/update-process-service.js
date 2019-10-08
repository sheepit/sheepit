import httpService from './../../../common/http/http-service'

export default {
    updateProcess(projectId, zipFileData) {
        const formData = new FormData();
        formData.append('ZipFile', zipFileData);
        formData.append('ProjectId', projectId);

        return httpService
            .postFormData('api/update-process', formData);
    }
}
