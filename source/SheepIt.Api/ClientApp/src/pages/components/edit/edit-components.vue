<template>
    <div v-if="components">
        <div class="view__title">
            Edit components
        </div>

        <div class="form">
            <div class="form__section">
                <div class="form__title">
                    Components
                </div>

                <div class="form__row">
                    <div class="form__column">
                        <label class="form__label">List of components</label>
                    </div>                   
                </div>

                <div class="form__row">
                    <div class="form__column">
                        <draggable
                            v-model="components"
                            draggable=".dragMe"
                        >
                            <div
                                v-for="(component, componentIndex) in components"
                                :key="componentIndex"
                                :class="{'dragMe': !components[componentIndex].edition}"
                            >
                                <div
                                    v-if="components[componentIndex].edition"
                                    class="input-group mb-3"
                                >
                                    <div class="form__inline">
                                        <input
                                            v-model="components[componentIndex].name"
                                            type="text"
                                            class="form__control form__inline__control"
                                            :class="{ 'is-invalid': submitted && $v.components.$each[componentIndex].name.$error }"
                                        >
                                        <button 
                                            class="button button--inline button--secondary form__inline__control"
                                            type="button"
                                            @click="disableEnvironmentEdition(componentIndex)"
                                        >
                                            <font-awesome-icon icon="check" />
                                        </button>
                                        <button
                                            class="button button--inline button--secondary"
                                            type="button"
                                            :disabled="component.persisted || components.length < 2"
                                            @click="removeComponent(componentIndex)"
                                        >
                                            <font-awesome-icon icon="trash" />
                                        </button>
                                    </div>

                                    <div
                                        v-if="submitted && $v.components.$each[componentIndex].name.$error"
                                        class="invalid-feedback"
                                    >
                                        <span v-if="!$v.components.$each[componentIndex].name.required">Field is required</span>
                                        <span v-if="!$v.components.$each[componentIndex].name.minLength">Field should have at least 3 characters</span>
                                    </div>
                                </div>

                                <div
                                    v-else
                                    class="input-group mb-3 form__draggable"
                                >
                                    <div class="form__inline">
                                        <font-awesome-icon icon="bars" size="lg" class="drag-icon" />

                                        <input
                                            v-model="components[componentIndex].name"
                                            type="text"
                                            class="form__control form__draggable form__inline__control"
                                            :class="{ 'is-invalid': submitted && $v.components.$each[componentIndex].name.$error }"
                                            disabled="disabled"
                                        >
                                        <button 
                                            class="button button--inline button--secondary form__inline__control"
                                            type="button"
                                            @click="enableEnvironmentEdition(componentIndex)"
                                        >
                                            <font-awesome-icon icon="pen" />
                                        </button>
                                        <button 
                                            class="button button--inline button--secondary"
                                            type="button"
                                            :disabled="components.length < 2 || component.persisted"
                                            @click="removeComponent(componentIndex)"
                                        >
                                            <font-awesome-icon icon="trash" />
                                        </button>
                                    </div>

                                    <div
                                        v-if="submitted && $v.components.$each[componentIndex].name.$error"
                                        class="invalid-feedback"
                                    >
                                        <span v-if="!$v.components.$each[componentIndex].name.required">Field is required</span>
                                        <span v-if="!$v.components.$each[componentIndex].name.minLength">Field should have at least 3 characters</span>
                                    </div>
                                </div>
                            </div>
                        </draggable>
                    </div>
                
                    <div class="form__column"></div>
                </div>

                <div class="button-container">
                    <button
                        class="button button--secondary"
                        @click="addNewComponent()"
                    >
                        Add new
                    </button>
                </div>
            </div>

            <div class="submit-button-container">
                <router-link
                    class="button button--secondary"
                    :to="{ name: 'components-list' }"
                    tag="button"
                    type="button"
                >
                    Cancel
                </router-link>

                <button
                    type="button"
                    class="button button--primary"
                    @click="save()"
                >
                    Save
                </button>
            </div>
        </div>
    </div>
</template>

<script>
import { required, minLength } from 'vuelidate/lib/validators'

import httpService from "./../../../common/http/http-service.js";
import draggable from 'vuedraggable';
import messageService from "./../../../common/message/message-service";

export default {
    name: 'EditComponents',

    components: {
        draggable
    },

    data() {
        return {
            project: null,
            components: null,

            submitted: false
        }
    },

    computed: {
        projectId() {
            return this.$route.params.projectId
        }
    },

    mounted() {
        this.getComponents();
    },

    methods: {
        save: function () {
            this.submitted = true;

            this.$v.$touch();
            
            if (this.$v.$invalid) {
                return;
            }

            const components = this.components.map((item, index) => {
                return {
                    id: item.id === 0 ? null : item.id, // todo: null should be there in the first place
                    name: item.name
                };
            });

            updateComponents(this.projectId, components)
                .then(() => {
                    this.markComponentsAsPersisted();
                    messageService.success('Components were updated.');
                    this.$router.push({ name: 'components-list' });
                });
        },

        markComponentsAsPersisted() {
            if (this.components)
            {
                // todo: y mapping instead of iterating?
                this.components = this.components.map(item => {
                    item.persisted = true;
                    return item;
                });

                this.$forceUpdate();
            }
        },

        addNewComponent() {
            this.components.push({
                persisted: false,
                edition: true
            });

            this.enableEnvironmentEdition(this.components.length - 1);
        },

        enableEnvironmentEdition(index) {
            if (this.components[index]) {
                this.$set(this.components[index], 'edition', true);
            }
            
            this.$forceUpdate();
        },

        disableEnvironmentEdition(index) {
            if (this.components[index]) {
                this.$set(this.components[index], 'edition', false);
            }
            
            this.$forceUpdate();
        },

        removeComponent: function(index) {
            if (this.components.length < 2) {
                return;
            }

            this.components.splice(index, 1);
        },

        getComponents() {
            httpService
                .post('api/project/components/get-for-update', {
                    projectId: this.projectId
                })
                .then(response => {
                    this.components = response.components.map(component => ({
                        ...component,
                        persisted: true,
                        edition: false
                    }));
                });
        }
    },
    validations: {
        components: {
            required,
            minLength: minLength(1),
            $each: {
                name: {
                    required,
                    minLength: minLength(3)
                }
            }
        }
    }
}

function updateComponents(projectId, components) {
    return httpService.post('api/project/components/update', {
        projectId: projectId,
        components: components
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

.add-component-button {
    height: 51px;
    width: 255px;
}

.component-card-header {
    min-height: 49px;
    height: 100%;
    width: 100%;
}

.disabled {
    background-color: $disabled-gray;
}

.drag-icon {
    margin-right: 10px;
    font-size: 16px;
    color: $font-color-light;
}
</style>
