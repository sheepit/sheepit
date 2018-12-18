<template>
    <div>

        <div v-if="release">
            
            <h3>Creating release for project {{ project.id }} based on release {{ release.id }}</h3>
            
            <h4>Variables</h4>
            
            <variable-editor v-bind:variables="release.variables" v-bind:environments="['dev', 'test', 'prod']">
            </variable-editor>
        </div>
        
        <button class="btn btn-success mt-4" v-on:click="createRelease()">Create release</button>
    </div>
</template>

<script>
    module.exports = {
        name: 'create-release',
        
        components: {
            'variable-editor': httpVueLoader('variable-editor.vue')
        },
        
        props: ['projects'],
        
        data() {
            return {
                release: null
            }
        },

        computed: {
            project() {
                return this.projects
                    .filter(project => project.id === this.$route.params.projectId)
                    [0]
            }
        },
        
        created() {
            // todo: y tho
            if (this.project) {
                this.getRelease()
            }
        },
        
        watch: {
            project() {
                this.getRelease()
            }
        },

        methods: {
            getRelease() {
                getLatestRelease(this.project.id)
                    .then(response => this.release = response)
            },
            createRelease() {
                
                const request = {
                    projectId: this.project.id,
                    newVariables: this.release.variables
                };
                
                postData('api/edit-release-variables', request)
                    .then(() => this.$router.push({ name: 'project', params: { projectId: this.project.id }}))
            }
        }
    }
    
    function getLatestRelease(projectId) {
        return postData('api/get-last-release', { projectId })
            .then(response => response.json())
    }
</script>