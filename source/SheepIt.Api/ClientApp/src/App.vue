<template>
    <div id="app">
        <div class="container">

            <navigation v-bind:projects="projects"></navigation>
            
            <main class="mt-5">
                <router-view v-bind:projects="projects"></router-view>
            </main>

        </div>
    </div>
</template>

<script>
import VueRouter from 'vue-router';

import httpService from "./common/http/http-service.js";

import Navigation from './components/layout/navigation.vue'

import CreateProject from './pages/create-project/create-project.vue'
import CreateRelease from './pages/create-release/create-release.vue'
import Default from './pages/default/default.vue'
import DeployRelease from './pages/deploy-release/deploy-release.vue'
import DeploymentDetails from './pages/deployment-details/deployment-details.vue'
import EditProject from './pages/edit-project/edit-project.vue'
import Project from './pages/project/project.vue'
import ProjectLayout from './pages/project-layout/project-layout.vue'
import ReleaseDetails from './pages/release-details/release-details.vue'

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
                {
                    path: 'edit',
                    name: 'edit-project',
                    component: EditProject
                },
                {
                    path: 'create-release',
                    name: 'create-release',
                    component: CreateRelease
                },
                {
                    path: 'deployment-details/:deploymentId',
                    name: 'deployment-details',
                    component: DeploymentDetails
                },
                {
                    path: 'deploy-release/:releaseId',
                    name: 'deploy-release',
                    component: DeployRelease
                },
                {
                    path: 'release-details/:releaseId',
                    name: 'release-details',
                    component: ReleaseDetails
                }
            ]
        },
        {
            path: '/',
            name: 'default',
            component: Default
        },
        {
            path: '/create-project',
            name: 'create-project',
            component: CreateProject
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
            return httpService
                .get('api/list-projects', null)
                .then(response => this.projects = response.projects)
        }
    }
};
</script>

<style>
#app {
    font-family: 'Avenir', Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    color: #2c3e50;
}
</style>
