<template>
    <div class="container" 
        data-event-handler 
        @unauthorized="handleUnauthorized()">
        
        <navigation />

        <main class="mt-5">
            <router-view />
        </main>
        
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