<template>
  <div 
    :class="[
      'p-6 flex items-center justify-between hover:bg-base-200 transition-colors flex-wrap gap-4',
      !isLast ? 'border-b border-base-300' : ''
    ]"
  >
    <div class="flex items-center gap-4 flex-1 min-w-0">
      <div class="w-14 h-14 bg-primary/10 rounded-lg flex items-center justify-center text-2xl flex-shrink-0">
        {{ activity.icon }}
      </div>
      <div class="min-w-0">
        <h3 class="font-bold text-base-content mb-1 truncate">
          {{ activity.title }}
        </h3>
        <p class="text-sm text-base-content/60">
          {{ activity.date }} • {{ activity.type }}
        </p>
      </div>
    </div>

    <div class="flex items-center gap-3 flex-shrink-0">
      <div class="flex items-center gap-2 text-base-content/60 text-sm">
        <i class="fas fa-check-circle text-success"></i>
        <span>{{ activity.status }}</span>
      </div>
      <button 
        @click="$emit('view', activity.id)"
        class="btn btn-outline btn-sm"
      >
        عرض
      </button>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ActivityItem',
  props: {
    activity: {
      type: Object,
      required: true
    },
    isLast: {
      type: Boolean,
      default: false
    }
  },
  emits: ['view']
}
</script>

<style scoped>
.rounded-lg {
  border-radius: 8px;
}

.btn {
  border-radius: 6px;
}
</style>