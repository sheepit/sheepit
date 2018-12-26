<template>
    <div v-if="deployment">

        <project-breadcrumbs v-bind:project-id="project.id">
            <li class="breadcrumb-item">
                deployments
            </li>
            <li class="breadcrumb-item active">
                {{ deployment.id }}
            </li>
        </project-breadcrumbs>
        
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
                        <release-badge v-bind:project-id="project.id" v-bind:release-id="deployment.releaseId"></release-badge>
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
        
        <deployment-used-variables v-bind:used-variables="usedVariables"></deployment-used-variables>

    </div>
</template>

<script>
    module.exports = {
        name: 'deployment-details',

        components: {
            'deployment-used-variables': httpVueLoader('deployment-used-variables.vue')
        },
        
        props: [
            'project'
        ],
        
        data() {
            return {
                deployment: null,
                usedVariables: null,
                
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
                    .then(() => getDeploymentUsedVariables(
                        this.project.id,
                        this.deploymentId,
                        this.deployment.environmentId)
                    )
                    .then(response => this.usedVariables = response.values);
            }
        }
    };

    function getDeploymentDetails(projectId, deploymentId) {
        return postData('api/get-deployment-details', { projectId, deploymentId })
            .then(response => response.json())
    }

    function getDeploymentUsedVariables(projectId, deploymentId, environmentId) {
        return postData('api/get-deployment-used-variables', { projectId, deploymentId, environmentId })
            .then(response => response.json())
    }
</script>