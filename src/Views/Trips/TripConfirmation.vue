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
        <div v-if="paymentStatus" class="flex justify-center mt-4">
          <span class="badge text-sm px-4 py-3" :class="paymentStatusClass">Payment {{ paymentStatusLabel }}</span>
        </div>
        <p v-if="paymentStatusNote" class="text-error text-sm mt-2">{{ paymentStatusNote }}</p>
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
                    <span class="font-bold text-sm">{{ formattedGuests }}</span>
                 </div>

                 <!-- Accommodation (If available) -->
                 <div v-if="booking?.itemData?.amenities?.accommodation" class="bg-base-200/50 rounded-xl p-4 flex flex-col items-center justify-center text-center">
                    <div class="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center text-primary mb-2">
                       <i class="fas fa-bed"></i>
                    </div>
                    <span class="text-xs text-gray-500 mb-1">Accommodation</span>
                    <span class="font-bold text-sm">{{ booking?.itemData?.amenities.accommodation }}</span>
                 </div>

                 <!-- Reference/ID -->
                 <div class="bg-base-200/50 rounded-xl p-4 flex flex-col items-center justify-center text-center">
                    <div class="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center text-primary mb-2">
                       <i class="fas fa-ticket-alt"></i>
                    </div>
                    <span class="text-xs text-gray-500 mb-1">Booking ID</span>
                    <span class="font-bold text-sm">#{{ booking?.id?.slice(0, 8).toUpperCase() }}</span>
                 </div>

                 <!-- Total Price -->
                 <div class="bg-base-200/50 rounded-xl p-4 flex flex-col items-center justify-center text-center">
                    <div class="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center text-primary mb-2">
                       <i class="fas fa-money-bill-wave"></i>
                    </div>
                    <span class="text-xs text-gray-500 mb-1">Total Price</span>
                    <span class="font-bold text-sm">{{ formatPrice(booking?.pricing?.total) }}</span>
                 </div>
              </div>
           </div>

           <!-- Multi-Column Summary (Price & Guest) -->
           <div class="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">
              <!-- Left Column: Guest Info -->
              <div class="space-y-6">
                 <div class="flex items-center gap-3">
                    <div class="w-1 h-5 bg-primary rounded-full"></div>
                    <h3 class="text-lg font-bold text-gray-800">Guest Information</h3>
                 </div>
                 <div class="bg-base-200/30 rounded-2xl p-6 space-y-4 border border-base-200/50">
                    <div class="flex justify-between items-center text-sm">
                       <span class="text-gray-500">Full Name</span>
                       <span class="font-bold">{{ booking?.guestInfo?.firstName }} {{ booking?.guestInfo?.lastName }}</span>
                    </div>
                    <div class="flex justify-between items-center text-sm">
                       <span class="text-gray-500">Email Address</span>
                       <span class="font-bold">{{ booking?.guestInfo?.email }}</span>
                    </div>
                    <div class="flex justify-between items-center text-sm">
                       <span class="text-gray-500">Phone Number</span>
                       <span class="font-bold">{{ booking?.guestInfo?.phone }}</span>
                    </div>
                    <div v-if="booking?.guestInfo?.specialRequests" class="pt-2">
                       <span class="text-xs text-gray-500 block mb-1">Special Requests</span>
                       <p class="text-sm text-gray-600 bg-white/50 p-3 rounded-lg border border-gray-100">{{ booking?.guestInfo?.specialRequests }}</p>
                    </div>
                 </div>

                 <!-- Payment Info -->
                 <div class="flex items-center gap-3 pt-4">
                    <div class="w-1 h-5 bg-primary rounded-full"></div>
                    <h3 class="text-lg font-bold text-gray-800">Payment Information</h3>
                 </div>
                 <div class="bg-base-200/30 rounded-2xl p-6 border border-base-200/50">
                    <div class="flex items-center justify-between">
                       <div class="flex items-center gap-3">
                          <div class="w-10 h-10 bg-white shadow-sm rounded-lg flex items-center justify-center">
                             <i :class="booking?.paymentInfo?.method === 'card' ? 'fas fa-credit-card text-primary' : 'fas fa-hand-holding-usd text-secondary'"></i>
                          </div>
                          <div>
                             <p class="text-sm font-bold capitalize">{{ booking?.paymentInfo?.method }} Payment</p>
                             <p v-if="booking?.paymentInfo?.cardLastFour" class="text-xs text-gray-500">Ending in **** {{ booking?.paymentInfo?.cardLastFour }}</p>
                             <p v-else class="text-xs text-gray-500">Paid</p>
                          </div>
                       </div>
                       <div class="badge badge-success badge-sm text-white gap-1 py-3 px-3">
                          <i class="fas fa-check-circle text-[10px]"></i>
                          {{ booking?.paymentStatus === 'paid' ? 'Paid' : 'Verified' }}
                       </div>
                    </div>
                 </div>
              </div>

              <!-- Right Column: Price Breakdown -->
              <div>
                 <div class="flex items-center gap-3 mb-6">
                    <div class="w-1 h-5 bg-primary rounded-full"></div>
                    <h3 class="text-lg font-bold text-gray-800">Price Summary</h3>
                 </div>
                 <div class="bg-base-200/30 rounded-2xl p-6 border border-base-200/50 space-y-4">
                    <div class="flex justify-between items-center text-sm text-gray-600">
                       <span>Base Price</span>
                       <span>{{ formatPrice(booking?.pricing?.basePrice) }}</span>
                    </div>
                    <div v-if="bookingType === 'trip'" class="flex justify-between items-center text-sm font-medium pl-2 border-l-2 border-primary/20">
                       <span class="text-gray-500 text-xs">Guest Breakdown</span>
                       <span class="text-xs">
                          {{ booking?.bookingData?.guests?.adults || 0 }} Adults, {{ booking?.bookingData?.guests?.children || 0 }} Children
                       </span>
                    </div>
                    <div class="flex justify-between items-center text-sm text-gray-600">
                       <span>Subtotal</span>
                       <span>{{ formatPrice(booking?.pricing?.subtotal) }}</span>
                    </div>
                    <div class="flex justify-between items-center text-sm text-gray-600">
                       <span>Service Fee (5%)</span>
                       <span>{{ formatPrice(booking?.pricing?.serviceFee) }}</span>
                    </div>
                    <div class="flex justify-between items-center text-sm text-gray-600">
                       <span>Taxes & VAT (14%)</span>
                       <span>{{ formatPrice(booking?.pricing?.taxes) }}</span>
                    </div>
                    <div class="h-px bg-gray-200 my-2"></div>
                    <div class="flex justify-between items-center text-lg font-bold text-gray-900 pt-2">
                       <span>Total Amount</span>
                       <span class="text-primary">{{ formatPrice(booking?.pricing?.total) }}</span>
                    </div>
                    
                    <div class="mt-6 p-4 bg-primary/5 rounded-xl border border-primary/10">
                       <div class="flex gap-3">
                          <i class="fas fa-info-circle text-primary mt-1"></i>
                          <p class="text-xs text-gray-600 leading-relaxed">
                             All prices include local taxes and booking fees. A confirmation email has been sent to {{ booking?.guestInfo?.email }}.
                          </p>
                       </div>
                    </div>
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
                     <span class="text-primary font-bold">{{ formatPrice(item.price) }}</span>
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
import { useTripsStore } from '@/stores/tripsStore';
import StepIndicator from '@/components/Common/StepIndicator.vue';
import Location from '@/components/Common/Location.vue';

