<template>
    <div class="row">
        <div
            v-if="environments && environments.length > 0"
            class="col-md-3"
        >
            <div
                v-for="environment in environments"
                :key="environment.environmentId"
            >
                <div class="card margin-bottom">
                    <div class="card-header environment-card-header">
                        {{ environment.displayName }}
                    </div>
                    <ul
                        v-if="environment.deployment"
                        class="list-group list-group-flush"
                    >
                        <li class="list-group-item lead">
                            <div>
                                <release-badge
                                    :project-id="project.id"
                                    :release-id="environment.deployment.currentReleaseId"
                                />
                            </div>
                            <div>
                                <deployment-badge
                                    :project-id="project.id"
                                    :deployment-id="environment.deployment.currentDeploymentId"
                                />
                            </div>
                        </li>
                        <li class="list-group-item">
                            Deployed: <br>
                            <humanized-date :date="environment.deployment.lastDeployedAt" />
                        </li>
                    </ul>
                    <ul
                        v-else
                        class="list-group list-group-flush"
                    >
                        <li class="list-group-item">
                            Nothing deployed yet
                        </li>
                    </ul>
                </div>
            </div>
        </div>
        <div v-else-if="environments && environments.length === 0">
            No environments found for this project
        </div>
        <preloader v-else />
    </div>
</template>

<script>
export default {
    name: "ProjectDashboard",

    props: [
        'project',
        'environments'
    ],

    data() {
        return {
        }
    }
};
</script>

<style lang="scss" scoped>
.card {
    margin-bottom: 1rem;
}

.environment-card-header {
    min-height: 49px;
    height: 100%;
    width: 100%;
}
</style>