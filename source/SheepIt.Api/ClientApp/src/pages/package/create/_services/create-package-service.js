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

        this.convertVariablesToFormData(formData, newVariables, environments);

        return httpService
            .postFormData(
                'frontendApi/project/package/create-package',
                formData
            );
    },

    convertVariablesToFormData(formData, newVariables, environments) {
        if (!newVariables) {
            return;
        }

        for(let variablesIndex = 0; variablesIndex < newVariables.length; variablesIndex++) {
            const variables = newVariables[variablesIndex];
            
            formData.append('VariableUpdates[' + variablesIndex + '].Name', variables.name);
            
            if (variables.defaultValue) {
                formData.append('VariableUpdates[' + variablesIndex + '].DefaultValue', variables.defaultValue);
            }

            this.mapEnvironmentValuesToFormData(
                formData, variablesIndex, variables.environmentValues, environments);
        }
    },

    mapEnvironmentValuesToFormData(formData, index, values, environments) {
        for(let eindex = 0; eindex < environments.length; eindex++) {
            let environmentId = environments[eindex].id;

            let valueForEnvironment = values[environmentId];
            if(valueForEnvironment) {
                formData.append('VariableUpdates[' + index + '].EnvironmentValues[' + eindex + '].Key', environmentId);
                formData.append('VariableUpdates[' + index + '].EnvironmentValues[' + eindex + '].Value', valueForEnvironment);
            }
        }
    }
}
