import './assets/main.css'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { initAuth } from './components/Authentication Service/Authentication'
import 'bootstrap/dist/css/bootstrap.min.css'
import { i18n } from './i18n'

initAuth()

const savedLang = localStorage.getItem('user_lang') || 'en';
i18n.global.locale.value = savedLang;
document.documentElement.dir = savedLang === 'ar' ? 'rtl' : 'ltr';
document.documentElement.lang = savedLang;

const app = createApp(App)

app.use(i18n)
app.use(router)
app.mount('#app')
