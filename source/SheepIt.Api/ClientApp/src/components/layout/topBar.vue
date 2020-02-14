<template>
    <div class="navigation">
        <div class="navigation__logo">
            <div class="navigation__logo--brand">
                <router-link
                    class="navigation__logo--link"
                    :to="{ name: 'default' }"
                >
                    <font-awesome-icon icon="dog" />
                    Sheep It
                </router-link>
            </div>
        </div>
        <div class="navigation__menu">
            <div class="navigation__menu--item">
                <router-link 
                    class="navigation__menu--item--link"
                    :to="{ name: 'project-list' }"
                >
                    Projects
                </router-link>
            </div>

        </div>
        <div class="navigation__profile">
            <div class="navigation__profile--icon">
                <font-awesome-icon icon="user-circle" @click="signOut()" />
            </div>
        </div>
    </div>
</template>

<script>

import jwtTokenStorage from "../../common/authentication/jwt-token-storage.js";


export default {
    name: 'TopBar',
        
    methods: {
        signOut() {
            jwtTokenStorage.remove()
            this.$router.push({ name: 'sign-in' })
        }
    }
}
    
</script>

<style lang="scss" scoped>
.navigation {
    display: flex;
    justify-content: space-between;

    height: 50px;
    background: #45A29E;

    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    font-size: 20px;
    font-weight: 400;
    color: #EBEBEB;

    &__logo {
        display: flex;
        align-items: center;
        flex: 0 0 250px;

        &--brand {
            margin-left: 20px;
        }

        &--link {
            color: #EBEBEB;
            text-decoration: none;

            &:hover {
                color: #797979;
            }
        }
    }

    &__menu {
        display: flex;
        align-items: center;
        flex: 75;

        &--item {
            margin-left: 30px;

            &--link {
                color: #EBEBEB;
                text-decoration: none;

                &:hover {
                    color: #797979;
                }
            }
        }
    }

    &__profile {
        display: flex;
        justify-content: flex-end;
        align-items: center;
        flex: 10;

        &--icon {
            margin-right: 20px;

            &:hover {
                color: #797979;
                cursor: pointer;
            }
        }
    }
}
</style>