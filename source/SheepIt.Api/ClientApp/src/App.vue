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

import Default from './pages/default.vue'
import Navigation from './components/layout/navigation.vue'
import ProjectLayout from './pages/project/project-layout.vue'
import Project from './pages/project/project.vue'
import CreateProject from './pages/project/create-project.vue'
import CreateRelease from './pages/create-release.vue'

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
                {
                    path: 'create-release',
                    name: 'create-release',
                    component: CreateRelease
                },
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
                .then(response => response.json())
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
    text-align: center;
    color: #2c3e50;
    margin-top: 60px;
}
</style>
