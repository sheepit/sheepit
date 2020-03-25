<template>
    <div v-if="deployment">
        <div class="view__title">
            Deployment details
        </div>

        <table>
            <thead>
                <tr>
                    <th>id</th>
                    <th>status</th>
                    <th>package id</th>
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
                        <package-badge
                            :project-id="project.id"
                            :package-id="deployment.packageId"
                            :description="deployment.packageDescription"
                        />
                    </td>
                    <td>
                        {{ deployment.environmentDisplayName }}
                    </td>
                    <td>
                        <humanized-date :date="deployment.deployedAt" />
                    </td>
                </tr>
            </tbody>
        </table>
        
        
        <div class="code__container">
            <h3>Output</h3>
            <div
                v-for="stepResult in deployment.stepResults"
                class="code__steps"
            >
                <pre :class="stepResult.successful ? '' : 'code__line--danger'"
                ><b><code class="code__line">{{ stepResult.command }}</code></b></pre>
                <pre :class="stepResult.successful ? '' : 'code__line--danger'"
                ><code class="code__line">{{ stepResult.output.join("\n") }}</code></pre>
            </div>
        </div>
        
        <deployment-used-variables :used-variables="deployment.variables" />
    </div>
</template>

<script>
import httpService from "./../../../common/http/http-service"

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

<style lang="scss" scoped>
.code {

    &__container {
        margin: 30px 0;
    }

    &__steps {
        border-radius: 0.25rem;
        padding: 15px;
        background: $font-form-color;
        color: white;
        font-family: monospace;
    }

    &__line {
        font-family: monospace;

        &--danger {
            color: $error;
        }
    }
}
</style>