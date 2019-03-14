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
    },

    post(url, request) {
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
    
        return fetch(requestUrl, fetchSettings)
    }
};