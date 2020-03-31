<template>
    <div>
        <div class="view__title">
            Environments
        </div>

        <div class="view__row view__row--right">
            <router-link
                class="button button--primary"
                :to="{ name: 'edit-environments' }"
            >
                Edit environments
            </router-link>
        </div>

        <expanding-list
            v-if="environments && environments.length > 0"
            class="mt-4"
            :all-items="environments"
            initial-length="5"
        >
            <template slot-scope="{ items }">
                <table>
                    <thead>
                        <tr>
                            <th scope="col">
                                name
                            </th>
                            <th scope="col">
                                package id
                            </th>
                            <th scope="col">
                                deployment id
                            </th>
                            <th scope="col">
                                deployed
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr
                            v-for="environment in items"
                            :key="environment.environmentId">
                            <td>
                                {{ environment.displayName }}
                            </td>
                            <td>
                                <span v-if="environment.deployment">
                                    <package-badge
                                        :project-id="project.id"
                                        :package-id="environment.deployment.currentPackageId"
                                        :description="environment.deployment.currentPackageDescription"
                                    />
                                </span>
                                <span v-else>-</span>
                            </td>
                            <td>
                                <span v-if="environment.deployment">
                                    <deployment-badge
                                        :project-id="project.id"
                                        :deployment-id="environment.deployment.currentDeploymentId"
                                    />
                                </span>
                                <span v-else>-</span>
                            </td>
                            <td>
                                <span v-if="environment.deployment">
                                    <humanized-date :date="environment.deployment.lastDeployedAt" />
                                </span>
                                <span v-else>-</span>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </template>
        </expanding-list>

        <div
            v-else-if="environments && environments.length === 0"
            class="row"
        >
            <div>
                No environments found for this project
            </div>
        </div>

        <div
            v-else
            class="row"
        >
            <preloader />
        </div>
    </div>
</template>

<script>
import environmentsService from "./../_services/environments-service";

export default {
    name: 'EnvironmentsList',

    props: [
        'project'
    ],
    
    data() {
        return {
            environments: null
        }
    },
     
    watch: {
        'project': 'getEnvironmentsList'
    },

    created() {
        this.getEnvironmentsList();
    },

    methods: {
        getEnvironmentsList() {
            environmentsService
                .getEnvironmentsList(this.project.id)
                .then(response => {
                    this.environments = response.environments
                });
        }
    }
}
</script>
