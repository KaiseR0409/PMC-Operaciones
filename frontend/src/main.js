import { createApp } from 'vue'
import App from './App.vue'
import './assets/main.css'

import { Toaster } from 'vue-sonner'
import 'vue-sonner/style.css'

const app = createApp(App)

app.component('Toaster', Toaster)
app.mount('#app')
