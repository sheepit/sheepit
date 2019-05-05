const key = "jwtToken"

export default {
    
    set(jwtToken) {
        window.localStorage.setItem(key, jwtToken)
    },
    
    remove() {
        window.localStorage.removeItem(key)
    }
    
}