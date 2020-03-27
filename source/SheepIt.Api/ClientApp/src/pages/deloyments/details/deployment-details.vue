<template>
    <div v-if="deployment" class="details">
        <div class="view__title">
            Deployment {{ deployment.id }}
        </div>

        <div class="details__section">
            <div class="details__title">
                Details
            </div>
            <div class="details__content">
                <div class="details__item">
                    <label class="details__label">status</label>
                    <span class="details__value">
                        <deployment-status-badge :status="deployment.status" />
                    </span>
                </div>
                <div class="details__item">
                    <label class="details__label">package id</label>
                    <span class="details__value">
                        <package-badge
                            :project-id="project.id"
                            :package-id="deployment.packageId"
                            :description="deployment.packageDescription"
                        />
                    </span>
                </div>
                <div class="details__item">
                    <label class="details__label">environment</label>
                    <span class="details__value">
                        {{ deployment.environmentDisplayName }}
                    </span>
                </div>
                <div class="details__item">
                    <label class="details__label">deployed at</label>
                    <span class="details__value">
                        <humanized-date :date="deployment.deployedAt" />
                    </span>
                </div>
            </div>
        </div>
        
        
        <div class="details__section">
            <div class="details__title">
                Output
            </div>
            
            <div v-if="deployment.stepResults && deployment.stepResults.length > 0">
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
            <div v-else>
                Dupa
            </div>
        </div>
        
        <div class="details__section">
            <div class="details__title">
                Deployment variables
            </div>
            <table v-if="deployment.variables && deployment.variables.length > 0">
                <thead>
                    <tr>
                        <th>name</th>
                        <th>value</th>
                    </tr>
                </thead>
                <tbody>
                    <tr
                        v-for="(variable, index) in deployment.variables"
                        :key="index"
                    >
                        <td>
                            <span>{{ variable.name }}</span>
                        </td>
                        <td>
                            <span>{{ variable.value }}</span>
                        </td>
                    </tr>
                </tbody>
            </table>
            <div v-else-if="deployment.variables && deployment.variables.length === 0">
                <span>Variables has not been defined for this deployment</span>
            </div>
            <preloader v-else />
        </div>
    </div>
</template>

<script>
import httpService from "./../../../common/http/http-service"

export default {
    name: 'DeploymentDetails',

    props: [
        'project'
    ],
    
    data() {
        return {
            deployment: null
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

    &__steps {
        border-radius: 0.25rem;
        padding: 15px;
        background: $font-form-color;
    }

    &__line {
        font-family: monospace;
        @include font($family: monospace, $color: $white, $size: 14px);

        &--danger {
            @include font($family: monospace, $color: $error, $size: 14px);
        }
    }
}
</style>