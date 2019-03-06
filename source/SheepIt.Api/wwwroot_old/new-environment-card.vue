<template>

    <button v-if="!addingNewEnvironment" type="button" v-on:click="onNewEnvironemnt()" class="btn btn-primary">Add new</button>
    <div v-else class="col-md-3">
        <div class="card">
            <div class="card-header">
                <input 
                    :value="newEnvironmentDisplayName"
                    @blur="onBlur"
                    @keyup.enter="onBlur($event)"
                    type="text"
                    v-focus=""/>
            </div>
        </div>
    </div>

</template>


<script>

module.exports = {
        name: 'new-environment-card',

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
        },

        directives: {
            focus: {
                inserted(el) {
                    el.focus();
                }
            }
        }
    };

</script>