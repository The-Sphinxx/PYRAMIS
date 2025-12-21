<template>

</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axios from 'axios';
import { useAuthStore } from '@/stores/authStore';
import { useRoute, useRouter } from 'vue-router';
import { useHotelsStore } from '@/stores/hotelsStore';
import StepIndicator from '@/components/Common/StepIndicator.vue';
import GuestInfoForm from '@/components/Common/GuestInfoForm.vue';
import PriceSummary from '@/components/Common/PriceSummary.vue';
import { calculateBookingCosts } from '@/Utils/bookingCalculator.js';

const route = useRoute();
const router = useRouter();
const hotelStore = useHotelsStore();
const authStore = useAuthStore();

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
  try {
    const user = authStore.user;
    // Use logged-in user ID, or generate a unique guest ID to prevent data mixing
    const bookingUserId = user ? user.id : `guest_${Date.now()}`;

    const newBooking = {
      userId: bookingUserId,
      title: hotel.value.name,
      location: hotel.value.city || 'Egypt',
      date: `${bookingDataForSummary.value.checkIn} - ${bookingDataForSummary.value.checkOut}`,
      image: hotel.value.images ? hotel.value.images[0] : hotel.value.image,
      status: 'Confirmed',
      type: 'Hotel', // or 'فندق'
      reference: 'H-' + Math.floor(Math.random() * 10000),
      price: costs.value.total,
      bookingData: bookingDataForSummary.value
    };

    await axios.post('http://localhost:3000/bookings', newBooking);

    router.push({
      name: 'HotelConfirmation',
      params: { id: hotel.value.id },
      query: { ...route.query }
    });
  } catch (error) {
    console.error("Booking failed:", error);
    alert("Failed to confirm booking. Please try again.");
  } finally {
    submitting.value = false;
  }
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