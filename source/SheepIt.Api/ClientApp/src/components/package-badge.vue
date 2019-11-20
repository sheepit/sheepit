<template>
    <router-link 
        :to="{
            name: 'package-details',
            params: { projectId: projectId, packageId: packageId }
        }"
    >
        <span 
            v-if="packageId && description"
            class="badge badge-primary"
        >
            package {{ packageId }} ({{ description }})
        </span>
        <span 
            v-else 
            class="badge badge-primary"
        >
            package {{ packageId }}
        </span>
    </router-link>
</template>

<script>
export default {
    name: 'PackageBadge',
    props: ['projectId', 'packageId', 'description']
}
</script>