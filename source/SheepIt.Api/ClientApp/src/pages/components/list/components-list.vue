<template>
    <div>
        <div class="view__title">
            Components
        </div>

        <div class="view__row view__row--right">
            <router-link
                class="button button--primary"
                :to="{ name: 'edit-components', params: { projectId: project.id }}"
            >
                Edit components
            </router-link>
        </div>

        <expanding-list
            v-if="components && components.length > 0"
            class="mt-4"
            :all-items="components"
            initial-length="5"
        >
            <template slot-scope="{ items }">
                <table>
                    <thead>
                        <tr>
                            <th scope="col">
                                name
                            </th>
                            <th scope="col">
                                operations
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr
                            v-for="component in items"
                            :key="component.id"
                        >
                            <td>
                                {{ component.name }}
                            </td>
                            <td>
                                <router-link
                                    tag="button"
                                    :to="{
                                        name: 'create-package',
                                        params: {
                                            projectId: project.id
                                        },
                                        query: {
                                            componentId: component.id
                                        }
                                    }"
                                    class="button button--inline button--secondary"
                                >
                                    add package
                                </router-link>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </template>
        </expanding-list>
        <div v-else-if="components && components.length === 0">
            No components found for this project
        </div>
        <preloader v-else />
    </div>
</template>

<script>
import componentsService from "./../_services/components.service";

export default {
    name: 'ComponentsList',

    props: [
        'project'
    ],

    data() {
        return {
            components: null
        }
    },

    created() {
        this.getComponentsList();
    },

    methods: {
        getComponentsList() {
            componentsService
                .getComponentsList(this.project.id)
                .then(response => {
                    this.components = response.components
                });
        }
    }
}
</script>
