<template>
    <div class="details">
        <div class="view__title">
            Package {{ packagee.id }} ({{ packagee.description }})
        </div>

        <div v-if="packagee">
            <div class="details__section">
                <div class="details__title">
                    Details
                </div>

                <div class="details__content">
                    <div class="details__item">
                        <label class="details__label">component</label>
                        <span class="details__value">
                            {{ packagee.componentName ? packagee.componentName : '-' }}
                        </span>
                    </div>
                    <div class="details__item">
                        <label class="details__label">description</label>
                        <span class="details__value">
                            {{ packagee.description }}
                        </span>
                    </div>
                    <div class="details__item">
                        <label class="details__label">created at</label>
                        <span class="details__value">
                            <humanized-date :date="packagee.createdAt" />
                        </span>
                    </div>
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
            httpService.post('frontendApi/project/environment/list-environments', { projectId: this.project.id })
                .then(response => this.environments = response.environments);
        }
    }
};

function getPackageDetails(projectId, packageId) {
    return httpService
        .post('frontendApi/project/package/get-package-details', { projectId, packageId });
}
</script>

<style lang="scss" scoped>
.value {
    padding-top: 7px;
    padding-bottom: 7px;
}
</style>