const path = require("path");

module.exports = {
    outputDir: path.resolve(__dirname, "../wwwroot"),
    devServer: {
        port: 8089,
        watchOptions: {
            poll: true
        }
    },
    css: {
        loaderOptions: {
            sass: {
                data: `@import "@/scss/styles.scss";`
            }
        }
    }
};