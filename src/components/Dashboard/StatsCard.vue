<template>
  <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 w-full mb-6">
    <div
      v-for="(stat, index) in stats"
      :key="index"
      class="bg-base-100 rounded-lg shadow-sm hover:shadow-md transition-all duration-300 p-4 border border-base-300"
    >
      <div class="flex items-center justify-between">
        <div class="flex-1">
          <p class="text-xs text-base-content/60 font-medium mb-1">
            {{ stat.label }}
          </p>
          <p 
            class="text-2xl font-bold"
            :class="getColorClass(index)"
          >
            {{ stat.value }}
          </p>
        </div>
        
        <!-- Icon (SVG) -->
        <div 
          v-if="stat.icon"
          class="w-10 h-10 rounded-lg flex items-center justify-center transition-colors duration-300"
          :class="getIconBgClass(index)"
        >
          <svg 
            xmlns="http://www.w3.org/2000/svg" 
            class="w-5 h-5" 
            :class="getColorClass(index)"
            fill="none" 
            viewBox="0 0 24 24" 
            stroke="currentColor"
            stroke-width="2"
          >
            <path 
              stroke-linecap="round" 
              stroke-linejoin="round" 
              :d="getIconPath(stat.icon)" 
            />
          </svg>
        </div>
      </div>
      
      <!-- Optional trend indicator -->
      <div v-if="stat.trend" class="mt-2 flex items-center gap-1.5">
        <span 
          class="text-xs font-semibold"
          :class="stat.trend > 0 ? 'text-success' : 'text-error'"
        >
          {{ stat.trend > 0 ? '↑' : '↓' }} {{ Math.abs(stat.trend) }}%
        </span>
        <span class="text-[10px] text-base-content/50">vs last period</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  stats: {
    type: Array,
    required: true,
    // Expected format: [{ label: 'Total', value: 20, icon: 'total', trend: 5 }]
  }
});

// Icon paths dictionary
const iconPaths = {
  total: 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4', // Office Building
  active: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z', // Check Circle
  featured: 'M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z', // Star
  available: 'M15 5v2a2 2 0 00-2 2 2 2 0 00-2-2V5a2 2 0 00-2-2H3a2 2 0 00-2 2v14a2 2 0 002 2h4v-2a2 2 0 002-2 2 2 0 002 2v2h4a2 2 0 002-2V5a2 2 0 00-2-2z', // Ticket
  // Default fallback
  default: 'M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z'
};

const getIconPath = (iconName) => {
  return iconPaths[iconName] || iconPaths.default;
};

// Color classes based on position (matching your images)
const getColorClass = (index) => {
  const colors = [
    'text-warning',      // Orange/Yellow for first stat
    'text-info',         // Blue for second stat
    'text-warning',      // Orange for third stat
    'text-success'       // Green for fourth stat
  ];
  return colors[index % colors.length];
};

// Background color for icons
const getIconBgClass = (index) => {
  const bgColors = [
    'bg-warning/10',
    'bg-info/10',
    'bg-warning/10',
    'bg-success/10'
  ];
  return bgColors[index % bgColors.length];
};
</script>

<style scoped>
/* Additional custom styles if needed */
</style>