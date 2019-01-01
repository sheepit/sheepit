<template>
    <div>

        <project-breadcrumbs v-bind:project-id="project.id">
            <li class="breadcrumb-item">
                edit variables
            </li>
        </project-breadcrumbs>
        
        <div v-if="release">
            <h4 class="mt-5">
                Editing variables based on
                <release-badge v-bind:project-id="project.id" v-bind:release-id="release.id"></release-badge>
            </h4>
            
            <variable-editor v-bind:variables="release.variables" v-bind:environments="environments">
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
        
        props: ['project'],
        
        data() {
            return {
                release: null,
                environments: null
            }
        },

        watch: {
            project: {
                immediate: true,
                handler: 'getRelease'
            }
        },

        methods: {
            getRelease() {
                getLatestRelease(this.project.id)
                    .then(response => this.release = response);
                
                this.getProjectEnvironments();
            },
            createRelease() {
                
                const request = {
                    projectId: this.project.id,
                    newVariables: this.release.variables
                };
                
                postData('api/edit-release-variables', request)
                    .then(() => this.$router.push({ name: 'project', params: { projectId: this.project.id }}))
            },
            getProjectEnvironments() {
                postData('api/list-environments', { projectId: this.project.id })
                    .then(response => response.json())
                    .then(response => this.environments = response.environments);
            }
        }
    };
    
    function getLatestRelease(projectId) {
        return postData('api/get-last-release', { projectId })
            .then(response => response.json())
    }
</script>