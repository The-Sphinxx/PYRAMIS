<template>
  <div class="fixed inset-0 pointer-events-none z-[1000]">
    <div class="absolute top-4 right-4 flex flex-col gap-3 w-[min(92vw,380px)]">
      <transition-group name="toast-fade" tag="div">
        <div
          v-for="t in toasts"
          :key="t.id"
          class="pointer-events-auto shadow-lg rounded-md px-4 py-3 text-sm flex items-start gap-3 border"
          :class="toastClass(t.type)"
        >
          <span class="mt-0.5 text-base">{{ icon(t.type) }}</span>
          <div class="flex-1">{{ t.message }}</div>
          <button
            class="ml-2 opacity-70 hover:opacity-100"
            @click="remove(t.id)"
            aria-label="Close"
          >
            ✕
          </button>
        </div>
      </transition-group>
    </div>
  </div>
  
</template>

<script setup>
import { storeToRefs } from 'pinia'
import { useNotifyStore } from '@/stores/notifyStore'

const notify = useNotifyStore()
const { toasts } = storeToRefs(notify)

const remove = (id) => notify.remove(id)

const toastClass = (type) => {
  switch (type) {
    case 'success':
      return 'bg-green-50 text-green-800 border-green-200'
    case 'error':
      return 'bg-red-50 text-red-800 border-red-200'
    default:
      return 'bg-slate-50 text-slate-800 border-slate-200'
  }
}

const icon = (type) => {
  switch (type) {
    case 'success':
      return '✅'
    case 'error':
      return '⚠️'
    default:
      return 'ℹ️'
  }
}
</script>

<style>
.toast-fade-enter-active, .toast-fade-leave-active { transition: all .2s ease; }
.toast-fade-enter-from { opacity: 0; transform: translateY(-6px); }
.toast-fade-leave-to { opacity: 0; transform: translateY(-6px); }
</style>
