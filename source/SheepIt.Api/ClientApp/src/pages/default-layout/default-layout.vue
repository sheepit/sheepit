<template>
    <div
        data-event-handler 
        @unauthorized="handleUnauthorized()"
    >
        <div class="container" :class="{ 'container--basic': !sideMenuEnabled, 'container--side-menu': sideMenuEnabled }">
            <header>
                <top-bar />
            </header>

            <nav v-if="sideMenuEnabled" class="container__navigation">
                <side-menu />
            </nav>

            <breadcrumb v-if="sideMenuEnabled" />

            <main>      
                <div class="ddd2">
                    <router-view />
                </div>
            </main>
        </div>

        
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

    mounted() {
        this.updateSideMenu();
    },

    data: function() {
        return {
            sideMenuEnabled: false
        }
    },

    watch: { 
        '$route' () { 
            this.updateSideMenu() 
        } 
    },

    methods: {
        handleUnauthorized() {
            this.$router.push({ name: 'sign-in' })
        },

        updateSideMenu() {
            if(this.$route.meta && this.$route.meta.sideMenu) {
                this.sideMenuEnabled = true;
            } else {
                this.sideMenuEnabled = false;
            }
        }
    }
}   
</script>

<style lang="scss" scoped>
.container {
    display: grid;

    grid-column-gap: 0px;

    height: 100vh;

    &--side-menu {
        grid-template-areas:
        "header header"
        "nav breadcrumb"
        "nav content";

        grid-template-columns: 220px 1fr;
        grid-template-rows: auto 35px 1fr;
    }

    &--basic {
        grid-template-areas:
        "header"
        "content";

        grid-template-columns: 1fr;
        grid-template-rows: auto 1fr;
    }

    &__navigation {
        border-style: solid;
        border-width: 0 1px 0 0;
        border-color: $font-color-light;

        height: 100%;
        width: 100%;

        background: $gray-main;
    }
}

.ddd2 {
    height: 100%;

    max-width: 990px;
    width: 990px;

    justify-self: center;
    align-self: center;
}

header {
  grid-area: header;
}

nav {
  grid-area: nav;
}

breadcrumb {
    grid-area: breadcrumb;
}

main {
    grid-area: content;
    justify-self: center;
}

</style>
