<template>
    <div
        data-event-handler 
        @unauthorized="handleUnauthorized()"
    >
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

<style scoped>
</style>