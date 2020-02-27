<template>
    <div class="component-signin d-flex align-items-center text-center">
        <form
            class="form-signin"
            @submit.prevent="authenticate()"
        >
            <h1 class="h3 mb-3 font-weight-normal">
                SheepIt
            </h1>
            <p>
                Please enter a single user password:
            </p>
            <label
                for="password"
                class="sr-only"
            >
                Password
            </label>
            <input
                id="password"
                v-model="password"
                type="password"
                class="form-control"
                placeholder="password"
            >
            <button
                class="button button--primary"
                type="submit"
                :disabled="!password"
            >
                Sign in
            </button>
        </form>
    </div>
</template>

<script>
import httpService from "../../common/http/http-service.js";
import jwtTokenStorage from "../../common/authentication/jwt-token-storage.js";
import messageService from "./../../common/message/message-service";
    
export default {
    name: "Authenticate",
        
    data() {
        return {
            password: ""
        }
    },
        
    mounted() {
        document.getElementById('password').focus()
    },
        
    methods: {
        authenticate() {
            httpService
                .post("api/authenticate", {
                    password: this.password
                })
                .then(response => {
                    if (response.authenticated) {
                        jwtTokenStorage.set(response.jwtToken)
                        this.$router.push({ name: "default" })
                    } else {
                        this.password = "";
                        messageService.error("Invalid password!");
                    }
                })
        }
    }
        
}
</script>

<style scoped>

    .component-signin {
        height: 100vh;
    }

    .form-signin {
        width: 100%;
        max-width: 330px;
        padding: 15px;
        margin: auto;
    }
    
</style>