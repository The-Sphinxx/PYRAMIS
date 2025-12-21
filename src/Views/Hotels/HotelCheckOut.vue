<template>
  <div class="page-container py-8 font-cairo" v-if="hotel">
    <!-- Step Indicator -->
    <StepIndicator :current-step="2" booking-type="hotel" class="mb-12" />

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Left Column: Guest Info & Payment -->
      <div class="lg:col-span-2 space-y-8">
        
        <GuestInfoForm 
           v-model="guest"
           :submitting="submitting"
           @submit="handleConfirm"
        />

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
    <p class="mt-4">Loading checkout details...</p>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useHotelStore } from '@/stores/hotelsStore';
import StepIndicator from '@/components/Common/StepIndicator.vue';
import GuestInfoForm from '@/components/Common/GuestInfoForm.vue';
import PriceSummary from '@/components/Common/PriceSummary.vue';
import { calculateBookingCosts } from '@/Utils/bookingCalculator.js';

const route = useRoute();
const router = useRouter();
const hotelStore = useHotelStore();

const hotel = ref(null);
const submitting = ref(false);

const guest = ref({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  specialRequests: '',
  paymentMethod: 'card',
  cardNumber: '',
  cardName: '',
  expiryDate: '',
  cvc: ''
});

// We can grab query params if passed from previous step
const bookingData = ref({
  checkIn: route.query.checkIn || null,
  checkOut: route.query.checkOut || null,
  guests: parseInt(route.query.guests) || 1,
  rooms: parseInt(route.query.rooms) || 1
});

// Computed Properties for Pricing
const numberOfNights = computed(() => {
  if (!bookingData.value.checkIn || !bookingData.value.checkOut) return 7; 
  const start = new Date(bookingData.value.checkIn);
  const end = new Date(bookingData.value.checkOut);
  const diffTime = Math.abs(end - start);
  const days = Math.ceil(diffTime / (1000 * 60 * 60 * 24)); 
  return days > 0 ? days : 7;
});

const bookingDataForSummary = computed(() => ({
  ...bookingData.value,
  nights: numberOfNights.value
}));

const costs = computed(() => {
  if (!hotel.value) return { subtotal: 0, serviceFee: 0, taxes: 0, total: 0 };
  
  return calculateBookingCosts('hotel', hotel.value.pricePerNight, bookingDataForSummary.value);
});


const handleConfirm = async () => {
  submitting.value = true;
  // Mock API call
  await new Promise(resolve => setTimeout(resolve, 1500));
  submitting.value = false;

  router.push({
    name: 'HotelConfirmation',
    params: { id: hotel.value.id },
    query: { ...route.query }
  });
};

onMounted(async () => {
  const hotelId = route.params.id;
  if (hotelId) {
    if (hotelStore.hotels.length === 0) {
      await hotelStore.fetchHotels();
    }
    hotel.value = hotelStore.getHotelById(hotelId);
  }
});
</script>