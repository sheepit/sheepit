<template>
    <div>
        <p>releases for {{project.id}}</p>

        <table class="table table-bordered">
            <thead>
                <tr>
                    <th scope="col">id</th>
                    <th scope="col">created at</th>
                    <th scope="col">commit sha</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="release in releases">
                    <th scope="row">
                        <span class="badge badge-primary">{{ release.id }}</span>                        
                    </th>
                    <td>{{ release.createdAt }}</td>
                    <td><code>{{ release.commitSha }}</code></td>
                </tr>
            </tbody>
        </table>

    </div>
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
            'project': 'updateReleases'
        },

        created() {
            this.updateReleases()
        },
        
        methods: {
            updateReleases() {
                getReleases(this.project.id)
                    .then(response => this.releases = response.releases)
            }
        }
    }
    
    function getReleases(projectId) {
        return postData('api/list-releases', { projectId })
            .then(response => response.json())
    }
</script>