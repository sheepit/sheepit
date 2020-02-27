<template>
    <div>
        <div class="view__title">
            Projects
        </div>

        <div class="view__row view__row--right">
            <router-link
                class="button button--primary"
                :to="{ name: 'create-project' }"
            >
                Create project
            </router-link>
        </div>

        <div v-if="loading">
            <preloader />
        </div>

        <div v-else>
            <expanding-list
                    v-if="projects && projects.length > 0"
                    class="mt-4"
                    :all-items="projects"
                    initial-length="10000"
            >
                <template slot-scope="{ items }">
                    <table>
                        <thead>
                            <tr>
                                <th scope="col">
                                    Project ID
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
                        </tr>
                        </tbody>
                    </table>

                </template>
            </expanding-list>
            <div v-else>
                There are no projects yet
            </div>
        </div>
    </div>
</template>

<script>
import getProjectListService from "./_services/get-project-list-service";

export default {
    name: 'ProjectList',

    data() {
        return {
            loading: true,
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
                .then(response => {
                    this.projects = response.projects
                    this.loading = false
                })
        }
    }
}
</script>

