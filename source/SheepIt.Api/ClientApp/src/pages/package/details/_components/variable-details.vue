<template>
    <div>
        <h3 class="mt-5">
            Variables
        </h3>
        <table>
            <thead>
                <tr>
                    <th>name</th>
                    <th>default value</th>
                    <th
                        v-for="environment in environments"
                        :key="environment.id"
                    >
                        {{ environment.displayName }}
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(variable) in variables">
                    <td>
                        <span>{{ variable.name }}</span>
                    </td>
                    <td>
                        <span>{{ variable.defaultValue }}</span>
                    </td>
                    <td
                        v-for="environment in environments"
                        :key="environment.id"
                    >
                        <span>{{ variable.environmentValues[environment.id] }}</span>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<script>
export default {
    name: 'VariableDetails',
    
    props: ['variables', 'environments'],
}
</script>