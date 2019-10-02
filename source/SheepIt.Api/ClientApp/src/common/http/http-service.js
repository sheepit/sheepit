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
                .then(response => response.json())
                .catch(error => {
                    messageService.error(error);
                })
        }

        return responsePromise
            .then(this.handleErrors)
            .catch(error => {
                messageService.error(error);
            });
    },

    handleErrors(response) {
        if (!response.ok) {
            
            if (response.status === 401) {
                events.emit('unauthorized')
            }
            
            response.json()
                .then(error => console.log(error));
            throw Error(response.statusText);
        }
        return response;
    }
};