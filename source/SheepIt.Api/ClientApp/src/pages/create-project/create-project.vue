<template>
    <div>
        <h4>Create new project</h4>
        <div>
            <div class="form-group">
                <label for="projectId">Project id</label>
                <input
                    id="projectId"
                    v-model="projectId"
                    type="text"
                    class="form-control"
                >
            </div>
            <div class="form-group">
                <label for="sourceUrl">Git repository URL</label>
                <input
                    id="sourceUrl"
                    v-model="repositoryUrl"
                    type="text"
                    class="form-control"
                >
            </div>

            <div class="form-group">
                <label>Environments (names):</label>
                
                <input
                    v-for="(environment, environmentIndex) in environments"
                    :key="environmentIndex"
                    v-model="environments[environmentIndex]"
                    type="text"
                    class="form-control"
                >
                
                <button
                    class="btn btn-secondary"
                    @click="newEnvironment()"
                >
                    Add new
                </button>
            </div>

            <button
                type="button"
                class="btn btn-primary"
                @click="create()"
            >
                Create
            </button>
        </div>
    </div>
</template>

<script>
import createProjectService from "./_services/create-project-service.js";

export default {
    name: "CreateProject",
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