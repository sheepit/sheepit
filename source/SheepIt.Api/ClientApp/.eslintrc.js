module.exports = {
    extends: [
        'plugin:vue/recommended'
    ],
    rules: {
        "vue/require-prop-types": ["off"], // todo: fix these issues
        "vue/require-v-for-key": ["off"], // todo: fix these issues
        "vue/html-indent": ["warn", 4],
        "indent": ["warn", 4]
    },
    env: {
        browser: true,
        // EDIT
        jquery: true
    }
}