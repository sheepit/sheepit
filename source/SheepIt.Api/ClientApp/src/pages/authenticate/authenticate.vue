<template>
    <div class="login">
        <div class="login__container">
            <form
                class="login__form"
                @submit.prevent="authenticate()"
            >
                <div class="login__logo">
                    <div class="login__logo--brand">
                        <font-awesome-icon icon="dog" size="lg" />
                        <span class="login__logo--text">SheepIt</span>
                    </div>
                </div>
                
                <p>
                    Please enter a single user password:
                </p>
                <input
                    id="password"
                    v-model="password"
                    type="password"
                    class="form__control login__form__input"
                    placeholder="password"
                >
                <button
                    class="button button--primary login__form__button"
                    type="submit"
                    :disabled="!password"
                >
                    SIGN IN
                </button>
            </form>
        </div>
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
                .post("frontendApi/authenticate", {
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

<style lang="scss" scoped>
.login {
    height: 100vh;
    background: $green-main;

    display: flex;
    justify-content: center;

    &__container {
        display: flex;
        align-items: center;
    }

    &__logo {
        display: flex;
        justify-content: center;

        &--brand {
            display: flex;
            align-items: center;

            svg {
                font-size: 25px;

                path {
                    color: $font-color;
                }
            }
        }

        &--text {
            @include font($size: 24px, $color: $font-color);
            margin-left: 6px;
        }
    }

    &__form {
        background: #FFFFFF;
        max-width: 360px;
        margin: 0 auto 100px;
        padding: 45px;
        text-align: center;
        box-shadow: 0 0 20px 0 rgba(0, 0, 0, 0.2), 0 5px 5px 0 rgba(0, 0, 0, 0.24);

        &__input {
            width: 100%;
            margin: 0 0 10px;
            padding: 10px;
            box-sizing: border-box;
            //font-size: 14px;
        }

        &__button {
            width: 100%;
        }
    }
}
</style>