<template>
    <div>
        <nav>
            <ol class="breadcrumb">
                <li class="breadcrumb-item">
                    <router-link :to="{ name: 'project-list' }">
                        projects
                    </router-link>
                    
                </li>
                <li class="breadcrumb-item">
                    create project
                </li>
                <slot />
            </ol>
        </nav>
        
        <div class="view-title">Add new project</div>

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
                        :class="{ 'is-invalid': submitted && $v.projectId.$error }"
                    >
                    <div v-if="submitted && $v.projectId.$error" class="invalid-feedback">
                        <span v-if="!$v.projectId.required">Field is required</span>
                        <span v-if="!$v.projectId.minLength">Field should have at least 3 characters</span>
                    </div>
                </div>
                <div class="form-group">
                    <label for="sourceUrl">Git repository URL</label>
                    <input
                        id="sourceUrl"
                        v-model="repositoryUrl"
                        type="text"
                        class="form-control"
                        :class="{ 'is-invalid': submitted && $v.repositoryUrl.$error }"
                    >
                    <div v-if="submitted && $v.repositoryUrl.$error" class="invalid-feedback">
                        <span v-if="!$v.repositoryUrl.required">Field is required</span>
                        <span v-if="!$v.repositoryUrl.url">URL invalid</span>
                    </div>
                </div>

            </div>

            <div class="form-section">
                <div class="form-title">Environments</div>
                <div class="form-group">
                    <label>List of environments</label>
                    
                    <div v-for="(environment, environmentIndex) in environments"
                        :key="environmentIndex"
                        class="input-group mb-3">
                        <input
                            v-model="environments[environmentIndex]"
                            type="text"
                            class="form-control"
                            :class="{ 'is-invalid': submitted && $v.environments.$each[environmentIndex].$error }"
                        >
                        <div class="input-group-append">
                            <button 
                                class="btn btn-outline-secondary"
                                type="button"
                                :disabled="environments.length < 2"
                                @click="removeEnvironment(environmentIndex)"
                            >
                                <span class="icon icon-trash" />
                            </button>
                        </div>
                        <div v-if="submitted && $v.environments.$each[environmentIndex].$error" class="invalid-feedback">
                            <span v-if="!$v.environments.$each[environmentIndex].required">Field is required</span>
                            <span v-if="!$v.environments.$each[environmentIndex].minLength">Field should have at least 3 characters</span>
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
import { required, minLength, url } from 'vuelidate/lib/validators'

import createProjectService from "./_services/create-project-service.js";

export default {
    name: "CreateProject",
    data() {
        return {
            projectId: "",
            repositoryUrl: "",
            environments: [''],

            submitted: false
        }
    },
    methods: {
        create: function () {
            this.submitted = true;

            this.$v.$touch();
            if (this.$v.$invalid) {
                return;
            }

            createProjectService.createProject(this.projectId, this.repositoryUrl, this.environments)
                // TODO: Update main app component, so it knows that new project was added
                // .then(() => window.app.updateProjects()) <===== todo
                .then(() => this.$router.push({ name: 'project', params: { projectId: this.projectId }}))
        },

        newEnvironment: function () {
            this.environments.push(null);
        },

        removeEnvironment: function(index) {
            if(this.environments.length < 2)
                return;

            this.environments.splice(index, 1);
        }
    },
    validations: {
        projectId: {
            required,
            minLength: minLength(3)
        },
        repositoryUrl: {
            required,
            url
        },
        environments: {
            required,
            minLength: minLength(1),
            $each: {
                required,
                minLength: minLength(3)
            }
        }
    }
}
</script>

<style lang="scss" scoped>
.view-title {
    margin-top: 2.5rem;
    font-size: 2.5rem;
}

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
