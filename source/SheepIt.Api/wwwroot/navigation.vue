<template>
    <nav class="navbar navbar-expand-lg navbar-light bg-light mt-4">
        <router-link class="navbar-brand" :to="{ name: 'default' }">Sheep It</router-link>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarSupportedContent">
            <ul class="navbar-nav mr-auto">
                <li class="nav-item dropdown">
                    <a class="nav-link dropdown-toggle" href="#" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        Projects
                    </a>
                    <div class="dropdown-menu" aria-labelledby="navbarDropdown">
                        <router-link v-for="project in projects" :key="project" class="dropdown-item" :to="{ name: 'project', params: { projectId: project }}">{{ project }}</router-link>
                        <div class="dropdown-divider"></div>
                        <router-link class="dropdown-item" :to="{ name: 'create-project' }">Create new project</router-link>
                    </div>
                </li>
            </ul>
            <span class="my-2 my-lg-0">
                    <a class="nav-link" href="#" role="button" aria-haspopup="true" aria-expanded="false">
                        Log Out
                    </a>
                </span>
        </div>
    </nav>
</template>

<script>
    module.exports = {
        name: "navigation",
        
        data() {
            return {
                projects: []
            }
        },
        
        created() {
            window.addEventListener('projectcreated', () => this.updateProjects())            
            this.updateProjects()
        },
        
        methods: {
            updateProjects() {
                loadProjects()
                    .then(response => this.projects = response.projects.map(project => project.id))
            }
        }
    }
    
    function loadProjects() {
        return fetch('api/list-projects')
            .then(response => response.json())
    }
</script>