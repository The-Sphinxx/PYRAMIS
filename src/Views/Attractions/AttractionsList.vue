<template>
  <div class="min-h-screen bg-base-200">
    <!-- Hero Section with Search -->
    <div 
      class="relative bg-cover bg-center min-h-[585px] flex items-start pt-32 lg:items-center lg:pt-20"
      style="background-image: linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url('/images/hero-attractions.jpg')"
    >
      <div class="page-container py-16">
        <!-- Hero Text -->
        <div class="text-center text-white mb-8">
          <h1 class="text-5xl font-bold mb-4 font-cairo">Explore Top Attractions in Egypt</h1>
          <p class="text-xl">Discover ancient wonders, vibrant culture, and unforgettable experiences across the land of pharaohs.</p>
        </div>

        <!-- Search Bar Component -->
        <Search
          type="attractions"
          :show-ai-planner="true"
          :cities="availableCities"
          @search="handleSearch"
          @ai-planner="openAiPlanner"
        />
      </div>
    </div>

    <!-- Categories Tabs -->
    <div class="page-container py-8">
      <div class="bg-base-100 rounded-2xl shadow-lg p-8 mb-8">
        <div class="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-8 gap-4">
          <button
            v-for="category in categories" 
            :key="category.id"
            @click="selectCategory(category.id)"
            :class="[
              'flex flex-col items-center gap-3 p-4 rounded-xl transition-all duration-300',
              selectedCategory === category.id 
                ? 'bg-primary/10 border-2 border-primary' 
                : 'bg-base-200 hover:bg-base-300 border-2 border-transparent'
            ]"
          >
            <div 
              :class="[
                'w-16 h-16 rounded-xl flex items-center justify-center text-3xl transition-all duration-300',
                selectedCategory === category.id 
                  ? 'bg-primary text-primary-content' 
                  : 'bg-base-100 text-neutral'
              ]"
            >
              <i :class="category.icon"></i>
            </div>
            <span 
              :class="[
                'text-sm font-semibold text-center leading-tight',
                selectedCategory === category.id 
                  ? 'text-primary' 
                  : 'text-base-content'
              ]"
            >
              {{ category.name }}
            </span>
          </button>
        </div>
      </div>

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

      <!-- Attractions Grid -->
      <div v-else-if="paginatedAttractions.length > 0" class="mb-8">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-8">
          <AttractionCard
            v-for="attraction in paginatedAttractions"
            :key="attraction.id"
            :id="Number(attraction.id)"
            :image="attraction.imageUrl"
            :title="attraction.name"
            :price="attraction.price"
            :location="attraction.city"
            :rating="attraction.rating"
            :reviews="attraction.totalReviews || 0"
            button-text="View Details"
            reviews-text="reviews"
            :show-price="true"
            @view-details="handleViewDetails"
          />
        </div>

        <!-- Pagination Component -->
        <Pagination
          :current-page="currentPage"
          :total="filteredAttractions.length"
          :per-page="itemsPerPage"
          :show-info="true"
          :show-per-page-selector="true"
          :per-page-options="[8, 12, 16, 24]"
          @update:current-page="handlePageChange"
          @update:per-page="handlePerPageChange"
        />
      </div>

      <!-- No Results -->
      <div v-else class="text-center py-20">
        <i class="fas fa-search text-6xl text-neutral mb-4"></i>
        <h3 class="text-2xl font-bold text-base-content mb-2">No attractions found</h3>
        <p class="text-neutral mb-4">Try adjusting your search or filters</p>
        <button @click="resetFilters" class="btn btn-primary">
          Reset Filters
        </button>
      </div>
    </div>

    <!-- Error State -->
    <div v-if="attractionStore.error" class="page-container alert alert-error my-6">
      <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l-2-2m0 0l-2-2m2 2l2-2m-2 2l-2 2m9-11a9 9 0 11-18 0 9 9 0 0118 0z" />
      </svg>
      <span>{{ attractionStore.error }}</span>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAttractionStore } from '@/stores/attractionStore';
import AttractionCard from '@/components/Attractions/AttractionCard.vue';
import Search from '@/components/Common/Search.vue';
import Pagination from '@/components/Common/Pagination.vue';

const router = useRouter();
const attractionStore = useAttractionStore();

// State
const selectedCategory = ref('all');
const currentPage = ref(1);
const itemsPerPage = ref(12);
const loading = ref(true);

