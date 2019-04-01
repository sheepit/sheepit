<template>
    <div class="row">
        <div
            v-for="environment in environments"
            v-if="environments && environments.length > 0"
            :key="environment.environmentId"
            class="col-md-3"
        >
            <div class="card margin-bottom">
                <div class="card-header">
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
                    <li class="list-group-item lead" />
                </ul>
            </div>
        </div>
        <div v-else>
            No environments found for this project
        </div>
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

<style>
.card {
    margin-bottom: 1rem;
}
</style>