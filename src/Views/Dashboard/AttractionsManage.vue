<template>
  <div class="space-y-2">
    <!-- Stats Cards -->
    <StatsCard v-if="!compactMode" :stats="stats" />

    <!-- Data Table -->
    <DataTable
      title="Attractions"
      :columns="columns"
      :data="filteredAttractions"
      :show-filter="true"
      add-button-text="Add New Attraction"
      :show-actions="{ edit: true, delete: true, view: true }"
      empty-message="No attractions available"
      :per-page="compactMode ? 5 : 8"
      :show-pagination="!compactMode"
      resource="attractions"
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
      :initial-data="selectedAttraction"
      @close="closeFormModal"
      @submit="handleFormSubmit"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router'; // Added useRoute import
import DataTable from '@/components/Dashboard/DataTable.vue';
import StatsCard from '@/components/Dashboard/StatsCard.vue';
import FilterModal from '@/components/Dashboard/FilterModal.vue';
import FormModal from '@/components/Dashboard/FormModal.vue';
import { attractionsAPI } from '@/Services/dashboardApi';
import { dashboardAttractionFilterConfig } from '@/Utils/dashboardFilterConfigs';
import { attractionFormConfig } from '@/Utils/dashboardFormConfigs';

const props = defineProps({
  compactMode: {
    type: Boolean,
    default: false
  }
});

// State
const attractions = ref([]);
const router = useRouter();
const route = useRoute(); // Added missing route definition
const loading = ref(false);
const showFilterModal = ref(false);
const showFormModal = ref(false);
const formMode = ref('add');
const selectedAttraction = ref({});
const activeFilters = ref({});

// Configs
const filterConfig = dashboardAttractionFilterConfig;
const formConfig = attractionFormConfig;

// Table columns configuration
const columns = [
  {
    label: 'Attraction',
    field: 'images',
    type: 'image',
    showNameWithImage: true,
    headerClass: 'w-1/4'
  },
  {
    label: 'Price',
    field: 'price',
    type: 'text',
    headerClass: 'w-1/6'
  },
  {
    label: 'Capacity',
    field: 'capacity',
    type: 'text',
    headerClass: 'w-1/6'
  },
  {
    label: 'Featured',
    field: 'isFeatured',
    type: 'toggle',
    headerClass: 'w-1/8'
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
    options: ['Active', 'Pending', 'Disabled', 'Suspended'],
    headerClass: 'w-1/8'
  },
  {
    label: 'Action',
    type: 'actions',
    headerClass: 'w-1/8'
  }
];

// Computed stats
const stats = computed(() => {
  const total = attractions.value.length;
  const active = attractions.value.filter(a => a.status === 'Active').length;
  const featured = attractions.value.filter(a => a.isFeatured).length;
  const available = attractions.value.filter(a => a.availability === 'Available').length;

  return [
    {
      label: 'Total Attractions',
      value: total,
      icon: 'total',
      trend: 5
    },
    {
      label: 'Active',
      value: active,
      icon: 'active',
      trend: 3
    },
    {
      label: 'Featured',
      value: featured,
      icon: 'featured',
      trend: -2
    },
    {
      label: 'Available',
      value: available,
      icon: 'available',
      trend: 8
    }
  ];
});

// Filtered attractions based on active filters
const filteredAttractions = computed(() => {
  let result = attractions.value;

  // Global Search
  if (route.query.q) {
    const search = route.query.q.toLowerCase();
    result = result.filter(a => 
      a.name?.toLowerCase().includes(search) || 
      a.city?.toLowerCase().includes(search) ||
      a.location?.toLowerCase().includes(search) ||
      a.status?.toLowerCase().includes(search) || // Status match
      (search === 'featured' && a.isFeatured) // "featured" keyword match
    );
  }

  // Apply price filter
  if (activeFilters.value.maxPrice && activeFilters.value.maxPrice < filterConfig.priceRange.max) {
    result = result.filter(a => {
      const price = typeof a.price === 'string' ? parseInt(a.price) || 0 : a.price;
      return price <= activeFilters.value.maxPrice;
    });
  }

  // Apply category filter
  if (activeFilters.value.categorySelected) {
    result = result.filter(a => a.categories?.includes(activeFilters.value.categorySelected));
  }

  // Apply city filter
  if (activeFilters.value.city) {
    const searchCity = activeFilters.value.city.toLowerCase();
    result = result.filter(a => a.city?.toLowerCase().includes(searchCity));
  }

  // Apply status filter
  if (activeFilters.value.statusSelected) {
    result = result.filter(a => a.status === activeFilters.value.statusSelected);
  }

  // Apply availability filter
  if (activeFilters.value.availabilitySelected) {
    result = result.filter(a => a.availability === activeFilters.value.availabilitySelected);
  }

  // Apply featured filter
  if (activeFilters.value.featuredSelected) {
    const isFeatured = activeFilters.value.featuredSelected === 'true';
    result = result.filter(a => a.isFeatured === isFeatured);
  }

  // Apply rating filter
  if (activeFilters.value.ratingSelected) {
    const minRating = parseFloat(activeFilters.value.ratingSelected);
    result = result.filter(a => a.rating >= minRating);
  }

return result;
});

