<template>
  <div class="p-6 space-y-6">
    <!-- Stats Grid -->
    <StatsCard :stats="stats" />

    <!-- Cars Data Table -->
    <DataTable
      title="Cars Fleet"
      :columns="columns"
      :data="cars"
      :show-filter="true"
      add-button-text="Add New Vehicle"
      :show-actions="{ edit: true, delete: true, view: true }"
      empty-message="No cars available"
      :per-page="8"
      resource="cars"
      :loading="loading"
      @add="handleAdd"
      @edit="handleEdit"
      @delete="handleDelete"
      @view="handleView"
      @toggle="handleToggle"
      @status-click="handleStatusClick"
      @status-change="handleStatusChange"
      @filter="handleFilter"
      v-model:data="cars"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import StatsCard from '@/components/Dashboard/StatsCard.vue';
import DataTable from '@/components/Dashboard/DataTable.vue';
import { carsAPI } from '@/Services/dashboardApi';

// Component State
const loading = ref(false);
const cars = ref([]);

// Table Columns Configuration
const columns = [
  {
    label: 'Vehicle',
    field: 'images',
    type: 'image',
    showNameWithImage: true,
    headerClass: 'w-1/4'
  },
  {
    label: 'City',
    field: 'city', // Adjusted from location if needed based on db.json
    type: 'text',
    headerClass: 'w-1/6'
  },
  {
    label: 'Price / Day',
    field: 'price', // db.json uses 'price' (int) and 'pricePerDay' (string)
    type: 'price',
    headerClass: 'w-1/6'
  },
  {
    label: 'Total Fleet',
    field: 'total_fleet',
    type: 'text',
    headerClass: 'w-1/12'
  },
  {
    label: 'Availability',
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
  const total = cars.value.reduce((acc, car) => acc + (car.total_fleet || 0), 0);
  const available = cars.value.reduce((acc, car) => acc + (car.available_now || 0), 0);
  const maintenance = cars.value.reduce((acc, car) => acc + (car.in_maintenance || 0), 0);
  const booked = cars.value.reduce((acc, car) => acc + (car.booked_today || 0), 0);

  return [
    {
      label: 'Total Fleet Size',
      value: total,
      icon: 'car',
      trend: 5
    },
    {
      label: 'Available Now',
      value: available,
      icon: 'check',
      trend: -1
    },
    {
      label: 'In Maintenance',
      value: maintenance,
      icon: 'tool',
      trend: 0
    },
    {
      label: 'Booked Today',
      value: booked,
      icon: 'calendar',
      trend: 8
    }
  ];
});

// data fetching
const fetchCars = async () => {
  loading.value = true;
  try {
    const response = await carsAPI.getAll();
    // Transform API data
    cars.value = response.data.map(car => ({
      ...car,
      // Handle image array
      images: Array.isArray(car.images) ? car.images[0] : car.images,
      // Normalize location if needed (db.json seemed to lack 'city' on some, but has it generally)
      location: car.city || 'Cairo', 
      price: car.price || car.pricePerDay
    }));
  } catch (error) {
    console.error('Error fetching cars:', error);
  } finally {
    loading.value = false;
  }
};

// Actions
const handleAdd = () => {
  console.log('Add car');
};

const handleEdit = (row) => {
  console.log('Edit car:', row);
};

const handleDelete = async (row) => {
  if (confirm('Delete this vehicle?')) {
    try {
      await carsAPI.delete(row.id);
      await fetchCars();
    } catch (error) {
      console.error('Error deleting car:', error);
    }
  }
};

const handleView = (row) => {
  console.log('View car:', row);
};

const handleToggle = async ({ row, field, newValue }) => {
  if (field === 'featured') {
    try {
      await carsAPI.toggleFeatured(row.id, newValue);
    } catch (error) {
      console.error('Error toggling featured:', error);
      fetchCars();
    }
  }
};

const handleStatusClick = async ({ row, value }) => {
  const newStatus = value === 'available' ? 'maintenance' : 'available';
  try {
    await carsAPI.updateStatus(row.id, newStatus);
    const car = cars.value.find(c => c.id === row.id);
    if (car) car.status = newStatus;
  } catch (error) {
    console.error('Status update failed:', error);
  }
};

const handleStatusChange = async ({ row, newValue }) => {
    try {
    await carsAPI.updateStatus(row.id, newValue);
  } catch (error) {
    console.error('Status update failed:', error);
  }
}

const handleFilter = () => {
  console.log('Filter cars');
};

onMounted(() => {
  fetchCars();
});
</script>