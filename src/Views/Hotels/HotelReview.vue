<template>
  <div class="page-container py-8 font-cairo" v-if="hotel">
    <!-- Step Indicator -->
    <StepIndicator :current-step="1" booking-type="hotel" class="mb-12" />

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Left Column: Hotel Info & Booking Details -->
      <div class="lg:col-span-2 space-y-8">
        <!-- Review Card -->
        <div class="bg-base-100 rounded-2xl shadow-sm border border-base-200 overflow-hidden">
          <!-- Hotel Image -->
          <div class="h-[300px] w-full relative">
            <img 
              :src="hotel.images?.[0] || '/images/placeholder-hotel.jpg'" 
              :alt="hotel.name" 
              class="w-full h-full object-cover"
            />
          </div>

          <!-- Content -->
          <div class="p-6">
            <p class="text-base-content/70 text-sm mb-4 leading-relaxed">
              {{ hotel.overview || hotel.description }}
            </p>

            <div class="flex items-center gap-2 text-sm text-base-content/80 mb-6">
               <i class="fas fa-map-marker-alt text-[#C86A3F]"></i>
               <span>{{ hotel.name }}, {{ hotel.city }}, Egypt</span>
            </div>

            <div class="divider"></div>

            <!-- Booking Details Section -->
            <div class="mt-6">
              <div class="flex items-center gap-3 mb-6">
                <div class="w-1 h-8 bg-[#C86A3F]"></div>
                <h3 class="text-xl font-bold text-base-content">Booking Details</h3>
              </div>
              
              <div class="space-y-4">
                <div v-for="(amenity, index) in displayAmenities" :key="index" class="bg-base-200/40 p-3 rounded-lg flex items-center gap-4">
                  <div class="w-10 h-10 rounded bg-[#FBEFE9] flex items-center justify-center text-[#C86A3F]">
                    <i :class="getAmenityIcon(amenity)"></i>
                  </div>
                  <span class="text-sm font-medium text-base-content/70">{{ amenity }}</span>
                </div>
                
                <div v-if="!displayAmenities.length" class="text-sm text-base-content/60 italic">
                  No specific amenities listed.
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Continue Button -->
        <button 
          @click="handleContinue"
          class="btn bg-[#C86A3F] hover:bg-[#B05D35] text-white w-full text-lg font-bold py-4 h-auto shadow-md border-none rounded-lg"
        >
          Continue to Guest Information
        </button>
      </div>

      <!-- Right Column: Price Summary -->
      <div class="lg:col-span-1">
        <PriceSummary
          :costs="costs"
          booking-type="hotel"
          :booking-data="bookingDataForSummary"
          :base-price="hotel.pricePerNight"
          :add-ons="500"
        />
      </div>
    </div>
  </div>
  
  <div v-else class="page-container py-12 text-center text-base-content/70">
    <span class="loading loading-spinner loading-lg text-primary"></span>
    <p class="mt-4">Loading review details...</p>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useHotelStore } from '@/stores/hotelsStore';
import StepIndicator from '@/components/Common/StepIndicator.vue';
import PriceSummary from '@/components/Common/PriceSummary.vue';
import { calculateBookingCosts } from '@/Utils/bookingCalculator.js';

const route = useRoute();
const router = useRouter();
const hotelStore = useHotelStore();

const hotel = ref(null);
const bookingData = ref({
  checkIn: null,
  checkOut: null,
  guests: 1,
  rooms: 1
});

// Computed Properties
const numberOfNights = computed(() => {
  if (!bookingData.value.checkIn || !bookingData.value.checkOut) return 7; // Default to 7
  const start = new Date(bookingData.value.checkIn);
  const end = new Date(bookingData.value.checkOut);
  const diffTime = Math.abs(end - start);
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24)); 
});

const bookingDataForSummary = computed(() => ({
  ...bookingData.value,
  nights: numberOfNights.value
}));

const costs = computed(() => {
  if (!hotel.value) return { subtotal: 0, serviceFee: 0, taxes: 0, total: 0 };
  
  // Use shared calculator for consistency
  // Note: calculateBookingCosts return object has: subtotal, serviceFee, taxes, total, duration
  return calculateBookingCosts('hotel', hotel.value.pricePerNight, bookingDataForSummary.value);
});

// Methods
const displayAmenities = computed(() => {
  if (!hotel.value?.amenities) return [];
  // Return top 3 amenities
  return hotel.value.amenities.slice(0, 3);
});

const getAmenityIcon = (amenity) => {
  const lower = amenity.toLowerCase();
  if (lower.includes('pool')) return 'fas fa-swimming-pool';
  if (lower.includes('spa') || lower.includes('wellness')) return 'fas fa-spa';
  if (lower.includes('gym') || lower.includes('fitness')) return 'fas fa-dumbbell';
  if (lower.includes('wifi') || lower.includes('wi-fi')) return 'fas fa-wifi';
  if (lower.includes('park')) return 'fas fa-parking';
  if (lower.includes('restaurant') || lower.includes('dining') || lower.includes('food')) return 'fas fa-utensils';
  if (lower.includes('bar')) return 'fas fa-glass-martini';
  if (lower.includes('beach')) return 'fas fa-umbrella-beach';
  if (lower.includes('air') || lower.includes('ac')) return 'fas fa-snowflake';
  if (lower.includes('transport') || lower.includes('shuttle')) return 'fas fa-shuttle-van';
  return 'fas fa-check-circle';
};

const formatPrice = (price) => {
  return `$${price}`;
};

const handleContinue = () => {
  router.push({
    name: 'HotelCheckOut',
    params: { id: hotel.value.id },
    query: {
      ...route.query
    }
  });
};

// Lifecycle
onMounted(async () => {
  const hotelId = route.params.id;
  if (hotelId) {
    if (hotelStore.hotels.length === 0) {
      await hotelStore.fetchHotels();
    }
    hotel.value = hotelStore.getHotelById(hotelId);
  }
  
  // Parse query params
  if (route.query.checkIn) {
    bookingData.value = {
      checkIn: route.query.checkIn,
      checkOut: route.query.checkOut,
      guests: parseInt(route.query.guests) || 1,
      rooms: parseInt(route.query.rooms) || 1
    };
  }
});
</script>