<template>
    <div>
        <h2>Dashboard</h2>

        <h3>Last deployments</h3>
        <expanding-list
            v-if="deployments && deployments.length > 0"
            class="mt-4"
            :all-items="deployments"
            initial-length="10000">
            <template slot-scope="{ items }">
                <table class="table table-bordered">
                    <thead>
                        <tr>
                            <th scope="col">
                                Deployment ID
                            </th>
                            <th scope="col">
                                Project
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="item in items"
                            :key="item.id">
                            
                            <td>
                                {{ item.deploymentId }}
                            </td>
                            <td>
                                {{ item.projectId }}
                            </td>
                        </tr>
                    </tbody>
                </table>
            </template>
        </expanding-list>
        <div v-else-if="deployments && deployments.length > 0">
            There are no deployments yet
        </div>
        <div v-else>
            Loading...
        </div>
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