// Fetch attractions from API
const fetchAttractions = async () => {
  loading.value = true;
  try {
    const response = await attractionsAPI.getAll();
    
    // Transform data to match table requirements
    attractions.value = response.data.map(attraction => ({
      id: attraction.id,
      name: attraction.name,
      location: `${attraction.city}, Egypt`,
      year: attraction.year || '',
      images: attraction.images?.[0] || '/placeholder-attraction.jpg',
      price: attraction.price || 'Free',
      capacity: attraction.capacity || '18 / 42 Ticket',
      isFeatured: attraction.isFeatured || false,
      availability: attraction.availability || (Math.random() > 0.5 ? 'Available' : 'Sold Out'),
      status: attraction.status || (Math.random() > 0.7 ? 'Active' : Math.random() > 0.5 ? 'Pending' : 'Disabled'),
      rating: attraction.rating,
      reviewsCount: attraction.reviews?.totalReviews || 0
    }));
  } catch (error) {
    console.error('Error fetching attractions:', error);
  } finally {
    loading.value = false;
  }
};

// Event handlers
const handleAdd = () => {
  formMode.value = 'add';
  selectedAttraction.value = {};
  showFormModal.value = true;
};

const handleEdit = (row) => {
  formMode.value = 'edit';
  const fullAttraction = attractions.value.find(a => a.id === row.id);
  selectedAttraction.value = { ...fullAttraction };
  showFormModal.value = true;
};

const handleDelete = async (row) => {
  if (confirm(`Are you sure you want to delete "${row.name}"?`)) {
    try {
      await attractionsAPI.delete(row.id);
      await fetchAttractions(); // Refresh data
      console.log('Deleted attraction:', row);
    } catch (error) {
      console.error('Error deleting attraction:', error);
      alert('Failed to delete attraction');
    }
  }
};

const handleView = (row) => {
  router.push({ name: 'DashboardDetails', params: { type: 'attractions', id: row.id } });
};

const handleToggle = async ({ row, field, newValue }) => {
  try {
    // Update the featured status
    await attractionsAPI.toggleFeatured(row.id, newValue);
    console.log(`Toggled ${field} for ${row.name} to ${newValue}`);
  } catch (error) {
    console.error('Error toggling featured:', error);
    // Revert the change
    await fetchAttractions();
  }
};

const handleStatusClick = async ({ row, field, value }) => {
  if (field === 'availability') {
    const newAvailability = value === 'Available' ? 'Sold Out' : 'Available';
    
    // Optimistic update
    const rowIndex = attractions.value.findIndex(a => a.id === row.id);
    if (rowIndex !== -1) {
      attractions.value[rowIndex].availability = newAvailability;
    }

    try {
      await attractionsAPI.updateAvailability(row.id, newAvailability);
      console.log(`Updated availability for ${row.name} to ${newAvailability}`);
    } catch (error) {
      console.error('Error updating availability:', error);
      // Revert on error
      if (rowIndex !== -1) {
        attractions.value[rowIndex].availability = value;
      }
      alert('Failed to update availability');
    }
  }
};

const handleStatusChange = async ({ row, field, newValue }) => {
  try {
    // Update the status
    if (field === 'status') {
      await attractionsAPI.updateStatus(row.id, newValue);
    } else if (field === 'availability') {
      await attractionsAPI.updateAvailability(row.id, newValue);
    }
    console.log(`Changed ${field} for ${row.name} to ${newValue}`);
  } catch (error) {
    console.error('Error updating status:', error);
    // Revert the change
    await fetchAttractions();
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
  selectedAttraction.value = {};
};

const handleFormSubmit = async ({ mode, data }) => {
  loading.value = true;
  try {
    if (mode === 'add') {
      await attractionsAPI.create(data);
    } else {
      await attractionsAPI.update(selectedAttraction.value.id, data);
    }
    await fetchAttractions();
    closeFormModal();
  } catch (error) {
    console.error('Error submitting form:', error);
    alert('Failed to save attraction');
  } finally {
    loading.value = false;
  }
};

// Lifecycle
onMounted(() => {
  fetchAttractions();
});
</script>

<style scoped>
/* Additional custom styles if needed */
</style>
