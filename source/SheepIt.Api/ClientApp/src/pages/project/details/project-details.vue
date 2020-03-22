<template>
    <div v-if="project">
        <div class="view__title">
            {{ project.id }}
        </div>

        <div class="row">
            <div class="col">
                <h3>
                    Environments
                </h3>
            </div>
            <div class="col text-right">
                <router-link
                    class="btn btn-primary link-button"
                    :to="{ name: 'edit-environments' }"
                >
                    Edit environments
                </router-link>
            </div>
        </div>

        <project-dashboard
            class="mt-4"
            :project="project"
            :environments="environments"
        />

    </div>
</template>

<script>
import getDashboardService from "./../_services/get-dashboard-service.js";

import ProjectDashboard from "./../_components/project-dashboard.vue";

export default {
    name: 'ProjectDetails',
    
    components: {
        'project-dashboard': ProjectDashboard
    },
    
    props: [
        'project'
    ],
    
    data() {
        return {
            environments: null
        }
    },
     
    watch: {
        'project': 'getDashboard'
    },

    created() {
        this.getDashboard();
    },

    methods: {
        getDashboard() {
            getDashboardService
                .getDashboard(this.project.id)
                .then(response => {
                    this.environments = response.environments
                });
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