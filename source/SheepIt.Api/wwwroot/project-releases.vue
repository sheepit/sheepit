<template>
    <expanding-list class="mt-4" v-bind:all-items="releases" initial-length="5">
        <template slot-scope="{ items }">
            <table class="table table-bordered">
                <thead>
                <tr>
                    <th scope="col">id</th>
                    <th scope="col">created</th>
                    <th scope="col">commit sha</th>
                    <th scope="col">operations</th>
                </tr>
                </thead>
                <tbody>
                <tr v-for="release in items">
                    <th scope="row">
                        <release-badge v-bind:project-id="project.id" v-bind:release-id="release.id"></release-badge>
                    </th>
                    <td>
                        <humanized-date v-bind:date="release.createdAt"></humanized-date>
                    </td>
                    <td>
                        <tooltip v-bind:text="release.commitSha">
                            <code>{{ shortCommitSha(release.commitSha) }}</code>
                        </tooltip>
                    </td>
                    <td>
                        <router-link tag="button" v-bind:to="{ name: 'deploy-release', params: { projectId: project.id, releaseId: release.id } }" class="btn btn-success">
                            Deploy!
                        </router-link>
                    </td>
                </tr>
                </tbody>
            </table>
        </template>
    </expanding-list>
</template>

<script>
    module.exports = {
        name: "project-releases",

        props: [
            'project'
        ],
        
        data() {
            return {
                releases: []
            }
        },
        
        watch: {
            project: {
                immediate: true,
                handler: 'updateReleases'
            }
        },

        methods: {
            updateReleases() {
                getReleases(this.project.id)
                    .then(response => this.releases = response.releases.reverse())
            },
            shortCommitSha(commitSha) {
                return commitSha.substring(0, 7)
            }
        }
    }
    
    function getReleases(projectId) {
        return postData('api/project/dashboard/list-releases', { projectId })
            .then(response => response.json())
    }
</script>