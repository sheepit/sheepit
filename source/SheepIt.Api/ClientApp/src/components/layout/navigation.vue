<template>
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark mt-4">
        <router-link
            class="navbar-brand"
            :to="{ name: 'default' }">
            Sheep It
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
                <li class="nav-item">
                    <router-link
                        class="nav-link"
                        :to="{ name: 'project-list' }">
                        Projects
                    </router-link>
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
        
        methods: {
            signOut() {
                jwtTokenStorage.remove()
                this.$router.push({ name: 'sign-in' })
            }
        }
    }
    
</script>