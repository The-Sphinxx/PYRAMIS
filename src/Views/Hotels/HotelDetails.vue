<template>
  <div class="min-h-screen bg-base-200">
    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center min-h-screen">
      <span class="loading loading-spinner loading-lg text-primary"></span>
    </div>

    <!-- Hotel Details Content -->
    <div v-else-if="hotel" class="page-container py-8">
      <!-- Breadcrumbs -->
      <div class="text-sm breadcrumbs mb-4 text-base-content/70">
        <ul>
          <li><router-link to="/">Home</router-link></li>
          <li><router-link to="/hotels/list">Hotels</router-link></li>
          <li class="font-semibold text-primary">{{ hotel.name }}</li>
        </ul>
      </div>

      <!-- Image Carousel -->
      <div class="mb-8">
        <Carousel :images="hotel.images || []" :alt-text="hotel.name" />
      </div>

      <!-- Hotel Name and Basic Info -->
      <div class="mb-8">
        <h1 class="text-4xl font-bold text-primary font-cairo mb-2">{{ hotel.name }}</h1>
        <p class="text-lg text-base-content/70 mb-4">Witness ancient wonders from the sky at sunrise</p>
        
        <div class="flex flex-wrap items-center gap-6 text-sm">
          <!-- Location -->
          <div class="flex items-center gap-2 text-error">
            <i class="fas fa-map-marker-alt"></i>
            <span class="text-base-content">{{ hotel.city }}, Egypt</span>
          </div>
          
          <!-- Rating -->
          <div class="flex items-center gap-2 text-[#D4852B]">
            <i class="fas fa-star"></i>
            <span class="text-base-content font-semibold">{{ hotel.rating }} ({{ hotel.reviews?.totalReviews || 0 }} reviews)</span>
          </div>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8 mb-12">
        <!-- Left Column - Hotel Info -->
        <div class="lg:col-span-2 space-y-10">
          <!-- Overview -->
          <div>
            <div class="border-l-4 border-primary pl-4 mb-4">
              <h2 class="text-2xl font-bold text-primary font-cairo">Overview</h2>
            </div>
            <p class="text-base-content leading-relaxed text-justify">
              {{ hotel.overview || 'Discover luxury and comfort at ' + hotel.name + ', located in the heart of ' + hotel.city + '. Experience world-class hospitality with premium amenities and exceptional service.' }}
            </p>
            <button class="text-primary font-semibold text-sm mt-2 hover:underline">Read More +</button>
          </div>

         <!-- Highlights -->
          <div v-if="hotel.highlights && hotel.highlights.length > 0">
            <div class="border-l-4 border-primary pl-4 mb-4">
              <h2 class="text-2xl font-bold text-primary font-cairo">Highlights</h2>
            </div>
            <ul class="grid grid-cols-1 md:grid-cols-2 gap-y-3 gap-x-8">
              <li v-for="(highlight, index) in hotel.highlights" :key="index" class="flex items-start gap-2">
                <i class="fas fa-check text-primary mt-1 text-sm"></i>
                <span class="text-base-content text-sm">{{ highlight }}</span>
              </li>
            </ul>
          </div>

          <!-- Amenities -->
          <div v-if="hotel.amenities && hotel.amenities.length > 0">
            <div class="border-l-4 border-primary pl-4 mb-4">
              <h2 class="text-2xl font-bold text-primary font-cairo">Amenities</h2>
            </div>
            <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div v-for="(amenity, index) in hotel.amenities" :key="index" 
                class="flex items-center justify-center gap-2 bg-primary/5 p-3 rounded-lg border border-primary/10 hover:bg-primary/10 transition-colors">
                <i :class="['fas', getAmenityIcon(amenity)]" class="text-primary text-sm"></i>
                <span class="text-base-content text-xs font-medium text-center">{{ amenity }}</span>
              </div>
            </div>
          </div>

          <!-- Room Amenities -->
          <div v-if="hotel.roomAmenities && hotel.roomAmenities.length > 0">
            <div class="border-l-4 border-primary pl-4 mb-4">
              <h2 class="text-2xl font-bold text-primary font-cairo">Room Amenities</h2>
            </div>
            <ul class="grid grid-cols-1 md:grid-cols-2 gap-y-3 gap-x-8">
              <li v-for="(amenity, index) in hotel.roomAmenities" :key="index" class="flex items-start gap-2">
                <i class="fas fa-check text-primary mt-1 text-sm"></i>
                <span class="text-base-content text-sm">{{ amenity }}</span>
              </li>
            </ul>
          </div>

          <Rating 
            v-if="hotel.reviews" 
            :reviews="hotel.reviews" 
          />

          <!-- Guest Reviews -->
          <div v-if="hotel.userReviews && hotel.userReviews.length > 0">
            <ReviewsCarousel :reviews="hotel.userReviews" />
          </div>

          <!-- Location -->
          <div>
            <div class="rounded-xl overflow-hidden shadow-sm border border-base-200">
              <Location 
                :name="hotel.name"
                :city="hotel.city"
                :latitude="hotel.latitude" 
                :longitude="hotel.longitude"
                :current-id="hotel.id"
                type="hotel"
                :show-nearby="true"
              />
            </div>
          </div>
        </div>

        <!-- Right Column - Booking Form -->
        <div class="lg:col-span-1">
          <div class="sticky top-24">
            <BookingForm 
              v-if="hotel"
              type="hotel"
              :base-price="hotel.pricePerNight"
              :loading="loading"
              @submit="handleBookNow"
            />
          </div>
        </div>
      </div>

      <!-- You May Also Like Section -->
      <div v-if="recommendedHotels.length > 0" class="mb-12">
        <h2 class="text-3xl font-bold text-primary font-cairo mb-6 text-center">You May also Like</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <HotelCard
            v-for="recommendedHotel in recommendedHotels"
            :key="recommendedHotel.id"
            :hotel="recommendedHotel"
            @book="handleRecommendedHotelClick"
          />
        </div>
      </div>
    </div>

    <!-- Hotel Not Found -->
    <div v-else class="flex flex-col items-center justify-center min-h-screen">
      <i class="fas fa-hotel text-6xl text-neutral mb-4"></i>
      <h2 class="text-2xl font-bold text-base-content mb-2">Hotel Not Found</h2>
      <p class="text-neutral mb-6">The hotel you're looking for doesn't exist.</p>
      <button @click="router.push({ name: 'HotelFilter' })" class="btn btn-primary">
        Browse Hotels
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useHotelsStore } from '@/stores/hotelsStore';
import Carousel from '@/components/Common/Carousel.vue';
import ReviewsCarousel from '@/components/Common/ReviewsCarousel.vue';
import Location from '@/components/Common/Location.vue';
import HotelCard from '@/components/Hotels/HotelCard.vue';
import BookingForm from '@/components/Common/BookingForm.vue';
import Rating from '@/components/Common/Rating.vue';

