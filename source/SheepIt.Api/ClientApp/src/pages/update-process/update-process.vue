<template>
    <div>

        <div class="view-title">Update process</div>

        <div class="form">
        
            <div class="form-section">

                <div class="form-group">
                    <label for="zipFile">Process definition</label>
                    <div class="input-group">
                        <div class="custom-file">
                            <input
                                id="zipFile"
                                type="file"
                                class="custom-file-input"
                                aria-describedby="inputGroupFileAddon01"
                                ref="zipFile"
                            />
                            <label class="custom-file-label" for="zipFile">Upload zip file</label>
                        </div>
                    </div>
                </div>

            </div>

            <div class="submit-button-container">
                <button
                    type="button"
                    class="btn btn-primary"
                    @click="onSubmit()"
                >
                    Save
                </button>
            </div>
        </div>

    </div>
</template>

<script>
import { required, minLength, url } from 'vuelidate/lib/validators'

import httpService from "./../../common/http/http-service.js";
import updateProcessService from "./_services/update-process-service.js";

export default {
    name: 'UpdateProcess',

    computed: {
        projectId() {
            return this.$route.params.projectId
        }
    },

    methods: {
        onSubmit: function () {
            let zipFile = this.$refs.zipFile;
            let zipFileData = this.$refs.zipFile.files[0];

            updateProcessService.updateProcess(
                this.projectId,
                this.environments,
                zipFileData
            )
            .then(response => {
                messageService.success('Proces został zaktualizowany');
                this.$router.push({ name: 'project', params: { projectId: this.projectId }});
            });
        }      
    }
}

</script>
