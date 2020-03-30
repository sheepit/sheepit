<template>
    <div>        
        <table>
            <thead>
                <tr>
                    <th class="action-column"></th>
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
                <tr
                    v-for="(variable, variableIndex) in variables"
                    :key="variableIndex"
                >
                    <td class="action-column">
                        <button
                            class="button button--inline button--secondary"
                            type="button"
                            @click="removeVariable(variableIndex)"
                        >
                            X
                        </button>
                    </td>
                    <td>
                        <input
                            v-model="variable.name"
                            type="text"
                            class="form__control table-item"
                        >
                    </td>
                    <td>
                        <input
                            v-model="variable.defaultValue"
                            type="text"
                            class="form__control table-item"
                        >
                    </td>
                    <td v-for="environment in environments">
                        <input
                            v-model="variable.environmentValues[environment.id]"
                            type="text"
                            class="form__control table-item"
                        >
                    </td>
                </tr>
                <tr>
                    <td />
                    <td />
                    <td v-for="environment in environments" />
                    <td class="button-container">
                        <button
                            class="button button--secondary"
                            @click="addVariable()"
                        >
                            Add variable
                        </button>
                    </td>
                </tr>
            </tbody>
        </table>     
    </div>
</template>

<script>
export default {
    name: 'VariableEditor',
    
    props: ['variables', 'environments'],
    
    methods: {
        addVariable() {
            this.variables.push({
                name: '',
                defaultValue: '',
                environmentValues: {}
            })
        },
        removeVariable(variableIndex) {
            this.variables.splice(variableIndex, 1)
        }
    }
}
</script>

<style lang="scss" scoped>
.action-column {
    width: 35px;
}

.table-item {
    width: 100%;
}
</style>