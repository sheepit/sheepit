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

            <button type="button" v-on:click="edit()" class="btn btn-primary">Save</button>
        </div>

        <h4>Environments</h4>
        <div>
            <draggable v-model="environments" class="row" @end="onEnvironmentDragEnd">
                <div v-for="(environment, index) in environments" class="col-md-3">
                    <div class="card">
                        <div class="card-header">
                            <editable-title v-bind:title="environment.displayName" @blur="(event) => { renameEnvironment(event, index) }" />
                        </div>
                        <ul class="list-group list-group-flush">
                            <li class="list-group-item lead"></li>
                        </ul>
                    </div>
                </div>
            </draggable>

            
        </div>
    </div>
</template>

<script>
    module.exports = {
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
                getProjectDetailss(this.projectId)
                    .then(response => {
                        this.project = response;
                        this.environments = this.project.environments;
                    });
            },

            edit: function () {
                editProject(this.projectId, this.repositoryUrl, this.environments)
                    .then(() => window.app.updateProjects())
                    .then(() => this.$router.push({ name: 'project', params: { projectId: this.projectId }}))
            },

            newEnvironment: function () {
                this.environments.push('');
            },

            onEnvironmentDragEnd($event) {
                const environmentIds = this.environments.map(f => (f.environmentId));
                updateEnvironmentRank(this.project.id, environmentIds);
            },

            renameEnvironment(displayName, index) {
                let environment = this.environments[index];
                updateEnvironmentDisplayName(environment.environmentId, displayName);
            }
        }
    }

    // TODO: [ts] Move such methods to service with typed contracts
    function getProjectDetailss(projectId) {
        return postData('api/get-project-details', { id: projectId })
            .then(response => response.json());
    }
 

    function editProject(projectId, repositoryUrl, environmentNames) {
        return postData('api/edit-project', {
            projectId: projectId,
            repositoryUrl: repositoryUrl,
            environmentNames: environmentNames
        })
    }
   
    function updateEnvironmentRank(projectId, environmentIds) {
        const request = {
            projectId: projectId,
            environmentIds: environmentIds
        };

        postData('api/update-environments-rank', request);
    }

    function updateEnvironmentDisplayName(environmentId, displayName) {
        const request = {
            environmentId: environmentId,
            displayName: displayName
        };

        postData('api/update-environment-display-name', request);
    } 
</script>