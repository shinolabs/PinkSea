import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import { createPersistedState } from 'pinia-plugin-persistedstate'
import i18next from 'i18next';
import I18NextVue from 'i18next-vue';
import i18n from '@/intl/i18n'

await i18next.init({
  resources: i18n,
  fallbackLng: "en"
});

const app = createApp(App)
const pinia = createPinia()

pinia.use(createPersistedState())

app.use(pinia)
app.use(router)
app.use(I18NextVue, { i18next });
app.mount('#app')
