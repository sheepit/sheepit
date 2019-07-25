<template>
    <div class="preloader-container">
        <img src="./../assets/preloader.gif" />
    </div>
</template>

<script>
export default {
    name: "Preloader"
}
</script>

<style lang="scss" scoped>
.preloader-container {
    width: 100%;
}

img {
    width: 6rem;
    display: block;
    margin-left: auto;
    margin-right: auto;
}
</style>