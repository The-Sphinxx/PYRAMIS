<template>
  <div class="space-y-2">
    <!-- Stats Cards -->
    <StatsCard v-if="!compactMode" :stats="stats" />

    <!-- Data Table -->
    <DataTable
      title="Attractions"
      :columns="columns"
      :data="attractions"
      :show-filter="true"
      add-button-text="Add New Attraction"
      :show-actions="{ edit: true, delete: true, view: true }"
      empty-message="No attractions available"
      :per-page="compactMode ? 5 : 8"
      :show-pagination="!compactMode"
      :server-side="true"
      :total-items="totalItems"
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
      @page-change="handlePageChange"
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
      :initial-data="selectedAttraction"
      @close="closeFormModal"
      @submit="handleFormSubmit"
    />
    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="modal modal-open">
      <div class="modal-box">
        <h3 class="font-bold text-lg text-error">Confirm Delete</h3>
        <p class="py-4">Are you sure you want to delete this attraction? This action cannot be undone.</p>
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
const totalItems = ref(0);
const currentPage = ref(1);
const itemsPerPage = ref(props.compactMode ? 5 : 8);
const router = useRouter();
const route = useRoute(); // Added missing route definition
import { useToast } from '@/composables/useToast.js';

const loading = ref(false);
const showFilterModal = ref(false);
const showFormModal = ref(false);
const showDeleteModal = ref(false);
const attractionToDelete = ref(null);
const formMode = ref('add');
const selectedAttraction = ref({});
const activeFilters = ref({});
const { toast } = useToast();

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
// Server-side: just return the current page data
const filteredAttractions = computed(() => attractions.value);

// Watch for search query changes
import { watch } from 'vue';
watch(() => route.query.q, () => {
  currentPage.value = 1;
  fetchAttractions();
});

// Fetch attractions from API
const fetchAttractions = async () => {
  loading.value = true;
  try {
    const params = {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      searchTerm: route.query.q || undefined,
      // Map filters
      city: activeFilters.value.city,
      category: activeFilters.value.categorySelected,
      minRating: activeFilters.value.rating,
      // Add other filters if supported by backend, otherwise they are ignored for now
      // Note: Backend might not support all these filters in GetAll yet
    };

    const response = await attractionsAPI.getAll(params);
    
    // Check if response conforms to paginated structure
    const data = response.data.data || response.data; // Helper for different response structures
    const items = Array.isArray(data) ? data : (data.items || []); 
    
    // Update total items for pagination
    totalItems.value = response.data.totalCount || response.data.totalRecords || items.length; // Fallback

    // Transform data to match table requirements
    attractions.value = items.map(attraction => ({
      ...attraction,
      location: `${attraction.city}, Egypt`,
      // Map images to string URLs for DataTable
      images: Array.isArray(attraction.images) 
        ? attraction.images.map(img => typeof img === 'string' ? img : img.url)
        : [attraction.images?.url || attraction.images || '/placeholder-attraction.jpg'],
      price: attraction.rawPrice || attraction.price || 0,
      capacity: attraction.capacity || 'N/A',
      isFeatured: attraction.isFeatured || false,
      status: attraction.status || 'Active',
      availability: attraction.availability || 'Available'
    }));
  } catch (error) {
    console.error('Error fetching attractions:', error);
    toast.error('Failed to load attractions');
  } finally {
    loading.value = false;
  }
};

