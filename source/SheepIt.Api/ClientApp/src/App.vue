<template>
    <div id="app">
        <router-view />
    </div>
</template>

<script>
import VueRouter from 'vue-router';
import Authenticate from './pages/authenticate/authenticate.vue'
import ComponentsList from './pages/components/list/components-list'
import CreateProject from './pages/create-project/create-project.vue'
import CreatePackage from './pages/create-package/create-package.vue'
import Default from './pages/default/default.vue'
import DefaultLayout from './pages/default-layout/default-layout.vue'
import DeployPackage from './pages/deploy-package/deploy-package.vue'
import DeploymentDetails from './pages/deployment-details/deployment-details.vue'
import DeploymentsList from './pages/deloyments/list/deployments-list'
import EnvironmentsList from './pages/environments/list/environments-list'
import EditEnvironments from './pages/edit-environments/edit-environments.vue'
import PackagesList from './pages/package/list/packages-list'
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
                    component: Default
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
                                sideMenu: true,
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
                            path: 'environments',
                            name: 'environments-list',
                            component: EnvironmentsList,
                            meta: {
                                sideMenu: true,
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
                                        name: 'environments'
                                    }
                                ]
                            }
                        },
                        {
                            path: 'edit-environments',
                            name: 'edit-environments',
                            component: EditEnvironments,
                            meta: {
                                sideMenu: true,
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
                            path: 'packages',
                            name: 'packages-list',
                            component: PackagesList,
                            meta: {
                                sideMenu: true,
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
                            path: 'deployments',
                            name: 'deployments-list',
                            component: DeploymentsList,
                            meta: {
                                sideMenu: true,
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
                        },
                        {
                            path: 'components',
                            name: 'components-list',
                            component: ComponentsList,
                            meta: {
                                sideMenu: true,
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
                                        name: 'components'
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
