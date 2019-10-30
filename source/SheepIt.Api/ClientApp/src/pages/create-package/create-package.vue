<template>
    <div>
        <div v-if="package">
            <h4 class="mt-5">
                Editing variables based on
                <package-badge
                    :project-id="project.id"
                    :package-id="package.id"
                />
            </h4>
            
            <variable-editor
                :variables="package.variables"
                :environments="environments"
            />
        </div>
        
        <button
            class="btn btn-success mt-4"
            @click="createPackage()"
        >
            Create package
        </button>
    </div>
</template>

<script>
import httpService from "./../../common/http/http-service.js";

import VariableEditor from "./_components/variable-editor.vue";

export default {
    name: 'CreatePackage',
    
    components: {
        'variable-editor': VariableEditor
    },
    
    props: ['project'],
    
    data() {
        return {
            package: null,
            environments: null
        }
    },

    watch: {
        project: {
            immediate: true,
            handler: 'getPackage'
        }
    },

    methods: {
        getPackage() {
            httpService
                .post('api/project/package/get-last-package', { projectId: this.project.id })
                .then(response => this.package = response);

            this.getProjectEnvironments();
        },
        createPackage() {
            
            const request = {
                projectId: this.project.id,
                newVariables: this.package ? this.package.variables : []
            };
            
            httpService
                .post('api/project/package/edit-package-variables', request, false)
                .then(() => this.$router.push({ name: 'project', params: { projectId: this.project.id }}))
        },
        getProjectEnvironments() {
            httpService
                .post('api/project/environment/list-environments', { projectId: this.project.id })
                .then(response => this.environments = response.environments);
        }
    }
};
</script>