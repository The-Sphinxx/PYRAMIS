<template>
  <div class="space-y-2">
    <!-- Stats Grid -->
    <StatsCard v-if="!compactMode" :stats="stats" />

    <!-- Cars Data Table -->
    <DataTable
      title="Cars Fleet"
      :columns="columns"
      :data="filteredCars"
      :show-filter="true"
      add-button-text="Add New Vehicle"
      :show-actions="{ edit: true, delete: true, view: true }"
      empty-message="No cars available"
      :per-page="compactMode ? 5 : 8"
      :show-pagination="!compactMode"
      resource="cars"
      :loading="loading"
      @add="handleAdd"
      @edit="handleEdit"
      @delete="handleDelete"
      @view="handleView"
      @toggle="handleToggle"
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
      :initial-data="selectedCar"
      @close="closeFormModal"
      @submit="handleFormSubmit"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import StatsCard from '@/components/Dashboard/StatsCard.vue';
import DataTable from '@/components/Dashboard/DataTable.vue';

const props = defineProps({
  compactMode: {
    type: Boolean,
    default: false
  }
});
import FilterModal from '@/components/Dashboard/FilterModal.vue';
import FormModal from '@/components/Dashboard/FormModal.vue';
import { carsAPI } from '@/Services/dashboardApi';
import { dashboardCarFilterConfig } from '@/Utils/dashboardFilterConfigs';
import { carFormConfig } from '@/Utils/dashboardFormConfigs';

// Component State
const loading = ref(false);
const router = useRouter();
const route = useRoute(); // Added missing route definition
const cars = ref([]);
const showFilterModal = ref(false);
const showFormModal = ref(false);
const formMode = ref('add');
const selectedCar = ref({});
const activeFilters = ref({});

const filterConfig = dashboardCarFilterConfig;
const formConfig = carFormConfig;

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
    label: 'Featured',
    field: 'featured',
    type: 'toggle',
    headerClass: 'w-1/12'
  },

  {
    label: 'Status',
    field: 'status',
    type: 'status-dropdown',
    options: ['Available', 'Rented', 'Maintenance'],
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
      location: car.city || 'Cairo', 
      price: car.price || car.pricePerDay,
      status: car.status || 'Available'
    }));
  } catch (error) {
    console.error('Error fetching cars:', error);
  } finally {
    loading.value = false;
  }
};

// Filtered cars
const filteredCars = computed(() => {
  let result = cars.value;

  // Global Search from Sidebar (route.query.q)
  if (route.query.q) {
    const search = route.query.q.toLowerCase();
    result = result.filter(c => 
      c.name?.toLowerCase().includes(search) || 
      c.brand?.toLowerCase().includes(search) || 
      c.model?.toLowerCase().includes(search) ||
      c.city?.toLowerCase().includes(search) ||
      c.status?.toLowerCase().includes(search) || // Status text match
      (search === 'featured' && c.featured) // "featured" keyword
    );
  }

  if (activeFilters.value.maxPrice && activeFilters.value.maxPrice < filterConfig.priceRange.max) {
    result = result.filter(c => c.price <= activeFilters.value.maxPrice);
  }

  if (activeFilters.value.city) {
    const searchCity = activeFilters.value.city.toLowerCase();
    result = result.filter(c => c.city?.toLowerCase().includes(searchCity));
  }

  if (activeFilters.value.carTypeSelected) {
    result = result.filter(c => c.type === activeFilters.value.carTypeSelected);
  }

  if (activeFilters.value.transmissionSelected) {
    result = result.filter(c => c.transmission === activeFilters.value.transmissionSelected);
  }

  if (activeFilters.value.statusSelected) {
    result = result.filter(c => c.status === activeFilters.value.statusSelected);
  }

  if (activeFilters.value.featuredSelected) {
    const isFeatured = activeFilters.value.featuredSelected === 'true';
    result = result.filter(c => c.featured === isFeatured);
  }

  return result;
});

// Actions
const handleAdd = () => {
  formMode.value = 'add';
  selectedCar.value = {};
  showFormModal.value = true;
};

const handleEdit = (row) => {
  formMode.value = 'edit';
  const fullCar = cars.value.find(c => c.id === row.id);
  selectedCar.value = { ...fullCar };
  showFormModal.value = true;
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
  router.push({ name: 'DashboardDetails', params: { type: 'cars', id: row.id } });
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



const handleStatusChange = async ({ row, field, newValue }) => {
  try {
    if (field === 'status') {
      await carsAPI.updateStatus(row.id, newValue);
    }
    await fetchCars();
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
  selectedCar.value = {};
};

const handleFormSubmit = async ({ mode, data }) => {
  loading.value = true;
  try {
    if (mode === 'add') {
      await carsAPI.create(data);
    } else {
      await carsAPI.update(selectedCar.value.id, data);
    }
    await fetchCars();
    closeFormModal();
  } catch (error) {
    console.error('Error submitting form:', error);
    alert('Failed to save car');
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  fetchCars();
});
</script>