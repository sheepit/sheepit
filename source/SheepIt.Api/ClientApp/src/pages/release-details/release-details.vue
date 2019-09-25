<template>
    <div v-if="release">
        <h1>Release details</h1>

        <div class="form-group row">
            <label class="col-2 col-form-label">Id</label>
            <div class="col-10">
                <span>{{ release.id }}</span>
            </div>
        </div>

        <div class="form-group row">
            <label class="col-2 col-form-label">Project</label>
            <div class="col-10">
                <span>{{ release.projectId }}</span>
            </div>
        </div>

        <div class="form-group row">
            <label class="col-2 col-form-label">Created At</label>
            <div class="col-10">
                <humanized-date :date="release.createdAt" />
            </div>
        </div>
        
        <variable-details
            :variables="release.variables"
            :environments="environments"
        />

        <release-deployments
            :project="project"
            :release="release"
        />
    </div>
</template>

<script>
import httpService from "./../../common/http/http-service.js";

import VariableDetails from "./_components/variable-details.vue";
import ReleaseDeployments from "./_components/release-deployments.vue";

export default {
    name: 'ReleaseDetails',

    components: {
        'variable-details': VariableDetails,
        'release-deployments': ReleaseDeployments
    },

    props: [
        'project'
    ],
    
    data() {
        return {
            release: null,
            environments: null
        }
    },

    computed: {
        releaseId() {
            return this.$route.params.releaseId
        }
    },

    watch: {
        'project': {
            immediate: true,
            handler: 'getReleaseDetails'
        },
        'releaseId': {
            immediate: true,
            handler: 'getReleaseDetails'
        }            
    },

    created() {
        this.getProjectEnvironments();
    },
    
    methods: {
        getReleaseDetails() {
            getReleaseDetails(this.project.id, this.releaseId)
                .then(response => this.release = response);

            this.getProjectEnvironments();
        },
        
        getProjectEnvironments() {
            httpService.post('api/project/environment/list-environments', { projectId: this.project.id })
                .then(response => this.environments = response.environments);
        }
    }
};

function getReleaseDetails(projectId, releaseId) {
    return httpService
        .post('api/project/release/get-release-details', { projectId, releaseId });
}
</script>