<template>
    <div>
        <div class="view__title">
            Deploy
        </div>

        <div class="details__section">
            <div class="details__title">
                Details
            </div>

            <div class="details__content">
                <div v-if="packagee" class="details__item">
                    <label class="details__label">package</label>
                    <span class="details__value">
                        <package-badge
                            :project-id="project.id"
                            :package-id="packagee.id"
                            :description="packagee.description"
                        />
                    </span>
                </div>
                <div v-else class="details__item">
                    <preloader />
                </div>
            </div>
        </div>

        <div v-if="environments" class="form">
            <div class="form__section">
                <div class="form__title">
                    Select Environment
                </div>

                <div class="form__row">
                    <div class="form__column">
                        <label
                            for="description"
                            class="form__label"
                        >
                            Description
                        </label>
                        <select class="form__control" v-model="selectedEnvironment">
                            <option></option>
                            <option v-for="environment in environments"  v-bind:value="environment.id">{{ environment.displayName }}</option>
                        </select>
                        <div
                            v-if="submitted && $v.description.$error"
                            class="invalid-feedback"
                        >
                            <span v-if="!$v.description.required">Field is required</span>
                            <span v-if="!$v.description.minLength">Field should have at least 1 character</span>
                        </div>
                    </div>
                </div>
            </div>


            <div class="submit-button-container">
                <router-link
                    class="button button--secondary"
                    :to="{ name: 'packages-list' }"
                    tag="button"
                    type="button"
                >
                    Cancel
                </router-link>

                <button
                    type="button"
                    class="button button--primary"
                    @click="deploy(0)"
                >
                    Deploy
                </button>
            </div>
        </div>
        <div v-else>
            <preloader />
        </div>
    </div>
</template>

<script>
import httpService from "./../../../common/http/http-service.js";
import deployPackageService from "./_services/deploy-package-service.js";

export default {
    name: 'DeployPackage',
    
    props: ['project'],
    
    data() {
        return {
            environments: null,
            packagee: null,
            selectedEnvironment: null
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
        deploy() {
            const environmentId = this.selectedEnvironment;

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