import Vue from 'vue'
import VueRouter from 'vue-router'
import App from './App.vue'

Vue.config.productionTip = false;

Vue.use(VueRouter);

new Vue({
  render: h => h(App)
}).$mount('#app');

// function postData(url, request) {
//     const fetchSettings = {
//         method: "POST",
//         mode: "cors",
//         cache: "no-cache",
//         credentials: "same-origin",
//         headers: {
//             "Content-Type": "application/json; charset=utf-8",
//         },
//         referrer: "no-referrer",
//         body: JSON.stringify(request),
//     }

//     return fetch(url, fetchSettings)
// }
