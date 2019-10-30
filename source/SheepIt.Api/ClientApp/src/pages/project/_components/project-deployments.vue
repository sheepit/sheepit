<template>
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
                            <package-badge
                                :project-id="project.id"
                                :package-id="deployment.packageId"
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
</template>

<script>
export default {
    name: "ProjectDeployments",

    props: [
        'project',
        'deployments'
    ]
};
</script>