import httpService from './../../../common/http/http-service'

export default {
    updateProcess(projectId, zipFileData) {
        const formData = new FormData();
        formData.append('ProjectId', projectId);
        formData.append('ZipFile', zipFileData);

        return httpService
            .postFormData(
                'api/project/package/update-package-process',
                formData
            );
    }
}
