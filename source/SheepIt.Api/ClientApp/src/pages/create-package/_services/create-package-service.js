import httpService from './../../../common/http/http-service'

export default {
    createPackage(
        projectId,
        environments,
        description,
        zipFileData,
        newVariables) {

        const formData = new FormData();
        formData.append('ProjectId', projectId);
        formData.append('Description', description);
        formData.append('ZipFile', zipFileData);

        this.convertVariablesToFormData(formData, newVariables, environments);

        return httpService
            .postFormData(
                'api/project/package/create-package',
                formData
            );
    },

    convertVariablesToFormData(formData, newVariables, environments) {
        if(!newVariables)
            return;

        for(let i = 0; i < newVariables.length; i++) {
            let variables = newVariables[i];
            formData.append('VariableUpdates[' + i + '].Name', variables.name);
            formData.append('VariableUpdates[' + i + '].DefaultValue', variables.defaultValue);

            this.mapEnvironmentValuesToFormData(
                formData, i, variables.environmentValues, environments);
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
