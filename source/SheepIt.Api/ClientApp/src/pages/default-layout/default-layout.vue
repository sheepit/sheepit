<template>
    <div
        data-event-handler 
        @unauthorized="handleUnauthorized()"
    >
        <Navvv />

        <navigation />

        <breadcrumb />

        <div class="container">
            <main class="mt-4">
                <router-view />
            </main>
        </div>
    </div>
</template>

<script>
import Navigation from '../../components/layout/navigation.vue'
import Navvv from '../../components/layout/navBar.vue'
import Breadcrumb from "../../components/breadcrumb.vue";

export default {
    name: "DefaultLayout",

    components: {
        Navigation,
        Breadcrumb,
        Navvv
    },

    methods: {
        handleUnauthorized() {
            this.$router.push({ name: 'sign-in' })
        }
    }
}   
</script>

<style scoped>
</style>