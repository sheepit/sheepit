<template>
    <div>
        <div class="sidemenu_placeholder"></div>

        <ul class="sidemenu">
            <li class="sidemenu__item">
                <router-link
                    class="sidemenu__link"
                    :to="{ name: 'default' }"
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
                <a class="sidemenu__link" href="#">
                    <div class="sidemenu__link--icon">
                        <font-awesome-icon icon="server" />
                    </div>
                    <div class="sidemenu__link--name">
                        Environments
                    </div>
                </a>
            </li>
            <li>
                <a class="sidemenu__link" href="#">
                    <div class="sidemenu__link--icon">
                        <font-awesome-icon icon="cubes" />
                    </div>
                    <div class="sidemenu__link--name">
                        Packages
                    </div>
                </a>
            </li>
            <li>
                <a class="sidemenu__link" href="#">
                    <div class="sidemenu__link--icon">
                        <font-awesome-icon icon="cogs" />
                    </div>
                    <div class="sidemenu__link--name">
                        Deployments
                    </div>
                </a>
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
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    padding-left: 0px;
    list-style: none;

    color: #898989;
    font-size: 16px;

    &__item {
        .router-link-active {
            background: #BABABA;
            color: #FFFFFF;
        }
    }

    &__link {
        display: flex;
        align-items: center;
        padding: 12px;
        line-height: 21px;
        color: #898989;
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
            background: #D7D7D7;
            color: #FFFFFF;
        }
    }
}
</style>
