<template>
    <div>
        <div class="view-title">
            Create package
        </div>
            
        <div class="form">
            <div class="form-section">
                <div class="form-group">
                    <label
                        for="description"
                        class="form-label"
                    >Description</label>
                    <input
                        id="description"
                        v-model="description"
                        type="text"
                        class="form-control"
                    >
                </div>

                <div class="form-group">
                    <label for="zipFile">Process definition</label>
                    <input
                        id="zipFile" 
                        ref="zipFile"
                        type="file"
                        class="form-control-file"
                    >
                </div>
            </div>
        </div>
 
        <div class="form">
            <div v-if="packagee">
                <h4 class="mt-5">
                    Editing variables based on
                    <package-badge
                        :project-id="project.id"
                        :package-id="packagee.id"
                    />
                </h4>
                
                <variable-editor
                    :variables="packagee.variables"
                    :environments="environments"
                />
            </div>
            
            <div class="submit-button-container">
                <button
                    type="button"
                    class="btn btn-primary"
                    @click="createPackage()"
                >
                    Create package
                </button>
            </div>
        </div>
    </div>
</template>

<script>
import httpService from "./../../common/http/http-service.js";

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

            description: null
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
            let zipFileData = this.$refs.zipFile.files[0];
            let newVariables = this.packagee
                ? this.packagee.variables
                : [];

            createPackageService
                .createPackage(
                    this.project.id,
                    this.environments,
                    this.description,
                    zipFileData,
                    newVariables                    
                )
                .then(response => {
                    messageService.success('Package created');
                    this.$router.push({ name: 'project', params: { projectId: this.project.id }});
                });
        },

        getPackage() {
            httpService
                .post('api/project/package/get-last-package', { projectId: this.project.id })
                .then(response => this.packagee = response);

            this.getProjectEnvironments();
        },

        getProjectEnvironments() {
            httpService
                .post('api/project/environment/list-environments', { projectId: this.project.id })
                .then(response => this.environments = response.environments);
        }
    }
};
</script>