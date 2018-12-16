function postData(url, request) {
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

    return fetch(url, fetchSettings)
}
