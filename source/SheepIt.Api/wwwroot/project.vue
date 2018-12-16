<template>
    <div>

        <h3>{{ $route.params.projectId }}</h3>

        <div class="row mt-4">
            
            <div v-for="environment in environments" class="col-md-3">

                <div class="card">
                    <div class="card-header">
                        {{ environment.environmentId }}
                    </div>
                    <ul class="list-group list-group-flush">
                        <li class="list-group-item">
                            Release: <br/>
                            <h4>
                                <span class="badge badge-primary">{{ environment.currentReleaseId }}</span>
                            </h4>                            
                        </li>
                        <li class="list-group-item">
                            Deployed at: <br/>
                            <small>{{ environment.lastDeployedAt }}</small>
                        </li>
                    </ul>
                </div>

            </div>
            
        </div>

    </div>
</template>

<script>
    module.exports = {
        name: 'project',
        
        data() {
            return {
                environments: []
            }
        },

        watch: {
            '$route': 'updateDashbaord'
        },

        created() {
            this.updateDashbaord()
        },
        
        methods: {
            updateDashbaord() {
                getDashbaord(this.$route.params.projectId)
                    .then(response => this.environments = response.environments)
            }
        }
    }
    
    function getDashbaord(projectId) {
        return postData('api/show-dashboard', { projectId })
            .then(response => response.json())
    }
</script>
