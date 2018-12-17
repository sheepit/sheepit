<template>
    <expanding-list class="mt-4" v-bind:all-items="releases" initial-length="5">
        <template slot-scope="{ items }">
            <table class="table table-bordered">
                <thead>
                <tr>
                    <th scope="col">id</th>
                    <th scope="col">created at</th>
                    <th scope="col">commit sha</th>
                </tr>
                </thead>
                <tbody>
                <tr v-for="release in items">
                    <th scope="row">
                        <span class="badge badge-primary">{{ release.id }}</span>
                    </th>
                    <td>{{ release.createdAt }}</td>
                    <td><code>{{ release.commitSha }}</code></td>
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
            }
        }
    }
    
    function getReleases(projectId) {
        return postData('api/list-releases', { projectId })
            .then(response => response.json())
    }
</script>