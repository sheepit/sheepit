<template>
    <div v-if="project">
        <router-view :project="project" />
    </div>
</template>

<script>
export default {
    name: 'Proj',
    
    props: ['projects'],

    computed: {
        project() {
            return this.projects
                .filter(project => project.id === this.$route.params.projectId)[0]
        }
    },
}
</script>