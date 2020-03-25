<template>
    <div>
        <h3>
            Deployment variables
        </h3>
        <table v-if="usedVariables && usedVariables.length > 0">
            <thead>
                <tr>
                    <th>name</th>
                    <th>value</th>
                </tr>
            </thead>
            <tbody>
                <tr
                    v-for="(variable, index) in usedVariables"
                    :key="index"
                >
                    <td>
                        <span>{{ variable.name }}</span>
                    </td>
                    <td>
                        <span>{{ variable.value }}</span>
                    </td>
                </tr>
            </tbody>
        </table>
        <div v-else-if="usedVariables && usedVariables.length === 0">
            <span class="not-found">Variables has not been defined for this deployment</span>
        </div>
        <preloader v-else />
    </div>
</template>

<script>
export default {
    name: 'DeploymentUsedVariables',
    
    props: ['usedVariables']
}
</script>

<style lang="scss" scoped>
.not-found {
    @include font($size: 14px)
}
</style>