<template>
    <div id="app">
        <router-view />
    </div>
</template>

<script>
import VueRouter from 'vue-router';

import Authenticate from './pages/authenticate/authenticate.vue'
import CreateProject from './pages/create-project/create-project.vue'
import CreateRelease from './pages/create-release/create-release.vue'
import Default from './pages/default/default.vue'
import DefaultLayout from './pages/default-layout/default-layout.vue'
import DeployRelease from './pages/deploy-release/deploy-release.vue'
import DeploymentDetails from './pages/deployment-details/deployment-details.vue'
import EditProject from './pages/edit-project/edit-project.vue'
import Project from './pages/project/project.vue'
import ProjectLayout from './pages/project-layout/project-layout.vue'
import ProjectList from './pages/project-list/project-list.vue'
import ReleaseDetails from './pages/release-details/release-details.vue'


const router = new VueRouter({
    routes: [
        {
            path: '/sign-in',
            name: 'sign-in',
            component: Authenticate
        },
        {
            path: '/',
            component: DefaultLayout,
            children: [
                {
                    path: '',
                    name: 'default',
                    component: Default
                },
                {
                    path: '/create-project',
                    name: 'create-project',
                    component: CreateProject
                },
                {
                    path: '/project-list',
                    name: 'project-list',
                    component: ProjectList
                },
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
                }
            ]
        }
    ]
});

export default {
    name: 'App',

    router
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
