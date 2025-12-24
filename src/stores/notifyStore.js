import { defineStore } from 'pinia'

export const useNotifyStore = defineStore('notify', {
  state: () => ({
    toasts: [],
    timeoutIds: []
  }),
  actions: {
    show(message, type = 'info', timeout = 3500) {
      const id = Date.now() + Math.random()
      this.toasts.push({ id, message, type })
      if (timeout > 0) {
        const timeoutId = setTimeout(() => this.remove(id), timeout)
        this.timeoutIds.push(timeoutId)
      }
      return id
    },
    success(message, timeout = 3000) {
      return this.show(message, 'success', timeout)
    },
    error(message, timeout = 4500) {
      return this.show(message, 'error', timeout)
    },
    info(message, timeout = 3500) {
      return this.show(message, 'info', timeout)
    },
    remove(id) {
      this.toasts = this.toasts.filter(t => t.id !== id)
    }
  }
})
