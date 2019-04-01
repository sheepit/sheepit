<template>
    <expanding-list
        v-if="releases && releases.length > 0"
        class="mt-4"
        :all-items="releases"
        initial-length="5"
    >
        <template slot-scope="{ items }">
            <table class="table table-bordered">
                <thead>
                    <tr>
                        <th scope="col">
                            id
                        </th>
                        <th scope="col">
                            created
                        </th>
                        <th scope="col">
                            commit sha
                        </th>
                        <th scope="col">
                            operations
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr
                        v-for="release in items"
                        :key="release.id"
                    >
                        <th scope="row">
                            <release-badge
                                :project-id="project.id"
                                :release-id="release.id"
                            />
                        </th>
                        <td>
                            <humanized-date :date="release.createdAt" />
                        </td>
                        <td>
                            <tooltip :text="release.commitSha">
                                <code>{{ shortCommitSha(release.commitSha) }}</code>
                            </tooltip>
                        </td>
                        <td>
                            <router-link
                                tag="button"
                                :to="{ name: 'deploy-release', params: { projectId: project.id, releaseId: release.id } }"
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
    <div v-else>
        No releases found for this project
    </div>
</template>

<script>
export default {
    name: "ProjectReleases",

    props: [
        'project',
        'releases'
    ],
    
    methods: {
        shortCommitSha(commitSha) {
            return commitSha.substring(0, 7)
        }
    }
}
</script>