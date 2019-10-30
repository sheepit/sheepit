<template>
    <div>
        <h4 class="mt-4">
            Deploy <package-badge
                :project-id="project.id"
                :package-id="packageId"
            /> to:
        </h4>
        <p>
            <button
                v-for="environment in environments"
                type="button"
                class="btn btn-outline-success mr-1"
                @click="deploy(environment.id)"
            >
                {{ environment.displayName }}
            </button>
        </p>
    </div>
</template>

<script>
import httpService from "./../../common/http/http-service.js";

export default {
    name: 'DeployPackage',
    
    props: ['project'],
    
    data() {
        return {
            environments: null
        }
    },
    
    computed: {
        packageId() {
            return this.$route.params.packageId
        }
    },

    created() {
        this.getEnvironments(this.project.id);
    },

    methods: {
        deploy(environmentId) {
            const request = {
                projectId: this.project.id,
                packageId: this.packageId,
                environmentId: environmentId
            };
            
            httpService
                .post('api/project/deployment/deploy-package', request)
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
            httpService
                .post('api/project/environment/list-environments', { projectId })
                .then(response => this.environments = response.environments)
        }
    }
}
</script>