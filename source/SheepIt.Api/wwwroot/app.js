window.onload = function() {

    httpVueLoader.register(Vue, 'expanding-list.vue')
    httpVueLoader.register(Vue, 'humanized-date.vue')
    httpVueLoader.register(Vue, 'tooltip.vue')
    
    window.app = new Vue({
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
        }),

        data() {
            return {
                projects: []
            }
        },

        created() {
            this.updateProjects()
        },

        methods: {
            updateProjects() {
                return loadProjects()
                    .then(response => this.projects = response.projects)
            }
        }
    })

    function loadProjects() {
        return fetch('api/list-projects')
            .then(response => response.json())
    }

}