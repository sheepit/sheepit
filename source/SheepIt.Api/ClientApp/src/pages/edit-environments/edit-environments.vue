<template>
    <div v-if="environments">

        <div class="view-title">Edit environments</div>

        <div class="form">
        
            <div class="form-section">
                <div class="form-title">Environments</div>

                <div class="form-group">

                    <label>List of environments</label>

                    <draggable v-model="environments" draggable=".dragMe">

                        <div v-for="(environment, environmentIndex) in environments"
                            :key="environmentIndex"
                            :class="{'dragMe': !environments[environmentIndex].edition}"
                            >

                            <div v-if="environments[environmentIndex].edition"
                                class="input-group mb-3">
                                <input
                                    v-model="environments[environmentIndex].displayName"
                                    type="text"
                                    class="form-control"
                                    :class="{ 'is-invalid': submitted && $v.environments.$each[environmentIndex].displayName.$error }"
                                >
                                <div class="input-group-append">
                                    <button 
                                        class="btn btn-outline-secondary"
                                        type="button"
                                        @click="disableEnvironmentEdition(environmentIndex)"
                                    >
                                        <span class="icon icon-ok" />
                                    </button>
                                </div>
                                <div class="input-group-append">
                                    <button
                                        class="btn btn-outline-secondary"
                                        type="button"
                                        :disabled="environment.persisted || environments.length < 2"
                                        @click="removeEnvironment(environmentIndex)"
                                    >
                                        <span class="icon icon-trash" />
                                    </button>
                                </div>

                                <div v-if="submitted && $v.environments.$each[environmentIndex].displayName.$error" class="invalid-feedback">
                                    <span v-if="!$v.environments.$each[environmentIndex].displayName.required">Field is required</span>
                                    <span v-if="!$v.environments.$each[environmentIndex].displayName.minLength">Field should have at least 3 characters</span>
                                </div>
                            </div>

                            <div v-else
                                class="input-group mb-3 form-draggable">
                                <input
                                    v-model="environments[environmentIndex].displayName"
                                    type="text"
                                    class="form-control form-draggable"
                                    :class="{ 'is-invalid': submitted && $v.environments.$each[environmentIndex].displayName.$error }"
                                    disabled="disabled"
                                >
                                <div class="input-group-append">
                                    <button 
                                        class="btn btn-outline-secondary"
                                        type="button"
                                        @click="enableEnvironmentEdition(environmentIndex)"
                                    >
                                        <span class="icon icon-pencil" />
                                    </button>
                                </div>
                                <div class="input-group-append">
                                    <button 
                                        class="btn btn-outline-secondary"
                                        type="button"
                                        :disabled="environments.length < 2 || environment.persisted"
                                        @click="removeEnvironment(environmentIndex)"
                                    >
                                        <span class="icon icon-trash" />
                                    </button>
                                </div>

                                <div v-if="submitted && $v.environments.$each[environmentIndex].displayName.$error" class="invalid-feedback">
                                    <span v-if="!$v.environments.$each[environmentIndex].displayName.required">Field is required</span>
                                    <span v-if="!$v.environments.$each[environmentIndex].displayName.minLength">Field should have at least 3 characters</span>
                                </div>
                            </div>

                        </div>

                    </draggable>

                </div>

                <div class="button-container">
                    <button
                        class="btn btn-secondary"
                        @click="addNewEnvironment()"
                    >
                        Add new
                    </button>
                </div>
            </div>

            <div class="submit-button-container">
                <button
                    type="button"
                    class="btn btn-primary"
                    @click="save()"
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
import draggable from 'vuedraggable';

export default {
    name: 'EditEnvironments',

    components: {
        draggable
    },

    data() {
        return {
            project: null,
            environments: null,

            submitted: false
        }
    },

    computed: {
        projectId() {
            return this.$route.params.projectId
        }
    },

    mounted() {
        this.getEnvironments();
    },

    methods: {
        save: function () {
            this.submitted = true;

            this.$v.$touch();
            
            if (this.$v.$invalid) {
                return;
            }

            const environments = this.environments.map((item, index) => {
                return {
                    id: item.environmentId,
                    displayName: item.displayName,
                    rank: index
                };
            });

            updateProject(this.projectId, environments)
                .then(() => {
                    this.markEnvironmentsAsPersisted();            
                });
        },

        markEnvironmentsAsPersisted() {
            if (this.environments)
            {
                // todo: y mapping instead of iterating?
                this.environments = this.environments.map(item => {
                    item.persisted = true;
                    return item;
                });

                this.$forceUpdate();
            }
        },

        addNewEnvironment() {
            this.environments.push({
                persisted: false,
                edition: true
            });

            this.enableEnvironmentEdition(this.environments.length - 1);
        },

        enableEnvironmentEdition(index) {
            // todo: y mapping instead of iterating?
            let environments = this.environments.map(item => item.edition = false);

            if (this.environments[index]) {
                this.$set(this.environments[index], 'edition', true);
            }
            
            this.$forceUpdate();
        },

        disableEnvironmentEdition(index) {
            if (this.environments[index]) {
                this.$set(this.environments[index], 'edition', false);
            }
            
            this.$forceUpdate();
        },

        removeEnvironment: function(index) {
            if (this.environments.length < 2) {
                return;
            }

            this.environments.splice(index, 1);
        },

        getEnvironments() {
            httpService
                .post('api/project/environment/get-environments-for-update', {
                    projectId: this.projectId
                })
                .then(response => {
                    // todo: use spread instead
                    this.environments = response.environments.map(env => {
                        env.persisted = true;
                        env.edition = false;

                        return env;
                    });
                });
        }
    },
    validations: {
        environments: {
            required,
            minLength: minLength(1),
            $each: {
                displayName: {
                    required,
                    minLength: minLength(3)
                }
            }
        }
    }
}

// todo: rename
function updateProject(projectId, environments) {
    return httpService.post('api/project/environment/update-environments', {
        projectId: projectId,
        environments: environments
    }, false);
}

</script>

<style lang="scss" scoped>
.card {
    margin-bottom: 1rem;
}

.save-button-container {
    display: flex;
    justify-content: flex-end;
}

.add-environment-button {
    height: 51px;
    width: 255px;
}

.environment-card-header {
    min-height: 49px;
    height: 100%;
    width: 100%;
}

.disabled {
    background-color: $disabled-gray;
}
</style>
