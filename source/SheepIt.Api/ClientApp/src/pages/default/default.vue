<template>
    <div>
        <div class="view__title">
            Dashboard
        </div>
        <h3>Last deployments</h3>
        <expanding-list
            v-if="deployments && deployments.length > 0"
            class="mt-4"
            :all-items="deployments"
            initial-length="10000"
        >
            <template slot-scope="{ items }">
                <table>
                    <thead>
                        <tr>
                            <th scope="col">
                                id
                            </th>
                            <th scope="col">
                                project
                            </th>
                            <th scope="col">
                                environment
                            </th>
                            <th scope="col">
                                deployed
                            </th>
                            <th scope="col">
                                status
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr
                            v-for="item in items"
                            :key="item.id"
                        >
                            <td>
                                <deployment-badge
                                    :project-id="item.projectId"
                                    :deployment-id="item.deploymentId"
                                />
                            </td>
                            <td>
                                <router-link
                                    :to="{ name: 'project', params: { projectId: item.projectId }}"
                                >
                                    {{ item.projectId }}
                                </router-link>
                            </td>
                            <td>
                                <span class="badge badge-warning">{{ item.environmentDisplayName }}</span>
                            </td>
                            <td>
                                <humanized-date :date="item.deployedAt" />
                            </td>
                            <td>
                                <deployment-status-badge :status="item.status" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </template>
        </expanding-list>
        <div v-else-if="deployments && deployments.length === 0">
            There are no deployments yet
        </div>
        <preloader v-else />
    </div>
</template>

<script>
import getDashboardService from "./_services/get-dashboard-service";

export default {
    name: "Default",
    
    data() {
        return {
            deployments: []
        }
    },

    created() {
        this.getLastDeploymentsList();
    },

    methods: {
        getLastDeploymentsList() {
            getDashboardService
                .getLastDeploymentsList()
                .then(response => this.deployments = response.lastDeployments)
        }
    }
}
</script>
