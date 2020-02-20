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
                <side-menu />
            </nav>

            <main>
                <div class="ddd2">
                    <router-view />
                </div>
            </main>
        </div>

        <breadcrumb />
    </div>
</template>

<script>
import TopBar from '../../components/layout/topBar.vue'
import SideMenu from '../../components/layout/sideMenu.vue'
import Breadcrumb from "../../components/breadcrumb.vue";

export default {
    name: "DefaultLayout",

    components: {
        TopBar,
        SideMenu,
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
    "header header"
    "nav content";

    grid-template-columns: 220px 1fr;
    grid-template-rows: auto 1fr;
    grid-column-gap: 10px;

    height: 100vh;
}

.ddd {
    border-style: solid;
    border-width: 0 1px 0 0;
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
</style>
