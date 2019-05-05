export default {
    
    emit(name) {
        document
            .querySelectorAll('[data-event-handler]')
            .forEach(node => node.dispatchEvent(new Event(name)))
    }
    
}