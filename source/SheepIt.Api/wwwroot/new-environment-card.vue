<template>
    <div>
        <div class="col-md-3">
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
    </div>

</template>


<script>

module.exports = {
        name: 'new-environment-card',

        data() {
            return {
                newEnvironmentDisplayName: ''
            }
        },

        methods: {
            onBlur($event) {
                this.newEnvironmentDisplayName = $event.target.value;
                this.$emit('blur', this.newEnvironmentDisplayName)
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