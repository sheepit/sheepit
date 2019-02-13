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

        props: ['projectId'],

        data() {
            return {
                project: null
            }
        },

        created() {
            this.getProjectDetails(this.projectId);
        },

        methods: {
            getProjectDetails: function() {
                getProjectDetails(this.projectId)
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

    function getProjectDetails(projectId) {
        debugger;
        return postData('api/get-project-details', { projectId })
            .then(response => response.json())
    }
 

    function editProject(projectId, repositoryUrl, environmentNames) {
        return postData('api/edit-project', {
            projectId: projectId,
            repositoryUrl: repositoryUrl,
            environmentNames: environmentNames
        })
    }
    
</script>