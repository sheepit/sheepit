<template>
    <div>
        <div class="view__title">
            Packages
        </div>

        <div class="view__row view__row--right">
            <router-link
                class="button button--primary"
                :to="{ name: 'create-package', params: { projectId: project.id }}"
            >
                Create package
            </router-link>
        </div>

        <expanding-list
            v-if="packages && packages.length > 0"
            class="mt-4"
            :all-items="packages"
            initial-length="5"
        >
            <template slot-scope="{ items }">
                <table>
                    <thead>
                        <tr>
                            <th scope="col">
                                id
                            </th>
                            <th scope="col">
                                created
                            </th>
                            <th scope="col">
                                operations
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr
                            v-for="_package in items"
                            :key="_package.id"
                        >
                            <td scope="row">
                                <package-badge
                                    :project-id="project.id"
                                    :package-id="_package.id"
                                    :description="_package.description"
                                />
                            </td>
                            <td>
                                <humanized-date :date="_package.createdAt" />
                            </td>
                            <td>
                                <router-link
                                    tag="button"
                                    :to="{ name: 'deploy-package', params: {
                                        projectId: project.id,
                                        packageId: _package.id
                                    } }"
                                    class="btn btn-success"
                                >
                                    Deploy!
                                </router-link>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </template>
        </expanding-list>
        <div v-else-if="packages && packages.length === 0">
            No packages found for this project
        </div>
        <preloader v-else />
    </div>
</template>

<script>
import packagesService from "./../_services/packages.service";

export default {
    name: 'PackagesList',

    props: [
        'project'
    ],

    data() {
        return {
            packages: null
        }
    },

    created() {
        this.getPackagesList();
    },

    methods: {
        getPackagesList() {
            packagesService
                .getPackagesList(this.project.id)
                .then(response => {
                    this.packages = response.packages
                });
        }
    }
}
</script>
