import './assets/main.css'
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'

// Font Awesome & Cairo Font
import '@fortawesome/fontawesome-free/css/all.css'
import '@fontsource/cairo'

// إنشاء التطبيق
const app = createApp(App)

// Pinia للحالة العامة
const pinia = createPinia()
app.use(pinia)

// Router
app.use(router)

// Mount التطبيق
app.mount('#app')

// تهيئة المصادقة بعد الـ mount
import { useAuthStore } from '@/stores/authStore'
const authStore = useAuthStore()
authStore.initAuth()