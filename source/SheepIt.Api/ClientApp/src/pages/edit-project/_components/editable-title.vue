<template>
    <div class="tile">
        <div class="content">
            <span v-if="!focused">{{ localTitle }}</span>
            <input
                v-if="focused"
                v-focus="" 
                :value="localTitle"
                type="text"
                @blur="onBlur"
                @keyup.enter="onBlur($event)"
            >
        </div>
        <span
            v-if="!focused"
            class="icon icon-pencil"
            @click="onFocus()"
        />
    </div>
</template>


<script>
export default {
    name: 'EditableTitle',

    directives: {
        focus: {
            inserted(el) {
                el.focus();
            }
        }
    },

    props: [
        'title'
    ],

    data() {
        return {
            focused: false,
            localTitle: this.title
        }
    },

    watch: {
        title: function() {
            this.localTitle = this.title;
        }
    },

    methods: {
        onFocus() {
            this.focused = true;
        },

        onBlur($event) {
            this.localTitle = $event.target.value;
            this.focused = false;
            this.$emit('blur', this.localTitle)
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