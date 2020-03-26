<template>
    <div>
        <div class="view__title">
            Deployments
        </div>

        <expanding-list
        v-if="deployments && deployments.length > 0"
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
                                component
                            </th>
                            <th scope="col">
                                package id
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
                                {{deployment.componentName}}
                            </td>
                            <td>
                                <package-badge
                                    :project-id="project.id"
                                    :package-id="deployment.packageId"
                                    :description="deployment.packageDescription"
                                />
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
        <div v-else-if="deployments && deployments.length === 0">
            No deployments found for this project
        </div>
        <preloader v-else />
    </div>
</template>

<script>
import deploymentsService from "./../_services/deployments.service";

export default {
    name: 'DeploymentsList',

    props: [
        'project'
    ],

    data() {
        return {
            deployments: null
        }
    },

    created() {
        this.getDeploymentsList();
    },

    methods: {
        getDeploymentsList() {
            deploymentsService
                .getDeploymentsList(this.project.id)
                .then(response => {
                    this.deployments = response.deployments
                });
        }
    }
}
</script>
