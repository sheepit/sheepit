import messageService from "./../message/message-service";
import events from './../events/events.js';
import defaults from './../defaults';

export default {
    baseUrl: (typeof BASE_URL === 'undefined'
        ? defaults.baseUrl
        : BASE_URL),

    get(url) {
        const requestUrl = this.baseUrl + url;

        const fetchSettings = {
            method: "GET",
            mode: "cors",
            cache: "no-cache",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/json; charset=utf-8",
                "Authorization": `Bearer ${window.localStorage.getItem("jwtToken")}` // todo: remove duplication, create jwt-token-storage
            },
            referrer: "no-referrer"
        }
    
        return fetch(requestUrl, fetchSettings)
            .then(this.handleErrors)
            .then(response => response.json())
            .catch(error => {
                messageService.error(error);
            });
    },

    post(url, request, jsonResponse = true) {
        const requestUrl = this.baseUrl + url;
        
        const fetchSettings = {
            method: "POST",
            mode: "cors",
            cache: "no-cache",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/json; charset=utf-8",
                "Authorization": `Bearer ${window.localStorage.getItem("jwtToken")}` // todo: remove duplication, create jwt-token-storage
            },
            referrer: "no-referrer",
            body: JSON.stringify(request),
        }
    
        const responsePromise = fetch(requestUrl, fetchSettings);

        if (jsonResponse) {
            return responsePromise
                .then(this.handleErrors)
                .then(response => response.json());
        }

        return responsePromise
            .then(this.handleErrors);
    },

    postFormData(url, formData) {
        const requestUrl = this.baseUrl + url;

        const fetchSettings = {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${window.localStorage.getItem("jwtToken")}` // todo: remove duplication, create jwt-token-storage
            },
            body: formData
        }
    
        const responsePromise = fetch(requestUrl, fetchSettings);

        return responsePromise
            .then(this.handleErrors);
    },

    handleErrors(response) {
        if (!response.ok) {
            
            if (response.status === 401) {
                events.emit('unauthorized');
            }
            
            response
                .json()
                .then(error => {
                    if(error.Type && error.Type === "CustomException") {
                        messageService.error(error.HumanReadableMessage);
                    }
                    else {
                        messageService.error('Server error. Please try again later.');
                    }
                    console.log(error)
                    return;
                });

            return Promise.reject(response.statusText);
        }

        return response;
    }
};