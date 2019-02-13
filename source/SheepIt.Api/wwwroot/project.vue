<template>
    <div v-if="project">
        
        <project-breadcrumbs v-bind:project-id="project.id">
        </project-breadcrumbs>
        
        <div class="row">
            <div class="col">
                <h2 class="display-4">{{ project.id }}</h2>
                <p><code>{{ project.repositoryUrl }}</code></p>
            </div>
            <div class="col text-right">
                <p>
                    <router-link class="btn btn-primary" :to="{ name: 'edit-project', params: { projectId: project.id }}">
                        Edit project
                    </router-link>
                </p>
                <p>
                    <router-link class="btn btn-primary" :to="{ name: 'create-release', params: { projectId: project.id }}">
                        Edit variables
                    </router-link>
                </p>
                <p>
                    <button class="btn btn-primary" v-on:click="updateProcess()">
                        Update process
                    </button>
                </p>
            </div>
        </div>
        
        <h3 class="mt-5">Dashboard</h3>
        <project-dashboard class="mt-4" v-bind:project="project" v-bind:environments="environments"></project-dashboard>

        <h3 class="mt-5">Releases</h3>
        <project-releases v-bind:project="project"></project-releases>

        <h3 class="mt-5">Deployments</h3>
        <project-deployments v-bind:project="project"></project-deployments>

    </div>
</template>

<script>
    module.exports = {
        name: 'project',
        
        components: {
            'project-dashboard': httpVueLoader('project-dashboard.vue'),
            'project-releases': httpVueLoader('project-releases.vue'),
            'project-deployments': httpVueLoader('project-deployments.vue')
        },
        
        props: [
            'project'
        ],
        
        data() {
            return {
                environments: []
            }
        },
        
        watch: {
            'project': 'getDeploymentDetails'
        },

        created() {
            this.getDeploymentDetails();
        },

        methods: {
            getDeploymentDetails() {
                getDashboard(this.project.id)
                    .then(response => this.environments = response.environments);
            },
            updateProcess() {
                updateProcess(this.project.id)
                    .then(() => window.app.updateProjects())
            }
        }
    };
    
    function getDashboard(projectId) {
        return postData('api/project/dashboard/show-dashboard', { projectId })
            .then(response => response.json())
    }
    
    function updateProcess(projectId) {
        return postData('api/update-release-process', { projectId })
            .then(response => response.json())
    }
</script>
