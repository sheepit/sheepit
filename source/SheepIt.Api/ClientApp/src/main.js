import Vue from 'vue'
import VueRouter from 'vue-router'
import Vuelidate from 'vuelidate'

import App from './App.vue'

import { library } from '@fortawesome/fontawesome-svg-core'
import { faUserSecret } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import jQuery from 'jquery'

window.jQuery = jQuery
window.$ = jQuery

library.add(faUserSecret)
Vue.component('font-awesome-icon', FontAwesomeIcon)

Vue.config.productionTip = false;

Vue.use(VueRouter);
Vue.use(Vuelidate);

import DeploymentBadge from "./components/deployment-badge.vue";
import DeploymentStatusBadge from "./components/deployment-status-badge.vue";
import ExpandingList from "./components/expanding-list.vue";
import HumanizedDate from "./components/humanized-date.vue";
import Preloader from "./components/preloader.vue";
import PackageBadge from "./components/package-badge.vue";
import Tooltip from "./components/tooltip.vue";

Vue.component('deployment-badge', DeploymentBadge);
Vue.component('deployment-status-badge', DeploymentStatusBadge);
Vue.component('expanding-list', ExpandingList);
Vue.component('humanized-date', HumanizedDate);
Vue.component('preloader', Preloader);
Vue.component('package-badge', PackageBadge);
Vue.component('tooltip', Tooltip);

new Vue({
    render: h => h(App)
}).$mount('#app');