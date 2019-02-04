<template>
    <div>
        <h3 class="mt-5">Deployments</h3>
        <expanding-list class="mt-4" v-bind:all-items="deployments" initial-length="5">
            <template slot-scope="{ items }">
                <table class="table table-bordered">
                    <thead>
                    <tr>
                        <th scope="col">id</th>
                        <th scope="col">status</th>
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
    </div>
</template>

<script>
    module.exports = {
        name: "release-deployments",

        props: [
            'project',
            'release'
        ],

        data() {
            return {
                deployments: []
            }
        },

        watch: {
            project: {
                immediate: true,
                handler: 'updateDeployments'
            },
            release: {
                immediate: true,
                handler: 'updateDeployments'``
            }
        },

        methods: {
            updateDeployments() {
                getDeployments(this.project.id, this.release.id)
                    .then(response => this.deployments = response.deployments.reverse())
            }
        }
    };

    function getDeployments(projectId, releaseId) {
        return postData('api/project/release/list-deployments', { projectId, releaseId })
            .then(response => response.json())
    }
</script>