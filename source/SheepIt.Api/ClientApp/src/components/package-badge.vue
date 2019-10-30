<template>
    <router-link :to="{ name: 'package-details', params: { projectId: projectId, packageId: packageId } }">
        <span class="badge badge-primary">package {{ packageId }}</span>
    </router-link>
</template>

<script>
export default {
    name: 'PackageBadge',
    props: ['projectId', 'packageId']
}
</script>