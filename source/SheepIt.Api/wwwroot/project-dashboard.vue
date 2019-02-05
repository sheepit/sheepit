<template>

    <draggable v-model="environments" class="row" @end="onEnvironmentDragEnd">
        <div v-for="(environment, index) in environments" class="col-md-3">
            <div class="card">
                <div class="card-header tile">
                    <div class="content">
                        <span v-show="focusedIndex !== index">{{ environment.displayName }}</span>
                        <input v-show="focusedIndex === index" v-model="environment.displayName" @focus="focusField(index)" @blur="blurField()" type="text" />
                        
                    </div>
                    <span v-if="focusedIndex !== index" class="icon icon-pencil" @click="focusField(index)"></span>
                </div>
                <ul class="list-group list-group-flush" v-if="environment.deployment">
                    <li class="list-group-item lead">
                        <div>
                            <release-badge v-bind:project-id="project.id" v-bind:release-id="environment.deployment.currentReleaseId"></release-badge>
                        </div>
                        <div>
                            <deployment-badge v-bind:project-id="project.id" v-bind:deployment-id="environment.deployment.currentDeploymentId"></deployment-badge>
                        </div>
                    </li>
                    <li class="list-group-item">
                        Deployed: <br/>
                        <humanized-date v-bind:date="environment.deployment.lastDeployedAt"></humanized-date>
                    </li>
                </ul>
                <ul class="list-group list-group-flush" v-else>
                    <li class="list-group-item lead"></li>
                </ul>
            </div>
        </div>
    </draggable>

</template>

<script>
    module.exports = {
        name: "project-dashboard",
        
        props: [
            'project'
        ],

        data() {
            return {
                environments: [],
                focusedIndex: null
            }
        },

        watch: {
            'project': 'getDeploymentDetails'
        },

        created() {
            this.getDeploymentDetails();
        },

        methods: {
            getDeploymentDetails() {
                getDashboard(this.project.id)
                    .then(response => this.environments = response.environments)
            },

            onEnvironmentDragEnd($event) {
                const environmentIds = this.environments.map(f => (f.environmentId));
                updateEnvironmentRank(this.project.id, environmentIds);
            },

            focusField(index) {
                this.focusedIndex = index;
            },

            blurField() {
                this.focusedIndex = null;
            }
        }
    };

    function getDashboard(projectId) {
        return postData('api/project/dashboard/show-dashboard', { projectId })
            .then(response => response.json())
    }

    function updateEnvironmentRank(projectId, environmentIds) {
        const request = {
            projectId: projectId,
            environmentIds: environmentIds
        };

        postData('api/update-environments-rank', request);
    }
</script>

<style scoped>
.tile {
    display: flex;
}

.content {
    flex: 1;
}

.icon {
    visibility: hidden;
    border-radius: 4px;
}

.tile:hover .icon {
    visibility: visible;
    color: #888888;
}
    
.icon:hover {
    background-color: #d3d9df;
}
</style>