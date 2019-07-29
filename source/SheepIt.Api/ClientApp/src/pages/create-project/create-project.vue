<template>
    <div>
        <h1>Add new project</h1>

        <div class="form">

            <div class="form-section">
                <div class="form-title">Details</div>
                <div class="form-group">
                    <label for="projectId" class="form-label">Project id</label>
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
            </div>

            <div class="form-section">
                <h2>Environments</h2>
                <div class="form-group">
                    <label>List of environments</label>
                    
                    <div v-for="(environment, environmentIndex) in environments"
                        :key="environmentIndex"
                        class="input-group mb-3">
                        <input
                            v-model="environments[environmentIndex]"
                            type="text"
                            class="form-control"
                        >
                        <div class="input-group-append">
                            <button 
                                class="btn btn-outline-secondary"
                                type="button"
                                @click="removeEnvironment(environmentIndex)"
                            >
                                <span class="icon icon-trash" />        
                            </button>
                        </div>
                    </div>
                </div>
                <div class="button-container">
                    <button
                        class="btn btn-secondary"
                        @click="newEnvironment()"
                    >
                        Add new
                    </button>
                </div>
            </div>

            <div class="submit-button-container">
                <button
                    type="button"
                    class="btn btn-primary"
                    @click="create()"
                >
                    Save
                </button>
            </div>
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
        },

        removeEnvironment: function(index) {
            if(this.environments.length === 1)
                return;

            this.environments.splice(index, 1);
        }
    }
}
</script>

<style lang="scss" scoped>
.form {
    &-section {
        padding: 15px;
        overflow: hidden;
        background: $white;
        border: 1px solid $light-gray;
        margin: 20px 0;
        border-radius: 0.25rem;
    }

    &-title {
        font-size: 2rem;
        margin-bottom: 1rem;
    }

    label {
        margin: 0;
        font-weight: 500;
    }

    .button-container {
        text-align: right !important;
        margin-top: 16px;
    }

    .submit-button-container {
        text-align: right !important;
        margin: 16px 0;
    }
}
</style>