const handlePageChange = ({ page, perPage }) => {
  currentPage.value = page;
  itemsPerPage.value = perPage;
  fetchAttractions();
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

const handleDelete = (row) => {
  attractionToDelete.value = row;
  showDeleteModal.value = true;
};

const confirmDelete = async () => {
  if (!attractionToDelete.value) return;
  
  loading.value = true;
  try {
    await attractionsAPI.delete(attractionToDelete.value.id);
    
    // Optimistic update
    attractions.value = attractions.value.filter(a => a.id !== attractionToDelete.value.id);
    toast.success('Attraction deleted successfully');
    showDeleteModal.value = false;
  } catch (error) {
    console.error('Error deleting attraction:', error);
    toast.error('Failed to delete attraction');
    await fetchAttractions();
  } finally {
    loading.value = false;
    attractionToDelete.value = null;
  }
};

const handleView = (row) => {
  router.push({ name: 'DashboardDetails', params: { type: 'attractions', id: row.id } });
};

const handleToggle = async ({ row, field, newValue }) => {
  try {
    // Optimistic update
    const idx = attractions.value.findIndex(a => a.id === row.id);
    if (idx !== -1) {
      if (field === 'isFeatured') {
        attractions.value[idx].isFeatured = newValue;
      }
    }

    // Update the featured status
    if (field === 'isFeatured') {
      await attractionsAPI.toggleFeatured(row.id, newValue);
      toast.success(`${newValue ? 'Added to' : 'Removed from'} featured attractions`);
    }
  } catch (error) {
    console.error('Error toggling featured:', error);
    toast.error('Failed to update featured status');
    // Revert without global loading if possible, or just refresh silently
    const response = await attractionsAPI.getAll();
    attractions.value = response.data.data.map(a => ({
       ...a,
       images: Array.isArray(a.images) ? a.images.map(img => typeof img === 'string' ? img : img.url) : [a.images?.url || a.images || '/placeholder-attraction.jpg'],
       price: a.rawPrice || a.price || 0
    }));
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
      toast.success(`Availability updated to ${newAvailability}`);
    } catch (error) {
      console.error('Error updating availability:', error);
      // Revert on error
      if (rowIndex !== -1) {
        attractions.value[rowIndex].availability = value;
      }
      toast.error('Failed to update availability');
    }
  }
};

const handleStatusChange = async ({ row, field, newValue }) => {
  try {
    if (field === 'status') {
      await attractionsAPI.updateStatus(row.id, newValue);
      // Optimistic
      const idx = attractions.value.findIndex(a => a.id === row.id);
      if (idx !== -1) attractions.value[idx].status = newValue;
      toast.success('Status updated successfully');
    } else if (field === 'availability') {
      await attractionsAPI.updateAvailability(row.id, newValue);
      // Optimistic
      const idx = attractions.value.findIndex(a => a.id === row.id);
      if (idx !== -1) attractions.value[idx].availability = newValue;
      toast.success('Availability updated successfully');
    }
  } catch (error) {
    console.error('Error updating status:', error);
    toast.error('Failed to update status');
    await fetchAttractions();
  }
};

const openFilterModal = () => {
  showFilterModal.value = true;
};

const applyFilters = (filters) => {
  activeFilters.value = filters;
  showFilterModal.value = false;
  currentPage.value = 1; // Reset to first page on filter change
  fetchAttractions();
};

const closeFormModal = () => {
  showFormModal.value = false;
  selectedAttraction.value = {};
};

const handleFormSubmit = async ({ mode, data }) => {
  loading.value = true;
  try {
    // Basic Validation
    if (!data.name?.trim()) {
      toast.error('Attraction name is required');
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
    
    // Transform
    const payload = {
      ...data,
      name: data.name?.trim(),
      city: data.city?.trim() || data.location?.trim(),
      location: data.location?.trim() || data.city?.trim(),
      price: Number(data.price),
      capacity: data.capacity?.toString(),
      images: Array.isArray(data.images) ? data.images.filter(img => typeof img === 'string') : []
    };

    if (mode === 'add') {
      const response = await attractionsAPI.create(payload);
      const newItem = {
        ...payload,
        id: response.data.id || response.data,
        isFeatured: payload.isFeatured || false,
        status: payload.status || 'Active'
      };
      attractions.value.unshift(newItem);
      toast.success('Attraction created successfully');
    } else {
      await attractionsAPI.update(selectedAttraction.value.id, { ...payload, Id: selectedAttraction.value.id });
      // Optimistic update
      const index = attractions.value.findIndex(a => a.id === selectedAttraction.value.id);
      if (index !== -1) {
        attractions.value[index] = { ...attractions.value[index], ...payload };
      }
      toast.success('Attraction updated successfully');
    }
    closeFormModal();
  } catch (error) {
    console.error('Error submitting form:', error);
    const msg = error.response?.data?.message || 'Failed to save attraction';
    toast.error(msg);
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
