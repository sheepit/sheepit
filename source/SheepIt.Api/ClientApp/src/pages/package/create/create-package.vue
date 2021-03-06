<template>
    <div>
        <div class="view__title">
            Create package
        </div>
            
        <div class="form">
            <div class="form__section">
                <div class="form__title">
                    Details
                </div>

                <div class="form__row">
                    <div class="form__column">
                        <label
                            for="description"
                            class="form__label"
                        >
                            Description
                        </label>
                        <input
                            id="description"
                            v-model="description"
                            type="text"
                            class="form__control"
                            :class="{ 'is-invalid': submitted && $v.description.$error }"
                        >
                        <div
                            v-if="submitted && $v.description.$error"
                            class="invalid-feedback"
                        >
                            <span v-if="!$v.description.required">Field is required</span>
                            <span v-if="!$v.description.minLength">Field should have at least 1 character</span>
                        </div>
                    </div>

                    <div class="form__column">
                        <label for="zipFile" class="form__label">Process definition</label>
                        <input
                            id="zipFile" 
                            ref="zipFile"
                            type="file"
                            class="form__control-file"
                        >
                    </div>

                    <div class="form__column"></div>
                </div>
            </div>
        </div>
 
        <div class="form">
            <div class="form__section">
                <div class="form__title">
                    Variables
                </div>

                <div v-if="packagee">
                    <h4 class="mt-5">
                        Editing variables based on
                        <package-badge
                            :project-id="project.id"
                            :package-id="packagee.id"
                            :description="packagee.description"
                        />
                    </h4>
                    
                    <variable-editor
                        :variables="packagee.variables"
                        :environments="environments"
                    />
                </div>
                <div v-else>
                    <h4 class="mt-5">
                        Editing variables based on
                    </h4>

                    <preloader />
                </div>
            </div>

            <div class="submit-button-container">
                <router-link
                    class="button button--secondary"
                    :to="{ name: 'components-list' }"
                    tag="button"
                    type="button"
                >
                    Cancel
                </router-link>

                <button
                    type="button"
                    class="button button--primary"
                    @click="createPackage()"
                >
                    Save
                </button>
            </div>
        </div>
    </div>
</template>

<script>
import { required, minLength } from "vuelidate/lib/validators";

import httpService from "./../../../common/http/http-service";
import messageService from "./../../../common/message/message-service.js";

import createPackageService from "./_services/create-package-service.js";

import VariableEditor from "./_components/variable-editor.vue";

export default {
    name: 'CreatePackage',
    
    components: {
        'variable-editor': VariableEditor
    },
    
    props: ['project'],
    
    data() {
        return {
            packagee: null,
            environments: null,
            description: null,
            submitted: false
        }
    },

    watch: {
        project: {
            immediate: true,
            handler: 'getPackage'
        }
    },

    methods: {
        createPackage() {
            this.submitted = true;

            this.$v.$touch();
            if (this.$v.$invalid) {
                return;
            }

            const zipFileData = this.$refs.zipFile.files[0];
            const newVariables = this.packagee
                ? this.packagee.variables
                : [];

            createPackageService
                .createPackage(
                    this.project.id,
                    this.$route.query.componentId,
                    this.environments,
                    this.description,
                    zipFileData,
                    newVariables                    
                )
                .then(response => {
                    messageService.success('Package created');
                    this.$router.push({ name: 'packages-list', params: { projectId: this.project.id }});
                });
        },

        getPackage() {
            httpService
                .post('frontendApi/project/package/get-last-package', { 
                    projectId: this.project.id,
                    componentId: this.$route.query.componentId
                })
                .then(response => this.packagee = response);

            this.getProjectEnvironments();
        },

        getProjectEnvironments() {
            httpService
                .post('frontendApi/project/environment/list-environments', { projectId: this.project.id })
                .then(response => this.environments = response.environments);
        }
    },

    validations: {
        description: {
            required,
            minLength: minLength(1)
        }
    }
};
</script>