import Vue from 'vue'
import VueRouter from 'vue-router'
import Vuelidate from 'vuelidate'

import Clipboard from 'v-clipboard';

import App from './App.vue'

import { library } from '@fortawesome/fontawesome-svg-core'
import {
    faBars,
    faBox,
    faCheck,
    faCheckCircle,
    faCode,
    faCogs,
    faCubes,
    faDog,
    faPen,
    faServer,
    faTrash,
    faUserCircle
} from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

library.add(faBars)
library.add(faBox)
library.add(faCheck)
library.add(faCheckCircle)
library.add(faCode)
library.add(faCogs)
library.add(faCubes)
library.add(faDog)
library.add(faPen)
library.add(faServer)
library.add(faTrash)
library.add(faUserCircle)

Vue.component('font-awesome-icon', FontAwesomeIcon)

Vue.config.productionTip = false;

Vue.use(VueRouter);
Vue.use(Vuelidate);

Vue.use(Clipboard);

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