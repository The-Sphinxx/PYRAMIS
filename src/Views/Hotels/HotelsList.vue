<template>
  <div class="min-h-screen bg-base-200">
    <!-- Hero Section with Search -->
    <div 
      class="relative bg-cover bg-center min-h-[585px] flex items-start pt-32 lg:items-center lg:pt-20"
      style="background-image: linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url('/images/hotels.png')"
    >
      <div class="page-container py-16">
        <!-- Hero Text -->
        <div class="text-center text-white mb-8">
          <h1 class="text-5xl font-bold mb-4 font-cairo">Where Your Trip Begins</h1>
          <p class="text-xl">Find your perfect hotel in the land of ancient wonders</p>
        </div>

        <!-- Search Bar Component -->
        <Search
          type="hotels"
          :show-ai-planner="false"
          :cities="availableCities"
          :client-side-mode="true"
          :data-to-filter="paginatedHotels"
          :initial-data="initialSearchData"
          @search="handleSearch"
          @filtered-results="handleFilteredResults"
        />
      </div>
    </div>

    <!-- Categories Tabs -->
    <div class="page-container py-8">
      <!-- Section Title -->
      <div class="text-center mb-8">
        <h2 class="text-4xl font-bold text-base-content font-cairo mb-2">
          {{ currentCategoryTitle }}
        </h2>
        <p class="text-lg text-neutral">
          {{ currentCategoryDescription }}
        </p>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center py-20">
        <span class="loading loading-spinner loading-lg text-primary"></span>
      </div>

      <!-- Hotels Grid -->
      <div v-else-if="paginatedHotels.length > 0" class="mb-8">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-8">
          <HotelCard
            v-for="hotel in paginatedHotels"
            :key="hotel.id"
            :hotel="hotel"
            @book="handleBookNow"
          />
        </div>

        <!-- Pagination Component -->
        <Pagination
          :current-page="currentPage"
          :total="totalCount"
          :per-page="itemsPerPage"
          :show-info="true"
          :show-per-page-selector="true"
          :per-page-options="[16, 20, 24, 32]"
          @update:current-page="handlePageChange"
          @update:per-page="handlePerPageChange"
        />
      </div>

      <!-- No Results -->
      <div v-else class="text-center py-20">
        <i class="fas fa-hotel text-6xl text-neutral mb-4"></i>
        <h3 class="text-2xl font-bold text-base-content mb-2">No hotels found</h3>
        <p class="text-neutral mb-4">Try adjusting your search or filters</p>
        <button @click="resetFilters" class="btn btn-primary">
          Reset Filters
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useHotelsStore } from '@/stores/hotelsStore';
import HotelCard from '@/components/Hotels/HotelCard.vue';
import Search from '@/components/Common/Search.vue';
import Pagination from '@/components/Common/Pagination.vue';

const router = useRouter();
const route = useRoute();
const hotelStore = useHotelsStore();

// State
const selectedCategory = ref('all');
const currentPage = ref(1);
const itemsPerPage = ref(16);
const loading = ref(true);
const fetchKey = ref(0);

// Client-side filtering state
const filteredHotels = ref([]);
const isFiltering = ref(false);
const initialSearchData = ref({});

const totalPages = computed(() => {
  if (isFiltering.value) {
    return Math.ceil(filteredHotels.value.length / itemsPerPage.value);
  }
  return hotelStore.pagination.totalPages;
});

const totalCount = computed(() => {
  if (isFiltering.value) {
    return filteredHotels.value.length;
  }
  return hotelStore.pagination.totalCount;
});

const fetchHotelsData = async (page = 1) => {
  const params = {
    pageIndex: page - 1, // API uses 0-based indexing
    pageSize: itemsPerPage.value
  };
  
  // Add filters if any
  if (selectedCategory.value !== 'all') {
    params.category = selectedCategory.value;
  }
  // Don't check store filters here - they cause conflicts
  
  await hotelStore.fetchHotels(params);
};

// Categories with icons and descriptions
const categories = ref([
  { 
    id: 'all', 
    name: "Luxury Hotels & Resorts", 
    icon: 'fas fa-hotel',
    description: 'Experience Egyptian hospitality at its finest in our handpicked selection of premium hotels'
  },
  { 
    id: 'LUXURY', 
    name: 'Luxury Hotels', 
    icon: 'fas fa-crown',
    description: 'Indulge in world-class luxury with exceptional service and premium amenities'
  },
  { 
    id: 'RESORT', 
    name: 'Resorts & Spas', 
    icon: 'fas fa-spa',
    description: 'Relax and rejuvenate at Egypt\'s finest resort destinations with spa facilities'
  },
  { 
    id: 'BUDGET', 
    name: 'Budget Friendly', 
    icon: 'fas fa-money-bill-wave',
    description: 'Comfortable and affordable accommodations without compromising on quality'
  },
  { 
    id: 'FIVE_STAR', 
    name: 'Five Star', 
    icon: 'fas fa-star',
    description: 'Top-rated hotels offering unparalleled luxury and impeccable service'
  },
  { 
    id: 'FAMILY', 
    name: 'Family Hotels', 
    icon: 'fas fa-users',
    description: 'Family-friendly hotels with activities and amenities for all ages'
  },
  { 
    id: 'BOUTIQUE', 
    name: 'Boutique Hotels', 
    icon: 'fas fa-building',
    description: 'Unique and intimate hotels with distinctive character and personalized service'
  },
  { 
    id: 'BEACH', 
    name: 'Beach Front', 
    icon: 'fas fa-umbrella-beach',
    description: 'Wake up to stunning sea views at Egypt\'s premier beachfront properties'
  }
]);

