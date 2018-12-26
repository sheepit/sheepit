<template>
    <div>
        <h3 class="mt-5">Deployment variables</h3>
        <table class="table table-bordered">
            <thead>
                <tr>
                    <th>name</th>
                    <th>value</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(variable, variableIndex) in usedVariables">
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
    module.exports = {
        name: 'deployment-used-variables',
        
        props: ['usedVariables']
    }
</script>