const route = useRoute();
const router = useRouter();
const hotelStore = useHotelsStore();

// State
const loading = ref(true);
const hotel = ref(null);
const recommendedHotels = ref([]);

// Methods
const getAmenityIcon = (amenity) => {
  const lowerAmenity = amenity.toLowerCase();
  
  if (lowerAmenity.includes('wifi')) return 'fa-wifi';
  if (lowerAmenity.includes('pool')) return 'fa-swimming-pool';
  if (lowerAmenity.includes('spa')) return 'fa-spa';
  if (lowerAmenity.includes('fitness') || lowerAmenity.includes('gym')) return 'fa-dumbbell';
  if (lowerAmenity.includes('restaurant') || lowerAmenity.includes('dining')) return 'fa-utensils';
  if (lowerAmenity.includes('bar')) return 'fa-cocktail';
  if (lowerAmenity.includes('parking')) return 'fa-parking';
  if (lowerAmenity.includes('shuttle')) return 'fa-bus';
  if (lowerAmenity.includes('desk')) return 'fa-concierge-bell';
  if (lowerAmenity.includes('service')) return 'fa-user-tie';
  if (lowerAmenity.includes('beach')) return 'fa-umbrella-beach';
  if (lowerAmenity.includes('view')) return 'fa-mountain';
  
  return 'fa-check-circle'; // Default icon
};

