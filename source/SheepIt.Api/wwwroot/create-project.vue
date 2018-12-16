<template>
    <div>
        <h4>Create new project</h4>
        <form>
            <div class="form-group">
                <label for="projectId">Project id</label>
                <input type="text" v-model="projectId" class="form-control" id="projectId">
            </div>
            <div class="form-group">
                <label for="sourceUrl">Git repository URL</label>
                <input type="text" v-model="repositoryUrl" class="form-control" id="sourceUrl">
            </div>
            <button type="button" v-on:click="create()" class="btn btn-primary">Create</button>
        </form>
    </div>
</template>

<script>
    module.exports = {
        name: "create-project",
        data() {
            return {
                projectId: "",
                repositoryUrl: ""
            }
        },
        methods: {
            create: function () {
                createProject(this.projectId, this.repositoryUrl)
                    .then(() => window.dispatchEvent(new Event('projectcreated')))
            }
        }
    }
    
    function createProject(projectId, repositoryUrl) {
        return postData('api/create-project', {
            projectId: projectId,
            repositoryUrl: repositoryUrl
        })
    }
    
    function postData(url, data) {
        const fetchSettings = {
            method: "POST",
            mode: "cors",
            cache: "no-cache",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/json; charset=utf-8",
            },
            referrer: "no-referrer",
            body: JSON.stringify(data),
        }
        
        return fetch(url, fetchSettings)            
    }
</script>