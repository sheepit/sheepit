<template>
    <expanding-list class="mt-4" v-bind:all-items="deployments" initial-length="5">
        <template slot-scope="{ items }">
            <table class="table table-bordered">
                <thead>
                    <tr>
                        <th scope="col">id</th>
                        <th scope="col">release id</th>
                        <th scope="col">environment id</th>
                        <th scope="col">deployed at</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="deployment in items">
                        <th scope="row">
                            <span class="badge badge-secondary">{{ deployment.id }}</span>
                        </th>
                        <td>
                            <span class="badge badge-success">{{ deployment.releaseId }}</span>
                        </td>
                        <td>
                            <span class="badge badge-warning">{{ deployment.environmentId }}</span>
                        </td>
                        <td>
                            <humanized-date v-bind:date="deployment.deployedAt"></humanized-date>                            
                        </td>
                    </tr>
                </tbody>
            </table>
        </template>
    </expanding-list>
</template>

<script>
    module.exports = {
        name: "project-deployments",

        props: [
            'project'
        ],
        
        data() {
            return {
                deployments: []
            }
        },
        
        watch: {
            project: {
                immediate: true,
                handler: 'updateDeployments'
            }
        },

        methods: {
            updateDeployments() {
                getDeployments(this.project.id)
                    .then(response => this.deployments = response.deployments.reverse())
            }
        }
    }
    
    function getDeployments(projectId) {
        return postData('api/list-deployments', { projectId })
            .then(response => response.json())
    }
</script>