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
                        {{ breadcrumb.name }}
                    </router-link>
                    <span v-else>
                        {{ breadcrumb.name }}
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
        updateBreadcrumbsBasedOnRouting() {
            if(!this.$route.meta || !this.$route.meta.breadcrumbs) {
                this.breadcrumbs = null;
                return;
            }

            this.breadcrumbs = this.$route.meta.breadcrumbs;

            this.breadcrumbs = this.breadcrumbs.map(item => {
                if(item.name && item.name.startsWith(":")){
                    let temp = item.name.replace(":", "");
                    item.name = this.$route.params[temp];
                }

                return item;
            });
        }
    }
}
</script>