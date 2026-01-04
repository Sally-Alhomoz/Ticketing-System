import './assets/main.css'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { initAuth } from './components/Authentication Service/Authentication'
import 'bootstrap/dist/css/bootstrap.min.css'

initAuth()

const app = createApp(App)

app.use(router)
app.mount('#app')
