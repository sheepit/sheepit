<template>
    <div class="navigation">
        <div class="navigation__logo">
            <div class="navigation__logo--brand">
                <router-link
                    class="navigation__link"
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
                    class="navigation__link"
                    :to="{ name: 'default' }"
                    exact
                >
                    Dashboard
                </router-link>
            </div>

            <div class="navigation__menu--item">
                <router-link 
                    class="navigation__link"
                    :to="{ name: 'projects' }"
                >
                    Projects
                </router-link>
            </div>

        </div>
        <div class="navigation__profile">
            <div class="navigation__profile--icon">
                <font-awesome-icon icon="user-circle" size="lg" @click="signOut()" />
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

    height: 42px;
    background: $green-main;

    font-size: 20px;
    font-weight: 400;
    color: $font-color-light;

    &__link {
        padding: 10px;
        color: $font-color-light;
        text-decoration: none;
        cursor: pointer;

        &:hover {
            background: $green-hover;
            color: $white;
        }
    }

    &__logo {
        display: flex;
        align-items: center;
        flex: 0 0 220px;
        color: $font-color-light;

        &--brand {
            margin-left: 10px;
            color: $font-color-light;
        }

        &--text {
            margin-left: 8px;
            color: $font-color-light;
        }
    }

    &__menu {
        display: flex;
        align-items: center;
        flex: 75;
        margin-left: 1px;

        &--item {
            font-size: 16px;

            .router-link-active {
                background: $green-active;
            }
        }


    }

    &__profile {
        display: flex;
        justify-content: flex-end;
        align-items: center;
        flex: 10;
        padding: 8px;
        cursor: pointer;

        &--icon {
            margin-right: 10px;
        }


    }
}
</style>