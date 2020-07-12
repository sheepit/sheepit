import httpService from './../../../../common/http/http-service'

export default {
    createPackage(
        projectId,
        componentId,
        environments,
        description,
        zipFileData,
        newVariables) {

        const formData = new FormData();
        formData.append('ProjectId', projectId);
        formData.append('ComponentId', componentId);
        formData.append('Description', description);
        formData.append('ZipFile', zipFileData);
        
        // we need to unwrap object from Vue's observables in order to process it
        const unwrappedNewVariables = JSON.parse(JSON.stringify(newVariables));

        this.convertVariablesToFormData(formData, unwrappedNewVariables, environments);

        return httpService
            .postFormData(
                'frontendApi/project/package/create-package',
                formData
            );
    },

    convertVariablesToFormData(formData, newVariables) {
        if (!newVariables) {
            return;
        }
        
        newVariables.forEach((variablesRow, variablesRowIndex) => {

            formData.append(`VariableUpdates[${variablesRowIndex}].Name`, variablesRow.name);

            if (variablesRow.defaultValue) {
                formData.append(`VariableUpdates[${variablesRowIndex}].DefaultValue`, variablesRow.defaultValue);
            }

            Object.entries(variablesRow.environmentValues)
                .map(([environmentId, value]) => ({
                    environmentId,
                    value
                }))
                .filter(variable => variable.value)
                .forEach((variable, variableIndex) => {
                    formData.append(`VariableUpdates[${variablesRowIndex}].EnvironmentValues[${variableIndex}].Key`, variable.environmentId);
                    formData.append(`VariableUpdates[${variablesRowIndex}].EnvironmentValues[${variableIndex}].Value`, variable.value);
                });
            
        });
        
    }

}
