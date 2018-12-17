<template>
    <span ref="spanRef" data-toggle="tooltip" v-bind:data-placement="dataPlacement" v-bind:title="text">
        <slot></slot>
    </span>
</template>

<script>
    module.exports = {
        name: 'tooltip',
        
        props: ['text', 'dataPlacement'],

        mounted() {
            $(this.$refs.spanRef).tooltip()
        },
    }
</script>