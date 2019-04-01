<template>
    <span
        ref="spanRef"
        data-toggle="tooltip"
        :data-placement="dataPlacement"
        :title="text"
    >
        <slot />
    </span>
</template>

<script>
export default {
    name: 'Tooltip',
    
    props: {
        'text': {
            type: String,
            required: true
        },
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