<template>
    <div v-if="project">

        <h2 class="display-4">{{ project.id }}</h2>
        <p><code>{{ project.repositoryUrl }}</code></p>

        <h3 class="mt-5">Dashboard</h3>
        <project-dashboard class="mt-4" v-bind:project="project"></project-dashboard>

    </div>
</template>

<script>
    module.exports = {
        name: 'project',
        
        components: {
            'project-dashboard': httpVueLoader('project-dashboard.vue')
        },
        
        props: [
            'projects'
        ],
        
        data() {
            return {
                environments: []
            }
        },

        computed: {
            project() {
                return this.projects
                    .filter(project => project.id === this.$route.params.projectId)
                    [0]
            }
        },

        watch: {
            '$route': 'updateDashboard'
        },

        created() {
            this.updateDashboard()
        },
        
        methods: {
            updateDashboard() {
                getDashboard(this.$route.params.projectId)
                    .then(response => this.environments = response.environments)
            }
        }
    }
    
    function getDashboard(projectId) {
        return postData('api/show-dashboard', { projectId })
            .then(response => response.json())
    }
</script>
