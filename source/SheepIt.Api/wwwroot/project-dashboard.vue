<template>

    <draggable v-model="environments" class="row" @end="onEnvironmentDragEnd">
        <div v-for="(environment, index) in environments" class="col-md-3">
            <div class="card">
                <div class="card-header">
                    <editable-title />
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
            'project'
        ],

        data() {
            return {
                environments: []
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
