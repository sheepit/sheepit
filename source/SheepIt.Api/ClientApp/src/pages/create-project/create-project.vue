<template>
  <div>
    <div class="view-title">Add new project</div>

    <div class="form">
      <div class="form-section">
        <div class="form-title">Details</div>

        <div class="form-group">
          <label for="projectId" class="form-label">Project id</label>
          <input
            id="projectId"
            v-model="projectId"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': submitted && $v.projectId.$error }"
          />
          <div v-if="submitted && $v.projectId.$error" class="invalid-feedback">
            <span v-if="!$v.projectId.required">Field is required</span>
            <span v-if="!$v.projectId.minLength">Field should have at least 3 characters</span>
          </div>
        </div>

        <div class="form-group">
            <label for="sourceUrl">Process definition</label>
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

      <div class="form-section">
        <div class="form-title">Environments</div>
        <div class="form-group">
          <label>List of environments</label>

          <div
            v-for="(environment, environmentIndex) in environments"
            :key="environmentIndex"
            class="input-group mb-3"
          >
            <input
              v-model="environments[environmentIndex]"
              type="text"
              class="form-control"
              :class="{ 'is-invalid': submitted && $v.environments.$each[environmentIndex].$error }"
            />
            <div class="input-group-append">
              <button
                class="btn btn-outline-secondary"
                type="button"
                :disabled="environments.length < 2"
                @click="removeEnvironment(environmentIndex)"
              >
                <span class="icon icon-trash" />
              </button>
            </div>
            <div
              v-if="submitted && $v.environments.$each[environmentIndex].$error"
              class="invalid-feedback"
            >
              <span v-if="!$v.environments.$each[environmentIndex].required">Field is required</span>
              <span
                v-if="!$v.environments.$each[environmentIndex].minLength"
              >Field should have at least 3 characters</span>
            </div>
          </div>
        </div>

        <div class="button-container">
          <button class="btn btn-secondary" @click="newEnvironment()">Add new</button>
        </div>
      </div>

      <div class="submit-button-container">
        <button type="button" class="btn btn-primary" @click="onSubmit()">Save</button>
      </div>
    </div>
  </div>
</template>

<script>
import { required, minLength, url } from "vuelidate/lib/validators";

import createProjectService from "./_services/create-project-service.js";

export default {
  name: "CreateProject",
  data() {
    return {
      projectId: "",
      environments: [""],

      submitted: false
    };
  },
  methods: {
    onSubmit: function() {
      this.submitted = true;

      this.$v.$touch();
      if (this.$v.$invalid) {
        return;
      }

      let zipFile = this.$refs.zipFile;
      let zipFileData = this.$refs.zipFile.files[0];

      createProjectService.createProject(
        this.projectId,
        this.environments,
        zipFileData
      );
    },

    newEnvironment: function() {
      this.environments.push(null);
    },

    removeEnvironment: function(index) {
      if (this.environments.length < 2) return;

      this.environments.splice(index, 1);
    }
  },
  validations: {
    projectId: {
      required,
      minLength: minLength(3)
    },
    environments: {
      required,
      minLength: minLength(1),
      $each: {
        required,
        minLength: minLength(3)
      }
    }
  }
};
</script>
