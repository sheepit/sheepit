import defaults from './../../../common/defaults'

export default {
    createProject(
        projectId,
        repositoryUrl,
        environmentNames,
        zipFileData) {

        const baseUrl = (typeof BASE_URL === 'undefined'
            ? defaults.baseUrl
            : BASE_URL);

        const requestUrl = baseUrl + 'api/create-project';

        const formData = new FormData();
        formData.append('ZipFile', zipFileData);
        formData.append('ProjectId', projectId);
        formData.append('RepositoryUrl', repositoryUrl);
        formData.append('EnvironmentNames', environmentNames);

        const fetchSettings = {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${window.localStorage.getItem("jwtToken")}` // todo: remove duplication, create jwt-token-storage
            },
            body: formData
        }
    
        const responsePromise = fetch(requestUrl, fetchSettings);

        return responsePromise
            .then(
                this.handleErrors,
                error => console.error(error))
            .catch(error => {
                console.error(error);
            });
    }
}