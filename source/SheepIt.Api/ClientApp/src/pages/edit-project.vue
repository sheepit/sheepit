<template>
    <div>
        <h4>Edit project</h4>
        <div>
            <div class="form-group">
                <label for="projectId">Project id</label>
                <input type="text" v-model="project.id" class="form-control" id="projectId" disabled="disabled">
            </div>

            <div class="form-group">
                <label for="repositoryUrl">Git repository URL</label>
                <input type="text" v-model="project.repositoryUrl" class="form-control" id="repositoryUrl">
            </div>

            <button type="button" v-on:click="save()" class="btn btn-primary">Save</button>
        </div>

        <h4>Environments</h4>
        <div>
            <draggable v-model="environments" class="row" @end="onEnvironmentDragEnd">
                <div v-for="(environment, index) in environments" class="col-md-3">
                    <div class="card">
                        <div class="card-header">
                            <editable-title v-bind:title="environment.displayName" @blur="(event) => { renameEnvironment(event, index) }" />
                        </div>
                    </div>
                </div>
                <new-environment-card @blur="addNewEnvironment($event)"></new-environment-card>
            </draggable>
        </div>
    </div>
</template>

<script>
import httpService from "./../common/http/http-service.js";

export default {
    name: 'edit-project',

    data() {
        return {
            project: null,
            environments: null
        }
    },

    computed: {
        projectId() {
            return this.$route.params.projectId
        }
    },

    mounted() {
        this.getProjectDetails();
    },

    methods: {
        getProjectDetails: function() {
            this.getProjectDetails();
        },

        save: function () {
            updateProject(this.projectId, this.project.repositoryUrl);
        },

        onEnvironmentDragEnd($event) {
            const environmentIds = this.environments.map(f => (f.environmentId));
            updateEnvironmentRank(this.project.id, environmentIds);
        },

        renameEnvironment(displayName, index) {
            let environment = this.environments[index];
            updateEnvironmentDisplayName(environment.environmentId, displayName, this.projectId);
        },

        addNewEnvironment(newEnvironmentDisplayName) {
            addNewEnvironment(this.project.id, newEnvironmentDisplayName)
                .then(response => {
                    this.getProjectDetails();
                });
        },

        getProjectDetails() {
            httpService
                .post('api/get-project-details', { id: this.projectId })
                .then(response => {
                    this.project = response;
                    this.environments = this.project.environments;
                });
        }
    }
}

function updateProject(projectId, repositoryUrl) {
    return httpService.post('api/update-project', {
        projectId: projectId,
        repositoryUrl: repositoryUrl
    })
}

function updateEnvironmentRank(projectId, environmentIds) {
    const request = {
        projectId: projectId,
        environmentIds: environmentIds
    };

    httpService.post('api/project/environment/update-environments-rank', request);
}

function updateEnvironmentDisplayName(environmentId, displayName, projectId) {
    const request = {
        projectId: projectId,
        environmentId: environmentId,
        displayName: displayName
    };

    httpService.post('api/project/environment/update-environment-display-name', request);
}

function addNewEnvironment(projectId, displayName) {
    const request = {
        projectId: projectId,
        displayName: displayName
    };

    return httpService.post('api/project/environment/add-environment', request);
} 
</script>