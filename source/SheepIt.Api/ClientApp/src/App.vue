<template>
    <div id="app">
        <router-view />
    </div>
</template>

<script>
import VueRouter from 'vue-router';
import Authenticate from './pages/authenticate/authenticate.vue'
import CreateProject from './pages/create-project/create-project.vue'
import CreatePackage from './pages/create-package/create-package.vue'
import Default from './pages/default/default.vue'
import DefaultLayout from './pages/default-layout/default-layout.vue'
import DeployPackage from './pages/deploy-package/deploy-package.vue'
import DeploymentDetails from './pages/deployment-details/deployment-details.vue'
import EditEnvironments from './pages/edit-environments/edit-environments.vue'
import Project from './pages/project/project.vue'
import ProjectLayout from './pages/project-layout/project-layout.vue'
import ProjectList from './pages/project-list/project-list.vue'
import PackageDetails from './pages/package-details/package-details.vue'

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
                    component: Default,
                    meta: {
                    }
                },
                {
                    path: '/create-project',
                    name: 'create-project',
                    component: CreateProject,
                    meta: {
                        breadcrumbs: [
                            {
                                name: 'projects',
                                link: 'projects'
                            },
                            {
                                name: 'create project'
                            }
                        ]
                    }
                },
                {
                    path: '/projects',
                    name: 'projects',
                    component: ProjectList,
                    meta: {
                    }
                },
                {
                    path: '/projects/:projectId',
                    component: ProjectLayout,
                    children: [
                        {
                            path: '',
                            name: 'project',
                            component: Project,
                            meta: {
                                breadcrumbs: [
                                    {
                                        name: 'projects',
                                        link: 'projects'
                                    },
                                    {
                                        name: ':projectId'
                                    }
                                ]
                            }
                        },
                        {
                            path: 'edit-environments',
                            name: 'edit-environments',
                            component: EditEnvironments,
                            meta: {
                                breadcrumbs: [
                                    {
                                        name: 'projects',
                                        link: 'projects'
                                    },
                                    {
                                        name: ':projectId',
                                        link: 'project'
                                    },
                                    {
                                        name: 'edit environments'
                                    }
                                ]
                            }
                        },
                        {
                            path: 'create-package',
                            name: 'create-package',
                            component: CreatePackage,
                            meta: {
                                breadcrumbs: [
                                    {
                                        name: 'projects',
                                        link: 'projects'
                                    },
                                    {
                                        name: ':projectId',
                                        link: 'project'
                                    },
                                    {
                                        name: 'create package'
                                    }
                                ]
                            }
                        },
                        {
                            path: 'deployment-details/:deploymentId',
                            name: 'deployment-details',
                            component: DeploymentDetails,
                            meta: {
                                breadcrumbs: [
                                    {
                                        name: 'projects',
                                        link: 'projects'
                                    },
                                    {
                                        name: ':projectId',
                                        link: 'project'
                                    },
                                    {
                                        name: 'deployments'
                                    },
                                    {
                                        name: ':deploymentId'
                                    }
                                ]
                            }
                        },
                        {
                            path: 'deploy-package/:packageId',
                            name: 'deploy-package',
                            component: DeployPackage,
                            meta: {
                                breadcrumbs: [
                                    {
                                        name: 'projects',
                                        link: 'projects'
                                    },
                                    {
                                        name: ':projectId',
                                        link: 'project'
                                    },
                                    {
                                        name: 'packages'
                                    },
                                    {
                                        name: ':packageId'
                                    },
                                    {
                                        name: 'deploy'
                                    }
                                ]
                            }
                        },
                        {
                            path: 'package-details/:packageId',
                            name: 'package-details',
                            component: PackageDetails,
                            meta: {
                                breadcrumbs: [
                                    {
                                        name: 'projects',
                                        link: 'projects'
                                    },
                                    {
                                        name: ':projectId',
                                        link: 'project'
                                    },
                                    {
                                        name: 'packages'
                                    },
                                    {
                                        name: ':packageId'
                                    }
                                ]
                            }
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
