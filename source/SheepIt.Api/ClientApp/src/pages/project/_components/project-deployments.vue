<template>
    <expanding-list class="mt-4" v-bind:all-items="deployments" initial-length="5" v-if="deployments && deployments.length > 0">
        <template slot-scope="{ items }">
            <table class="table table-bordered">
                <thead>
                    <tr>
                        <th scope="col">id</th>
                        <th scope="col">status</th>
                        <th scope="col">release id</th>
                        <th scope="col">environment</th>
                        <th scope="col">deployed</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="deployment in items">
                        <th scope="row">
                            <deployment-badge v-bind:project-id="project.id" v-bind:deployment-id="deployment.id"></deployment-badge>
                        </th>
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
        </template>
    </expanding-list>
    <div v-else>No deployments found for this project</div>
</template>

<script>
export default {
    name: "project-deployments",

    props: [
        'project',
        'deployments'
    ]
};
</script>