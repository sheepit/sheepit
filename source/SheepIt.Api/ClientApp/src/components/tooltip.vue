<template>
    <span ref="spanRef" data-toggle="tooltip" v-bind:data-placement="dataPlacement" v-bind:title="text">
        <slot></slot>
    </span>
</template>

<script>
export default {
    name: 'tooltip',
    
    props: {
        'text': String,
        'dataPlacement': {
            type: String,
            default: 'top'
        }
    },

    mounted() {
        $(this.$refs.spanRef).tooltip()
    },
}
</script>