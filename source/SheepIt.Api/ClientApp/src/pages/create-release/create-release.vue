<template>
    <div>
  
        <div v-if="release">
            <h4 class="mt-5">
                Editing variables based on
                <release-badge
                    :project-id="project.id"
                    :release-id="release.id"
                />
            </h4>
            
            <variable-editor
                :variables="release.variables"
                :environments="environments"
            />
        </div>
        
        <button
            class="btn btn-success mt-4"
            @click="createRelease()"
        >
            Create release
        </button>
    </div>
</template>

<script>
import httpService from "./../../common/http/http-service.js";

import VariableEditor from "./_components/variable-editor.vue";

export default {
    name: 'CreateRelease',
    
    components: {
        'variable-editor': VariableEditor
    },
    
    props: ['project'],
    
    data() {
        return {
            release: null,
            environments: null
        }
    },

    watch: {
        project: {
            immediate: true,
            handler: 'getRelease'
        }
    },

    methods: {
        getRelease() {
            httpService
                .post('api/project/release/get-last-release', { projectId: this.project.id })
                .then(response => this.release = response);

            this.getProjectEnvironments();
        },
        createRelease() {
            
            const request = {
                projectId: this.project.id,
                newVariables: this.release ? this.release.variables : []
            };
            
            httpService
                .post('api/project/release/edit-release-variables', request, false)
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