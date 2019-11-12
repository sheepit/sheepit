<template>
    <div>
        <div 
            v-if="environments && environments.length > 0"
            class="row"
        >
            <div 
                v-for="environment in environments"
                :key="environment.environmentId"
                class="card margin-bottom col-md-3"
            >
                <div class="card-header environment-card-header">
                    {{ environment.displayName }}
                </div>
                <ul
                    v-if="environment.deployment"
                    class="list-group list-group-flush"
                >
                    <li class="list-group-item lead">
                        <div>
                            <package-badge
                                :project-id="project.id"
                                :package-id="environment.deployment.currentPackageId"
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

        <div
            v-else-if="environments && environments.length === 0"
            class="row"
        >
            <div>
                No environments found for this project
            </div>
        </div>

        <div
            v-else
            class="row"
        >
            <preloader />
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

<style lang="scss" scoped>
.card {
    margin-bottom: 1rem;
    margin-right: 1rem;
    padding-left: 0;
    padding-right: 0;
}

.environment-card-header {
    min-height: 49px;
    width: 100%;
}
</style>