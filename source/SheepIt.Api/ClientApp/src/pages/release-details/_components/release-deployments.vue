<template>
    <div>
        <h3 class="mt-5">
            Deployments
        </h3>
        <expanding-list
            class="mt-4"
            :all-items="deployments"
            initial-length="5"
        >
            <template slot-scope="{ items }">
                <table class="table table-bordered">
                    <thead>
                        <tr>
                            <th scope="col">
                                id
                            </th>
                            <th scope="col">
                                status
                            </th>
                            <th scope="col">
                                environment
                            </th>
                            <th scope="col">
                                deployed
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr
                            v-for="deployment in items"
                            :key="deployment.id"
                        >
                            <th scope="row">
                                <deployment-badge
                                    :project-id="project.id"
                                    :deployment-id="deployment.id"
                                />
                            </th>
                            <td>
                                <deployment-status-badge :status="deployment.status" />
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
            </template>
        </expanding-list>
    </div>
</template>

<script>
import httpService from "./../../../common/http/http-service.js";

export default {
    name: "ReleaseDeployments",

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
            handler: 'updateDeployments'
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
    return httpService.post('api/project/release/list-deployments', { projectId, releaseId });
}
</script>