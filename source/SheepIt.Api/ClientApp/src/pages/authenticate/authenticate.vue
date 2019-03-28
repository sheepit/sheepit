<template>
    <div>
        <h4>Authenticate</h4>
        <div>
            <div class="form-group">
                <label for="singleUserPassword">Password</label>
                <input
                        id="singleUserPassword"
                        v-model="password"
                        type="password"
                        class="form-control"
                >
            </div>

            <button
                    type="button"
                    class="btn btn-primary"
                    @click="authenticate()"
            >
                Log in
            </button>
        </div>
    </div>
</template>

<script>
    import httpService from "../../common/http/http-service.js";
    import messageService from "./../../common/message/message-service";
    
    export default {
        name: "Authenticate",
        
        data() {
            return {
                password: ""
            }
        },
        
        methods: {
            authenticate() {
                httpService
                    .post("api/authenticate", {
                        password: this.password
                    })
                    .then(response => {
                        if (response.authenticated) {
                            window.localStorage.setItem("jwtToken", response.jwtToken)
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

</style>