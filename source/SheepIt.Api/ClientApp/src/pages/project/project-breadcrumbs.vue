<template>
    <nav>
        <ol class="breadcrumb">
            <li class="breadcrumb-item">
                projects
            </li>
            <li class="breadcrumb-item">
                <router-link v-bind:to="{ name: 'project', params: { projectId: projectId }}">{{ projectId }}</router-link>
            </li>
            <slot></slot>
        </ol>
    </nav>
</template>

<script>
export default {
    name: 'project-breadcrumbs',
    props: ['projectId']
}
</script>