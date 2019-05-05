<template>
    <div class="container">
        <navigation :projects="projects" />

        <main class="mt-5">
            <router-view :projects="projects" />
        </main>
    </div>
</template>

<script>

    import Navigation from '../../components/layout/navigation.vue'
    import httpService from "../../common/http/http-service.js"

    export default {
        name: "DefaultLayout",

        components: {
            Navigation
        },

        data() {
            return {
                projects: []
            }
        },

        created() {
            this.updateProjects()
        },

        methods: {
            updateProjects() {
                return httpService
                    .get('api/list-projects', null)
                    .then(response => this.projects = response.projects)
            }
        }
    }
    
</script>

<style scoped>
</style>