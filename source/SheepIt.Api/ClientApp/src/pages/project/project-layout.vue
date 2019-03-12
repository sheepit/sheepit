<template>
    <div v-if="project">
        <router-view v-bind:project="project"></router-view>
    </div>
</template>

<script>
    export default {
        name: 'proj',
        
        props: ['projects'],

        computed: {
            project() {
                return this.projects
                    .filter(project => project.id === this.$route.params.projectId)[0]
            }
        },
    }
</script>