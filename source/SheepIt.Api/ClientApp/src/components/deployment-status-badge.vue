<template>
    <span
        class="badge"
        :class="'badge-' + statuses[status].style"
    >
        {{ statuses[status].label }}
    </span>
</template>

<script>
export default {
    name: 'DeploymentStatusBadge',
    
    props: ['status'],
    
    data() {
        return {
            statuses: {
                InProgress: { 
                    style: 'info',
                    label: 'in progress'
                },
                Succeeded: { 
                    style: 'success',
                    label: 'succeeded'
                },
                ProcessFailed: { 
                    style: 'danger',
                    label: 'process failed'
                },
                ExecutionFailed: { 
                    style: 'danger',
                    label: 'execution failed'
                }
            }
        }
    }
}
</script>