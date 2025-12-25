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
      :initial-filters="activeFilters"
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
    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="modal modal-open">
      <div class="modal-box">
        <h3 class="font-bold text-lg text-error">Confirm Delete</h3>
        <p class="py-4">Are you sure you want to delete this vehicle? This action cannot be undone.</p>
        <div class="modal-action">
          <button @click="showDeleteModal = false" class="btn">Cancel</button>
          <button @click="confirmDelete" class="btn btn-error text-white" :disabled="loading">
            <span v-if="loading" class="loading loading-spinner loading-sm"></span>
            Delete
          </button>
        </div>
      </div>
    </div>
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
import { useToast } from '@/composables/useToast.js';

const cars = ref([]);
const showFilterModal = ref(false);
const showFormModal = ref(false);
const showDeleteModal = ref(false);
const carToDelete = ref(null);
const formMode = ref('add');
const selectedCar = ref({});
const activeFilters = ref({});
const { toast } = useToast();

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
    field: 'totalFleet',
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
    options: ['Available', 'Rented', 'Maintenance', 'Pending'],
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
  const total = cars.value.reduce((acc, car) => acc + (car.totalFleet || 0), 0);
  const available = cars.value.reduce((acc, car) => acc + (car.availableNow || 0), 0);
  const maintenance = cars.value.reduce((acc, car) => acc + (car.status === 'Maintenance' ? 1 : 0), 0);
  const booked = cars.value.reduce((acc, car) => acc + (car.bookedToday || 0), 0);
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
    cars.value = response.data.data.map(car => ({
      ...car,
      // Keep images as array for FormModal/ImageUploader
      images: Array.isArray(car.images) ? car.images : [car.images],
      location: car.city || 'Cairo', 
      price: car.rawPrice || car.price, // Use raw numeric price if available
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

  if (activeFilters.value.featured) {
    const isFeatured = activeFilters.value.featured === 'true';
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

const handleDelete = (row) => {
  carToDelete.value = row;
  showDeleteModal.value = true;
};

const confirmDelete = async () => {
  if (!carToDelete.value) return;
  
  loading.value = true;
  try {
    await carsAPI.delete(carToDelete.value.id);
    
    // Optimistic update
    cars.value = cars.value.filter(c => c.id !== carToDelete.value.id);
    toast.success('Vehicle deleted successfully');
    showDeleteModal.value = false;
  } catch (error) {
    console.error('Error deleting car:', error);
    toast.error('Failed to delete vehicle');
    await fetchCars();
  } finally {
    loading.value = false;
    carToDelete.value = null;
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
      
      // Optimistic update
      const carIndex = cars.value.findIndex(c => c.id === row.id);
      if (carIndex !== -1) {
        cars.value[carIndex].status = newValue;
      }
      toast.success('Status updated successfully');
    }
  } catch (error) {
    console.error('Error:', error);
    toast.error('Failed to update status');
    await fetchCars();
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
    // Basic Validation
    if (!data.name?.trim() && !data.brand?.trim()) {
      toast.error('Brand/Name is required');
      return;
    }
    if (!data.price || data.price <= 0) {
      toast.error('Valid price is required');
      return;
    }
    if (!data.location?.trim() && !data.city?.trim()) {
      toast.error('Location is required');
      return;
    }
    
    // Transform payload to match backend DTO
    const payload = {
      ...data,
      name: data.name?.trim() || `${data.brand} ${data.model}`,
      brand: data.brand?.trim(),
      model: data.model?.trim(),
      year: Number(data.year || new Date().getFullYear()),
      city: (data.city || data.location)?.trim(),
      price: Number(data.price),
      seats: Number(data.seats || 4),
      totalFleet: Number(data.totalFleet || 1),
      availableNow: Number(data.availableNow || 0),
      images: Array.isArray(data.images) ? data.images.filter(img => typeof img === 'string') : [],
      features: Array.isArray(data.features) ? data.features : [],
      amenities: Array.isArray(data.amenities) ? data.amenities : [],
      highlights: Array.isArray(data.highlights) ? data.highlights : []
    };

    if (mode === 'add') {
      const response = await carsAPI.create(payload);
      // Add new item with returned ID and transformed data
      const newItem = {
        ...payload,
        id: response.data.id || response.data, // Backend returns ID or object
        // Ensure consistent field names for the table
        totalFleet: payload.totalFleet,
        availableNow: payload.availableNow,
        price: payload.price
      };
      cars.value.unshift(newItem); // Add to top
      toast.success('Vehicle created successfully');
    } else {
      await carsAPI.update(selectedCar.value.id, { ...payload, Id: selectedCar.value.id });
      // Update existing item in local state
      const index = cars.value.findIndex(c => c.id === selectedCar.value.id);
      if (index !== -1) {
        cars.value[index] = { 
          ...cars.value[index], 
          ...payload,
          // Explicitly map these to ensure table updates
          totalFleet: payload.totalFleet,
          availableNow: payload.availableNow
        };
      }
      toast.success('Vehicle updated successfully');
    }
    closeFormModal();
  } catch (error) {
    console.error('Error submitting form:', error);
    const msg = error.response?.data?.message || 'Failed to save vehicle';
    toast.error(msg);
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  fetchCars();
});
</script>