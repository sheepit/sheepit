<template>
    <div v-if="package">
        <h1>Package details</h1>

        <div class="form-group row">
            <label class="col-2 col-form-label">Id</label>
            <div class="col-10">
                <span>{{ package.id }}</span>
            </div>
        </div>

        <div class="form-group row">
            <label class="col-2 col-form-label">Project</label>
            <div class="col-10">
                <span>{{ package.projectId }}</span>
            </div>
        </div>

        <div class="form-group row">
            <label class="col-2 col-form-label">Created At</label>
            <div class="col-10">
                <humanized-date :date="package.createdAt" />
            </div>
        </div>
        
        <variable-details
            :variables="package.variables"
            :environments="environments"
        />

        <package-deployments
            :project="project"
            :package="package"
        />
    </div>
</template>

<script>
import httpService from "./../../common/http/http-service.js";

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
            package: null,
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
                .then(response => this.package = response);

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