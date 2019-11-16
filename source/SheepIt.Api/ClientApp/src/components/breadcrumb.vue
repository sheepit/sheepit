<template>
    <div>
        <nav v-if="breadcrumbs && breadcrumbs.length > 0">
            <ol class="breadcrumb">
                <li
                    v-for="(breadcrumb, index) in breadcrumbs"
                    :key="index"
                    class="breadcrumb-item"
                >
                    <router-link
                        v-if="breadcrumb.link" 
                        :to="{ name: breadcrumb.link }"
                    >
                        {{ breadcrumb.text }}
                    </router-link>
                    <span v-else>
                        {{ breadcrumb.text }}
                    </span>
                </li>
            </ol>
        </nav>
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