<template>
  <div class="page-container py-12 font-cairo min-h-screen bg-base-100">
    <div class="max-w-5xl mx-auto">
      <!-- Step Indicator -->
      <div class="mb-12">
        <StepIndicator :current-step="3" :booking-type="bookingType" />
      </div>

      <!-- Success Header -->
      <div class="text-center mb-12">
        <div class="w-16 h-16 bg-success/10 rounded-xl flex items-center justify-center mx-auto mb-4">
          <i class="fas fa-check text-3xl text-success"></i>
        </div>
        <h1 class="text-3xl font-bold text-gray-800 mb-2">Booking Confirmed !</h1>
        <p class="text-gray-500">Your reservation has been successfully processed</p>
        <div v-if="paymentStatus" class="mt-4">
          <span class="badge" :class="paymentStatusClass">Payment {{ paymentStatusLabel }}</span>
        </div>
      </div>

      <!-- Main Confirmation Card -->
      <div class=" rounded-2xl shadow-lg overflow-hidden mb-16 border border-gray-100">
        <!-- Image Header -->
        <div class="h-[300px] w-full relative">
           <img 
             :src="bookingImage" 
             alt="Booking Item" 
             class="w-full h-full object-cover"
           />
           <div class="absolute bottom-0 left-0 w-full bg-gradient-to-t from-black/80 to-transparent p-6 pt-20">
              <h2 class="text-2xl font-bold text-white mb-1">{{ booking?.itemName }}</h2>
              <div class="flex items-center gap-2 text-white/90 text-sm">
                <i class="fas fa-map-marker-alt text-primary"></i>
                <span>{{ booking?.itemData?.city || booking?.itemData?.location }}, Egypt</span>
              </div>
           </div>
        </div>

        <div class="p-8">
           <!-- Booking Details Section -->
           <div class="mb-8">
              <div class="flex items-center gap-3 mb-6">
                <div class="w-1 h-6 bg-primary rounded-full"></div>
                <h3 class="text-xl font-bold text-gray-800">Booking Details</h3>
              </div>

              <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                 <!-- Date Card -->
                 <div class="bg-base-200/50 rounded-xl p-4 flex flex-col items-center justify-center text-center">
                    <div class="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center text-primary mb-2">
                       <i class="fas fa-calendar-alt"></i>
                    </div>
                    <span class="text-xs text-gray-500 mb-1">Date</span>
                    <span class="font-bold text-sm">{{ formattedDate }}</span>
                 </div>

                 <!-- Guests Card -->
                 <div class="bg-base-200/50 rounded-xl p-4 flex flex-col items-center justify-center text-center">
                    <div class="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center text-primary mb-2">
                       <i class="fas fa-users"></i>
                    </div>
                    <span class="text-xs text-gray-500 mb-1">Guests</span>
                    <span class="font-bold text-sm">{{ booking?.bookingData?.guests }} Guests</span>
                 </div>

                 <!-- Reference/ID (Placeholder for Transport/Other) -->
                 <div class="bg-base-200/50 rounded-xl p-4 flex flex-col items-center justify-center text-center">
                    <div class="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center text-primary mb-2">
                       <i class="fas fa-ticket-alt"></i>
                    </div>
                    <span class="text-xs text-gray-500 mb-1">Booking ID</span>
                    <span class="font-bold text-sm">#{{ booking?.id?.slice(0, 8).toUpperCase() }}</span>
                 </div>
              </div>
           </div>

           <!-- Location Section -->
           <div class="mb-8" v-if="locationData?.latitude && locationData?.longitude">
              <Location
                :name="booking.itemName"
                :city="locationData.city"
                :latitude="locationData.latitude"
                :longitude="locationData.longitude"
                :current-id="booking.itemId"
                type="attraction"
                :show-nearby="false"
              />
           </div>

           <!-- Home Button -->
           <button 
             @click="goHome" 
             class="btn btn-primary w-full text-white font-bold text-lg rounded-xl shadow-md hover:brightness-105"
           >
             Back To Home Page
           </button>
        </div>
      </div>

      <!-- Enhance Your Stay -->
      <div v-if="recommendations.length > 0">
         <h2 class="text-2xl font-bold text-center mb-8 font-cairo">Enhance Your stay</h2>
         <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div 
              v-for="item in recommendations" 
              :key="item.id"
              class="bg-white rounded-xl shadow-md overflow-hidden hover:shadow-xl transition-shadow cursor-pointer group"
              @click="handleRecommendationClick(item)"
            >
               <div class="h-48 relative overflow-hidden">
                  <img :src="item.image" :alt="item.title" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"/>
               </div>
               <div class="p-4">
                  <div class="flex justify-between items-start mb-2">
                     <h3 class="font-bold text-lg">{{ item.title }}</h3>
                     <span class="text-primary font-bold">{{ item.price }}</span>
                  </div>
                  <p class="text-xs text-gray-500 mb-3 line-clamp-2">{{ item.description }}</p>
                  <div class="flex items-center gap-2 text-xs text-gray-400">
                     <i class="fas fa-star text-warning"></i>
                     <span>{{ item.rating }} ({{ item.reviews }} reviews)</span>
                  </div>
                  <button class="btn btn-primary btn-sm w-full mt-4 text-white">Book Now</button>
               </div>
            </div>
         </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { loadStripe } from '@stripe/stripe-js';
