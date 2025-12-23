<template>
  <div class="page-container py-12 font-cairo min-h-screen bg-base-100">
    <div class="max-w-7xl mx-auto">
      <!-- Step Indicator -->
      <div class="mb-8">
        <StepIndicator :current-step="2" :booking-type="bookingType" />
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

      <!-- Checkout Content -->
      <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-8 items-start relative">
        <!-- Left Side - Guest Info & Payment -->
        <div ref="contentColumn" class="lg:col-span-2">
          <GuestInfoForm 
            ref="guestFormRef"
            v-model="guestData"
            :submitting="submitting"
            @submit="handlePlaceOrder"
          />
        </div>

        <!-- Right Side - Price Summary (Sticky CSS) -->
        <div class="lg:col-span-1 h-full relative hidden lg:block">
           <div class="sticky top-24 self-start transition-all duration-300">
            <PriceSummary
                :costs="bookingStore.bookingCosts"
                :booking-type="bookingType"
                :booking-data="bookingStore.bookingInProgress.bookingData"
                :base-price="bookingStore.bookingInProgress.basePrice"
                :add-ons="0"
            />
          </div>
        </div>

        <!-- Mobile Price Summary -->
        <div class="lg:hidden">
            <PriceSummary
                :costs="bookingStore.bookingCosts"
                :booking-type="bookingType"
                :booking-data="bookingStore.bookingInProgress.bookingData"
                :base-price="bookingStore.bookingInProgress.basePrice"
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
import { useAuthStore } from '@/stores/authStore';
import StepIndicator from '@/components/Common/StepIndicator.vue';
import PriceSummary from '@/components/Common/PriceSummary.vue';
import GuestInfoForm from '@/components/Common/GuestInfoForm.vue';

const router = useRouter();
const bookingStore = useBookingStore();
const authStore = useAuthStore(); 

const guestFormRef = ref(null);
const guestData = ref({
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

const loading = ref(false);
const submitting = ref(false);
const error = ref(null);

// Get booking type from store
const bookingType = computed(() => {
  return bookingStore.bookingInProgress?.type || 'attraction';
});

onMounted(() => {
  // Check if there's a booking in progress
  if (!bookingStore.bookingInProgress) {
    error.value = 'No booking in progress';
    setTimeout(() => {
      router.push({ name: 'Home' });
    }, 2000);
  }
  
  // Pre-fill user data if logged in
  if (authStore.user) {
    guestData.value.firstName = authStore.user.firstName || '';
    guestData.value.lastName = authStore.user.lastName || '';
    guestData.value.email = authStore.user.email || '';
    guestData.value.phone = authStore.user.phone || '';
  }
});


const handlePlaceOrder = async () => {
  submitting.value = true;
  error.value = null;

  try {
    const userId = authStore.user?.id || `guest_${Date.now()}`;

    // Submit booking through store, passing user ID and guest data
    const result = await bookingStore.submitBooking(userId, guestData.value);
    
    console.log('Booking created:', result);

    // Navigate to confirmation page
    router.push({ 
      name: 'AttractionConfirmation', 
      params: { id: result.id } 
    });
  } catch (err) {
    error.value = err.message || 'Failed to place order';
    console.error('Order error:', err);
    // alert(`❌ Failed to place order: ${error.value}`); // Basic alert
  } finally {
    submitting.value = false;
  }
};

const goBack = () => {
  router.back();
};


</script>