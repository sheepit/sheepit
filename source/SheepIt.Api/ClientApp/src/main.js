import Vue from 'vue'
import VueRouter from 'vue-router'
import App from './App.vue'

import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import jQuery from 'jquery'

window.jQuery = jQuery
window.$ = jQuery

Vue.config.productionTip = false;

Vue.use(VueRouter);

import DeploymentBadge from "./components/deployment-badge.vue";
import DeploymentStatusBadge from "./components/deployment-status-badge.vue";
import ExpandingList from "./components/expanding-list.vue";
import HumanizedDate from "./components/humanized-date.vue";
import ProjectBreadcrumbs from "./components/project-breadcrumbs.vue";
import ReleaseBadge from "./components/release-badge.vue";
import Tooltip from "./components/tooltip.vue";

Vue.component('deployment-badge', DeploymentBadge);
Vue.component('deployment-status-badge', DeploymentStatusBadge);
Vue.component('expanding-list', ExpandingList);
Vue.component('humanized-date', HumanizedDate);
Vue.component('project-breadcrumbs', ProjectBreadcrumbs);
Vue.component('release-badge', ReleaseBadge);
Vue.component('tooltip', Tooltip);

new Vue({
    render: h => h(App)
}).$mount('#app');