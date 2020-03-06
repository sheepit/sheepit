<template>
    <div>
        <div class="view__title">
            Package details
        </div>

        <div v-if="packagee">
            <div class="form__row row">
                <label class="col-2 col-form__label">Id</label>
                <div class="col-10 value">
                    <span>{{ packagee.id }}</span>
                </div>
            </div>

            <div class="form__row row">
                <label class="col-2 col-form__label">Project</label>
                <div class="col-10 value">
                    <span>{{ packagee.projectId }}</span>
                </div>
            </div>

            <div class="form__row row">
                <label class="col-2 col-form__label">Description</label>
                <div class="col-10 value">
                    <span>{{ packagee.description }}</span>
                </div>
            </div>

            <div class="form__row row">
                <label class="col-2 col-form__label">Created At</label>
                <div class="col-10 value">
                    <humanized-date :date="packagee.createdAt" />
                </div>
            </div>
            
            <variable-details
                :variables="packagee.variables"
                :environments="environments"
            />

            <package-deployments
                :project="project"
                :package="packagee"
            />
        </div>
        <div v-else>
            <preloader />
        </div>
    </div>
</template>

<script>
import httpService from "./../../../common/http/http-service.js";

import VariableDetails from "./_components/variable-details.vue";
import PackageDeployments from "./_components/package-deployments.vue";

export default {
    name: 'PackageDetails',

    components: {
        'variable-details': VariableDetails,
        'package-deployments': PackageDeployments
    },

    props: [
        'project'
    ],
    
    data() {
        return {
            packagee: null,
            environments: null
        }
    },

    computed: {
        packageId() {
            return this.$route.params.packageId
        }
    },

    watch: {
        'project': {
            immediate: true,
            handler: 'getPackageDetails'
        },
        'packageId': {
            immediate: true,
            handler: 'getPackageDetails'
        }            
    },

    created() {
        this.getProjectEnvironments();
    },
    
    methods: {
        getPackageDetails() {
            getPackageDetails(this.project.id, this.packageId)
                .then(response => this.packagee = response);

            this.getProjectEnvironments();
        },
        
        getProjectEnvironments() {
            httpService.post('api/project/environment/list-environments', { projectId: this.project.id })
                .then(response => this.environments = response.environments);
        }
    }
};

function getPackageDetails(projectId, packageId) {
    return httpService
        .post('api/project/package/get-package-details', { projectId, packageId });
}
</script>

<style lang="scss" scoped>
.value {
    padding-top: 7px;
    padding-bottom: 7px;
}
</style>