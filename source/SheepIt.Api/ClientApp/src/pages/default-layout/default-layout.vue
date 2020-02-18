<template>
    <div
        data-event-handler 
        @unauthorized="handleUnauthorized()"
    >
        <div class="container">
            <header>
                <top-bar />
            </header>

            <nav class="ddd">
                <div class="sidemenu_placeholder"></div>

                <ul class="sidemenu">
                    <li class="sidemenu__item">
                        <a class="sidemenu__link" href="#">
                            <div class="sidemenu__link--icon">
                                <font-awesome-icon icon="code" />
                            </div>
                            <div class="sidemenu__link--name">
                                Components
                            </div>
                        </a>
                    </li>
                    <li>
                        <a class="sidemenu__link" href="#">
                            <div class="sidemenu__link--icon">
                                <font-awesome-icon icon="server" />
                            </div>
                            <div class="sidemenu__link--name">
                                Environments
                            </div>
                        </a>
                    </li>
                    <li>
                        <a class="sidemenu__link" href="#">
                            <div class="sidemenu__link--icon">
                                <font-awesome-icon icon="cubes" />
                            </div>
                            <div class="sidemenu__link--name">
                                Packages
                            </div>
                        </a>
                    </li>
                    <li>
                        <a class="sidemenu__link" href="#">
                            <div class="sidemenu__link--icon">
                                <font-awesome-icon icon="cogs" />
                            </div>
                            <div class="sidemenu__link--name">
                                Deployments
                            </div>
                        </a>
                    </li>
                </ul>
            </nav>

            <main>
                <div class="ddd2">
                    CONTENT
                </div>
            </main>
        </div>

        <top-bar />

        <breadcrumb />

        <div class="container">
            <main class="mt-4">
                <router-view />
            </main>
        </div>
    </div>
</template>

<script>
import TopBar from '../../components/layout/topBar.vue'
import Breadcrumb from "../../components/breadcrumb.vue";

export default {
    name: "DefaultLayout",

    components: {
        TopBar,
        Breadcrumb
    },

    methods: {
        handleUnauthorized() {
            this.$router.push({ name: 'sign-in' })
        }
    }
}   
</script>

<style lang="scss" scoped>
.container {
    display: grid;

    grid-template-areas:
    "header header header"
    "nav content .";

    grid-template-columns: 220px 1fr 220px;
    grid-template-rows: auto 1fr auto;
    grid-column-gap: 10px;

    height: 100vh;
}

.ddd {
    border-style: solid;
    border-width: 0 1px 0 1px;
    border-color: #EBEBEB;

    height: 100%;
    width: 100%;

    background: #FAFAFA;
}

.ddd2 {
    border-style: solid;
    border-width: 0 1px 0 1px;
    border-color: #EBEBEB;

    height: 100%;

    max-width: 990px;
    width: 990px;

    background: #FAFAFA;

    justify-self: center;
    align-self: center;
}

header {
  grid-area: header;
}

nav {
  grid-area: nav;
}

main {
    grid-area: content;

    justify-self: center;
}

.sidemenu_placeholder {
    height: 30px;
}

.sidemenu {
    padding-left: 0px;
    list-style: none;

    color: #898989;
    font-size: 16px;

    &__item {
    }

    &__link {
        display: flex;
        align-items: center;
        margin: 0 1px 0 4px;
        padding: 12px;
        line-height: 21px;
        color: #898989;
        text-decoration: none;

        &--icon {
            display: flex;
            align-items: center;
            margin-right: 9px;

            svg {
                height: 16px;
                width: 16px;
            }
        }

        &--name {
            flex: 1;
        }

        &:hover {
            background: #BABABA;
            color: #FFFFFF;
        }
    }
}
</style>
