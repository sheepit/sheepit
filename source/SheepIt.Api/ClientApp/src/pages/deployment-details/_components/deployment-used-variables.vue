<template>
    <div>
        <h3 class="mt-5">
            Deployment variables
        </h3>
        <table>
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
    </div>
</template>

<script>
export default {
    name: 'DeploymentUsedVariables',
    
    props: ['usedVariables']
}
</script>