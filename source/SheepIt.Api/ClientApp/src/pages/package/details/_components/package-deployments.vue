<template>
    <div class="details__section">
        <div class="details__title">
            Deployments
        </div>

        <div v-if="deployments && deployments.length > 0">
            <expanding-list
                class="mt-4"
                :all-items="deployments"
                initial-length="5"
            >
                <template slot-scope="{ items }">
                    <table>
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
        <div v-else>
            No deployments found for this package
        </div>
    </div>
</template>

<script>
import httpService from "./../../../../common/http/http-service.js";

export default {
    name: "PackageDeployments",

    props: [
        'project',
        'package'
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
        package: {
            immediate: true,
            handler: 'updateDeployments'
        }
    },

    methods: {
        updateDeployments() {
            getDeployments(this.project.id, this.package.id)
                .then(response => this.deployments = response.deployments.reverse())
        }
    }
};

function getDeployments(projectId, packageId) {
    return httpService.post('api/project/package/list-deployments', { projectId, packageId });
}
</script>