// Computed
const availableCities = computed(() => {
  const cities = ['All Cities', ...hotelStore.uniqueCities];
  return cities;
});

const currentCategoryTitle = computed(() => {
  const category = categories.value.find(cat => cat.id === selectedCategory.value);
  return category ? category.name : 'All Hotels';
});

const currentCategoryDescription = computed(() => {
  const category = categories.value.find(cat => cat.id === selectedCategory.value);
  return category ? category.description : 'Experience Egyptian hospitality at its finest';
});

const paginatedHotels = computed(() => {
  // Map hotels to match HotelCard's expected prop structure
  const sourceData = isFiltering.value ? filteredHotels.value : hotelStore.hotels;
  
  return sourceData.map(hotel => {
    // Extract top 3 amenities from hotel data
    let amenities = ['Wifi', 'Pool', 'Gym']; // Default fallback
    
    if (hotel.amenities && hotel.amenities.length > 0) {
      // Select up to 3 diverse amenities from the hotel's actual amenities
      const amenityMap = {
        'Free Wi-Fi': 'Wifi',
        'Wi-Fi': 'Wifi',
        'WiFi': 'Wifi',
        'Outdoor Swimming Pool': 'Pool',
        'Swimming Pool': 'Pool',
        'Pool': 'Pool',
        'Fitness Center': 'Gym',
        'Gym': 'Gym',
        'Spa and Wellness Center': 'Spa',
        'Spa': 'Spa',
        'Restaurant': 'Restaurant',
        'Multiple Restaurants and Bars': 'Restaurant',
        'Bar': 'Bar',
        'Parking': 'Parking',
        'Valet Parking': 'Parking',
        'Airport Shuttle': 'Shuttle',
        'Beach Access': 'Beach',
        'Private Beach': 'Beach'
      };
      
      amenities = hotel.amenities
        .slice(0, 5) // Take first 5 amenities
        .map(amenity => {
          // Find matching amenity type
          for (const [key, value] of Object.entries(amenityMap)) {
            if (amenity.includes(key)) return value;
          }
          return null;
        })
        .filter((a, index, self) => a && self.indexOf(a) === index) // Remove duplicates and nulls
        .slice(0, 3); // Take only 3
      
      // Fallback to default if we couldn't extract any
      if (amenities.length === 0) {
        amenities = ['Wifi', 'Pool', 'Gym'];
      }
    }
    
    return {
      id: hotel.id,
      name: hotel.name,
      image: hotel.images && hotel.images.length > 0 ? hotel.images[0] : '/images/placeholder-hotel.jpg',
      price: hotel.pricePerNight || 200,
      location: hotel.city || 'Cairo',
      rating: hotel.rating || 4.5,
      reviews: hotel.reviews?.totalReviews || hotel.totalReviews || '0',
      amenities: amenities
    };
  });
});

// Methods
const selectCategory = async (categoryId) => {
  selectedCategory.value = categoryId;
  currentPage.value = 1;
  await fetchHotelsData(1);
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

// Handle filtered results from Search component
const handleFilteredResults = (results) => {
  filteredHotels.value = results;
  isFiltering.value = results.length < hotelStore.hotels.length || 
                      (initialSearchData.value.destination || initialSearchData.value.guests || initialSearchData.value.rooms);
  currentPage.value = 1;
};

const handleSearch = (searchData) => {
  // Client-side filtering is handled automatically by the Search component
  console.log('Search triggered:', searchData);
};

const handlePageChange = async (page) => {
  currentPage.value = page;
  await fetchHotelsData(page);
  window.scrollTo({ top: 400, behavior: 'smooth' });
};

const handlePerPageChange = async (perPage) => {
  itemsPerPage.value = perPage;
  currentPage.value = 1;
  await fetchHotelsData(1);
};

const handleBookNow = (hotel) => {
  // Navigate to hotel details page (not directly to checkout)
  router.push({
    name: 'HotelDetails',
    params: { id: hotel.id }
  });
};

const resetFilters = async () => {
  selectedCategory.value = 'all';
  isFiltering.value = false;
  filteredHotels.value = [];
  initialSearchData.value = {};
  hotelStore.resetFilters();
  currentPage.value = 1;
  await fetchHotelsData(1);
};

// Lifecycle
onMounted(async () => {
  try {
    // Read query parameters from route (from Home page navigation)
    if (route.query.destination || route.query.guests || route.query.rooms) {
      initialSearchData.value = {
        destination: route.query.destination || '',
        guests: route.query.guests ? parseInt(route.query.guests) : 2,
        rooms: route.query.rooms ? parseInt(route.query.rooms) : 1,
        checkIn: route.query.checkIn || null,
        checkOut: route.query.checkOut || null
      };
    }
    
    await fetchHotelsData(currentPage.value);
  } catch (error) {
    console.error('Error loading hotels:', error);
  } finally {
    loading.value = false;
  }
});

onUnmounted(() => {
  hotelStore.resetFilters();
  selectedCategory.value = 'all';
  isFiltering.value = false;
  filteredHotels.value = [];
});
</script>

<style scoped>
.font-cairo {
  font-family: 'Cairo', sans-serif;
}

/* Smooth transitions */
* {
  transition: all 0.3s ease;
}
</style>