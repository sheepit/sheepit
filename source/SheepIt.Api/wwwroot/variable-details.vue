<template>
    <div>
        <h4>Variables</h4>
        <table class="table table-bordered">
            <thead>
                <tr>
                    <th>name</th>
                    <th>default value</th>
                    <th v-for="environment in environments">{{ environment }}</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(variable, variableIndex) in variables">
                    <td>
                        <span>{{ variable.name }}</span>
                    </td>
                    <td>
                        <span>{{ variable.defaultValue }}</span>
                    </td>
                    <td v-for="environment in environments">
                        <span>{{ variable.environmentValues[environment] }}</span>
                    </td>
                </tr>
            </tbody>
        </table>
        
    </div>
</template>

<script>
    module.exports = {
        name: 'variable-details',
        
        props: ['variables', 'environments'],
    }
</script>