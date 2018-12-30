<template>
    <div>
        <project-breadcrumbs v-bind:project-id="project.id">
            <li class="breadcrumb-item">
                releases
            </li>
            <li class="breadcrumb-item">
                {{ releaseId }}
            </li>
            <li class="breadcrumb-item">
                deploy
            </li>
        </project-breadcrumbs>
        
        <h4 class="mt-4">
            Deploy <release-badge v-bind:project-id="project.id" v-bind:release-id="releaseId"></release-badge> to:
        </h4>
        <p>
            <button v-for="environment in environments" v-on:click="deploy(environment.id)" type="button" class="btn btn-outline-success mr-1">
                {{ environment.displayName }}
            </button>
        </p>
    </div>
</template>

<script>
    module.exports = {
        name: 'deploy-release',
        
        props: ['project'],
        
        data() {
            return {
                environments: null
            }
        },
        
        computed: {
            releaseId() {
                return this.$route.params.releaseId
            }
        },

        created() {
            this.getEnvironments(this.project.id);
        },

        methods: {
            deploy(environmentId) {
                const request = {
                    projectId: this.project.id,
                    releaseId: this.releaseId,
                    environmentId: environmentId
                };
                
                postData('api/deploy-release', request)
                    .then(response => response.json())
                    .then(response => this.redirectToDeployment(response.createdDeploymentId))
            },
            redirectToDeployment(deploymentId) {
                this.$router.push({
                    name: 'deployment-details',
                    params: {
                        projectId: this.project.id,
                        deploymentId: deploymentId
                    }
                })
            },
            getEnvironments(projectId) {
                postData('api/list-environments', { projectId })
                    .then(response => response.json())
                    .then(response => this.environments = response.environments)
            }
        }
    }
</script>