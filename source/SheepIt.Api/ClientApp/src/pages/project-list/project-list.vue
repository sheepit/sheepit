<template>
    <div>
        <h2>Projects</h2>
        <div class="row">
            <div class="col text-right">
                <router-link
                    class="btn btn-primary"
                    :to="{ name: 'create-project' }"
                >
                    Create project
                </router-link>
            </div>
        </div>
        <expanding-list
            v-if="projects && projects.length > 0"
            class="mt-4"
            :all-items="projects"
            initial-length="10000"
        >
            <template slot-scope="{ items }">
                <table class="table table-bordered">
                    <thead>
                        <tr>
                            <th scope="col">
                                Project ID
                            </th>
                            <th scope="col">
                                Repository url
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr
                            v-for="item in items"
                            :key="item.id"
                        >
                            <td scope="row">
                                <router-link
                                    :to="{ name: 'project', params: { projectId: item.id }}"
                                >
                                    {{ item.id }}
                                </router-link>
                            </td>
                            <td>
                                {{ item.repositoryUrl }}
                            </td>
                        </tr>
                    </tbody>
                </table>
            </template>
        </expanding-list>
        <div v-else-if="projects && projects.length > 0">
            There are no projects yet
        </div>
        <preloader v-else />
    </div>
</template>

<script>
import getProjectListService from "./_services/get-project-list-service";

export default {
    name: 'ProjectList',

    data() {
        return {
            projects: []
        }
    },

    created() {
        this.getProjectsList();
    },

    methods: {
        getProjectsList() {
            getProjectListService
                .getProjectList()
                .then(response => this.projects = response.projects)
        }
    }
}
</script>

