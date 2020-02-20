<template>
    <div>
        <div class="sidemenu_placeholder"></div>

        <ul class="sidemenu">
            <li class="sidemenu__item">
                <router-link
                    class="sidemenu__link"
                    :to="{ name: 'default' }"
                    exact
                >
                    <div class="sidemenu__link--icon">
                        <font-awesome-icon icon="code" />
                    </div>
                    <div class="sidemenu__link--name">
                        Components
                    </div>
                </router-link>
            </li>
            <li>
                <router-link
                    class="sidemenu__link"
                    :to="{ name: 'create-project' }"
                >
                    <div class="sidemenu__link--icon">
                        <font-awesome-icon icon="server" />
                    </div>
                    <div class="sidemenu__link--name">
                        Environments
                    </div>
                </router-link>
            </li>
            <li>
                <router-link
                    class="sidemenu__link"
                    :to="{ name: 'create-project' }"
                >
                    <div class="sidemenu__link--icon">
                        <font-awesome-icon icon="cubes" />
                    </div>
                    <div class="sidemenu__link--name">
                        Packages
                    </div>
                </router-link>
            </li>
            <li>
                <router-link
                    class="sidemenu__link"
                    :to="{ name: 'create-project' }"
                >
                    <div class="sidemenu__link--icon">
                        <font-awesome-icon icon="cogs" />
                    </div>
                    <div class="sidemenu__link--name">
                        Deployments
                    </div>
                </router-link>
            </li>
        </ul>
    </div>
</template>

<script>
export default {
    name: "SideMenu"
}   
</script>

<style lang="scss" scoped>
.sidemenu_placeholder {
    height: 30px;
}

.sidemenu {
    padding-left: 0px;
    list-style: none;

    color: $font-color;
    font-size: 16px;

    &__item {
        .router-link-active {
            background: $gray-active;
            color: $white;
        }
    }

    &__link {
        display: flex;
        align-items: center;
        padding: 12px;
        line-height: 21px;
        color: $font-color;
        text-decoration: none;

        &--icon {
            display: flex;
            align-items: center;
            margin-left: 9px;
            margin-right: 9px;

            svg {
                height: 16px;
                width: 16px;
            }
        }

        &--name {
            flex: 1;
        }

        &:hover {
            background: $gray-hover;
            color: $white;
        }
    }
}
</style>
