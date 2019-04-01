<template>
    <router-link :to="{ name: 'release-details', params: { projectId: this.projectId, releaseId: this.releaseId } }">
        <span class="badge badge-primary">release {{ releaseId }}</span>
    </router-link>
</template>

<script>
export default {
    name: 'ReleaseBadge',
    props: ['projectId', 'releaseId']
}
</script>