// Categories with icons and descriptions
const categories = ref([
  { 
    id: 'all', 
    name: "Discover Egypt's Gems", 
    icon: 'fas fa-star',
    description: 'The best attractions recommended for you'
  },
  { 
    id: 'HISTORICAL', 
    name: 'Historical Sites', 
    icon: 'fas fa-landmark',
    description: 'Step back in time and explore ancient monuments and archaeological wonders'
  },
  { 
    id: 'MUSEUMS', 
    name: 'Museums & Galleries', 
    icon: 'fas fa-university',
    description: 'Discover Egypt\'s rich heritage through world-class museums and exhibitions'
  },
  { 
    id: 'RELIGIOUS', 
    name: 'Religious Sites', 
    icon: 'fas fa-mosque',
    description: 'Visit sacred places and experience spiritual heritage across centuries'
  },
  { 
    id: 'NATURE', 
    name: 'Beaches & Nature', 
    icon: 'fas fa-tree',
    description: 'Relax on pristine beaches and explore Egypt\'s natural beauty'
  },
  { 
    id: 'TOP PICKS', 
    name: 'Adventure & Outdoor', 
    icon: 'fas fa-person-hiking',
    description: 'Experience thrilling adventures and outdoor activities across Egypt'
  },
  { 
    id: 'FAMILY', 
    name: 'Family & Friendly', 
    icon: 'fas fa-users',
    description: 'Fun-filled attractions perfect for families and children of all ages'
  },
  { 
    id: 'CULTURE', 
    name: 'Cultural Experiences', 
    icon: 'fas fa-masks-theater',
    description: 'Immerse yourself in authentic Egyptian culture and traditions'
  }
]);

// Computed
const availableCities = computed(() => {
  const cities = ['All Cities', ...attractionStore.uniqueCities];
  return cities;
});

const currentCategoryTitle = computed(() => {
  const category = categories.value.find(cat => cat.id === selectedCategory.value);
  return category ? category.name : 'All Attractions';
});

const currentCategoryDescription = computed(() => {
  const category = categories.value.find(cat => cat.id === selectedCategory.value);
  return category ? category.description : 'The best attractions recommended for you';
});

const filteredAttractions = computed(() => {
  return attractionStore.getFilteredAttractions;
});

const paginatedAttractions = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return filteredAttractions.value.slice(start, end);
});

// Methods
const selectCategory = (categoryId) => {
  selectedCategory.value = categoryId;
  
  if (categoryId === 'all') {
    attractionStore.setCategoriesFilter([]);
  } else {
    // Pass as array to store
    attractionStore.setCategoriesFilter([categoryId]);
  }

  currentPage.value = 1;
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

const handleSearch = (searchData) => {
  // Update store filters
  if (searchData.query) {
    attractionStore.setFilter('searchQuery', searchData.query);
  }
  
  if (searchData.city && searchData.city !== 'All Cities') {
    attractionStore.setFilter('city', searchData.city);
  }
  
  currentPage.value = 1;
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

const handlePageChange = (page) => {
  currentPage.value = page;
  window.scrollTo({ top: 400, behavior: 'smooth' });
};

const handlePerPageChange = (perPage) => {
  itemsPerPage.value = perPage;
  currentPage.value = 1;
};

const handleViewDetails = (attraction) => {
  router.push({
    name: 'AttractionDetails',
    params: { id: attraction.id }
  });
};

const openAiPlanner = () => {
  router.push({ name: 'AiTripPlanner' });
};

const resetFilters = () => {
  selectedCategory.value = 'all';
  attractionStore.resetFilters();
  currentPage.value = 1;
};

/* Random reviews removed to prevent hydration mismatch */

// Get first image from images array or fallback to imageUrl
const getAttractionImage = (attraction) => {
  if (attraction.images && attraction.images.length > 0) {
    return attraction.images[0];
  }
  return attraction.imageUrl || '/images/placeholder.jpg';
};

// Get reviews count from attraction data or random
const getAttractionReviews = (attraction) => {
  if (attraction.reviews && attraction.reviews.totalReviews) {
    return attraction.reviews.totalReviews;
  }
  return Math.floor(Math.random() * 5000) + 100;
};

// Lifecycle
onMounted(async () => {
  try {
    await attractionStore.fetchAttractions();
  } catch (error) {
    console.error('Error loading attractions:', error);
  } finally {
    loading.value = false;
  }
});

onUnmounted(() => {
  attractionStore.resetFilters();
  selectedCategory.value = 'all';
});
</script>

<style scoped>
.font-cairo {
  font-family: 'Cairo', sans-serif;
}

/* Smooth transitions */
/* Smooth transitions removed for better performance */
</style>