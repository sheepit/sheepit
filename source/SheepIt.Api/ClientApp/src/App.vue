<template>
    <div id="app">
        <Navigation/>
    </div>
</template>

<script>
import Navigation from './components/layout/navigation.vue'
import ProjectLayout from './pages/project/project-layout.vue'
import Project from './pages/project/project.vue'

const router = new VueRouter({
    routes: [
        {
            path: '/project/:projectId',
            component: ProjectLayout,
            children: [
                {
                    path: '',
                    name: 'project',
                    component: Project
                },
                // {
                //     path: 'edit',
                //     name: 'edit-project',
                //     component: httpVueLoader('edit-project.vue')
                // },
                // {
                //     path: 'create-release',
                //     name: 'create-release',
                //     component: httpVueLoader('create-release.vue')
                // },
                // {
                //     path: 'deployment-details/:deploymentId',
                //     name: 'deployment-details',
                //     component: httpVueLoader('deployment-details.vue')
                // },
                // {
                //     path: 'deploy-release/:releaseId',
                //     name: 'deploy-release',
                //     component: httpVueLoader('deploy-release.vue')
                // },
                // {
                //     path: 'release-details/:releaseId',
                //     name: 'release-details',
                //     component: httpVueLoader('release-details.vue')
                // }
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
});

export default {
    name: 'app',

    components: {
        Navigation
    },

    router,

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
}
</script>

<style>
#app {
    font-family: 'Avenir', Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    text-align: center;
    color: #2c3e50;
    margin-top: 60px;
}
</style>
