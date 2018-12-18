<template>
    <div v-if="deployment">

        <nav>
            <ol class="breadcrumb">
                <li class="breadcrumb-item"><a href="#">Projects</a></li>
                <li class="breadcrumb-item"><a href="#">{{ project.id }}</a></li>
                <li class="breadcrumb-item"><a href="#">Deployments</a></li>
                <li class="breadcrumb-item active">{{ deployment.id }}</li>
            </ol>
        </nav>

        <table class="table table-bordered">
            <thead>
                <tr>
                    <th>id</th>
                    <th>status</th>
                    <th>release id</th>
                    <th>environment id</th>
                    <th>deployed at</th>                    
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        <deployment-badge v-bind:project-id="project.id" v-bind:deployment-id="deployment.id"></deployment-badge>
                    </td>
                    <td>
                        <deployment-status-badge v-bind:status="deployment.status"></deployment-status-badge>
                    </td>
                    <td>
                        <span class="badge badge-primary">{{ deployment.releaseId }}</span>
                    </td>
                    <td>
                        <span class="badge badge-warning">{{ deployment.environmentId }}</span>
                    </td>
                    <td>
                        <humanized-date v-bind:date="deployment.deployedAt"></humanized-date>
                    </td>
                </tr>
            </tbody>
        </table>
        
        <div>
            <div class="mt-4" v-for="stepResult in deployment.stepResults">
                <pre class="mb-0" v-bind:class="stepResult.successful ? '' : 'text-danger'"><b><code>{{ stepResult.command }}</code></b></pre>
                <pre v-bind:class="stepResult.successful ? '' : 'text-danger'"><code>{{ stepResult.output.join("\n") }}</code></pre>
            </div>
        </div>

    </div>
</template>

<script>
    module.exports = {
        name: 'deployment-details',

        props: [
            'project'
        ],
        
        data() {
            return {
                deployment: null,
                
                // todo: this is duplicated
                deploymentStatusStyles: {
                    InProgress: 'info',
                    Succeeded: 'success',
                    ProcessFailed: 'danger',
                    ExecutionFailed: 'danger'
                }
            }
        },

        computed: {
            deploymentId() {
                return this.$route.params.deploymentId
            }
        },

        watch: {
            'project': {
                immediate: true,
                handler: 'getDeploymentDetails'
            },
            'deploymentId': {
                immediate: true,
                handler: 'getDeploymentDetails'
            }            
        },
        
        methods: {
            getDeploymentDetails() {
                getDeploymentDetails(this.project.id, this.deploymentId)
                    .then(response => this.deployment = response)
            }
        }
    }

    function getDeploymentDetails(projectId, deploymentId) {
        return postData('api/get-deployment-details', { projectId, deploymentId })
            .then(response => response.json())
    }
</script>