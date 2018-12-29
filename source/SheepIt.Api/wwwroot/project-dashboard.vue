<template>
    
    <div class="row">
        <div v-for="environment in environments" class="col-md-3">

            <div class="card">
                <div class="card-header">
                    {{ environment.displayName }}
                </div>
                <ul class="list-group list-group-flush" v-if="environment.deployment">
                    <li class="list-group-item lead">
                        <div>
                            <release-badge v-bind:project-id="project.id" v-bind:release-id="environment.currentReleaseId"></release-badge>
                        </div>
                        <div>
                            <deployment-badge v-bind:project-id="project.id" v-bind:deployment-id="environment.currentDeploymentId"></deployment-badge>
                        </div>
                    </li>
                    <li class="list-group-item">
                        Deployed: <br/>
                        <humanized-date v-bind:date="environment.lastDeployedAt"></humanized-date>
                    </li>
                </ul>
                <ul class="list-group list-group-flush" v-else>
                    <li class="list-group-item lead"></li>
                </ul>
            </div>

        </div>
    </div>
    
</template>

<script>
    module.exports = {
        name: "project-dashboard",
        
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
            }
        }
    };

    function getDashboard(projectId) {
        return postData('api/show-dashboard', { projectId })
            .then(response => response.json())
    }
</script>