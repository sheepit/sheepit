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
                <tr v-for="(variable) in usedVariables">
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
    name: 'deployment-used-variables',
    
    props: ['usedVariables']
}
</script>