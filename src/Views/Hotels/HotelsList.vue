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
          :total="filteredHotels.length"
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
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useHotelsStore } from '@/stores/hotelsStore';
import HotelCard from '@/components/Hotels/HotelCard.vue';
import Search from '@/components/Common/Search.vue';
import Pagination from '@/components/Common/Pagination.vue';

const router = useRouter();
const hotelStore = useHotelsStore();

// State
const selectedCategory = ref('all');
const currentPage = ref(1);
const itemsPerPage = ref(16);
const loading = ref(true);

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

const filteredHotels = computed(() => {
  let result = hotelStore.hotels;

  // Filter by category
  if (selectedCategory.value !== 'all') {
    result = result.filter(hotel => {
      if (!hotel.category) return false;
      return hotel.category === selectedCategory.value;
    });
  }

  // Filter by store filters (search query from SearchBar)
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
const selectCategory = (categoryId) => {
  selectedCategory.value = categoryId;
  currentPage.value = 1;
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

const handleSearch = (searchData) => {
  // Update store filters
  if (searchData.query) {
    hotelStore.setFilter('searchQuery', searchData.query);
  }
  
  if (searchData.city && searchData.city !== 'All Cities') {
    hotelStore.setFilter('city', searchData.city);
  }
  
  // Navigate to filter page with search results
  router.push({ name: 'HotelFilter' });
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
  // Navigate to hotel details page (not directly to checkout)
  router.push({
    name: 'HotelDetails',
    params: { id: hotel.id }
  });
};

const resetFilters = () => {
  selectedCategory.value = 'all';
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