<template>
    <div>
        <div class="view__title">
            Deploy
        </div>

        <div v-if="packagee">
            <h3 class="mt-4 package-desciption">
                Package: 
                <package-badge
                    :project-id="project.id"
                    :package-id="packageId"
                    :description="packagee.description"
                />
            </h3>
        </div>
        <div v-else>
            <h3 class="mt-4 package-desciption">
                Package: 
                <preloader />
            </h3>
        </div>
        
        <div class="row">
            <div class="col">
                <h3>
                    Environments
                </h3>
            </div>
        </div>

        <p v-if="environments">
            <!-- <select class="form__control" id="exampleFormControlSelect1">
                <option
                    v-for="environment in environments"
                    :key="environment.id">
                    {{ environment.displayName }}
                </option>
            </select> -->

            <button
                v-for="environment in environments"
                :key="environment.id"
                type="button"
                class="btn btn-outline-success mr-1"
                @click="deploy(environment.id)"
            >
                {{ environment.displayName }}
            </button>
        </p>
        <p v-else>
            <preloader />
        </p>
    </div>
</template>

<script>
import httpService from "./../../common/http/http-service.js";
import deployPackageService from "./_services/deploy-package-service.js";

export default {
    name: 'DeployPackage',
    
    props: ['project'],
    
    data() {
        return {
            environments: null,
            packagee: null
        }
    },
    
    computed: {
        packageId() {
            return this.$route.params.packageId
        }
    },

    created() {
        this.getEnvironments(this.project.id);
        this.getPackageDetails(this.project.id, this.packageId);
    },

    methods: {
        deploy(environmentId) {
            deployPackageService
                .deploy(this.project.id, this.packageId, environmentId)
                .then(response => this.redirectToDeployment(response.createdDeploymentId));
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
            deployPackageService
                .getEnvironments(this.project.id)
                .then(response => this.environments = response.environments);
        },
        getPackageDetails() {
            deployPackageService
                .getPackageDetails(this.project.id, this.packageId)
                .then(response => this.packagee = response);
        }
    }
};
</script>

<style lang="scss" scoped>
.package-desciption {
    margin-bottom: 2.5rem;
}
</style>