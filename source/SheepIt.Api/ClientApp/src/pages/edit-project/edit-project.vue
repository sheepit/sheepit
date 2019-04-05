<template>
    <div v-if="project">
        <div class="row project-title">
            <div class="col">
                <h2 class="display-4">
                    {{ project.id }}
                </h2>
            </div>
        </div>

        <h3 class="mt-5">
            Edit project
        </h3>
        <div>
            <div class="form-group">
                <label for="projectId">Project id</label>
                <input
                    id="projectId"
                    v-model="project.id"
                    type="text"
                    class="form-control"
                    disabled="disabled"
                >
            </div>

            <div class="form-group">
                <label for="repositoryUrl">Git repository URL</label>
                <input
                    id="repositoryUrl"
                    v-model="project.repositoryUrl"
                    type="text"
                    class="form-control"
                >
            </div>

            <div class="save-button-container">
                <button
                    type="button"
                    class="btn btn-primary"
                    @click="save()"
                >
                    Save
                </button>
            </div>
        </div>

        <h3 class="mt-5">
            Environments
        </h3>
        <div>
            <draggable
                v-model="environments"
                class="row"
                @end="onEnvironmentDragEnd"
            >
                <div
                    v-for="(environment, index) in environments"
                    :key="index"
                    class="col-md-3"
                >
                    <div class="card">
                        <div class="card-header environment-card-header">
                            <editable-title
                                :title="environment.displayName"
                                @blur="(event) => { renameEnvironment(event, index) }"
                            />
                        </div>
                    </div>
                </div>
                <new-environment-card
                    class="add-environment-button"
                    @blur="addNewEnvironment($event)"
                />
            </draggable>
        </div>
    </div>
</template>

<script>
import httpService from "./../../common/http/http-service.js";
import draggable from 'vuedraggable';

import EditableTitle from "./_components/editable-title.vue";
import NewEnvironmentCard from "./_components/new-environment-card.vue";

export default {
    name: 'EditProject',

    components: {
        draggable,
        EditableTitle,
        NewEnvironmentCard
    },

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
        save: function () {
            updateProject(this.projectId, this.project.repositoryUrl)
                .then(() => { });
        },

        onEnvironmentDragEnd() {
            const environmentIds = this.environments.map(f => (f.environmentId));
            updateEnvironmentRank(this.project.id, environmentIds);
        },

        renameEnvironment(displayName, index) {
            let environment = this.environments[index];
            updateEnvironmentDisplayName(environment.environmentId, displayName, this.projectId);
        },

        addNewEnvironment(newEnvironmentDisplayName) {
            addNewEnvironment(this.project.id, newEnvironmentDisplayName)
                .then((response) => {
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
    }, false);
}

function updateEnvironmentRank(projectId, environmentIds) {
    const request = {
        projectId: projectId,
        environmentIds: environmentIds
    };

    httpService.post('api/project/environment/update-environments-rank', request, false);
}

function updateEnvironmentDisplayName(environmentId, displayName, projectId) {
    const request = {
        projectId: projectId,
        environmentId: environmentId,
        displayName: displayName
    };

    httpService.post('api/project/environment/update-environment-display-name', request, false);
}

function addNewEnvironment(projectId, displayName) {
    const request = {
        projectId: projectId,
        displayName: displayName
    };

    return httpService.post('api/project/environment/add-environment', request, false);
} 
</script>

<style lang="scss" scoped>
.card {
    margin-bottom: 1rem;
}

.save-button-container {
    display: flex;
    justify-content: flex-end;
}

.add-environment-button {
    height: 51px;
    width: 255px;
}

.environment-card-header {
    min-height: 49px;
    height: 100%;
    width: 100%;
}
</style>
