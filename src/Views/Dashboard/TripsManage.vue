<template>
  <div class="space-y-2">
    <!-- Stats Grid -->
    <StatsCard v-if="!compactMode" :stats="stats" />

    <!-- Trips Data Table -->
    <DataTable
      title="Trips"
      :columns="columns"
      :data="filteredTrips"
      :show-filter="true"
      add-button-text="Add New Trip"
      :show-actions="{ edit: true, delete: true, view: true }"
      empty-message="No trips available"
      :per-page="compactMode ? 5 : 8"
      :show-pagination="!compactMode"
      resource="trips"
      :loading="loading"
      @add="handleAdd"
      @edit="handleEdit"
      @delete="handleDelete"
      @toggle="handleToggle"
      @status-change="handleStatusChange"
      @view="handleView"
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
    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="modal modal-open">
      <div class="modal-box">
        <h3 class="font-bold text-lg text-error">Confirm Delete</h3>
        <p class="py-4">Are you sure you want to delete this trip package? This action cannot be undone.</p>
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
import { useToast } from '@/composables/useToast.js'; // Added import
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
import { tripsAPI } from '@/Services/dashboardApi';
import { dashboardTripFilterConfig } from '@/Utils/dashboardFilterConfigs';
import { tripFormConfig } from '@/Utils/dashboardFormConfigs';

