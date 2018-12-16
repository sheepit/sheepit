window.onload = function() {
    
    const app = new Vue({
        el: '#app',
        
        components: {
            'navigation': httpVueLoader('navigation.vue')
        },

        router: new VueRouter({
            routes: [
                {
                    path: '/',
                    name: 'default',
                    component: httpVueLoader('default.vue')
                },
                {
                    path: '/create-project',
                    name: 'create-project',
                    component: httpVueLoader('create-project.vue')
                },
                {
                    path: '/project/:projectId',
                    name: 'project',
                    component: httpVueLoader('project.vue')
                }
            ]
        })
    })

}