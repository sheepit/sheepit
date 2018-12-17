<template>
    <div>
        <slot v-bind:items="items"></slot>
        <p>
            <button class="btn btn-outline-primary" v-if="canExpand" v-on:click="showAll = true">
                Show more ({{ allItems.length }})
            </button>
            <button class="btn btn-outline-primary" v-if="canCompress" v-on:click="showAll = false">
                Show less ({{ initialLength }})
            </button>
        </p>
    </div>
</template>

<script>
    module.exports = {
        name: 'expanding-list',
        
        props: ['allItems', 'initialLength'],

        data() {
            return {
                showAll: false
            }
        },

        computed: {
            canExpand() {
                return !this.showAll && this.allItems.length > this.initialLength
            },
            canCompress() {
                return this.showAll  
            },
            items() {
                return this.showAll
                    ? this.allItems
                    : this.allItems.slice(0, this.initialLength)
            }
        },
        
    }
</script>
