<template>
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark mt-4">
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
                        <router-link v-for="project in projects" :key="project.id" class="dropdown-item" :to="{ name: 'project', params: { projectId: project.id }}">
                            {{ project.id }}
                        </router-link>
                        <div class="dropdown-divider"></div>
                        <router-link class="dropdown-item" :to="{ name: 'create-project' }">
                            Create new project
                        </router-link>
                    </div>
                </li>
            </ul>
            <span class="my-2 my-lg-0">
                <button class="btn btn-outline-light my-2 my-sm-0" type="submit">Log Out</button>
            </span>
        </div>
    </nav>
</template>

<script>
export default {
    name: "navigation",
    props: ['projects'],
}
</script>