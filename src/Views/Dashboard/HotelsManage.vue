<template>
  <div class="space-y-2">
    <!-- Stats Grid -->
    <StatsCard v-if="!compactMode" :stats="stats" />

    <!-- Hotels Data Table -->
    <DataTable
      title="Hotels"
      :columns="columns"
      :data="filteredHotels"
      :show-filter="true"
      add-button-text="Add New Hotel"
      :show-actions="{ edit: true, delete: true, view: true }"
      empty-message="No hotels available"
      :per-page="compactMode ? 5 : 8"
      :show-pagination="!compactMode"
      resource="hotels"
      :loading="loading"
      @add="handleAdd"
      @edit="handleEdit"
      @delete="handleDelete"
      @view="handleView"
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
      :initial-data="selectedHotel"
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
import { hotelsAPI } from '@/Services/dashboardApi';
import { dashboardHotelFilterConfig } from '@/Utils/dashboardFilterConfigs';
import { hotelFormConfig } from '@/Utils/dashboardFormConfigs';

// Component State
const loading = ref(false);
const router = useRouter();
const route = useRoute(); // Added missing route definition
const hotels = ref([]);
const showFilterModal = ref(false);
const showFormModal = ref(false);
const formMode = ref('add');
const selectedHotel = ref({});
const activeFilters = ref({});

// Configs
const filterConfig = dashboardHotelFilterConfig;
const formConfig = hotelFormConfig;

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
    label: 'Availability',
    field: 'availability',
    type: 'text',
    headerClass: 'w-1/8'
  },
  {
    label: 'Status',
    field: 'status',
    type: 'status-dropdown',
    options: ['Active', 'Pending', 'Suspended'],
    headerClass: 'w-1/8'
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
      images: Array.isArray(hotel.images) ? hotel.images[0] : hotel.images,
      availability: hotel.availability 
        ? `${hotel.availability.availableRooms}/${hotel.availability.totalRooms} room`
        : 'N/A',
      status: hotel.status || 'Draft'
    }));
  } catch (error) {
    console.error('Error fetching hotels:', error);
  } finally {
    loading.value = false;
  }
};

// Filtered hotels
const filteredHotels = computed(() => {
  let result = hotels.value;

  // Global Search
  if (route.query.q) {
    const search = route.query.q.toLowerCase();
    result = result.filter(h => 
      h.name?.toLowerCase().includes(search) || 
      h.city?.toLowerCase().includes(search) ||
      h.status?.toLowerCase().includes(search) || // Status text match
      (search === 'featured' && h.featured) // "featured" keyword
    );
  }

  if (activeFilters.value.maxPrice && activeFilters.value.maxPrice < filterConfig.priceRange.max) {
    result = result.filter(h => h.pricePerNight <= activeFilters.value.maxPrice);
  }

  if (activeFilters.value.city) {
    const searchCity = activeFilters.value.city.toLowerCase();
    result = result.filter(h => h.city?.toLowerCase().includes(searchCity));
  }

  if (activeFilters.value.statusSelected) {
    result = result.filter(h => h.status === activeFilters.value.statusSelected);
  }

  if (activeFilters.value.featuredSelected) {
    const isFeatured = activeFilters.value.featuredSelected === 'true';
    result = result.filter(h => h.featured === isFeatured);
  }

  if (activeFilters.value.ratingSelected) {
    result = result.filter(h => h.stars >= parseInt(activeFilters.value.ratingSelected));
  }

  return result;
});

// Action Handlers
const handleAdd = () => {
  formMode.value = 'add';
  selectedHotel.value = {};
  showFormModal.value = true;
};

const handleEdit = (row) => {
  formMode.value = 'edit';
  const fullHotel = hotels.value.find(h => h.id === row.id);
  selectedHotel.value = { ...fullHotel };
  showFormModal.value = true;
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
  router.push({ name: 'DashboardDetails', params: { type: 'hotels', id: row.id } });
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
  const newStatus = value === 'Active' ? 'Suspended' : 'Active';
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

const openFilterModal = () => {
  showFilterModal.value = true;
};

const applyFilters = (filters) => {
  activeFilters.value = filters;
  showFilterModal.value = false;
};

const closeFormModal = () => {
  showFormModal.value = false;
  selectedHotel.value = {};
};

const handleFormSubmit = async ({ mode, data }) => {
  loading.value = true;
  try {
    if (mode === 'add') {
      await hotelsAPI.create(data);
    } else {
      await hotelsAPI.update(selectedHotel.value.id, data);
    }
    await fetchHotels();
    closeFormModal();
  } catch (error) {
    console.error('Error submitting form:', error);
    alert('Failed to save hotel');
  } finally {
    loading.value = false;
  }
};

// Lifecycle Hooks
onMounted(() => {
  fetchHotels();
});

</script>
