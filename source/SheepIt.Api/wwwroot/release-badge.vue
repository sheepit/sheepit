<template>
    <span class="badge badge-primary">release {{ releaseId }}</span>
</template>

<script>
    module.exports = {
        name: 'release-badge',
        props: ['projectId', 'releaseId']
    }
</script>