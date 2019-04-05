<template>
    <div>        
        <table class="table table-bordered">
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
                <tr
                    v-for="(variable, variableIndex) in variables"
                    :key="variableIndex"
                >
                    <td>
                        <div class="input-group mb-3">
                            <div class="input-group-prepend">
                                <button
                                    class="btn btn-sm btn-danger"
                                    @click="removeVariable(variableIndex)"
                                >
                                    X
                                </button>
                            </div>
                            <input
                                v-model="variable.name"
                                type="text"
                                class="form-control"
                            >
                        </div>
                    </td>
                    <td>
                        <input
                            v-model="variable.defaultValue"
                            type="text"
                            class="form-control"
                        >
                    </td>
                    <td v-for="environment in environments">
                        <input
                            v-model="variable.environmentValues[environment.id]"
                            type="text"
                            class="form-control"
                        >
                    </td>
                </tr>
                <tr>
                    <td>
                        <button
                            class="btn btn-sm btn-primary"
                            @click="addVariable()"
                        >
                            Add variable
                        </button>
                    </td>
                    <td />
                    <td v-for="environment in environments" />
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