const router = useRouter();
const route = useRoute();
const bookingStore = useBookingStore();
const tripsStore = useTripsStore();

const booking = ref(null);
const tripDetails = ref(null);
const paymentStatus = ref('');
const paymentStatusNote = ref('');

const bookingType = computed(() => booking.value?.type || 'trip');

const bookingImage = computed(() => {
  const item = tripDetails.value || booking.value?.itemData; 
  if (item?.images?.length > 0) return item.images[0];
  return item?.imageUrl || '/images/placeholder.jpg';
});

const formattedDate = computed(() => {
  const dateStr = booking.value?.bookingData?.date;
  if (!dateStr) return 'N/A';
  
  const d = new Date(dateStr);
  if (isNaN(d.getTime())) return dateStr;

  return d.toLocaleDateString('en-US', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
});

const formattedGuests = computed(() => {
    const guests = booking.value?.bookingData?.guests;
    if (!guests) return '0 Guests';
    
    // If it's the complex object from trips
    if (typeof guests === 'object') {
        const adults = guests.adults || 0;
        const children = guests.children || 0;
        const total = adults + children;
        
        const parts = [];
        if (adults) parts.push(`${adults} ${adults === 1 ? 'Adult' : 'Adults'}`);
        if (children) parts.push(`${children} ${children === 1 ? 'Child' : 'Children'}`);
        
        return `${total} Guests${parts.length ? ' (' + parts.join(', ') + ')' : ''}`;
    }
    
    // Fallback for simple number
    return `${guests} Guests`;
});

// Location Data Computed - Prefers fresh trip details, falls back to booking data
const locationData = computed(() => {
    return tripDetails.value || booking.value?.itemData || {};
});

// Recommendations Logic
const recommendations = ref([]);

const formatPrice = (price) => {
    if (typeof price === 'number') return `${price.toFixed(2)}$`;
    if (typeof price === 'string' && price.includes('$')) return price;
    return price ? `${price}$` : '0$';
};

const paymentStatusLabel = computed(() => {
  switch (paymentStatus.value) {
    case 'succeeded':
      return 'Paid';
    case 'processing':
      return 'Processing';
    case 'requires_payment_method':
      return 'Payment failed';
    case 'requires_action':
      return 'Action needed';
    default:
      return paymentStatus.value || 'Pending';
  }
});

const paymentStatusClass = computed(() => {
  switch (paymentStatus.value) {
    case 'succeeded':
      return 'badge-success';
    case 'processing':
      return 'badge-warning';
    case 'requires_payment_method':
      return 'badge-error';
    default:
      return 'badge-neutral';
  }
});

const verifyPaymentStatus = async () => {
  const clientSecret = route.query.payment_intent_client_secret || sessionStorage.getItem('stripe_client_secret');
  const publishableKey = sessionStorage.getItem('stripe_publishable_key') || import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY;

  if (!clientSecret || !publishableKey) return;

  try {
    const stripe = await loadStripe(publishableKey);
    const { paymentIntent, error } = await stripe.retrievePaymentIntent(clientSecret);

    if (paymentIntent) {
      paymentStatus.value = paymentIntent.status;
      paymentStatusNote.value = paymentIntent.last_payment_error?.message || '';
    } else if (error) {
      paymentStatus.value = 'requires_payment_method';
      paymentStatusNote.value = error.message || '';
    }
  } catch (err) {
    console.error('Failed to verify payment intent', err);
  }
};

onMounted(async () => {
   const id = route.params.id;
   
   // 1. Verify Stripe payment first
   await verifyPaymentStatus();
   
   // 2. Fetch Booking with Fallback (use in-progress for now since mock API is down)
   booking.value = bookingStore.bookingInProgress;

   if (!booking.value) {
      console.warn("No booking data found for confirmation");
      // Fallback if needed
      return;
   }

   // 3. Fetch Fresh Trip Details (For Location/Images/etc)
   if (booking.value.itemId) {
       try {
           const trip = await tripsStore.fetchTripById(booking.value.itemId);
           tripDetails.value = trip;
       } catch (err) {
           console.error("Failed to fetch fresh trip details", err);
       }
   }

   // 4. Fetch Recommendations (Trips)
   try {
       await tripsStore.fetchTrips();
       // Get top 3 trips (excluding current)
       recommendations.value = tripsStore.trips
          .filter(t => t.id != booking.value.itemId)
          .slice(0, 3);
   } catch (err) {
       console.error("Failed to fetch recommendations", err);
   }
});

const goHome = () => {
  router.push('/');
};

const handleRecommendationClick = (item) => {
   router.push({ name: 'TripDetails', params: { id: item.id } });
};
</script>