<template>
  <div class="w-full">
    <!-- Hotels Search Bar -->
    <div v-if="type === 'hotels'" class="flex flex-wrap items-end gap-4 p-4 bg-base-100 rounded-glass-radius shadow-lg">
      <div class="flex-1 min-w-[200px]">
        <label class="text-sm text-base-content/70 mb-1 block font-cairo text-start">{{ labels.destination }}</label>
        <div class="relative">
          <span class="absolute left-3 top-1/2 -translate-y-1/2 text-primary z-10">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
          </span>
          <input 
            type="text" 
            :placeholder="placeholders.destination"
            v-model="searchData.destination"
            class="input input-bordered w-full pl-10 bg-base-200 font-cairo"
          />
        </div>
      </div>

      <div class="flex-1 min-w-[150px]">
        <DatePicker
          v-model="searchData.checkIn"
          :label="labels.checkIn"
          placeholder="Select check-in date"
          input-class="bg-base-200"
        />
      </div>

      <div class="flex-1 min-w-[150px]">
        <DatePicker
          v-model="searchData.checkOut"
          :label="labels.checkOut"
          placeholder="Select check-out date"
          input-class="bg-base-200"
        />
      </div>

      <div class="flex-1 min-w-[120px]">
        <label class="text-sm text-base-content/70 mb-1 block font-cairo text-start">{{ labels.guests }}</label>
        <div class="relative">
          <span class="absolute left-3 top-1/2 -translate-y-1/2 text-primary z-10">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
          </span>
          <input 
            type="number" 
            min="1"
            v-model="searchData.guests"
            class="input input-bordered w-full pl-10 bg-base-200 font-cairo"
          />
        </div>
      </div>

      <div class="flex-1 min-w-[120px]">
        <label class="text-sm text-base-content/70 mb-1 block font-cairo text-start">{{ labels.rooms }}</label>
        <div class="relative">
          <span class="absolute left-3 top-1/2 -translate-y-1/2 text-primary z-10">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
            </svg>
          </span>
          <input 
            type="number" 
            min="1"
            v-model="searchData.rooms"
            class="input input-bordered w-full pl-10 bg-base-200 font-cairo"
          />
        </div>
      </div>

      <button @click="handleSearch" class="btn btn-primary min-w-[120px] h-12 font-cairo">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
        {{ labels.search }}
      </button>
    </div>

    <!-- Attractions Search Bar -->
    <div v-else-if="type === 'attractions'" class="flex flex-wrap items-end gap-4 p-4 bg-base-100 rounded-glass-radius shadow-lg">
      <div class="flex-1 min-w-[200px]">
        <select v-model="searchData.city" class="select select-bordered w-full bg-base-200 font-cairo">
          <option disabled selected value="">{{ placeholders.city }}</option>
          <option v-for="city in cities" :key="city" :value="city">{{ city }}</option>
        </select>
      </div>

      <div class="flex-1 min-w-[250px]">
        <input 
          type="text" 
          :placeholder="placeholders.searchActivities"
          v-model="searchData.query"
          class="input input-bordered w-full bg-base-200 font-cairo"
        />
      </div>

      <button @click="handleSearch" class="btn btn-primary min-w-[120px] h-12 font-cairo">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
        {{ labels.search }}
      </button>

      <button v-if="showAiPlanner" @click="openAiPlanner" class="btn btn-outline btn-primary min-w-[150px] h-12 font-cairo">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
        </svg>
        {{ labels.aiPlanner }}
      </button>
    </div>

    <!-- Trips Search Bar (بدون Time) -->
    <div v-else-if="type === 'trips'" class="flex flex-wrap items-end gap-4 p-4 bg-base-100 rounded-glass-radius shadow-lg">
      <div class="flex-1 min-w-[200px]">
        <label class="text-sm text-base-content/70 mb-1 block font-cairo text-start">{{ labels.pickupLocation }}</label>
        <div class="relative">
          <span class="absolute left-3 top-1/2 -translate-y-1/2 text-primary z-10">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
          </span>
          <input 
            type="text" 
            :placeholder="placeholders.pickupLocation"
            v-model="searchData.pickupLocation"
            class="input input-bordered w-full pl-10 bg-base-200 font-cairo"
          />
        </div>
      </div>

      <div class="flex-1 min-w-[180px]">
        <DatePicker
          v-model="searchData.pickupDate"
          :label="labels.pickupDate"
          placeholder="Select pickup date"
          input-class="bg-base-200"
        />
      </div>

      <div class="flex-1 min-w-[180px]">
        <DatePicker
          v-model="searchData.dropoffDate"
          :label="labels.dropoffDate"
          placeholder="Select drop-off date"
          input-class="bg-base-200"
        />
      </div>

      <button @click="handleSearch" class="btn btn-primary min-w-[120px] h-12 font-cairo">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
        {{ labels.search }}
      </button>

      <button v-if="showAiPlanner" @click="openAiPlanner" class="btn btn-outline btn-primary min-w-[150px] h-12 font-cairo">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
        </svg>
        {{ labels.aiPlanner }}
      </button>
    </div>

    <!-- Car Rental Search Bar (مع Time) -->
    <div v-else-if="type === 'cars'" class="flex flex-wrap items-end gap-4 p-4 bg-base-100 rounded-glass-radius shadow-lg">
      <div class="flex-1 min-w-[200px]">
        <label class="text-sm text-base-content/70 mb-1 block font-cairo text-start">{{ labels.pickupLocation }}</label>
        <div class="relative">
          <span class="absolute left-3 top-1/2 -translate-y-1/2 text-primary z-10">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
          </span>
          <input 
            type="text" 
            :placeholder="placeholders.pickupLocation"
            v-model="searchData.pickupLocation"
            class="input input-bordered w-full pl-10 bg-base-200 font-cairo"
          />
        </div>
      </div>

      <div class="flex-1 min-w-[150px]">
        <DatePicker
          v-model="searchData.pickupDate"
          :label="labels.pickupDate"
          placeholder="Select pickup date"
          input-class="bg-base-200"
        />
      </div>

      <div class="flex-1 min-w-[120px]">
        <TimePicker
          v-model="searchData.pickupTime"
          :label="labels.pickupTime"
          placeholder="Select time"
          input-class="bg-base-200"
        />
      </div>

      <div class="flex-1 min-w-[150px]">
        <DatePicker
          v-model="searchData.dropoffDate"
          :label="labels.dropoffDate"
          placeholder="Select drop-off date"
          input-class="bg-base-200"
        />
      </div>

      <div class="flex-1 min-w-[120px]">
        <TimePicker
          v-model="searchData.dropoffTime"
          :label="labels.dropoffTime"
          placeholder="Select time"
          input-class="bg-base-200"
        />
      </div>

      <button @click="handleSearch" class="btn btn-primary min-w-[120px] h-12 font-cairo">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
        {{ labels.search }}
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';
import DatePicker from '@/components/Common/DatePicker.vue';
import TimePicker from '@/components/Common/TimePicker.vue';

