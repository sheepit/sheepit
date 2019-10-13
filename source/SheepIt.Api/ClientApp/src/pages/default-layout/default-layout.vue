<template>
    <div
        data-event-handler 
        @unauthorized="handleUnauthorized()"
    >
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
import Breadcrumb from "../../components/breadcrumb.vue";

export default {
    name: "DefaultLayout",

    components: {
        Navigation,
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