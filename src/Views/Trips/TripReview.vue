<template>
  <div class="page-container py-12 font-cairo min-h-screen bg-base-100">
    <div class="max-w-7xl mx-auto">
      <!-- Step Indicator -->
      <div class="mb-8">
        <StepIndicator :current-step="1" :booking-type="bookingType" />
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center min-h-[400px]">
        <span class="loading loading-spinner loading-lg text-primary"></span>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="alert alert-error shadow-lg">
        <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <span>{{ error }}</span>
        <button @click="goBack" class="btn btn-sm">Go Back</button>
      </div>

      <!-- Review Content -->
      <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-8 items-start relative">
        <!-- Left Side - Booking Details (Ref for Bottom Boundary) -->
        <div ref="contentColumn" class="lg:col-span-2 space-y-8">
          
          <!-- Image Header -->
          <div class="w-full h-[400px] rounded-xl overflow-hidden shadow-lg relative">
             <img 
               :src="itemImage" 
               alt="Item Image"
               class="w-full h-full object-cover"
             />
             <div class="absolute bottom-0 left-0 w-full bg-gradient-to-t from-black/70 to-transparent p-6 text-white">
                <h2 class="text-3xl font-bold mb-2">{{ bookingStore.bookingInProgress?.itemName }}</h2>
                <div class="flex items-center gap-2 text-white/90">
                  <i class="fas fa-map-marker-alt text-primary"></i>
                  <span>{{ bookingStore.bookingInProgress?.itemData?.city || bookingStore.bookingInProgress?.itemData?.location }}, Egypt</span>
                </div>
             </div>
          </div>

          <!-- Description / Info -->
          <div>
            <p class="text-base-content/80 leading-relaxed text-lg">
               {{ bookingStore.bookingInProgress?.itemData?.description }}
            </p>
          </div>

          <!-- Vertical Booking Details Cards -->
          <div class="space-y-4">
             <div class="flex items-center gap-4 border-l-4 border-primary pl-4 bg-base-200/50 p-4 rounded-r-lg">
                <div class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center text-primary text-xl">
                  <i class="fas fa-users"></i>
                </div>
                <div>
                  <p class="text-sm text-base-content/60">Guests</p>
                  <p class="font-bold text-lg">{{ formattedGuests }}</p>
                </div>
             </div>

             <div class="flex items-center gap-4 border-l-4 border-primary pl-4 bg-base-200/50 p-4 rounded-r-lg">
                <div class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center text-primary text-xl">
                  <i class="fas fa-calendar-alt"></i>
                </div>
                <div>
                  <p class="text-sm text-base-content/60">Date</p>
                  <p class="font-bold text-lg">{{ formattedDate }}</p>
                </div>
             </div>

             <!-- Total Price -->
             <!-- <div class="flex items-center gap-4 border-l-4 border-primary pl-4 bg-base-200/50 p-4 rounded-r-lg">
                <div class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center text-primary text-xl">
                  <i class="fas fa-tag"></i>
                </div>
                <div>
                  <p class="text-sm text-base-content/60">Total Price</p>
                  <p class="font-bold text-xl text-primary">{{ formatPrice(bookingStore.bookingCosts?.total || 0) }}</p>
                </div>
             </div>
              -->
             
             <!-- Dynamic Amenities -->
              <div v-if="bookingStore.bookingInProgress?.itemData?.amenities?.transport" class="flex items-center gap-4 border-l-4 border-primary pl-4 bg-base-200/50 p-4 rounded-r-lg">
                <div class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center text-primary text-xl">
                  <i class="fas fa-bus"></i>
                </div>
                <div>
                  <p class="text-sm text-base-content/60">Transport</p>
                  <p class="font-bold text-lg">Included</p>
                </div>
             </div>

             <div v-if="bookingStore.bookingInProgress?.itemData?.amenities?.meals" class="flex items-center gap-4 border-l-4 border-primary pl-4 bg-base-200/50 p-4 rounded-r-lg">
                <div class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center text-primary text-xl">
                  <i class="fas fa-utensils"></i>
                </div>
                <div>
                  <p class="text-sm text-base-content/60">Meals</p>
                  <p class="font-bold text-lg">{{ bookingStore.bookingInProgress?.itemData?.amenities.meals }}</p>
                </div>
             </div>

             <!-- Total Price moved up -->
          </div>

          <!-- Proceed Button -->
          <button 
              @click="handleProceed"
              class="btn btn-primary btn-lg w-full text-white font-bold text-xl rounded-lg shadow-lg hover:brightness-110 border-0"
            >
              Continue to Guest Information
          </button>
        
        </div>

        <!-- Right Side - Price Summary (Sticky JS) -->
        <div ref="bookingColumn" class="lg:col-span-1 h-full min-h-[500px] relative hidden lg:block">
          <!-- Placeholder to prevent layout shift when fixed -->
          <div ref="bookingWrapper" :style="{ minHeight: isSticky ? '1px' : 'auto' }"></div>
          
          <div 
            ref="stickyForm"
            :style="stickyStyle"
            :class="{ 'fixed top-24 z-50': isSticky }"
            class="transition-all duration-300"
          >
             <PriceSummary
              :costs="bookingStore.bookingCosts"
              :booking-type="bookingType"
              :booking-data="bookingStore.bookingInProgress?.bookingData"
              :base-price="bookingStore.bookingInProgress?.basePrice"
              :add-ons="0"
            />
          </div>
        </div>
        
        <!-- Mobile Price Summary (Static) -->
         <div class="lg:hidden">
            <PriceSummary
              :costs="bookingStore.bookingCosts"
              :booking-type="bookingType"
              :booking-data="bookingStore.bookingInProgress?.bookingData"
              :base-price="bookingStore.bookingInProgress?.basePrice"
              :add-ons="0"
            />
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { useBookingStore } from '@/stores/bookingStore';
import StepIndicator from '@/components/Common/StepIndicator.vue';
import PriceSummary from '@/components/Common/PriceSummary.vue';
import { formatPrice as formatPriceUtil } from '@/Utils/bookingCalculator.js';

