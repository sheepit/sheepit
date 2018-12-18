<template>
    <div>
        <h4>Deploy release <span class="badge badge-primary">{{ releaseId }}</span> to:</h4>
        <p>
            <button v-for="environment in environments" v-on:click="deploy(environment)" type="button" class="btn btn-outline-success mr-1">
                {{ environment }}
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
                // todo: should not be hardcoded
                environments: ['dev', 'test', 'prod']
            }
        },
        
        computed: {
            releaseId() {
                return this.$route.params.releaseId
            }
        },
        
        methods: {
            deploy(environmentId) {
                const request = {
                    projectId: this.project.id,
                    releaseId: this.releaseId,
                    environmentId: environmentId
                }
                
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
            }
        }
    }
</script>