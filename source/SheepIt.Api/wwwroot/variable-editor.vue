<template>
    <div>
        
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
                        <div class="input-group mb-3">
                            <div class="input-group-prepend">
                                <button class="btn btn-sm btn-danger" v-on:click="removeVariable(variableIndex)">X</button>
                            </div>
                            <input type="text" class="form-control" v-model="variable.name">
                        </div>
                    </td>
                    <td>
                        <input type="text" class="form-control" v-model="variable.defaultValue">
                    </td>
                    <td v-for="environment in environments">
                        <input type="text" class="form-control" v-model="variable.environmentValues[environment]">
                    </td>
                </tr>
                <tr>
                    <td>
                        <button class="btn btn-sm btn-primary" v-on:click="addVariable()">Add variable</button>
                    </td>
                    <td>
                    </td>
                    <td v-for="environment in environments">
                    </td>
                </tr>
            </tbody>
        </table>
        
    </div>
</template>

<script>
    module.exports = {
        name: 'variable-editor',
        
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