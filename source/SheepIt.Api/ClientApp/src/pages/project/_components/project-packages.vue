<template>
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
</template>

<script>
export default {
    name: "ProjectPackages",

    props: [
        'project',
        'packages'
    ],
    
    methods: {
    }
}
</script>