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
      :initial-filters="activeFilters"
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

    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="modal modal-open">
      <div class="modal-box">
        <h3 class="font-bold text-lg text-base-content">Confirm Delete</h3>
        <p class="py-4 text-base-content">
          Are you sure you want to delete <span class="font-semibold">"{{ hotelToDelete?.name }}"</span>? 
          This action cannot be undone.
        </p>
        <div class="modal-action">
          <button 
            class="btn btn-ghost" 
            @click="cancelDelete"
          >
            Cancel
          </button>
          <button 
            class="btn btn-error" 
            @click="confirmDelete"
          >
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
import { hotelsAPI } from '@/Services/dashboardApi';
import { dashboardHotelFilterConfig } from '@/Utils/dashboardFilterConfigs';
import { hotelFormConfig } from '@/Utils/dashboardFormConfigs';
import { useToast } from '@/composables/useToast';

// Component State
const loading = ref(false);
const router = useRouter();
const route = useRoute(); // Added missing route definition
const hotels = ref([]);
const showFilterModal = ref(false);
const showFormModal = ref(false);
const showDeleteModal = ref(false);
const formMode = ref('add');
const selectedHotel = ref({});
const hotelToDelete = ref(null);
const activeFilters = ref({});

// Toast notifications
const { toast } = useToast();

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
  const active = hotels.value.filter(h => h.status === 'Active').length;
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
    hotels.value = response.data.data.map(hotel => ({
      ...hotel,
      images: Array.isArray(hotel.images) ? hotel.images[0] : hotel.images,
      pricePerNight: hotel.rawPricePerNight || hotel.pricePerNight,
      basePrice: hotel.rawBasePrice || hotel.basePrice,
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

  if (activeFilters.value.maxPrice && typeof activeFilters.value.maxPrice === 'number' && activeFilters.value.maxPrice > 0) {
    result = result.filter(h => {
      const hotelPrice = typeof h.pricePerNight === 'string' ? parseFloat(h.pricePerNight) : h.pricePerNight;
      return hotelPrice <= activeFilters.value.maxPrice;
    });
  }

  if (activeFilters.value.city && typeof activeFilters.value.city === 'string' && activeFilters.value.city.trim()) {
    const searchCity = activeFilters.value.city.toLowerCase().trim();
    result = result.filter(h => h.city?.toLowerCase().includes(searchCity));
  }

  if (activeFilters.value.statusSelected && typeof activeFilters.value.statusSelected === 'string') {
    result = result.filter(h => h.status === activeFilters.value.statusSelected);
  }

  if (activeFilters.value.featuredSelected && typeof activeFilters.value.featuredSelected === 'string') {
    const isFeatured = activeFilters.value.featuredSelected === 'true';
    result = result.filter(h => h.featured === isFeatured);
  }

  if (activeFilters.value.ratingSelected && typeof activeFilters.value.ratingSelected === 'string') {
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
  // Transform backend data to match form field names
  selectedHotel.value = {
    ...fullHotel,
    pricePerNight: fullHotel.rawPricePerNight || fullHotel.pricePerNight || 0,
    basePrice: fullHotel.rawBasePrice || fullHotel.basePrice || 0,
    overview: fullHotel.description || fullHotel.overview || "",
    amenities: fullHotel.facilities || fullHotel.amenities || []
  };
  showFormModal.value = true;
};

const handleDelete = (row) => {
  hotelToDelete.value = row;
  showDeleteModal.value = true;
};

const confirmDelete = async () => {
  if (!hotelToDelete.value) return;

  // Close modal and show success immediately for better UX
  showDeleteModal.value = false;
  toast.success('Hotel deleted successfully');
  
  // Remove from local state immediately
  hotels.value = hotels.value.filter(h => h.id !== hotelToDelete.value.id);
  
  const hotelId = hotelToDelete.value.id;
  hotelToDelete.value = null;

  // Perform API call in background
  try {
    await hotelsAPI.delete(hotelId);
  } catch (error) {
    console.error('Error deleting hotel:', error);
    toast.error('Failed to delete hotel from server');
    // Optionally refetch to restore the item if deletion failed
    await fetchHotels();
  }
};

const cancelDelete = () => {
  showDeleteModal.value = false;
  hotelToDelete.value = null;
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
};

const closeFormModal = () => {
  showFormModal.value = false;
  selectedHotel.value = {};
};

const handleFormSubmit = async ({ mode, data }) => {
  loading.value = true;
  try {
    // Validate required fields
    if (!data.name?.trim()) {
      toast.error('Hotel name is required');
      return;
    } 
    if (!data.city?.trim()) {
      toast.error('City is required');
      return;
    }
    if (!data.pricePerNight || data.pricePerNight <= 0) {
      toast.error('Price per night must be greater than 0');
      return;
    }
    if (!data.overview?.trim()) {
      toast.error('Description is required');
      return;
    }

    const priceVal = Number(String(data.pricePerNight || 0).replace(/[^0-9.]/g, ''));

    const payload = {
      Id: mode === 'edit' ? Number(selectedHotel.value.id) : 0,
      Name: data.name?.trim() || "",
      City: data.city?.trim() || "",
      BasePrice: priceVal,
      PricePerNight: priceVal,
      Description: (data.overview || data.description || "")?.trim(),
      Images: Array.isArray(data.images) ? data.images.filter(img => typeof img === 'string') : [],
      Facilities: Array.isArray(data.amenities) ? data.amenities.filter(f => typeof f === 'string') : [],
      Rooms: [], // Default empty rooms
      Latitude: Number(data.latitude || 0),
      Longitude: Number(data.longitude || 0)
    };

    if (mode === 'add') {
      await hotelsAPI.create(payload);
      toast.success('Hotel created successfully');
    } else {
      await hotelsAPI.update(selectedHotel.value.id, payload);
      toast.success('Hotel updated successfully');
    }
    closeFormModal();
    await fetchHotels();
  } catch (error) {
    console.error('Error submitting form:', error);
    console.error('Error response:', error.response?.data);
    const errorMessage = error.response?.data?.errors 
      ? Object.values(error.response.data.errors).flat().join(', ')
      : error.response?.data?.message || error.message;
    toast.error(`Failed to save hotel: ${errorMessage}`);
  } finally {
    loading.value = false;
  }
};

// Lifecycle Hooks
onMounted(() => {
  fetchHotels();
});

</script>
