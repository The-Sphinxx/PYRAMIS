<template>
  <div class="min-h-screen bg-base-200">
    <div v-if="loading" class="flex justify-center items-center min-h-screen">
      <span class="loading loading-spinner loading-lg text-primary"></span>
    </div>

    <div v-else-if="error" class="page-container py-20 text-center">
      <i class="fas fa-exclamation-triangle text-6xl text-error mb-4"></i>
      <h2 class="text-3xl font-bold mb-4">Attraction Not Found</h2>
      <p class="text-neutral mb-6">{{ error }}</p>
      <button @click="router.push('/attractions')" class="btn btn-primary">
        Back to Attractions
      </button>
    </div>

    <div v-else-if="attraction" class="bg-base-200 pb-16 !overflow-visible">
      <div class="page-container py-4  ">
        <div class="flex items-center gap-2 text-sm text-base-content/70">
          <router-link to="/" class="hover:text-primary">Home</router-link>
          <i class="fas fa-chevron-right text-xs"></i>
          <router-link to="/attractions/list" class="hover:text-primary">Attractions</router-link>
          <i class="fas fa-chevron-right text-xs"></i>
          <span class="text-primary font-medium">{{ attraction.name }}</span>
        </div>
      </div>

      <div class="page-container mb-8">
        <Carousel
          :images="images"
          :auto-play="true"
          :auto-play-interval="5000"
        />
      </div>

      <div class="page-container !overflow-visible">
        <div class="mb-8">
          <h1 class="text-3xl lg:text-4xl font-bold text-primary font-cairo mb-4">
            {{ attraction.name }}
          </h1>
          <p class="text-lg text-base-content/70 mb-4">
            {{ attraction.description }}
          </p>
          
          <div class="flex flex-wrap items-center gap-4 text-base-content/70">
            <div class="flex items-center gap-2">
              <i class="fas fa-map-marker-alt text-primary"></i>
              <span>{{ attraction.city }}, Egypt</span>
            </div>
            <div class="flex items-center gap-2">
              <i class="fas fa-star text-warning"></i>
              <span class="font-bold text-base-content">{{ attraction.rating }}</span>
              <span>({{ attraction.reviews?.totalReviews || 0 }} reviews)</span>
            </div>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          
          <div ref="contentColumn" class="lg:col-span-2 space-y-8">
            <div class="bg-base-100 rounded-lg shadow-lg p-8">
              <div class="flex items-center gap-3 mb-6">
                <div class="w-1 h-8 bg-primary rounded-full"></div>
                <h2 class="text-2xl font-bold text-base-content font-cairo">Overview</h2>
              </div>
              <p 
                class="text-base-content/80 leading-relaxed mb-4"
                :class="{ 'line-clamp-3': !showFullOverview }"
              >
                {{ attraction.overview || attraction.description }}
              </p>
              <button 
                @click="showFullOverview = !showFullOverview"
                class="text-primary font-semibold hover:underline"
              >
                {{ showFullOverview ? 'Read Less' : 'Read More' }}
              </button>

               <div class="flex items-center gap-3 my-6">
                <div class="w-1 h-8 bg-primary rounded-full"></div>
                <h2 class="text-2xl font-bold text-base-content font-cairo">Highlights</h2>
              </div>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div 
                  v-for="(highlight, index) in attraction.highlights" 
                  :key="index"
                  class="flex items-start gap-3"
                >
                  <i class="fas fa-check text-primary mt-1"></i>
                  <span class="text-base-content/80">{{ highlight }}</span>
                </div>
              </div>

              <div class="flex items-center gap-3 my-6">
                <div class="w-1 h-8 bg-primary rounded-full"></div>
                <h2 class="text-2xl font-bold text-base-content font-cairo">Amenities</h2>
              </div>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div 
                  v-for="(amenity, index) in attraction.amenities" 
                  :key="index"
                  class="flex items-center gap-3 p-4 bg-base-200 rounded-lg"
                >
                  <div class="w-10 h-10 bg-primary/10 rounded-lg flex items-center justify-center flex-shrink-0">
                    <i :class="[getAmenityIcon(amenity), 'text-primary']"></i>
                  </div>
                  <span class="text-base-content/80">{{ amenity }}</span>
                </div>
              </div>

               <Rating
              v-if="attraction.reviews"
              :reviews="attraction.reviews"
            />

             <ReviewsCarousel
              v-if="attraction.userReviews && attraction.userReviews.length > 0"
              :user-reviews="attraction.userReviews"
              :auto-play="true"
            />

            <Location
              v-if="attraction.latitude && attraction.longitude"
              :name="attraction.name"
              :city="attraction.city"
              :latitude="attraction.latitude"
              :longitude="attraction.longitude"
              :current-id="attraction.id"
              type="attraction"
            />
            </div>
          </div>


          <!-- Right Column: Booking Form (Sticky JS) -->
          <div ref="bookingColumn" class="lg:col-span-1 h-full min-h-[500px] relative">
            <!-- Placeholder to prevent layout shift when fixed -->
            <div ref="bookingWrapper" :style="{ minHeight: isSticky ? '1px' : 'auto' }"></div>
            
            <div 
              ref="stickyForm"
              :style="stickyStyle"
              :class="{ 'fixed top-24 z-50': isSticky }"
            >
              <BookingForm
                type="attraction"
                :base-price="attraction.price"
                :loading="bookingLoading"
                @submit="handleBooking"
              />
            </div>
          </div>
        </div>

         <div v-if="similarAttractions.length > 0" class="mt-16">
            <h2 class="text-3xl font-bold mb-8 font-cairo text-center text-primary">You may also like</h2>
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              <AttractionCard
                v-for="similar in similarAttractions"
                :key="similar.id"
                :id="Number(similar.id)"
                :image="getSimilarImage(similar)"
                :title="similar.name"
                :price="similar.price"
                :location="similar.city"
                :rating="similar.rating"
                :reviews="getSimilarReviews(similar)"
                button-text="View Details"
                reviews-text="reviews"
                :show-price="true"
                @view-details="handleSimilarClick"
              />
            </div>
      </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch, onUnmounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useBookingStore } from '@/stores/bookingStore';
