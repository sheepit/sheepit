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
                    <th>environment</th>
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
                        <span class="badge badge-warning">{{ deployment.environmentDisplayName }}</span>
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
        
        <deployment-used-variables v-bind:used-variables="deployment.variables"></deployment-used-variables>

    </div>
</template>

<script>
import httpService from "./../common/http/http-service.js"

import DeploymentUsedVariables from "./deployment-used-variables"

export default {
    name: 'deployment-details',

    components: {
        'deployment-used-variables': DeploymentUsedVariables
    },
    
    props: [
        'project'
    ],
    
    data() {
        return {
            deployment: null,
            usedVariables: null
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
            handler: 'getDashboard'
        },
        'deploymentId': {
            immediate: true,
            handler: 'getDashboard'
        }            
    },
    
    methods: {
        getDashboard() {
            getDeploymentDetails(this.project.id, this.deploymentId)
                .then(response => this.deployment = response)
        }
    }
};

function getDeploymentDetails(projectId, deploymentId) {
    return httpService.post('api/project/deployment/get-deployment-details', { projectId, deploymentId });
}
</script>