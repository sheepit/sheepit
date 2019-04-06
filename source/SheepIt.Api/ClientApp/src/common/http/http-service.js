import messageService from "./../message/message-service";

export default {
    baseUrl: 'https://localhost:44380/',

    get(url) {
        const requestUrl = this.baseUrl + url;

        const fetchSettings = {
            method: "GET",
            mode: "cors",
            cache: "no-cache",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/json; charset=utf-8",
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
            },
            referrer: "no-referrer",
            body: JSON.stringify(request),
        }
    
        const responsePromise = fetch(requestUrl, fetchSettings);

        if(jsonResponse) {
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
            response.json()
                .then(error => console.log(error));
            throw Error(response.statusText);
        }
        return response;
    }
};