import { useAttractionStore } from '@/stores/attractionStore';
import Carousel from '@/components/Common/Carousel.vue';
import Rating from '@/components/Common/Rating.vue';
import ReviewsCarousel from '@/components/Common/ReviewsCarousel.vue';
import Location from '@/components/Common/Location.vue';
import BookingForm from '@/components/Common/BookingForm.vue';
import AttractionCard from '@/components/Attractions/AttractionCard.vue';

const route = useRoute();
const router = useRouter();
const bookingStore = useBookingStore();
const attractionStore = useAttractionStore();

// State
const loading = ref(true);
const error = ref(null);
const attraction = ref(null);
const bookingLoading = ref(false);
const showFullOverview = ref(false);

// Images for carousel
const images = computed(() => {
  if (!attraction.value) return [];
  
  if (attraction.value.images && attraction.value.images.length > 0) {
    return attraction.value.images;
  }
  
  if (attraction.value.imageUrl) {
    return [attraction.value.imageUrl];
  }
  
  return ['/images/placeholder.jpg'];
});

// Similar attractions
const similarAttractions = computed(() => {
  if (!attraction.value) return [];
  
  return attractionStore.attractions
    .filter(attr => {
      if (attr.id === attraction.value.id) return false;
      
      const sameCity = attr.city === attraction.value.city;
      const sameCategory = attraction.value.categories?.some(cat => 
        attr.categories?.includes(cat)
      );
      
      return sameCity || sameCategory;
    })
    .slice(0, 4);
});

// Methods
const getAmenityIcon = (amenity) => {
  if (!amenity) return 'fas fa-check-circle';
  const name = amenity.toLowerCase().trim();
  
  const iconMap = {
    'wifi': 'fas fa-wifi',
    'internet': 'fas fa-wifi',
    'parking': 'fas fa-parking',
    'car': 'fas fa-car',
    'restaurant': 'fas fa-utensils',
    'food': 'fas fa-utensils',
    'dining': 'fas fa-utensils',
    'cafe': 'fas fa-coffee',
    'coffee': 'fas fa-coffee',
    'toilet': 'fas fa-restroom',
    'wc': 'fas fa-restroom',
    'restroom': 'fas fa-restroom',
    'bathroom': 'fas fa-restroom',
    'accessibility': 'fas fa-wheelchair',
    'wheelchair': 'fas fa-wheelchair',
    'accessible': 'fas fa-wheelchair',
    'guide': 'fas fa-user-tie',
    'tour': 'fas fa-flag',
    'shop': 'fas fa-store',
    'store': 'fas fa-store',
    'gift': 'fas fa-gift',
    'souvenir': 'fas fa-gift',
    'ticket': 'fas fa-ticket-alt',
    'entry': 'fas fa-ticket-alt',
    'audio': 'fas fa-headphones',
    'headphone': 'fas fa-headphones',
    'locker': 'fas fa-lock',
    'storage': 'fas fa-box',
    'medical': 'fas fa-briefcase-medical',
    'first aid': 'fas fa-briefcase-medical',
    'kid': 'fas fa-child',
    'child': 'fas fa-child',
    'family': 'fas fa-users',
    'transport': 'fas fa-bus',
    'bus': 'fas fa-bus',
    'shuttle': 'fas fa-shuttle-van',
    'transfer': 'fas fa-car-side',
    'camera': 'fas fa-camera',
    'photo': 'fas fa-camera',
    'photography': 'fas fa-camera',
    'prayer': 'fas fa-mosque',
    'mosque': 'fas fa-mosque',
    'museum': 'fas fa-landmark',
    'history': 'fas fa-landmark',
    'security': 'fas fa-shield-alt',
    'information': 'fas fa-info-circle',
    'map': 'fas fa-map'
  };

  // Check for partial matches
  for (const [key, icon] of Object.entries(iconMap)) {
    if (name.includes(key)) return icon;
  }
  
  return 'fas fa-check-circle'; // Default icon
};