import { useBookingStore } from '@/stores/bookingStore';
import { useAttractionStore } from '@/stores/attractionStore';
import StepIndicator from '@/components/Common/StepIndicator.vue';
import Location from '@/components/Common/Location.vue';

const router = useRouter();
const route = useRoute();
const bookingStore = useBookingStore();
const attractionStore = useAttractionStore();

const booking = ref(null);
const attractionDetails = ref(null);
const paymentStatus = ref('processing');
const paymentStatusNote = ref('');

const bookingType = computed(() => booking.value?.type || 'attraction');

const bookingImage = computed(() => {
  const item = attractionDetails.value || booking.value?.itemData; 
  if (item?.images?.length > 0) return item.images[0];
  return item?.imageUrl || '/images/placeholder.jpg';
});

const formattedDate = computed(() => {
  const date = booking.value?.bookingData?.date;
  if (!date) return 'N/A';
  return new Date(date).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  });
});

// Location Data Computed - Prefers fresh attraction details, falls back to booking data
const locationData = computed(() => {
    return attractionDetails.value || booking.value?.itemData || {};
});

const paymentStatusLabel = computed(() => {
  if (paymentStatus.value === 'succeeded') return 'Succeeded';
  if (paymentStatus.value === 'processing') return 'Processing';
  return 'Failed';
});

const paymentStatusClass = computed(() => {
  if (paymentStatus.value === 'succeeded') return 'badge-success';
  if (paymentStatus.value === 'processing') return 'badge-warning';
  return 'badge-error';
});

const verifyPaymentStatus = async () => {
  try {
    const clientSecret = route.query.payment_intent_client_secret || sessionStorage.getItem('attraction_payment_client_secret');
    if (!clientSecret) {
      paymentStatus.value = 'unknown';
      paymentStatusNote.value = 'Payment verification unavailable';
      return;
    }

    const stripe = await loadStripe(import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY || 'pk_test_51QauWoP5jgpKEGjvRGUxEJbvLh2lnWZe1j6GmKvqfCKAcWFAv9YJxHg8YYCXmzIINl7IpVyTxBv0FdC6UkdN5Hpa00DFWIBkJx');
    const { paymentIntent } = await stripe.retrievePaymentIntent(clientSecret);

    paymentStatus.value = paymentIntent.status;
    if (paymentIntent.status === 'succeeded') {
      paymentStatusNote.value = 'Your payment has been successfully processed';
    } else if (paymentIntent.status === 'processing') {
      paymentStatusNote.value = 'Your payment is being processed';
    } else {
      paymentStatusNote.value = 'There was an issue with your payment';
    }
  } catch (error) {
    console.error('Error verifying payment status:', error);
    paymentStatus.value = 'unknown';
    paymentStatusNote.value = 'Could not verify payment status';
  }
};

// Mock Data for Recommendations
const recommendations = ref([
 {
    id: 'luxury-suv',
    title: 'Luxury SUV',
    price: '200$',
    description: 'Perfect for family trips and long journeys with premium comfort.',
    image: '/images/cars/luxury_suv.jpg', // Placeholder path
    rating: 4.9,
    reviews: 144,
    type: 'car'
 },
 {
    id: 'fayoum',
    title: 'Fayoum',
    price: '200$/Person',
    description: 'A quick adventure: lakes, waterfalls, deserts, and sandboarding.',
    image: '/images/trips/fayoum.jpg', // Placeholder path
    rating: 4.9,
    reviews: 2344,
    type: 'trip' 
 },
 {
    id: 'luxury-suv-2',
    title: 'Luxury SUV',
    price: '200$',
    description: 'Perfect for family trips and long journeys with premium comfort.',
    image: '/images/cars/luxury_suv.jpg', 
    rating: 4.9,
    reviews: 144,
    type: 'car'
 }
]);

onMounted(async () => {
   // 1. Verify payment status
   await verifyPaymentStatus();

   // 2. Get booking from store (avoid mock API)
   booking.value = bookingStore.bookingInProgress;
   if (!booking.value) {
      console.warn("No booking data found for confirmation");
      return;
   }

   // 3. Fetch fresh attraction details for location data
   if (booking.value.itemId) {
       try {
           const attr = await attractionStore.fetchAttractionById(booking.value.itemId);
           attractionDetails.value = attr;
       } catch (err) {
           console.error("Failed to fetch fresh attraction details", err);
       }
   }
});

const goHome = () => {
  router.push('/');
};

const handleRecommendationClick = (item) => {
   // Navigate based on type (placeholder logic)
   if (item.type === 'car') router.push('/cars');
   else if (item.type === 'trip') router.push('/trips');
};
</script>