<template>

    <div class="row">
        <div v-for="environment in environments" class="col-md-3" v-if="environments && environments.length > 0">
            <div class="card margin-bottom">
                <div class="card-header">
                    {{ environment.displayName }}
                </div>
                <ul class="list-group list-group-flush" v-if="environment.deployment">
                    <li class="list-group-item lead">
                        <div>
                            <release-badge v-bind:project-id="project.id" v-bind:release-id="environment.deployment.currentReleaseId"></release-badge>
                        </div>
                        <div>
                            <deployment-badge v-bind:project-id="project.id" v-bind:deployment-id="environment.deployment.currentDeploymentId"></deployment-badge>
                        </div>
                    </li>
                    <li class="list-group-item">
                        Deployed: <br/>
                        <humanized-date v-bind:date="environment.deployment.lastDeployedAt"></humanized-date>
                    </li>
                </ul>
                <ul class="list-group list-group-flush" v-else>
                    <li class="list-group-item lead"></li>
                </ul>
            </div>
        </div>
        <div v-else>No environments found for this project</div>
    </div>

</template>

<script>
export default {
    name: "project-dashboard",

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