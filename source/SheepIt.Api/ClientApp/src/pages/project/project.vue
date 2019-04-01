<template>
    <div v-if="project">
        
        <project-breadcrumbs v-bind:project-id="project.id"></project-breadcrumbs>
        
        <div class="row project-title">
            <div class="col">
                <h2 class="display-4">{{ project.id }}</h2>
                <p><code>{{ project.repositoryUrl }}</code></p>
            </div>
            <div class="col text-right">
                <router-link class="btn btn-primary link-button" :to="{ name: 'edit-project' }">
                    Edit project
                </router-link>
                <router-link class="btn btn-primary link-button" :to="{ name: 'create-release', params: { projectId: project.id }}">
                    Edit variables
                </router-link>
                <button class="btn btn-primary" v-on:click="updateProcess()">
                    Update process
                </button>
            </div>
        </div>
        
        <h3 class="mt-5">Environments</h3>
        <project-dashboard class="mt-4" :project="project" :environments="environments"></project-dashboard>

        <h3 class="mt-5">Releases</h3>
        <project-releases :project="project" :releases="releases"></project-releases>

        <h3 class="mt-5">Deployments</h3>
        <project-deployments :project="project" :deployments="deployments"></project-deployments>

    </div>
</template>

<script>
import getDashboardService from "./_services/get-dashboard-service.js";
import updateProcessService from "./_services/update-process-service.js";

import ProjectDashboard from "./_components/project-dashboard.vue";
import ProjectReleases from "./_components/project-releases.vue";
import ProjectDeployments from "./_components/project-deployments.vue";

export default {
    name: 'project',
    
    components: {
        'project-dashboard': ProjectDashboard,
        'project-releases': ProjectReleases,
        'project-deployments': ProjectDeployments
    },
    
    props: [
        'project'
    ],
    
    data() {
        return {
            deployments: [],
            environments: [],
            releases: []
        }
    },
     
    watch: {
        'project': 'getDashboard'
    },

    created() {
        this.getDashboard();
    },

    methods: {
        getDashboard() {
            getDashboardService
                .getDashboard(this.project.id)
                .then(response => {
                    this.environments = response.environments
                    this.deployments = response.deployments
                    this.releases = response.releases
                });
        },

        updateProcess() {
            updateProcessService
                .updateProcess(this.project.id)
                .then(() => window.app.updateProjects())
        }
    }
};
</script>

<style>
.project-title {
    text-align: left;
}

.link-button {
    margin-right: 0.5rem;
}
</style>