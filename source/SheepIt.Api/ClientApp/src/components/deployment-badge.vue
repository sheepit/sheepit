<template>
    <router-link :to="{ name: 'deployment-details', params: { projectId: this.projectId, deploymentId: this.deploymentId } }">
        <span class="badge badge-success">deployment {{ deploymentId }}</span>
    </router-link>
</template>

<script>
export default {
    name: 'DeploymentBadge',
    props: ['projectId', 'deploymentId']
}
</script>