<template>
    <div>

        <project-breadcrumbs v-bind:project-id="project.id">
            <li class="breadcrumb-item">
                edit variables
            </li>
        </project-breadcrumbs>
        
        <div v-if="release">
            <h4 class="mt-5">
                Editing variables based on
                <release-badge v-bind:project-id="project.id" v-bind:release-id="release.id"></release-badge>
            </h4>
            
            <variable-editor v-bind:variables="release.variables" v-bind:environments="environments">
            </variable-editor>
        </div>
        
        <button class="btn btn-success mt-4" v-on:click="createRelease()">Create release</button>
    </div>
</template>

<script>
import httpService from "./../../common/http/http-service.js";

import VariableEditor from "./_components/variable-editor.vue";

export default {
    name: 'create-release',
    
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
                newVariables: this.release.variables
            };
            
            httpService
                .post('api/project/release/edit-release-variables', request)
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