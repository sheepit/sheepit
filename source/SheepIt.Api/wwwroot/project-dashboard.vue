<template>

    <draggable v-model="environments" class="row" @end="onEnvironmentDragEnd">
        <div v-for="(environment, index) in environments" class="col-md-3">
            <div class="card">
                <div class="card-header">
                    <editable-title v-bind:title="environment.displayName" @blur="(event) => { renameEnvironment(event, index) }" />
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
        
        components: {
            'editable-title': httpVueLoader('editable-title.vue'),
        },

        props: [
            'project',
            'environments'
        ],

        data() {
            return {
            }
        },

        methods: {
            onEnvironmentDragEnd($event) {
                const environmentIds = this.environments.map(f => (f.environmentId));
                updateEnvironmentRank(this.project.id, environmentIds);
            },

            renameEnvironment(displayName, index) {
                let environment = this.environments[index];
                updateEnvironmentDisplayName(environment.environmentId, displayName);
            }
        },

        directives: {
            focus: {
                inserted(el) {
                    el.focus();
                }
            }
        }
    };

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
