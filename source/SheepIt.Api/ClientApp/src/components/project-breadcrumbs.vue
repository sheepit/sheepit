<template>
    <nav>
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                projects
            </li>
            <li class="breadcrumb-item">
                <router-link :to="{ name: 'project', params: { projectId: projectId }}">
                    {{ projectId }}
                </router-link>
            </li>
            <slot />
        </ol>
    </nav>
</template>

<script>
export default {
    name: 'ProjectBreadcrumbs',
    props: ['projectId']
}
</script>