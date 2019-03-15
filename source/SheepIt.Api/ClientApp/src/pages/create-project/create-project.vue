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
                
                <input type="text"
                       v-model="environments[environmentIndex]"
                       class="form-control"
                       v-for="(environment, environmentIndex) in environments"
                       v-bind:key="environmentIndex">
                
                <button class="btn btn-secondary" v-on:click="newEnvironment()">Add new</button>
            </div>

            <button type="button" v-on:click="create()" class="btn btn-primary">Create</button>
        </div>
    </div>
</template>

<script>
import createProjectService from "./_services/create-project-service.js";

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
            createProjectService.createProject(this.projectId, this.repositoryUrl, this.environments)
                // TODO: Update main app component, so it knows that new project was added
                // .then(() => window.app.updateProjects()) <===== todo
                .then(() => this.$router.push({ name: 'project', params: { projectId: this.projectId }}))
        },

        newEnvironment: function () {
            this.environments.push('');
        }
    }
}
</script>