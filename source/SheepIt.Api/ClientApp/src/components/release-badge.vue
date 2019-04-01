<template>
    <router-link v-bind:to="{ name: 'release-details', params: { projectId: this.projectId, releaseId: this.releaseId } }">
        <span class="badge badge-primary">release {{ releaseId }}</span>
    </router-link>
</template>

<script>
export default {
    name: 'release-badge',
    props: ['projectId', 'releaseId']
}
</script>