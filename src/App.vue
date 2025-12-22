<template>
  <div>
    <router-view />
    <ToastContainer />
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useTheme } from '@/composables/useTheme';
import ToastContainer from '@/components/Common/ToastContainer.vue'
import { useNotifyStore } from '@/stores/notifyStore'

const { initializeTheme } = useTheme();
const notify = useNotifyStore()

// Initialize theme on app mount and handle verify-email link
onMounted(async () => {
  initializeTheme();
  try {
    const params = new URLSearchParams(window.location.search);
    const email = params.get('verifyEmail');
    const token = params.get('token');
    if (email && token) {
      const { authApi } = await import('@/Services/api');
      await authApi.verifyEmailByLink(email, token);
      notify.success('Your email has been verified successfully.')
      const url = new URL(window.location.href);
      url.searchParams.delete('verifyEmail');
      url.searchParams.delete('token');
      window.history.replaceState(null, '', url.pathname + (url.search ? '?' + url.searchParams.toString() : '') + url.hash);
    }
  } catch (e) {
    console.warn('Email verification via link failed', e);
    notify.error('Email verification failed. Please try again or request a new link.')
    const url = new URL(window.location.href);
    url.searchParams.delete('verifyEmail');
    url.searchParams.delete('token');
    window.history.replaceState(null, '', url.pathname + (url.search ? '?' + url.searchParams.toString() : '') + url.hash);
  }
});
</script>
