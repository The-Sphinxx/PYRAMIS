<template>
  <div class="p-6 space-y-6">
    <!-- Stats Grid -->
    <StatsCard :stats="stats" />

    <!-- Trips Data Table -->
    <DataTable
      title="Trips & Tours"
      :columns="columns"
      :data="trips"
      :show-filter="true"
      add-button-text="Create New Trip"
      :show-actions="{ edit: true, delete: true, view: true }"
      empty-message="No trips available"
      :per-page="8"
      resource="trips"
      :loading="loading"
      @add="handleAdd"
      @edit="handleEdit"
      @delete="handleDelete"
      @view="handleView"
      @toggle="handleToggle"
      @status-click="handleStatusClick"
      @status-change="handleStatusChange"
      @filter="handleFilter"
      v-model:data="trips"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import StatsCard from '@/components/Dashboard/StatsCard.vue';
import DataTable from '@/components/Dashboard/DataTable.vue';
import { tripsAPI } from '@/Services/dashboardApi';

// Component State
const loading = ref(false);
const trips = ref([]);

// Table Columns Configuration
const columns = [
  {
    label: 'Trip Name',
    field: 'images', // Using images for the column to leverage image+name display
    type: 'image',
    showNameWithImage: true, 
    // Data transform will ensure 'name' is populated with title for unified display
    headerClass: 'w-1/4'
  },
  {
    label: 'Destination',
    field: 'city',
    type: 'text',
    headerClass: 'w-1/6'
  },
  {
    label: 'Duration',
    field: 'duration',
    type: 'text',
    headerClass: 'w-1/6'
  },
  {
    label: 'Dates',
    field: 'nextDate', // Computed field for display
    type: 'text',
    headerClass: 'w-1/6'
  },
  {
    label: 'Price',
    field: 'price',
    type: 'price',
    headerClass: 'w-1/12'
  },
  {
    label: 'Featured',
    field: 'featured', // db.json might need check for 'featured' vs 'isFeatured'
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
  const total = trips.value.length;
  // db.json for trips lacked a status field in preview, assuming active if present or checking dates
  const active = trips.value.length; 
  const featured = trips.value.filter(t => t.featured).length;
  // Mock logic for fully booked as db.json data is simple
  const fullyBooked = 0; 

  return [
    {
      label: 'Total Trips',
      value: total,
      icon: 'map',
      trend: 4
    },
    {
      label: 'Active Packages',
      value: active,
      icon: 'check',
      trend: 2
    },
    {
      label: 'Featured',
      value: featured,
      icon: 'star',
      trend: 1
    },
    {
      label: 'Fully Booked',
      value: fullyBooked,
      icon: 'user', // closest icon
      trend: 0
    }
  ];
});

// data fetching
const fetchTrips = async () => {
  loading.value = true;
  try {
    const response = await tripsAPI.getAll();
    // Transform API data
    trips.value = response.data.map(trip => ({
      ...trip,
      name: trip.title, // Map title to name for DataTable image column
      images: Array.isArray(trip.images) ? trip.images[0] : trip.trip.image || trip.image,
      nextDate: trip.availableDates ? trip.availableDates[0] : 'TBD',
      // Ensure featured boolean exists
      featured: trip.featured || false,
      status: 'Active' // Default status as it was missing in db.json preview
    }));
  } catch (error) {
    console.error('Error fetching trips:', error);
  } finally {
    loading.value = false;
  }
};

// Actions
const handleAdd = () => {
  console.log('Add trip');
};

const handleEdit = (row) => {
  console.log('Edit trip:', row);
};

const handleDelete = async (row) => {
  if (confirm('Delete this trip package?')) {
    try {
      await tripsAPI.delete(row.id);
      await fetchTrips();
    } catch (error) {
      console.error('Error deleting trip:', error);
    }
  }
};

const handleView = (row) => {
  console.log('View trip:', row);
};

const handleToggle = async ({ row, field, newValue }) => {
  if (field === 'featured') {
    try {
      // Assuming API supports this, if not it might need implementing in store/backend adapter
       await tripsAPI.toggleFeatured(row.id, newValue);
    } catch (error) {
       console.error('Featured toggle error:', error);
       fetchTrips();
    }
  }
};

const handleStatusClick = (row) => {
  console.log('Status clicked', row);
};

const handleStatusChange = (e) => {
  console.log('Status changed', e);
};

const handleFilter = () => {
  console.log('Filter trips');
};

onMounted(() => {
  fetchTrips();
});
</script>