const formatKey = (key) => {
  return key.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase());
};

const scrollToSection = (sectionId) => {
  activeTab.value = sectionId;
  const element = document.getElementById(sectionId);
  if (element) {
    element.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }
};

const handleBookNow = ({ bookingData, costs }) => {
  if (!hotel.value) return;
  
  // Format dates for query params
  const checkInDate = bookingData.checkIn ? new Date(bookingData.checkIn).toISOString() : null;
  const checkOutDate = bookingData.checkOut ? new Date(bookingData.checkOut).toISOString() : null;

  router.push({
    name: 'HotelReview',
    params: { id: hotel.value.id },
    query: {
      checkIn: checkInDate,
      checkOut: checkOutDate,
      rooms: bookingData.rooms,
      guests: bookingData.guests,
      // Pass calculated costs slightly if needed across steps, but best to recalculate
    }
  });
};

const handleRecommendedHotelClick = (recommendedHotel) => {
  // Navigate to the recommended hotel's details page
  router.push({
    name: 'HotelDetails',
    params: { id: recommendedHotel.id }
  });
  // Scroll to top
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

const loadRecommendedHotels = () => {
  // Get hotels from the same city or similar category
  const allHotels = hotelStore.hotels.filter(h => h.id !== hotel.value.id);
  
  // Prioritize hotels from same city
  const sameCityHotels = allHotels.filter(h => h.city === hotel.value.city);
  const otherHotels = allHotels.filter(h => h.city !== hotel.value.city);
  
  const hotelsToShow = [...sameCityHotels, ...otherHotels].slice(0, 4);
  
  // Map to HotelCard format
  recommendedHotels.value = hotelsToShow.map(h => {
    let amenities = ['Wifi', 'Pool', 'Gym'];
    
    if (h.amenities && h.amenities.length > 0) {
      const amenityMap = {
        'Free Wi-Fi': 'Wifi',
        'Wi-Fi': 'Wifi',
        'Outdoor Swimming Pool': 'Pool',
        'Swimming Pool': 'Pool',
        'Fitness Center': 'Gym',
        'Spa and Wellness Center': 'Spa',
        'Restaurant': 'Restaurant',
        'Multiple Restaurants and Bars': 'Restaurant',
        'Parking': 'Parking',
        'Valet Parking': 'Parking',
        'Airport Shuttle': 'Shuttle',
        'Beach Access': 'Beach',
        'Private Beach': 'Beach'
      };
      
      amenities = h.amenities
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
      id: h.id,
      name: h.name,
      image: h.images && h.images.length > 0 ? h.images[0] : '/images/placeholder-hotel.jpg',
      price: h.pricePerNight || 200,
      location: h.city || 'Cairo',
      rating: h.rating || 4.5,
      reviews: h.reviews?.totalReviews || h.totalReviews || '0',
      amenities: amenities
    };
  });
};

// Lifecycle
onMounted(async () => {
  try {
    // Fetch hotels if not already loaded
    if (hotelStore.hotels.length === 0) {
      await hotelStore.fetchHotels();
    }
    
    // Get hotel by ID
    const hotelId = route.params.id;
    hotel.value = hotelStore.getHotelById(hotelId);
    
    if (hotel.value) {
      loadRecommendedHotels();
    }
  } catch (error) {
    console.error('Error loading hotel details:', error);
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

/* Ensure sticky sidebar doesn't overlap footer */
.sticky {
  position: sticky;
}
</style>