<template>
    <button
        v-if="!addingNewEnvironment"
        type="button"
        class="btn btn-primary"
        @click="onNewEnvironemnt()"
    >
        Add new
    </button>
    <div
        v-else
        class="col-md-3"
    >
        <div class="card">
            <div class="card-header">
                <input 
                    v-focus=""
                    :value="newEnvironmentDisplayName"
                    type="text"
                    @blur="onBlur"
                    @keyup.enter="onBlur($event)"
                >
            </div>
        </div>
    </div>
</template>


<script>
export default {
    name: 'NewEnvironmentCard',

    directives: {
        focus: {
            inserted(el) {
                el.focus();
            }
        }
    },

    data() {
        return {
            newEnvironmentDisplayName: '',
            addingNewEnvironment: false
        }
    },

    methods: {
        onNewEnvironemnt() {
            this.addingNewEnvironment = true;
        },

        onBlur($event) {
            this.newEnvironmentDisplayName = $event.target.value;
            this.$emit('blur', this.newEnvironmentDisplayName);
            this.addingNewEnvironment = false;
        },
    }
};
</script>