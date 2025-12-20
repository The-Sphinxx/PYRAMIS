<template>
  <div class="min-h-screen bg-base-200">
    <!-- Hero Section with Search -->
    <div 
      class="relative bg-cover bg-center min-h-[585px] flex items-center"
      style="background-image: linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url('/images/hero-hotels.jpg')"
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
          @search="handleSearch"
        />
      </div>
    </div>

    <!-- Results Section -->
    <div class="page-container py-8">
      <!-- Header with Filter Button -->
      <div class="flex items-center justify-between mb-8">
        <h2 class="text-3xl font-bold text-orange-600 font-cairo">All Results</h2>
        
        <!-- Filter Component -->
        <Filter
          :show-price-filter="true"
          :price-range="{ min: 50, max: 1000 }"
          :category-options="categoryOptions"
          :custom-filters="customFilterOptions"
          @filter-change="handleFilterChange"
        />
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center py-20">
        <span class="loading loading-spinner loading-lg text-primary"></span>
      </div>

      <!-- Hotels Grid (4 columns like HotelsList) -->
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
          :total="filteredHotels.length"
          :per-page="itemsPerPage"
          :show-info="true"
          :show-per-page-selector="true"
          :per-page-options="[16, 20, 24, 32]"
          @update:current-page="handlePageChange"
          @update:per-page="handlePerPageChange"
        />
      </div>

      <!-- No Resul ts -->
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
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useHotelStore } from '@/stores/hotelsStore';
import HotelCard from '@/components/Hotels/HotelCard.vue';
import Search from '@/components/Common/Search.vue';
import Filter from '@/components/Common/Filter.vue';
import Pagination from '@/components/Common/Pagination.vue';

const router = useRouter();
const hotelStore = useHotelStore();

// State
const currentPage = ref(1);
const itemsPerPage = ref(16); // 4 columns x 4 rows
const loading = ref(true);
const appliedFilters = ref({
  maxPrice: 1000,
  categories: [],
  rating: [],
  amenities: []
});

// Category options for filter
const categoryOptions = ref([
  { label: 'Luxury Hotels', value: 'LUXURY' },
  { label: 'Resorts & Spas', value: 'RESORT' },
  { label: 'Budget Friendly', value: 'BUDGET' },
  { label: 'Five Star', value: 'FIVE_STAR' },
  { label: 'Family Hotels', value: 'FAMILY' },
  { label: 'Boutique Hotels', value: 'BOUTIQUE' },
  { label: 'Beach Front', value: 'BEACH' }
]);

// Custom filter options
const customFilterOptions = ref([
  {
    key: 'rating',
    title: 'Rating',
    options: [
      { label: '5 Stars', value: 5 },
      { label: '4+ Stars', value: 4 },
      { label: '3+ Stars', value: 3 }
    ]
  },
  {
    key: 'amenities',
    title: 'Amenities',
    options: [
      { label: 'Free Wi-Fi', value: 'wifi' },
      { label: 'Swimming Pool', value: 'pool' },
      { label: 'Fitness Center', value: 'gym' },
      { label: 'Spa', value: 'spa' },
      { label: 'Restaurant', value: 'restaurant' },
      { label: 'Parking', value: 'parking' }
    ]
  }
]);

// Computed
const availableCities = computed(() => {
  const cities = ['All Cities', ...hotelStore.uniqueCities];
  return cities;
});

const filteredHotels = computed(() => {
  let result = hotelStore.hotels;

  // Filter by search query from store
  if (hotelStore.filters.searchQuery) {
    const query = hotelStore.filters.searchQuery.toLowerCase();
    result = result.filter(hotel =>
      hotel.name.toLowerCase().includes(query) ||
      hotel.city.toLowerCase().includes(query) ||
      (hotel.highlights && hotel.highlights.some(h => h.toLowerCase().includes(query)))
    );
  }

  // Filter by city from store
  if (hotelStore.filters.city && hotelStore.filters.city !== 'All' && hotelStore.filters.city !== 'All Cities') {
    result = result.filter(hotel => hotel.city === hotelStore.filters.city);
  }

  // Filter by categories
  if (appliedFilters.value.categories?.length > 0) {
    result = result.filter(hotel => 
      appliedFilters.value.categories.includes(hotel.category)
    );
  }

  // Filter by price
  if (appliedFilters.value.maxPrice) {
    result = result.filter(hotel => 
      hotel.pricePerNight <= appliedFilters.value.maxPrice
    );
  }

  // Filter by rating
  if (appliedFilters.value.rating?.length > 0) {
    const minRating = Math.min(...appliedFilters.value.rating);
    result = result.filter(hotel => hotel.rating >= minRating);
  }

  return result;
});

const paginatedHotels = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  
  // Map hotels to match HotelCard's expected prop structure
  return filteredHotels.value.slice(start, end).map(hotel => {
    // Extract top 3 amenities from hotel data
    let amenities = ['Wifi', 'Pool', 'Gym']; // Default fallback
    
    if (hotel.amenities && hotel.amenities.length > 0) {
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
        .slice(0, 5)
        .map(amenity => {
          for (const [key, value] of Object.entries(amenityMap)) {
            if (amenity.includes(key)) return value;
          }
          return null;
        })
        .filter((a, index, self) => a && self.indexOf(a) === index)
        .slice(0, 3);
      
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
const handleSearch = (searchData) => {
  if (searchData.query) {
    hotelStore.setFilter('searchQuery', searchData.query);
  }
  
  if (searchData.city && searchData.city !== 'All Cities') {
    hotelStore.setFilter('city', searchData.city);
  }
  
  currentPage.value = 1;
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

const handleFilterChange = (filters) => {
  appliedFilters.value = { ...filters };
  currentPage.value = 1;
  window.scrollTo({ top: 400, behavior: 'smooth' });
};

const handlePageChange = (page) => {
  currentPage.value = page;
  window.scrollTo({ top: 400, behavior: 'smooth' });
};

const handlePerPageChange = (perPage) => {
  itemsPerPage.value = perPage;
  currentPage.value = 1;
};

const handleBookNow = (hotel) => {
  // Navigate to hotel details page
  router.push({
    name: 'HotelDetails',
    params: { id: hotel.id }
  });
};

const resetFilters = () => {
  appliedFilters.value = {
    maxPrice: 1000,
    categories: [],
    rating: [],
    amenities: []
  };
  hotelStore.resetFilters();
  currentPage.value = 1;
};

// Lifecycle
onMounted(async () => {
  try {
    await hotelStore.fetchHotels();
  } catch (error) {
    console.error('Error loading hotels:', error);
  } finally {
    loading.value = false;
  }
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
