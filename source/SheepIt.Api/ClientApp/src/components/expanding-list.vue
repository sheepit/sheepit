<template>
    <div>
        <slot :items="items" />
        <div class="view__row view__row--right">
            <button
                v-if="canExpand"
                class="button button--secondary"
                @click="showAll = true"
            >
                Show more ({{ allItems.length }})
            </button>
            <button
                v-if="canCompress"
                class="button button--secondary"
                @click="showAll = false"
            >
                Show less ({{ initialLength }})
            </button>
        </div>
    </div>
</template>

<script>
export default {
    name: 'ExpandingList',
    
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
