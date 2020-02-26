<template>
    <div class="breadcrumbs" v-if="breadcrumbs && breadcrumbs.length > 0">
        <ol class="breadcrumbs__main">
            <li
                v-for="(breadcrumb, index) in breadcrumbs"
                :key="index"
                class="breadcrumbs__item"
            >
                <router-link
                    class="breadcrumbs__link"
                    v-if="breadcrumb.link" 
                    :to="{ name: breadcrumb.link }"
                >
                    {{ breadcrumb.text }}
                </router-link>
                <span v-else class="breadcrumbs__link">
                    {{ breadcrumb.text }}
                </span>
            </li>
        </ol>
    </div>
</template>

<script>
export default {
    name: 'Breadcrumb',
    data() {
        return {
            breadcrumbs: null
        }
    },
    watch: { 
        '$route' () { 
            this.updateBreadcrumbsBasedOnRouting() 
        } 
    },
    mounted() {
        this.updateBreadcrumbsBasedOnRouting()
    },
    methods: {
        updateBreadcrumbsBasedOnRouting: function () {
            this.breadcrumbs = this.getBreadcrumbs();
        },
        
        getBreadcrumbs() {
            const routerBreadcrumbs = this.$route.meta && this.$route.meta.breadcrumbs
                ? this.$route.meta.breadcrumbs
                : null;

            if (!routerBreadcrumbs) {
                return null;
            }

            return routerBreadcrumbs.map(routerBreadcrumb => ({
                text: this.getBreadcrumbText(routerBreadcrumb.name),
                link: routerBreadcrumb.link
            }));
        },

        getBreadcrumbText(breadcrumbName) {
            if (breadcrumbName && breadcrumbName.startsWith(":")) {
                const parameterName = breadcrumbName.replace(":", "");
                return this.$route.params[parameterName];
            }
            
            return breadcrumbName;
        }
    }
}
</script>


<style lang="scss" scoped>
.breadcrumbs {
    height: 33px;
    background: $font-color-light;
    font-size: 14px;
    display: flex;

    &__main {
        display: flex;
        align-items: center;
        padding: 0;
        list-style: none;
        margin: 0;
    }

    &__item {
        margin-right: 5px;
        color: $font-color;

        a {
            &:hover {
                background-color: $gray-hover;
                border-radius: 0.25rem;
            }
        }

        &:before {
            content: '/'
        }

        &:first-child {
            margin-left: 16px;

            &:before {
                content: none;
            }
        }
    }

    &__link {
        color: $font-color;
        text-transform: uppercase;
        text-decoration: none;
    }
}
</style>