<template>
    <div>
        <div class="view__title">
            Create project
        </div>

        <div class="form">
            <div class="form__section">
                <div class="form__title">
                    Details
                </div>

                <div class="form__row">
                    <div class="form__column">
                        <label
                            for="projectId"
                            class="form__label"
                        >Project id</label>
                        <input
                            id="projectId"
                            v-model="projectId"
                            type="text"
                            class="form__control"
                            :class="{ 'is-invalid': submitted && $v.projectId.$error }"
                        >
                        <div
                            v-if="submitted && $v.projectId.$error"
                            class="form__error-section"
                        >
                            <span v-if="!$v.projectId.required">Field is required</span>
                            <span v-if="!$v.projectId.minLength">Field should have at least 3 characters</span>
                        </div>
                    </div>
                    <div class="form__column"></div>
                </div>
            </div>

            <div class="form__section">
                <div class="form__title">
                    Environments
                </div>

                <div class="form__row">
                    <div class="form__column">
                        <label class="form__label">List of environments</label>
                    </div>                   
                </div>

                <div class="form__row"
                    v-for="(environment, environmentIndex) in environments"
                    :key="environmentIndex"
                >
                    <div class="form__column">
                        <div class="form__inline">
                            <input
                                v-model="environments[environmentIndex]"
                                type="text"
                                class="form__control form__inline__control"
                                :class="{ 'is-invalid': submitted && $v.environments.$each[environmentIndex].$error }"
                            >
                            <button
                                class="button button--inline button--secondary"
                                type="button"
                                :disabled="environments.length < 2"
                                @click="removeEnvironment(environmentIndex)"
                            >
                                <font-awesome-icon icon="trash" />
                            </button>
                        </div>
                        <div
                            v-if="submitted && $v.environments.$each[environmentIndex].$error"
                            class="form__error-section"
                        >
                            <span v-if="!$v.environments.$each[environmentIndex].required">Field is required</span>
                            <span
                                v-if="!$v.environments.$each[environmentIndex].minLength"
                            >Field should have at least 3 characters</span>
                        </div>
                    </div>

                    <div class="form__column"></div>
                    <div class="form__column"></div>
                </div>

                <div class="button-container">
                    <button
                        class="button button--secondary"
                        @click="newEnvironment()"
                    >
                        Add new
                    </button>
                </div>
            </div>

            <div class="form__section">
                <div class="form__title">
                    Components
                </div>

                <div class="form__row">
                    <div class="form__column">
                        <label class="form__label">List of components</label>
                    </div>
                </div>

                <div class="form__row"
                     v-for="(component, componentIndex) in components"
                     :key="componentIndex"
                >
                    <div class="form__column">
                        <div class="form__inline">
                            <input
                                    v-model="components[componentIndex]"
                                    type="text"
                                    class="form__control form__inline__control"
                                    :class="{ 'is-invalid': submitted && $v.components.$each[componentIndex].$error }"
                            >
                            <button
                                    class="button button--inline button--secondary"
                                    type="button"
                                    :disabled="components.length < 2"
                                    @click="removeComponent(componentIndex)"
                            >
                                <font-awesome-icon icon="trash" />
                            </button>
                        </div>
                        <div
                                v-if="submitted && $v.components.$each[componentIndex].$error"
                                class="form__error-section"
                        >
                            <span v-if="!$v.components.$each[componentIndex].required">Field is required</span>
                            <span
                                    v-if="!$v.components.$each[componentIndex].minLength"
                            >Field should have at least 3 characters</span>
                        </div>
                    </div>

                    <div class="form__column"></div>
                    <div class="form__column"></div>
                </div>

                <div class="button-container">
                    <button
                            class="button button--secondary"
                            @click="newComponent()"
                    >
                        Add new
                    </button>
                </div>
            </div>

            <div class="submit-button-container">
                <button
                    type="button"
                    class="button button--primary"
                    @click="onSubmit()"
                >
                    Save
                </button>
            </div>
        </div>
    </div>
</template>

<script>
import { required, minLength, url } from "vuelidate/lib/validators";

import messageService from "./../../../common/message/message-service";
import createProjectService from "./_services/create-project-service";

export default {
    name: "CreateProject",
    data() {
        return {
            projectId: "",
            environments: [""],
            components: [""],

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

            createProjectService
                .createProject(
                    this.projectId,
                    this.environments,
                    this.components
                )
                .then(() => {
                    messageService.success('The project was created.');
                    this.$router.push({ name: 'project', params: { projectId: this.projectId }});
                });
        },

        newEnvironment: function() {
            this.environments.push(null);
        },

        removeEnvironment: function(index) {
            if (this.environments.length < 2) return;

            this.environments.splice(index, 1);
        },

        newComponent: function() {
            this.components.push(null);
        },

        removeComponent: function(index) {
            if (this.components.length < 2) return;

            this.components.splice(index, 1);
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
        },
        components: {
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
