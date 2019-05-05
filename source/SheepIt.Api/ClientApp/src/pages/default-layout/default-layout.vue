<template>
    <div class="container" data-event-handler @unauthorized="handleUnauthorized()">
        
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
            },
            handleUnauthorized() {
                this.$router.push({ name: 'sign-in' })
            }
        }
    }
    
</script>

<style scoped>
</style>