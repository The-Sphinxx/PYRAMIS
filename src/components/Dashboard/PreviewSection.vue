<template>
  <div class="bg-white rounded-lg shadow-md overflow-hidden">
    <!-- Header with Title and View All Link -->
    <div class="flex items-center justify-between p-4 border-b bg-gray-50">
      <div class="flex items-center gap-3">
        <span class="text-2xl">{{ icon }}</span>
        <h2 class="text-xl font-semibold text-gray-800">{{ title }}</h2>
      </div>
      <router-link 
        :to="route" 
        class="flex items-center gap-2 text-blue-600 hover:text-blue-800 font-medium transition-colors"
      >
        View All
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
        </svg>
      </router-link>
    </div>

    <!-- Content Area -->
    <div class="p-4">
      <!-- Slot for custom content -->
      <slot></slot>

      <!-- Summary Stats -->
      <div v-if="summary && !isChartSection" class="mt-4 pt-4 border-t flex flex-wrap gap-4 text-sm text-gray-600">
        <span v-if="summary.total">
          Total: <strong class="text-gray-900">{{ summary.total }}</strong>
        </span>
        <span v-if="summary.active !== undefined">
          Active: <strong class="text-green-600">{{ summary.active }}</strong>
        </span>
        <span v-if="summary.featured !== undefined">
          Featured: <strong class="text-blue-600">{{ summary.featured }}</strong>
        </span>
        <span v-if="summary.available !== undefined">
          Available: <strong class="text-green-600">{{ summary.available }}</strong>
        </span>
        <span v-if="summary.rented !== undefined">
          Rented: <strong class="text-orange-600">{{ summary.rented }}</strong>
        </span>
        <span v-if="summary.upcoming !== undefined">
          Upcoming: <strong class="text-blue-600">{{ summary.upcoming }}</strong>
        </span>
        <span v-if="summary.completed !== undefined">
          Completed: <strong class="text-gray-600">{{ summary.completed }}</strong>
        </span>
        <span v-if="summary.confirmed !== undefined">
          Confirmed: <strong class="text-green-600">{{ summary.confirmed }}</strong>
        </span>
        <span v-if="summary.pending !== undefined">
          Pending: <strong class="text-yellow-600">{{ summary.pending }}</strong>
        </span>
        <span v-if="summary.verified !== undefined">
          Verified: <strong class="text-green-600">{{ summary.verified }}</strong>
        </span>
        <span v-if="summary.admins !== undefined">
          Admins: <strong class="text-purple-600">{{ summary.admins }}</strong>
        </span>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  icon: {
    type: String,
    required: true
  },
  route: {
    type: String,
    required: true
  },
  summary: {
    type: Object,
    default: null
  },
  isChartSection: {
    type: Boolean,
    default: false
  }
});
</script>
