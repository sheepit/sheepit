<template>
    <div class="tile">
        <div class="content">
            <span v-if="!focused">{{ title }}</span>
            <input v-if="focused" v-model="title" type="text" v-focus="" />
        </div>
        <span v-if="!focused" class="icon icon-pencil" @click="focus()"></span>
    </div>
</template>

<script>
    module.exports = {
        name: 'editable-title',

        props: [
            'title'
        ],

        data() {
            return {
                focused: false,
                displayName: 'test'
            }
        },

        methods: {
            focus() {
                this.focused = true;
            }
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

<style scoped>
.tile {
    display: flex;
}

.content {
    flex: 1;
}

.icon {
    visibility: hidden;
    border-radius: 4px;
    cursor: pointer;
}

.tile:hover .icon {
    visibility: visible;
    color: #888888;
}
    
.icon:hover {
    background-color: #d3d9df;
}
</style>