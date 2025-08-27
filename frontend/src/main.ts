import { createApp } from 'vue';
import router from './router.ts';
import App from './App.vue';
import { createVuetify } from 'vuetify';
import * as components from 'vuetify/components';
import * as directives from 'vuetify/directives';
import Toast, { POSITION, type PluginOptions } from 'vue-toastification';
import L from 'leaflet'

import '@mdi/font/css/materialdesignicons.css';
import 'vuetify/dist/vuetify.min.css';
import 'vue-toastification/dist/index.css';
import 'leaflet/dist/leaflet.css';
import '@vuepic/vue-datepicker/dist/main.css';

const vuetify = createVuetify({
  components,
  directives,
  theme: {
    themes: {},
  },
  defaults: {
    VTooltip: {
      openDelay: 100,
    },
  },
});

const toastOptions: PluginOptions = {
  position: POSITION.BOTTOM_CENTER,
  newestOnTop: false,
  maxToasts: 5,
  timeout: 3000,
};

const app = createApp(App);

app.use(router);
app.use(vuetify);
app.use(Toast, toastOptions);

app.mount('#app');
