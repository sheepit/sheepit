<template>
    <div data-event-handler 
        @unauthorized="handleUnauthorized()">
        
        <navigation />

        <div class="container">
            <main class="mt-5">
                <router-view />
            </main>
        </div>
        
    </div>
</template>

<script>
import Navigation from '../../components/layout/navigation.vue'

export default {
    name: "DefaultLayout",

    components: {
        Navigation
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