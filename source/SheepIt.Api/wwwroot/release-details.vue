<template>
    <div v-if="release">
        <h1>Release details</h1>

        <project-breadcrumbs v-bind:project-id="project.id">
            <li class="breadcrumb-item">
                deployments
            </li>
            <li class="breadcrumb-item active">
                {{ release.id }}
            </li>
        </project-breadcrumbs>

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
            <label class="col-2 col-form-label">Commit SHA</label>
            <div class="col-10">
                <span>{{ release.commitSha }}</span>
            </div>
        </div>

        <div class="form-group row">
            <label class="col-2 col-form-label">Created At</label>
            <div class="col-10">
                <humanized-date v-bind:date="release.createdAt"></humanized-date>
            </div>
        </div>
        
        <div>
            <variable-details v-bind:variables="release.variables" v-bind:environments="['dev', 'test', 'prod']">
            </variable-details>
        </div>
    </div>
</template>

<script>
    module.exports = {
        name: 'release-details',

        components: {
            'variable-details': httpVueLoader('variable-details.vue')
        },

        props: [
            'project'
        ],
        
        data() {
            return {
                release: null
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
        
        methods: {
            getReleaseDetails() {
                getReleaseDetails(this.project.id, this.releaseId)
                    .then(response => this.release = response)
            }
        }
    };

    function getReleaseDetails(projectId, releaseId) {
        return postData('api/get-release-details', { projectId, releaseId })
            .then(response => response.json());
    }
</script>