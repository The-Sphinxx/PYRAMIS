<template>
  <div class="toast toast-end toast-top z-50">
    <div 
      v-for="toast in toasts" 
      :key="toast.id" 
      class="alert shadow-lg transition-all duration-300"
      :class="{
        'alert-success': toast.type === 'success',
        'alert-error': toast.type === 'error',
        'alert-info': toast.type === 'info',
        'alert-warning': toast.type === 'warning'
      }"
    >
      <div class="flex items-center gap-2">
        <svg v-if="toast.type === 'success'" xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
        <svg v-if="toast.type === 'error'" xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
        <svg v-if="toast.type === 'info'" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="stroke-current shrink-0 w-6 h-6"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
        <span>{{ toast.message }}</span>
      </div>
      <button class="btn btn-sm btn-ghost" @click="removeToast(toast.id)">✕</button>
    </div>
  </div>
</template>

<script setup>
import { useToast } from '@/composables/useToast';

const { toasts, removeToast } = useToast();
</script>

<style scoped>
.toast {
  position: fixed;
  top: 1rem;
  right: 1rem;
  max-width: 24rem;
}
</style>
