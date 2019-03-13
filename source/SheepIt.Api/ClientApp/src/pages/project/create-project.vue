<template>
    <div>
        <h4>Create new project</h4>
        <div>
            <div class="form-group">
                <label for="projectId">Project id</label>
                <input type="text" v-model="projectId" class="form-control" id="projectId">
            </div>
            <div class="form-group">
                <label for="sourceUrl">Git repository URL</label>
                <input type="text" v-model="repositoryUrl" class="form-control" id="sourceUrl">
            </div>

            <div class="form-group">
                <label>Environments (names):</label>
                
                <input type="text" v-model="environments[environmentIndex]" class="form-control"
                       v-for="(environment, environmentIndex) in environments">
                
                <button class="btn btn-secondary" v-on:click="newEnvironment()">Add new</button>
            </div>

            <button type="button" v-on:click="create()" class="btn btn-primary">Create</button>
        </div>
    </div>
</template>

<script>
export default {
    name: "create-project",
    data() {
        return {
            projectId: "",
            repositoryUrl: "",
            environments: ['']
        }
    },
    methods: {
        create: function () {
            createProject(this.projectId, this.repositoryUrl, this.environments)
                .then(() => window.app.updateProjects())
                .then(() => this.$router.push({ name: 'project', params: { projectId: this.projectId }}))
        },

        newEnvironment: function () {
            this.environments.push('');
        }
    }
}

function createProject(projectId, repositoryUrl, environmentNames) {
    return postData('api/create-project', {
        projectId: projectId,
        repositoryUrl: repositoryUrl,
        environmentNames: environmentNames
    })
}    
</script>