const router = useRouter();
const bookingStore = useBookingStore();

const loading = ref(false);
const error = ref(null);

const bookingType = computed(() => {
  return bookingStore.bookingInProgress?.type || 'trip';
});

const formattedDate = computed(() => {
  const dateStr = bookingStore.bookingInProgress?.bookingData?.date;
  if (!dateStr) return 'Not selected';
  
  const d = new Date(dateStr);
  // Check if valid date
  if (isNaN(d.getTime())) {
      // If invalid date (maybe a custom string like 'Summer 2024'), return as is
      return dateStr;
  }
  
  return d.toLocaleDateString('en-US', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
});

const formattedGuests = computed(() => {
    const guests = bookingStore.bookingInProgress?.bookingData?.guests;
    if (!guests) return '0 Guests';
    
    // If it's the complex object from trips
    if (typeof guests === 'object') {
        const total = (guests.adults || 0) + (guests.children || 0);
        const parts = [];
        if (guests.adults) parts.push(`${guests.adults} Adults`);
        if (guests.children) parts.push(`${guests.children} Children`);
        
        return `${total} Guests${parts.length ? ' (' + parts.join(', ') + ')' : ''}`;
    }
    
    // Fallback for simple number
    return `${guests} Guests`;
});

const itemImage = computed(() => {
  const item = bookingStore.bookingInProgress?.itemData;
  if (item?.images && item.images.length > 0) return item.images[0];
  if (item?.image) return item.image;
  return '/images/placeholder.jpg';
});

onMounted(() => {
  // Check if there's a booking in progress
  if (!bookingStore.bookingInProgress) {
    error.value = 'No booking in progress';
    setTimeout(() => {
      // router.push({ name: 'Home' }); // Commented out for dev convenience, uncomment in prod
    }, 2000);
  }
   
  window.addEventListener('scroll', handleScroll);
  window.addEventListener('resize', handleResize);
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
  window.removeEventListener('resize', handleResize);
});

const handleProceed = () => {
  router.push({ 
    name: 'TripCheckOut', 
    params: { id: bookingStore.bookingInProgress.itemId } 
  });
};

const goBack = () => {
  router.back();
};

// -- Sticky Logic --
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
  const formHeight = stickyForm.value.offsetHeight;
  
  const offsetTop = 100;

  // Bottom Boundary Logic
  if (contentRect.bottom <= offsetTop + formHeight) {
    isSticky.value = false;
    stickyStyle.value = {
      position: 'absolute',
      bottom: '0',
      left: '0',
      width: '100%',
      zIndex: 40
    };
  } else if (rect.top <= offsetTop) {
     // Sticky Logic
    isSticky.value = true;
    stickyStyle.value = {
      position: 'fixed',
      top: `${offsetTop}px`,
      width: `${bookingWrapper.value.getBoundingClientRect().width}px`,
      left: `${bookingWrapper.value.getBoundingClientRect().left}px`,
      zIndex: 50
    };
  } else {
    // Normal Logic
    isSticky.value = false;
    stickyStyle.value = {};
  }
};

const handleResize = () => {
  if (isSticky.value && bookingWrapper.value) {
    const rect = bookingWrapper.value.getBoundingClientRect();
    stickyStyle.value.width = `${rect.width}px`;
    stickyStyle.value.left = `${rect.left}px`;
  }
};

function formatPrice(price) {
  return formatPriceUtil(price, '$');
}
</script>
