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
            <div class="navigation__profile--icon" @click="signOut()">
                <font-awesome-icon icon="user-circle" size="lg" />
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

    &__link {
        @include font($size: 16px, $color: $font-color-light);
        padding-left: 10px;
        padding-right: 10px;
        color: $font-color-light;
        text-decoration: none;
        cursor: pointer;
        height: 100%;
        display: flex;
        align-items: center;

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
            @include font($color: $font-color-light, $size: 16px);
            margin-left: 10px;
            display: flex;
            align-items: center;
            height: 100%;

            svg {
                font-size: 25px;

                path {
                    color: $font-color-light;
                }
            }
        }

        &--text {
            @include font($color: $font-color-light, $size: 20px);
            margin-left: 6px;
        }
    }

    &__menu {
        display: flex;
        align-items: center;
        flex: 75;
        margin-left: 1px;

        &--item {
            font-size: 16px;
            height: 100%;

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
        cursor: pointer;
        height: 100%;

        &--icon {
            margin-right: 10px;
            display: flex;
            align-items: center;
            height: 100%;
            padding: 0 10px 0 10px;

            &:hover {
                background: $green-hover;
                color: $white;
            }

            svg {
                font-size: 25px;

                path {
                    color: $font-color-light;
                }
            }
        }
    }
}
</style>