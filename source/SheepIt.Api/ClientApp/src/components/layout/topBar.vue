<template>
    <div class="navigation">
        <div class="navigation__logo">
            <div class="navigation__logo--brand">
                <router-link
                    class="navigation__logo--link"
                    :to="{ name: 'default' }"
                >
                    <font-awesome-icon icon="dog" size="lg" />
                    <span class="navigation__logo--text">SheepIt</span>
                </router-link>
            </div>
        </div>
        <div class="navigation__menu">
            <div class="navigation__menu--item">
                <router-link 
                    class="navigation__menu--item--link"
                    :to="{ name: 'projects' }"
                >
                    Projects
                </router-link>
            </div>

        </div>
        <div class="navigation__profile">
            <div class="navigation__profile--icon">
                <div class="navigation__profile--icon--link">
                    <font-awesome-icon icon="user-circle"  size="lg" @click="signOut()" />
                </div>
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
        flex: 0 0 220px;

        &--brand {
            margin-left: 10px;
        }

        &--link {
            padding: 11px;
            color: #EBEBEB;
            text-decoration: none;

            &:hover {
                color: #FFFFFF;
            }
        }

        &--text {
            margin-left: 8px;
        }
    }

    &__menu {
        display: flex;
        align-items: center;
        flex: 75;
        margin-left: 1px;

        &--item {

            &--link {
                padding: 11px;
                color: #EBEBEB;
                text-decoration: none;

                &:hover {
                    color: #FFFFFF;
                }
            }
        }

        .router-link-active {
            background: #3B8A86;
        }
    }

    &__profile {
        display: flex;
        justify-content: flex-end;
        align-items: center;
        flex: 10;

        &--icon {
            margin-right: 10px;

            &--link {
                padding: 10px;
                color: #EBEBEB;
                text-decoration: none;
                cursor: pointer;

                &:hover {
                    color: #FFFFFF;
                }
            }
        }
    }
}
</style>