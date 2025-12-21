<template>
  <div class="p-6 space-y-6">
    <!-- Stats Grid -->
    <StatsCard :stats="stats" />

    <!-- Hotels Data Table -->
    <DataTable
      title="Hotels"
      :columns="columns"
      :data="hotels"
      :show-filter="true"
      add-button-text="Add New Hotel"
      :show-actions="{ edit: true, delete: true, view: true }"
      empty-message="No hotels available"
      :per-page="8"
      resource="hotels"
      :loading="loading"
      @add="handleAdd"
      @edit="handleEdit"
      @delete="handleDelete"
      @view="handleView"
      @toggle="handleToggle"
      @status-click="handleStatusClick"
      @status-change="handleStatusChange"
      @filter="handleFilter"
      v-model:data="hotels"
    />

    <!-- Edit/Add Modal would go here -->
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import StatsCard from '@/components/Dashboard/StatsCard.vue';
import DataTable from '@/components/Dashboard/DataTable.vue';
import { hotelsAPI } from '@/Services/dashboardApi';

// Component State
const loading = ref(false);
const hotels = ref([]);

// Table Columns Configuration
const columns = [
  {
    label: 'Property',
    field: 'images',
    type: 'image',
    showNameWithImage: true,
    headerClass: 'w-1/4'
  },
  {
    label: 'Rating',
    field: 'rating',
    type: 'rating',
    headerClass: 'w-1/6'
  },
  {
    label: 'City',
    field: 'city',
    type: 'text',
    headerClass: 'w-1/6'
  },
  {
    label: 'Price / Night',
    field: 'pricePerNight',
    type: 'price',
    headerClass: 'w-1/6'
  },
  {
    label: 'Status',
    field: 'status',
    type: 'status',
    headerClass: 'w-1/6',
    clickable: true
  },
  {
    label: 'Featured',
    field: 'featured',
    type: 'toggle',
    headerClass: 'w-1/12'
  },
  {
    label: 'Actions',
    type: 'actions',
    headerClass: 'w-1/6'
  }
];

// Stats Computation
const stats = computed(() => {
  const total = hotels.value.length;
  const active = hotels.value.filter(h => h.status === 'active').length;
  const featured = hotels.value.filter(h => h.featured).length;
  const available = hotels.value.filter(h => h.availability?.availableRooms > 0).length;

  return [
    {
      label: 'Total Hotels',
      value: total,
      icon: 'total',
      trend: 12
    },
    {
      label: 'Active Hotels',
      value: active,
      icon: 'active',
      trend: 8
    },
    {
      label: 'Featured Properties',
      value: featured,
      icon: 'star',
      trend: 5
    },
    {
      label: 'Available Now',
      value: available,
      icon: 'check',
      trend: -2
    }
  ];
});

// data fetching
const fetchHotels = async () => {
  loading.value = true;
  try {
    const response = await hotelsAPI.getAll();
    // Transform API data to match table requirements if needed
    hotels.value = response.data.map(hotel => ({
      ...hotel,
      // Ensure image is the first one if it's an array
      images: Array.isArray(hotel.images) ? hotel.images[0] : hotel.images,
      // Normalize any other fields if necessary
      status: hotel.status || 'Draft' 
    }));
  } catch (error) {
    console.error('Error fetching hotels:', error);
  } finally {
    loading.value = false;
  }
};

// Action Handlers
const handleAdd = () => {
  console.log('Add new hotel clicked');
  // Implement add modal logic
};

const handleEdit = (row) => {
  console.log('Edit hotel:', row);
  // Implement edit modal logic
};

const handleDelete = async (row) => {
  if (confirm('Are you sure you want to delete this hotel?')) {
    try {
      await hotelsAPI.delete(row.id);
      await fetchHotels(); // Refresh list
    } catch (error) {
      console.error('Error deleting hotel:', error);
    }
  }
};

const handleView = (row) => {
  console.log('View hotel:', row);
  // Implement view details logic
};

const handleToggle = async ({ row, field, newValue }) => {
  try {
    // Optimistic update handled by DataTable, perform API call
    if (field === 'featured') {
      await hotelsAPI.toggleFeatured(row.id, newValue);
    }
  } catch (error) {
    console.error('Error updating toggle:', error);
    // Revert handling would be complex here without a refresh, 
    // ideally store would handle state
    await fetchHotels();
  }
};

const handleStatusClick = async ({ row, field, value }) => {
  // Simple status toggle logic for demo
  const newStatus = value === 'active' ? 'Maintenance' : 'active';
  try {
    await hotelsAPI.updateStatus(row.id, newStatus);
    // Manually update local state if not refreshing entire list
    const hotel = hotels.value.find(h => h.id === row.id);
    if (hotel) hotel.status = newStatus;
  } catch (error) {
    console.error('Error updating status:', error);
  }
};

const handleStatusChange = async ({ row, newValue }) => {
  try {
    await hotelsAPI.updateStatus(row.id, newValue);
  } catch (error) {
    console.error('Error changing status:', error);
  }
};

const handleFilter = () => {
  console.log('Filter clicked');
  // Implement filter logic
};

// Lifecycle Hooks
onMounted(() => {
  fetchHotels();
});

</script>
