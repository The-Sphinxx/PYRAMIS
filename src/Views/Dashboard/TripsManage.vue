<template>
  <div class="p-6 space-y-6">
    <!-- Stats Grid -->
    <StatsCard :stats="stats" />

    <!-- Trips Data Table -->
    <DataTable
      title="Trips"
      :columns="columns"
      :data="filteredTrips"
      :show-filter="true"
      add-button-text="Add New Trip"
      :show-actions="{ edit: true, delete: true, view: true }"
      empty-message="No trips available"
      :per-page="8"
      resource="trips"
      :loading="loading"
      @add="handleAdd"
      @edit="handleEdit"
      @delete="handleDelete"
      @toggle="handleToggle"
      @status-click="handleStatusClick"
      @status-change="handleStatusChange"
      @filter="openFilterModal"
    />

    <!-- Filter Modal -->
    <FilterModal 
      :is-open="showFilterModal"
      v-bind="filterConfig"
      @filter-change="applyFilters"
      @close="showFilterModal = false"
    />

    <!-- Form Modal -->
    <FormModal
      :is-open="showFormModal"
      :mode="formMode"
      :config="formConfig"
      :initial-data="selectedTrip"
      @close="closeFormModal"
      @submit="handleFormSubmit"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import StatsCard from '@/components/Dashboard/StatsCard.vue';
import DataTable from '@/components/Dashboard/DataTable.vue';
import FilterModal from '@/components/Dashboard/FilterModal.vue';
import FormModal from '@/components/Dashboard/FormModal.vue';
import { tripsAPI } from '@/Services/dashboardApi';
import { dashboardTripFilterConfig } from '@/Utils/dashboardFilterConfigs';
import { tripFormConfig } from '@/Utils/dashboardFormConfigs';

// Component State
const loading = ref(false);
const trips = ref([]);
const showFilterModal = ref(false);
const showFormModal = ref(false);
const formMode = ref('add');
const selectedTrip = ref({});
const activeFilters = ref({});

const filterConfig = dashboardTripFilterConfig;
const formConfig = tripFormConfig;

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
    field: 'featured',
    type: 'toggle',
    headerClass: 'w-1/12'
  },
  {
    label: 'Availability',
    field: 'availability',
    type: 'status',
    headerClass: 'w-1/8',
    clickable: true
  },
  {
    label: 'Status',
    field: 'status',
    type: 'status-dropdown',
    options: ['Active', 'Inactive', 'Full'],
    headerClass: 'w-1/8'
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
      featured: trip.featured || false,
      availability: trip.availability || 'Available',
      status: trip.status || 'Active'
    }));
  } catch (error) {
    console.error('Error fetching trips:', error);
  } finally {
    loading.value = false;
  }
};

// Filtered trips
const filteredTrips = computed(() => {
  let result = trips.value;

  if (activeFilters.value.maxPrice && activeFilters.value.maxPrice < filterConfig.priceRange.max) {
    result = result.filter(t => t.price <= activeFilters.value.maxPrice);
  }

  if (activeFilters.value.destination) {
    const searchDest = activeFilters.value.destination.toLowerCase();
    result = result.filter(t => t.destination?.toLowerCase().includes(searchDest));
  }

  if (activeFilters.value.tripTypeSelected) {
    result = result.filter(t => t.tripType === activeFilters.value.tripTypeSelected);
  }

  if (activeFilters.value.statusSelected) {
    result = result.filter(t => t.status === activeFilters.value.statusSelected);
  }

  if (activeFilters.value.featuredSelected) {
    const isFeatured = activeFilters.value.featuredSelected === 'true';
    result = result.filter(t => t.featured === isFeatured);
  }

  return result;
});

// Actions
const handleAdd = () => {
  formMode.value = 'add';
  selectedTrip.value = {};
  showFormModal.value = true;
};

const handleEdit = (row) => {
  formMode.value = 'edit';
  const fullTrip = trips.value.find(t => t.id === row.id);
  selectedTrip.value = { ...fullTrip };
  showFormModal.value = true;
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

const handleStatusClick = async ({ row, field, value }) => {
  if (field === 'availability') {
    const newAvailability = value === 'Available' ? 'Sold Out' : 'Available';
    const rowIndex = trips.value.findIndex(t => t.id === row.id);
    if (rowIndex !== -1) {
      trips.value[rowIndex].availability = newAvailability;
    }
    try {
      await tripsAPI.patch(row.id, { availability: newAvailability });
    } catch (error) {
      console.error('Error:', error);
      if (rowIndex !== -1) {
        trips.value[rowIndex].availability = value;
      }
    }
  }
};

const handleStatusChange = async ({ row, field, newValue }) => {
  try {
    if (field === 'status') {
      await tripsAPI.patch(row.id, { status: newValue });
    }
    await fetchTrips();
  } catch (error) {
    console.error('Error:', error);
  }
};

const openFilterModal = () => {
  showFilterModal.value = true;
};

const applyFilters = (filters) => {
  activeFilters.value = filters;
  showFilterModal.value = false;
};

const closeFormModal = () => {
  showFormModal.value = false;
  selectedTrip.value = {};
};

const handleFormSubmit = async ({ mode, data }) => {
  loading.value = true;
  try {
    if (mode === 'add') {
      await tripsAPI.create(data);
    } else {
      await tripsAPI.update(selectedTrip.value.id, data);
    }
    await fetchTrips();
    closeFormModal();
  } catch (error) {
    console.error('Error submitting form:', error);
    alert('Failed to save trip');
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  fetchTrips();
});
</script>