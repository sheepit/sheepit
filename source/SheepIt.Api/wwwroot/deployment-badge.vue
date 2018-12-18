<template>
    <router-link v-bind:to="{ name: 'deployment-details', params: { projectId: this.projectId, deploymentId: this.deploymentId } }">
        <span class="badge badge-success">deployment {{ deploymentId }}</span>
    </router-link>
</template>

<script>
    module.exports = {
        name: 'deployment-badge',
        props: ['projectId', 'deploymentId']
    }
</script>