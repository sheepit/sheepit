<template>
    <div v-if="project">
        <div class="view__title">
            {{ project.id }}
        </div>

        <div v-if="loading">
            <preloader />
        </div>

        <div v-else>
            <table>
                <thead>
                    <tr>
                        <th></th>
                        <th
                            v-for="environment in environments"
                            :key="environment.environmentId"
                        >
                            {{ environment.displayName }}
                        </th>
                    </tr>
                </thead>
                <tbody>
                <tr
                    v-for="component in components"
                    :key="component.componentId"
                >
                    <td>
                        {{ component.displayName }}
                    </td>
                    <td
                        v-for="environment in environments"
                        :key="environment.environmentId"
                    >
                        <div v-if="found = getDeployment(environment.environmentId, component.componentId)">
                            <package-badge
                                :project-id="project.id"
                                :package-id="found.packageId"
                                :description="found.packageDescription"
                            />
                            <br />
                            <deployment-badge
                                :project-id="project.id"
                                :deployment-id="found.deploymentId"
                            />
                            <br />
                            <humanized-date :date="found.startedAt" />
                        </div>
                        <div v-else>
                            -
                        </div>
                    </td>
                </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
import projectService from "./../_services/project-service";

export default {
    name: 'ProjectDetails',
    
    props: [
        'project'
    ],

    data() {
        return {
            loading: true,
            environments: [],
            components: [],
            deployments: []
        }
    },

    created() {
        this.getProjectsDetails();
    },

    methods: {
        getProjectsDetails() {
            projectService
                .getDashboard(this.project.id)
                .then(response => {
                    this.environments = response.environments
                    this.components = response.components
                    this.deployments = response.deployments
                    this.loading = false
                })
        },

        getDeployment(environmentId, componentId) {
            const found = this.deployments.find(element =>
                element.environmentId === environmentId
                && element.componentId === componentId);

            if(found)
                return found;
            else
                return null;
        }
    }
};
</script>

<style lang="scss">
.project-title {
    text-align: left;
    padding-bottom: 3rem;
}

.link-button {
    margin-right: 0.5rem;
}

.deployments-section {
    margin-bottom: 1rem;
}
</style>