// Component State
const loading = ref(false);
const router = useRouter();
const route = useRoute(); // Added missing route definition
const trips = ref([]);
const showFilterModal = ref(false);
const showFormModal = ref(false);
const showDeleteModal = ref(false);
const formMode = ref('add');
const selectedTrip = ref({});
const tripToDelete = ref(null);
const activeFilters = ref({});
const { toast } = useToast();

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
    field: 'destination', // Updated from 'city' to 'destination'
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
    label: 'Status',
    field: 'status',
    type: 'status-dropdown',
    options: ['Upcoming', 'Ongoing', 'Completed'],
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
    trips.value = response.data.data.map(trip => ({
      ...trip,
      name: trip.title, // Map title to name for DataTable image column
      images: Array.isArray(trip.images) ? trip.images[0] : trip.trip.image || trip.image,
      nextDate: trip.availableDates ? trip.availableDates[0] : 'TBD',
      destination: trip.destination || trip.city || 'Unknown', // Fallback for destination
      featured: trip.featured || trip.isFeatured || false,
      status: trip.status || 'Upcoming',
      price: Number(trip.price.toString().replace(/[^0-9.-]+/g,"")) // Parse currency string to number
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

  // Global Search
  if (route.query.q) {
    const search = route.query.q.toLowerCase();
    result = result.filter(t => 
      (t.title?.toLowerCase().includes(search)) || 
      (t.city?.toLowerCase().includes(search)) ||
      (t.destination?.toLowerCase().includes(search)) ||
      (t.tripType?.toLowerCase().includes(search)) ||
      (t.status?.toLowerCase().includes(search)) || // Status text match
      (search.includes('featured') && t.featured) || // "featured" keyword
      (t.price?.toString().includes(search)) // Price match
    );
  }

  if (activeFilters.value.maxPrice && activeFilters.value.maxPrice < filterConfig.priceRange.max) {
    result = result.filter(t => t.price <= activeFilters.value.maxPrice);
  }

  if (activeFilters.value.destination) {
    const searchCity = activeFilters.value.destination.toLowerCase();
    result = result.filter(t => t.city?.toLowerCase().includes(searchCity));
  }

  if (activeFilters.value.tripType) {
    result = result.filter(t => t.tripType === activeFilters.value.tripType);
  }

  if (activeFilters.value.status) {
    result = result.filter(t => t.status === activeFilters.value.status);
  }

  if (activeFilters.value.featured) {
    const isFeatured = activeFilters.value.featured === 'true';
    result = result.filter(t => (t.featured === isFeatured || t.isFeatured === isFeatured));
  }

  return result;
});

// Actions
const handleAdd = () => {
  formMode.value = 'add';
  selectedTrip.value = {};
  showFormModal.value = true;
};

const handleEdit = async (row) => {
  formMode.value = 'edit';
  loading.value = true;
  try {
    const response = await tripsAPI.getOne(row.id);
    const fullTrip = response.data;

    // Map Backend DTO to Form Structure
    // 1. Amenities: DTO has Object, Form expects Array of Strings (Tags)
    const amenitiesArray = [];
    if (fullTrip.amenities) {
      if (fullTrip.amenities.transport) amenitiesArray.push('Transport');
      if (fullTrip.amenities.accommodation) amenitiesArray.push('Accommodation');
      if (fullTrip.amenities.meals) amenitiesArray.push('Meals');
    }

    // 2. HotelInfo: DTO has hotelInfo, Form expects hotel.* keys
    selectedTrip.value = { 
      ...fullTrip,
      hotel: fullTrip.hotelInfo || {},
      amenities: amenitiesArray,
      // Ensure images is list
      images: fullTrip.images || [],
      // Ensure specific fields match form keys (camelCase)
      maxPeople: fullTrip.maxPeople || fullTrip.MaxPeople,
      availableSpots: fullTrip.availableSpots || fullTrip.AvailableSpots
    };
    showFormModal.value = true;
  } catch (error) {
    console.error("Failed to fetch details", error);
    toast.error("Could not load trip details");
  } finally {
    loading.value = false;
  }
};

const handleDelete = (row) => {
  tripToDelete.value = row;
  showDeleteModal.value = true;
};

const confirmDelete = async () => {
  if (!tripToDelete.value) return;
  
  loading.value = true;
  try {
    await tripsAPI.delete(tripToDelete.value.id);
    
    // Optimistic update
    trips.value = trips.value.filter(t => t.id !== tripToDelete.value.id);
    toast.success('Trip deleted successfully');
    showDeleteModal.value = false;
  } catch (error) {
    console.error('Error deleting trip:', error);
    toast.error('Failed to delete trip');
    await fetchTrips(); // Revert on error
  } finally {
    loading.value = false;
    tripToDelete.value = null;
  }
};

const handleView = (row) => {
  router.push({ name: 'DashboardDetails', params: { type: 'trips', id: row.id } });
};

const handleToggle = async ({ row, field, newValue }) => {
  if (field === 'featured') {
    try {
      // Assuming API supports this, if not it might need implementing in store/backend adapter
       await tripsAPI.toggleFeatured(row.id, newValue);
       await fetchTrips(); // Refresh to underlying data sync
    } catch (error) {
       console.error('Featured toggle error:', error);
       fetchTrips();
    }
  }
};



const handleStatusChange = async ({ row, field, newValue }) => {
  try {
    if (field === 'status') {
      await tripsAPI.patch(row.id, { status: newValue });
      
      // Optimistic update
      const tripIndex = trips.value.findIndex(t => t.id === row.id);
      if (tripIndex !== -1) {
        trips.value[tripIndex].status = newValue;
      }
      toast.success('Status updated successfully');
    }
  } catch (error) {
    console.error('Error:', error);
    toast.error('Failed to update status');
    await fetchTrips(); // Revert
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
    // Basic Validation
    if (!data.title?.trim()) {
      toast.error('Trip title is required');
      return;
    }
    if (!data.destination?.trim() && !data.city?.trim()) {
      toast.error('Destination is required');
      return;
    }
    if (!data.price || data.price <= 0) {
      toast.error('Valid price is required');
      return;
    }

    const payload = {
      ...data,
      title: data.title?.trim() || "",
      description: data.description?.trim() || data.title?.trim() || "",
      city: data.destination?.trim() || data.city?.trim() || "",
      destination: data.destination?.trim() || data.city?.trim() || "",
      price: Number(data.price || 0),
      duration: String(data.duration || "1 Day / 1 Night"),
      maxPeople: Number(data.maxPeople || 10),
      availableSpots: Number(data.availableSpots || data.maxPeople || 10),
      tripType: data.tripType || "Beach Getaway",
      
      hotelInfo: data['hotel.name'] ? {
        name: data['hotel.name'],
        rating: Number(data['hotel.rating'] || 4.5),
        image: "hotel-placeholder.jpg",
        features: []
      } : null,

      amenities: {
         transport: (data.amenities || []).some(a => a.toLowerCase().includes('transport')),
         accommodation: (data.amenities || []).some(a => a.toLowerCase().includes('hotel')),
         meals: (data.amenities || []).some(a => a.toLowerCase().includes('meal')) ? 1 : 0
      },

      highlights: Array.isArray(data.highlights) ? data.highlights : [],
      availableDates: Array.isArray(data.availableDates) ? data.availableDates : [],
      
      itinerary: (data.itinerary || []).map(day => ({
        day: Number(day.day || 1),
        title: day.title || "Day Title",
        description: day.description || day.title || "Day Description",
        activities: (day.activities || []).map(act => ({
          time: act.time || "09:00 AM",
          title: act.title || "Activity Title",
          description: act.desc || act.description || "Activity Description"
        }))
      })),

      images: (data.images || []).filter(img => typeof img === 'string'),
      mainImage: (data.images && data.images.length > 0 && typeof data.images[0] === 'string') ? data.images[0] : "placeholder.jpg"
    };

    if (mode === 'add') {
      const response = await tripsAPI.create(payload);
      const newId = response.data.id || response.data;
      
      const newItem = {
        ...payload,
        id: newId,
        name: payload.title, // Map title to name for DataTable
        featured: payload.isFeatured || false,
        status: payload.status || 'Upcoming',
        price: payload.price
      };
      trips.value.unshift(newItem);
      toast.success('Trip created successfully');
    } else {
      await tripsAPI.update(selectedTrip.value.id, { 
        ...payload, 
        id: selectedTrip.value.id
      });
      
      const index = trips.value.findIndex(t => t.id === selectedTrip.value.id);
      if (index !== -1) {
        trips.value[index] = { 
          ...trips.value[index], 
          ...payload,
          name: payload.title,
          featured: payload.isFeatured
        };
      }
      toast.success('Trip updated successfully');
    }
    closeFormModal();
    await fetchTrips();
  } catch (error) {
    console.error('Error submitting form:', error);
    const msg = error.response?.data?.message || 'Failed to save trip';
    toast.error(msg);
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  fetchTrips();
});
</script>