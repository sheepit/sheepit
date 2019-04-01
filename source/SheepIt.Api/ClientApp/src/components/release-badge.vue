<template>
    <router-link :to="{ name: 'release-details', params: { projectId: projectId, releaseId: releaseId } }">
        <span class="badge badge-primary">release {{ releaseId }}</span>
    </router-link>
</template>

<script>
export default {
    name: 'ReleaseBadge',
    props: ['projectId', 'releaseId']
}
</script>