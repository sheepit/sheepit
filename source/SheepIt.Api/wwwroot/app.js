setTimeout(() => {
    
    const Default = {
        template: '<h4>Welcome to Sheep It</h4>'
    }
    
    const CreateProject = {
        template: '<div>creating project!</div>' 
    }
    
    const Project = { 
        template: '<div>{{ $route.params.projectId }}</div>' 
    }

    const router = new VueRouter({
        routes: [
            {
                path: '/',
                name: 'default',
                component: Default
            },
            {
                path: '/create-project',
                name: 'create-project',
                component: CreateProject
            },
            {
                path: '/project/:projectId',
                name: 'project',
                component: Project
            }
        ]
    })

    const app = new Vue({
        el: '#app',
        router
    })

}, 0)