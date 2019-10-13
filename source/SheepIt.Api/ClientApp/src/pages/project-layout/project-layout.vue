<template>
    <div>
        <div v-if="project">
            <router-view :project="project" />
        </div>
    </div>
</template>

<script>
import httpService from "../../common/http/http-service.js"

export default {
    name: 'Proj',

    data() {
        return {
            projects: []
        }
    },

    computed: {
        project() {
            return this.projects
                .filter(project => project.id === this.$route.params.projectId)[0]
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