const fetchAttraction = async () => {
  loading.value = true;
  error.value = null;
  
  try {
    const id = route.params.id;
    await attractionStore.fetchAttractionById(id);
    attraction.value = attractionStore.selectedAttraction;
    
    if (!attraction.value) {
      error.value = 'Attraction not found';
    }
  } catch (err) {
    error.value = err.message || 'Failed to load attraction';
    console.error('Error loading attraction:', err);
  } finally {
    loading.value = false;
  }
};

const handleBooking = async (eventData) => {
  bookingLoading.value = true;
  
  try {
    const { bookingData } = eventData;
    
    // Initialize booking in store
    bookingStore.initializeBooking('attraction', attraction.value.id, attraction.value);
    
    // Direct assignment to match TripDetails pattern
    const bookingDataVal = eventData.bookingData || eventData;
    const clonedData = JSON.parse(JSON.stringify(bookingDataVal));
    
    bookingStore.bookingInProgress.bookingData = clonedData;
    bookingStore.persistState();

    // Validate (double check)
    if (!bookingStore.isBookingValid) {
      throw new Error('Invalid booking data');
    }

    // Go to Review Page
    router.push({ 
      name: 'AttractionReview', 
      params: { id: attraction.value.id } 
    });
    
  } catch (err) {
    console.error('Booking init error:', err);
    alert('Failed to proceed to booking. Please try again.');
  } finally {
    bookingLoading.value = false;
  }
};

const getSimilarImage = (attraction) => {
  if (attraction.images && attraction.images.length > 0) {
    return attraction.images[0];
  }
  return attraction.imageUrl || '/images/placeholder.jpg';
};

const getSimilarReviews = (attraction) => {
  if (attraction.reviews && attraction.reviews.totalReviews) {
    return attraction.reviews.totalReviews;
  }
  return Math.floor(Math.random() * 5000) + 100;
};

const handleSimilarClick = (similarAttraction) => {
  router.push({
    name: 'AttractionDetails',
    params: { id: similarAttraction.id }
  });
};

// Watch route changes
watch(() => route.params.id, () => {
  if (route.name === 'AttractionDetails') {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    fetchAttraction();
  }
});

// Sticky Logic
const bookingWrapper = ref(null);
const stickyForm = ref(null);
const isSticky = ref(false);
const stickyStyle = ref({});
const bookingColumn = ref(null);
const contentColumn = ref(null);

const handleScroll = () => {
  if (!bookingWrapper.value || !bookingColumn.value || !contentColumn.value) return;

  const rect = bookingWrapper.value.getBoundingClientRect();
  const contentRect = contentColumn.value.getBoundingClientRect();
  const formRect = stickyForm.value.getBoundingClientRect();
  // Use current form height, or if hidden/fixed, assume standard or measure. 
  // stickyForm is the moving element.
  const formHeight = stickyForm.value.offsetHeight;
  
  const offsetTop = 100; // 100px from top

  // Calculate the stop point (relative to viewport)
  // When contentRect.bottom aligns with (offsetTop + formHeight)
  // If contentRect.bottom < offsetTop + formHeight, we are past the end.
  
  if (contentRect.bottom <= offsetTop + formHeight) {
    // STATE: Bottom Reached (Docked)
    isSticky.value = false;
    stickyStyle.value = {
      position: 'absolute',
      bottom: '0',
      left: '0',
      width: '100%', // Match parent width
      zIndex: 40
    };
  } else if (rect.top <= offsetTop) {
    // STATE: Sticky (Fixed)
    isSticky.value = true;
    stickyStyle.value = {
      position: 'fixed',
      top: `${offsetTop}px`,
      width: `${bookingWrapper.value.getBoundingClientRect().width}px`, // Match wrapper width
      zIndex: 50
    };
  } else {
    // STATE: Top (Normal)
    isSticky.value = false;
    stickyStyle.value = {};
  }
};

const handleResize = () => {
  if (isSticky.value && bookingWrapper.value) {
    const rect = bookingWrapper.value.getBoundingClientRect();
    stickyStyle.value.width = `${rect.width}px`;
  }
};

// Lifecycle
onMounted(() => {
  fetchAttraction();
  window.addEventListener('scroll', handleScroll, { passive: true });
  window.addEventListener('resize', handleResize, { passive: true });
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
  window.removeEventListener('resize', handleResize);
});
</script>

<style scoped>
.font-cairo {
  font-family: 'Cairo', sans-serif;
}

.line-clamp-1 {
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Fallback for sticky if needed, but Tailwind classes usually handle it */
@media (min-width: 1024px) {
  .lg\:sticky {
    position: -webkit-sticky !important;
    position: sticky !important;
  }
}
</style>