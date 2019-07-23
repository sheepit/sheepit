<template>
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark mt-4">
        <router-link
            class="navbar-brand"
            :to="{ name: 'default' }">
            Sheep It
        </router-link>
        <router-link
            class="navbar-brand"
            :to="{ name: 'project-list' }">
            Projects
        </router-link>
        <button
            class="navbar-toggler"
            type="button"
            data-toggle="collapse"
            data-target="#navbarSupportedContent"
            aria-controls="navbarSupportedContent"
            aria-expanded="false"
            aria-label="Toggle navigation"
        >
            <span class="navbar-toggler-icon" />
        </button>
        <div
            id="navbarSupportedContent"
            class="collapse navbar-collapse"
        >
            <ul class="navbar-nav mr-auto">
                <li class="nav-item dropdown">
                    <a
                        id="navbarDropdown"
                        class="nav-link dropdown-toggle"
                        href="#"
                        role="button"
                        data-toggle="dropdown"
                        aria-haspopup="true"
                        aria-expanded="false"
                    >
                        Projects
                    </a>
                    <div
                        class="dropdown-menu"
                        aria-labelledby="navbarDropdown"
                    >
                        <router-link
                            v-for="(project, index) in projects"
                            :key="index"
                            class="dropdown-item"
                            :to="{ name: 'project', params: { projectId: project.id }}"
                        >
                            {{ project.id }}
                        </router-link>
                        <div class="dropdown-divider" />
                        <router-link
                            class="dropdown-item"
                            :to="{ name: 'create-project' }"
                        >
                            Create new project
                        </router-link>
                    </div>
                </li>
            </ul>
            <span class="my-2 my-lg-0">
                <button
                    class="btn btn-outline-light my-2 my-sm-0"
                    type="submit"
                    @click="signOut()"
                >
                    Log Out
                </button>
            </span>
        </div>
    </nav>
</template>

<script>

    import jwtTokenStorage from "../../common/authentication/jwt-token-storage.js";


    export default {
        name: 'Navigation',
        
        props: ['projects'],
    
        methods: {
            signOut() {
                jwtTokenStorage.remove()
                this.$router.push({ name: 'sign-in' })
            }
        }
    }
    
</script>