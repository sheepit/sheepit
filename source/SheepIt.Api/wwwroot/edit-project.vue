<template>
    <div>
        <h4>Edit project</h4>
        <div>
            <div class="form-group">
                <label for="projectId">Project id</label>
                <span></span>
            </div>

            <button type="button" v-on:click="edit()" class="btn btn-primary">Save</button>
        </div>
    </div>
</template>

<script>
    module.exports = {
        name: 'edit-project',

        props: ['projectId', 'project'],

        data() {
            return {
                project: null
            }
        },

        mounted() {
            this.getProjectDetails();
        },

        methods: {
            getProjectDetails: function() {
                getProjectDetailss(this.project.id)
                    .then(response => console.log(response));
            },

            edit: function () {
                editProject(this.projectId, this.repositoryUrl, this.environments)
                    .then(() => window.app.updateProjects())
                    .then(() => this.$router.push({ name: 'project', params: { projectId: this.projectId }}))
            },

            newEnvironment: function () {
                this.environments.push('');
            }
        }
    }

    function getProjectDetailss(projectId) {
        return postData('api/get-project-details', { id: projectId })
            .then(response => response.json());
    }
 

    function editProject(projectId, repositoryUrl, environmentNames) {
        return postData('api/edit-project', {
            projectId: projectId,
            repositoryUrl: repositoryUrl,
            environmentNames: environmentNames
        })
    }
    
</script>