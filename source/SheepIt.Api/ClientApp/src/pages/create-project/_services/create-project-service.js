import defaults from './../../../common/defaults'
import messageService from "./../../../common/message/message-service";

export default {
    createProject(
        projectId,
        environmentNames,
        zipFileData) {

        const baseUrl = (typeof BASE_URL === 'undefined'
            ? defaults.baseUrl
            : BASE_URL);

        const requestUrl = baseUrl + 'api/create-project';

        const formData = new FormData();
        formData.append('ZipFile', zipFileData);
        formData.append('ProjectId', projectId);

        for(let i = 0; i < environmentNames.length; i++) {
            formData.append('EnvironmentNames[]', environmentNames[i]);
        }

        const fetchSettings = {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${window.localStorage.getItem("jwtToken")}` // todo: remove duplication, create jwt-token-storage
            },
            body: formData
        }
    
        const responsePromise = fetch(requestUrl, fetchSettings);

        return responsePromise
            .then(this.handleErrors)
            .catch(error => {
                messageService.error(error);
                return Promise.reject(error);
            });
    },

    handleErrors(response) {
        if (!response.ok) {
            
            if (response.status === 401) {
                events.emit('unauthorized')
            }
            
            response
                .json()
                .then(error => console.log(error));

            throw Error(response.statusText);
        }

        return response;
    }
}