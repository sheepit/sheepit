<template>
    <div v-if="deployment">
        <table>
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
                        <deployment-badge
                            :project-id="project.id"
                            :deployment-id="deployment.id"
                        />
                    </td>
                    <td>
                        <deployment-status-badge :status="deployment.status" />
                    </td>
                    <td>
                        <release-badge
                            :project-id="project.id"
                            :release-id="deployment.releaseId"
                        />
                    </td>
                    <td>
                        <span class="badge badge-warning">{{ deployment.environmentDisplayName }}</span>
                    </td>
                    <td>
                        <humanized-date :date="deployment.deployedAt" />
                    </td>
                </tr>
            </tbody>
        </table>
        
        <div>
            <div
                v-for="stepResult in deployment.stepResults"
                class="mt-4"
            >
                <pre
                    class="mb-0"
                    :class="stepResult.successful ? '' : 'text-danger'"
                ><b><code>{{ stepResult.command }}</code></b></pre>
                <pre :class="stepResult.successful ? '' : 'text-danger'"><code>{{ stepResult.output.join("\n") }}</code></pre>
            </div>
        </div>
        
        <deployment-used-variables :used-variables="deployment.variables" />
    </div>
</template>

<script>
import httpService from "./../../common/http/http-service.js"

import DeploymentUsedVariables from "./_components/deployment-used-variables"

export default {
    name: 'DeploymentDetails',

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