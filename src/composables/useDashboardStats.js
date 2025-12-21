// composables/useDashboardStats.js
import { ref, computed } from 'vue';

export const useDashboardStats = (category) => {
  const data = ref(null);
  const loading = ref(false);
  const error = ref(null);

  const fetchData = async () => {
    loading.value = true;
    error.value = null;
    
    try {
      const response = await fetch('/db.json');
      if (!response.ok) throw new Error('Failed to fetch data');
      
      const jsonData = await response.json();
      data.value = jsonData[category];
    } catch (err) {
      error.value = err.message;
      console.error('Error fetching dashboard stats:', err);
    } finally {
      loading.value = false;
    }
  };

  // Define stats configurations for each category
  const statsConfig = {
    attractions: [
      { key: 'total_inventory', label: 'Total Inventory', icon: '🎫' },
      { key: 'active_now', label: 'Active Now', icon: '✓' },
      { key: 'tickets_sold', label: 'Tickets Sold', icon: '💰' },
      { key: 'low_capacity', label: 'Low Capacity', icon: '⚠️' }
    ],
    bookings: [
      { key: 'total_booking', label: 'Total Booking', icon: '📋' },
      { key: 'pending_action', label: 'Pending Action', icon: '⏳' },
      { key: 'revenue', label: 'Revenue', icon: '💵' },
      { key: 'resolved', label: 'Resolved', icon: '✅' }
    ],
    cars: [
      { key: 'total_fleet', label: 'Total Fleet', icon: '🚗' },
      { key: 'available_now', label: 'Available Now', icon: '✓' },
      { key: 'booked_today', label: 'Booked Today', icon: '📅' },
      { key: 'in_maintenance', label: 'In Maintenance', icon: '🔧' }
    ],
    hotels: [
      { key: 'total_hotels', label: 'Total Hotels', icon: '🏨' },
      { key: 'active_listings', label: 'Active Listings', icon: '✓' },
      { key: 'pending_review', label: 'Pending Review', icon: '📝' },
      { key: 'bookings_today', label: 'Bookings Today', icon: '📊' }
    ],
    trips: [
      { key: 'total_trips', label: 'Total Trips', icon: '✈️' },
      { key: 'active_now', label: 'Active Now', icon: '🌍' },
      { key: 'featured', label: 'Featured', icon: '⭐' },
      { key: 'drafts', label: 'Drafts', icon: '📝' }
    ]
  };

  const stats = computed(() => {
    if (!data.value || !statsConfig[category]) return [];
    
    return statsConfig[category].map(config => ({
      label: config.label,
      value: data.value[config.key] || 0,
      icon: config.icon
    }));
  });

  return {
    data,
    loading,
    error,
    stats,
    fetchData
  };
};


// Example usage in any component:
/*
<script setup>
import { onMounted } from 'vue';
import StatsCard from '@/components/StatsCard.vue';
import { useDashboardStats } from '@/composables/useDashboardStats';

const { stats, loading, error, fetchData } = useDashboardStats('attractions');

onMounted(() => {
  fetchData();
});
</script>

<template>
  <div class="page-container py-6">
    <h1 class="text-2xl font-bold mb-6">Dashboard</h1>
    
    <div v-if="loading" class="text-center py-10">
      <span class="loading loading-spinner loading-lg"></span>
    </div>
    
    <div v-else-if="error" class="alert alert-error">
      <span>{{ error }}</span>
    </div>
    
    <StatsCard v-else :stats="stats" />
  </div>
</template>
*/