const props = defineProps({
  type: {
    type: String,
    required: true,
    validator: (value) => ['hotels', 'attractions', 'trips', 'cars'].includes(value)
  },
  showAiPlanner: {
    type: Boolean,
    default: false
  },
  cities: {
    type: Array,
    default: () => ['All Cities', 'Cairo', 'Alexandria', 'Luxor', 'Aswan', 'Hurghada', 'Sharm El Sheikh']
  },
  initialData: {
    type: Object,
    default: () => ({})
  },
  // Client-side filtering props
  clientSideMode: {
    type: Boolean,
    default: false
  },
  dataToFilter: {
    type: Array,
    default: () => []
  }
});

const emit = defineEmits(['search', 'ai-planner', 'filtered-results']);

// Search data state
const searchData = ref({
  // Hotels
  destination: '',
  checkIn: null,
  checkOut: null,
  guests: 2,
  rooms: 1,
  
  // Attractions
  city: '',
  query: '',
  
  // Trips (بدون Time)
  pickupLocation: '',
  pickupDate: null,
  dropoffDate: null,
  
  // Cars (مع Time)
  pickupTime: '10:00',
  dropoffTime: '10:00'
});

// Debounce timer
let debounceTimer = null;

// Initialize with any provided data
watch(() => props.initialData, (newData) => {
  searchData.value = { ...searchData.value, ...newData };
}, { immediate: true });

