<template>
    <router-link :to="{ name: 'deployment-details', params: { projectId: projectId, deploymentId: deploymentId } }">
        <span class="badge badge-success">deployment {{ deploymentId }}</span>
    </router-link>
</template>

<script>
export default {
    name: 'DeploymentBadge',
    props: ['projectId', 'deploymentId']
}
</script>