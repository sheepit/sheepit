window.onload = function() {

    httpVueLoader.register(Vue, 'editable-title.vue');
    httpVueLoader.register(Vue, 'expanding-list.vue');
    httpVueLoader.register(Vue, 'humanized-date.vue');
    httpVueLoader.register(Vue, 'tooltip.vue');
    httpVueLoader.register(Vue, 'release-badge.vue');
    httpVueLoader.register(Vue, 'deployment-badge.vue');
    httpVueLoader.register(Vue, 'deployment-status-badge.vue');
    httpVueLoader.register(Vue, 'project-breadcrumbs.vue');
    
    window.app = new Vue({
        el: '#app',
        
        components: {
            'navigation': httpVueLoader('navigation.vue')
        },

        router: new VueRouter({
            routes: [
                {
                    path: '/project/:projectId',
                    component: httpVueLoader('project-layout.vue'),
                    children: [
                        {
                            path: '',
                            name: 'project',
                            component: httpVueLoader('project.vue')
                        },
                        {
                            path: 'edit',
                            name: 'edit-project',
                            component: httpVueLoader('edit-project.vue')
                        },
                        {
                            path: 'create-release',
                            name: 'create-release',
                            component: httpVueLoader('create-release.vue')
                        },
                        {
                            path: 'deployment-details/:deploymentId',
                            name: 'deployment-details',
                            component: httpVueLoader('deployment-details.vue')
                        },
                        {
                            path: 'deploy-release/:releaseId',
                            name: 'deploy-release',
                            component: httpVueLoader('deploy-release.vue')
                        },
                        {
                            path: 'release-details/:releaseId',
                            name: 'release-details',
                            component: httpVueLoader('release-details.vue')
                        }
                    ]
                },
                {
                    path: '/',
                    name: 'default',
                    component: httpVueLoader('default.vue')
                },
                {
                    path: '/create-project',
                    name: 'create-project',
                    component: httpVueLoader('create-project.vue')
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