// Client-side filtering logic
const filteredResults = computed(() => {
  if (!props.clientSideMode || !props.dataToFilter || props.dataToFilter.length === 0) {
    return props.dataToFilter;
  }

  let results = [...props.dataToFilter];

  if (props.type === 'hotels') {
    // Filter by destination
    if (searchData.value.destination && searchData.value.destination.trim() !== '') {
      const searchTerm = searchData.value.destination.toLowerCase();
      results = results.filter(hotel => 
        hotel.name?.toLowerCase().includes(searchTerm) ||
        hotel.city?.toLowerCase().includes(searchTerm) ||
        hotel.location?.toLowerCase().includes(searchTerm)
      );
    }

    // Filter by guests
    if (searchData.value.guests) {
      results = results.filter(hotel => 
        !hotel.maxGuests || hotel.maxGuests >= searchData.value.guests
      );
    }

    // Filter by rooms
    if (searchData.value.rooms) {
      results = results.filter(hotel => 
        !hotel.availableRooms || hotel.availableRooms >= searchData.value.rooms
      );
    }

    // Note: Date filtering would require availability data from the hotel object
  } else if (props.type === 'attractions') {
    // Filter by city
    if (searchData.value.city && searchData.value.city !== '' && searchData.value.city !== 'All Cities') {
      results = results.filter(attraction => 
        attraction.city?.toLowerCase() === searchData.value.city.toLowerCase()
      );
    }

    // Filter by search query
    if (searchData.value.query && searchData.value.query.trim() !== '') {
      const searchTerm = searchData.value.query.toLowerCase();
      results = results.filter(attraction => 
        attraction.name?.toLowerCase().includes(searchTerm) ||
        attraction.description?.toLowerCase().includes(searchTerm) ||
        attraction.category?.toLowerCase().includes(searchTerm) ||
        attraction.city?.toLowerCase().includes(searchTerm)
      );
    }
  } else if (props.type === 'trips') {
    // Filter by pickup location
    if (searchData.value.pickupLocation && searchData.value.pickupLocation.trim() !== '') {
      const searchTerm = searchData.value.pickupLocation.toLowerCase();
      results = results.filter(trip => 
        trip.pickupLocation?.toLowerCase().includes(searchTerm) ||
        trip.destination?.toLowerCase().includes(searchTerm) ||
        trip.city?.toLowerCase().includes(searchTerm) ||
        trip.name?.toLowerCase().includes(searchTerm)
      );
    }

    // Note: Date filtering would require availability data from the trip object
  } else if (props.type === 'cars') {
    // Filter by pickup location
    if (searchData.value.pickupLocation && searchData.value.pickupLocation.trim() !== '') {
      const searchTerm = searchData.value.pickupLocation.toLowerCase();
      results = results.filter(car => 
        car.pickupLocation?.toLowerCase().includes(searchTerm) ||
        car.location?.toLowerCase().includes(searchTerm) ||
        car.city?.toLowerCase().includes(searchTerm) ||
        car.name?.toLowerCase().includes(searchTerm)
      );
    }

    // Note: Date and time filtering would require availability data from the car object
  }

  return results;
});

// Watch for changes in search data and emit filtered results in client-side mode
watch([searchData, () => props.dataToFilter], () => {
  if (props.clientSideMode) {
    // Debounce the filtering to improve performance
    if (debounceTimer) {
      clearTimeout(debounceTimer);
    }
    
    debounceTimer = setTimeout(() => {
      emit('filtered-results', filteredResults.value);
    }, 300);
  }
}, { deep: true });

// Labels based on type
const labels = computed(() => {
  const common = {
    search: 'Search'
  };

  if (props.type === 'hotels') {
    return {
      ...common,
      destination: 'Destination',
      checkIn: 'Check-in',
      checkOut: 'Check-out',
      guests: 'Guests',
      rooms: 'Rooms'
    };
  } else if (props.type === 'attractions') {
    return {
      ...common,
      city: 'City',
      searchActivities: 'Search',
      aiPlanner: 'AI Trip Planner'
    };
  } else if (props.type === 'trips') {
    return {
      ...common,
      pickupLocation: 'Pick-up Location',
      pickupDate: 'Pick-up date',
      dropoffDate: 'Drop-off date',
      aiPlanner: 'AI Trip Planner'
    };
  } else {
    // Trips & Cars
    return {
      ...common,
      pickupLocation: 'Pick-up Location',
      pickupDate: 'Pick-up date',
      pickupTime: 'Time',
      dropoffDate: 'Drop-off date',
      dropoffTime: 'Time'
    };
  }
});

// Placeholders
const placeholders = computed(() => {
  if (props.type === 'hotels') {
    return {
      destination: 'City or station'
    };
  } else if (props.type === 'attractions') {
    return {
      city: 'All Cities',
      searchActivities: 'Search for museums, parks, activities...'
    };
  } else {
    return {
      pickupLocation: 'City or station'
    };
  }
});

const handleSearch = () => {
  if (props.clientSideMode) {
    // In client-side mode, emit the filtered results immediately
    emit('filtered-results', filteredResults.value);
  }
  // Always emit the search event for backward compatibility
  emit('search', searchData.value);
};

const openAiPlanner = () => {
  emit('ai-planner');
};
</script>

<style scoped>
/* تأكد إن الـ DatePicker و TimePicker يظهروا فوق باقي العناصر */
:deep(.z-50) {
  z-index: 50 !important;
}

.font-cairo {
  font-family: 'Cairo', sans-serif;
}
</style>