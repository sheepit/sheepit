<template>
    <div v-if="project">
        <div class="view__title">
            {{ project.id }}
        </div>

    </div>
</template>

<script>
import ProjectDashboard from "./../_components/project-dashboard.vue";

export default {
    name: 'ProjectDetails',
    
    props: [
        'project'
    ]
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