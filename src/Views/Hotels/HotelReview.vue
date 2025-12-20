<template>
  <div class="page-container py-8 font-cairo" v-if="hotel">
    <!-- Step Indicator -->
    <StepIndicator :current-step="1" booking-type="hotel" class="mb-12" />

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Left Column: Hotel Info & Booking Details -->
      <div class="lg:col-span-2 space-y-8">
        <!-- Review Card -->
        <div class="bg-base-100 rounded-2xl shadow-lg overflow-hidden border border-base-200">
          <!-- Hotel Image -->
          <div class="h-64 w-full relative">
            <img 
              :src="hotel.images?.[0] || '/images/placeholder-hotel.jpg'" 
              :alt="hotel.name" 
              class="w-full h-full object-cover"
            />
            <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
            <div class="absolute bottom-4 left-4 text-white">
               <h2 class="text-3xl font-bold mb-1">{{ hotel.name }}</h2>
               <div class="flex items-center gap-2 text-sm opacity-90">
                 <i class="fas fa-map-marker-alt text-primary"></i>
                 <span>{{ hotel.city }}, Egypt</span>
               </div>
            </div>
          </div>

          <!-- Content -->
          <div class="p-8">
            <p class="text-base-content/70 mb-8 leading-relaxed">
              {{ hotel.overview ? hotel.overview.substring(0, 150) + '...' : 'Witness ancient wonders from the sky at sunrise' }}
            </p>

            <!-- Booking Details Section -->
            <div class="pl-4 border-l-4 border-primary">
              <h3 class="text-xl font-bold text-base-content mb-6">Booking Details</h3>
              
              <div class="space-y-4">
                <!-- Check-in/Check-out -->
                <div class="flex items-center gap-4 bg-base-200/50 p-4 rounded-xl">
                  <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center text-primary flex-shrink-0">
                    <i class="far fa-calendar-alt"></i>
                  </div>
                  <div>
                    <div class="font-bold text-base-content">Dates</div>
                    <div class="text-sm text-base-content/70">
                      {{ formatDate(bookingData.checkIn) }} - {{ formatDate(bookingData.checkOut) }}
                      <span class="ml-2 badge badge-ghost badge-sm">{{ numberOfNights }} Nights</span>
                    </div>
                  </div>
                </div>

                <!-- Rooms & Guests -->
                <div class="flex items-center gap-4 bg-base-200/50 p-4 rounded-xl">
                  <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center text-primary flex-shrink-0">
                    <i class="fas fa-bed"></i>
                  </div>
                  <div>
                    <div class="font-bold text-base-content">Accommodation</div>
                    <div class="text-sm text-base-content/70">
                      {{ bookingData.guests }} Guests, 1 Room
                    </div>
                  </div>
                </div>
                
                <!-- Room Type (Static for now) -->
                <div class="flex items-center gap-4 bg-base-200/50 p-4 rounded-xl">
                  <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center text-primary flex-shrink-0">
                    <i class="fas fa-concierge-bell"></i>
                  </div>
                  <div>
                    <div class="font-bold text-base-content">Room Type</div>
                    <div class="text-sm text-base-content/70 font-medium text-success">
                      Free Upgrade to Deluxe View
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Continue Button -->
        <button 
          @click="handleContinue"
          class="btn btn-primary w-full text-white text-lg font-bold py-4 h-auto shadow-lg hover:brightness-110"
        >
          Continue to Guest Information
        </button>
      </div>

      <!-- Right Column: Price Summary -->
      <div class="lg:col-span-1">
        <PriceSummary 
          :costs="costs"
          :booking-type="'hotel'"
          :booking-data="priceSummaryBookingData"
          :base-price="hotel.pricePerNight"
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

const route = useRoute();
const router = useRouter();
const hotelStore = useHotelStore();

const hotel = ref(null);
const bookingData = ref({
  checkIn: null,
  checkOut: null,
  guests: 1
});

// Computed Properties
const numberOfNights = computed(() => {
  if (!bookingData.value.checkIn || !bookingData.value.checkOut) return 0;
  const start = new Date(bookingData.value.checkIn);
  const end = new Date(bookingData.value.checkOut);
  const diffTime = Math.abs(end - start);
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24)); 
});

const costs = computed(() => {
  if (!hotel.value || numberOfNights.value === 0) return { subtotal: 0, serviceFee: 0, taxes: 0, total: 0 };
  
  const subtotal = hotel.value.pricePerNight * numberOfNights.value;
  const serviceFee = 50; // Fixed fee for now
  const taxes = subtotal * 0.14;
  
  return {
    subtotal,
    serviceFee,
    taxes,
    total: subtotal + serviceFee + taxes
  };
});

const priceSummaryBookingData = computed(() => ({
  nights: numberOfNights.value,
  rooms: 1, // Default to 1 room
  guests: bookingData.value.guests
}));

// Methods
const formatDate = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric', year: 'numeric' });
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
  if (route.query.checkIn) bookingData.value.checkIn = route.query.checkIn;
  if (route.query.checkOut) bookingData.value.checkOut = route.query.checkOut;
  if (route.query.guests) bookingData.value.guests = parseInt(route.query.guests);
});
</script>