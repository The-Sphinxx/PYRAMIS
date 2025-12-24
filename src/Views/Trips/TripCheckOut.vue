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
            :use-stripe-elements="true"
            @submit="handlePlaceOrder"
          >
            <template #payment-element>
              <div class="space-y-3">
                <div id="trip-payment-element" class="rounded-xl border border-base-300 bg-base-200/60 p-4"></div>
                <p class="text-xs text-base-content/60">Secured by Stripe. Your card details never hit our servers.</p>
                <p v-if="paymentError" class="text-sm text-error">{{ paymentError }}</p>
                <p v-if="paymentMessage" class="text-sm text-success">{{ paymentMessage }}</p>
              </div>
            </template>
          </GuestInfoForm>
        </div>

        <!-- Right Side - Price Summary (CSS Sticky) -->
        <div class="lg:col-span-1 h-full min-h-[500px] relative hidden lg:block">
           <div class="sticky top-24 z-40 transition-all duration-300">
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
import { ref, computed, onMounted } from 'vue';
import { loadStripe } from '@stripe/stripe-js';
import { useRouter } from 'vue-router';
import { useBookingStore } from '@/stores/bookingStore';
import { useTripsStore } from '@/stores/tripsStore';
import { useAuthStore } from '@/stores/authStore';
import StepIndicator from '@/components/Common/StepIndicator.vue';
import PriceSummary from '@/components/Common/PriceSummary.vue';
import GuestInfoForm from '@/components/Common/GuestInfoForm.vue';
import tripsApi from '@/Services/tripsApi';

const router = useRouter();
const bookingStore = useBookingStore();
const tripsStore = useTripsStore();
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
const paymentError = ref('');
const paymentMessage = ref('');
const clientSecret = ref('');
const bookingId = ref(null);
const paymentIntentId = ref(null);
const stripeInstance = ref(null);
const elements = ref(null);
const paymentElement = ref(null);
const currentPublishableKey = ref('');

// Get booking type from store
const bookingType = computed(() => {
  return bookingStore.bookingInProgress?.type || 'trip';
});

const totalCost = computed(() => bookingStore.bookingCosts?.total ?? bookingStore.bookingInProgress?.basePrice ?? 0);
const entityId = computed(() => bookingStore.bookingInProgress?.itemId ?? 0);
const bookingData = computed(() => bookingStore.bookingInProgress?.bookingData ?? {});

const appearance = {
  theme: 'stripe',
  variables: {
    colorPrimary: '#C86A3F',
    colorBackground: '#ffffff'
  }
};

const ensureStripe = async (publishableKey) => {
  if (!publishableKey) throw new Error('Stripe publishable key is missing');
  if (!stripeInstance.value || currentPublishableKey.value !== publishableKey) {
    stripeInstance.value = await loadStripe(publishableKey);
    currentPublishableKey.value = publishableKey;
  }
  return stripeInstance.value;
};

const mountPaymentElement = async (intentPayload) => {
  const publishableKey = import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY || intentPayload.publishableKey;
  const stripe = await ensureStripe(publishableKey);

  clientSecret.value = intentPayload.clientSecret;
  bookingId.value = intentPayload.bookingId;
  paymentIntentId.value = intentPayload.paymentIntentId;

  sessionStorage.setItem('stripe_publishable_key', publishableKey);
  sessionStorage.setItem('stripe_client_secret', intentPayload.clientSecret);
  sessionStorage.setItem('stripe_payment_intent_id', intentPayload.paymentIntentId);
  sessionStorage.setItem('stripe_booking_id', String(intentPayload.bookingId));

  if (paymentElement.value) {
    paymentElement.value.unmount();
  }

  elements.value = stripe.elements({ clientSecret: intentPayload.clientSecret, appearance });
  paymentElement.value = elements.value.create('payment');
  paymentElement.value.mount('#trip-payment-element');
};

const initializePaymentIntent = async () => {
  if (!bookingStore.bookingInProgress) throw new Error('No booking in progress');

  const user = authStore.user;
  const bookingUserId = user ? user.id : `guest_${Date.now()}`;
  const totalAmount = Number(totalCost.value);

  let startDate;
  try {
    startDate = bookingData.value.date ? new Date(bookingData.value.date).toISOString() : new Date().toISOString();
  } catch {
    startDate = new Date().toISOString();
  }
  const intentPayload = await tripsApi.createBookingIntent({
    userId: bookingUserId,
    entityId: Number(entityId.value),
    bookingType: 'Trip',
    startDate,
    endDate: null,
    totalPrice: totalAmount
  });

  await mountPaymentElement(intentPayload);
};

const handlePlaceOrder = async () => {
  submitting.value = true;
  paymentError.value = '';
  paymentMessage.value = '';

  try {
    // Check if user selected "Pay on Arrival"
    if (guestData.value.paymentMethod === 'arrival') {
      // Skip Stripe payment, go directly to confirmation
      bookingStore.enrichBookingWithDetails(
        {
          firstName: guestData.value.firstName,
          lastName: guestData.value.lastName,
          email: guestData.value.email,
          phone: guestData.value.phone,
          specialRequests: guestData.value.specialRequests
        },
        {
          method: 'arrival',
          cardLastFour: null,
          status: 'pending'
        }
      );
      
      router.push({
        name: 'TripConfirmation',
        params: { id: entityId.value }
      });
      return;
    }

    // Card payment flow
    if (!clientSecret.value) {
      await initializePaymentIntent();
    }

    if (!stripeInstance.value || !elements.value) {
      throw new Error('Payment form is not ready. Please retry.');
    }

    const returnUrl = `${window.location.origin}/trips/confirmation/${entityId.value}?bookingId=${bookingId.value ?? ''}`;

    const result = await stripeInstance.value.confirmPayment({
      elements: elements.value,
      confirmParams: {
        return_url: returnUrl
      },
      redirect: 'if_required'
    });

    if (result.error) {
      paymentError.value = result.error.message || 'Payment could not be processed.';
      return;
    }

    const status = result.paymentIntent?.status;
    if (status === 'succeeded') {
      paymentMessage.value = 'Payment succeeded! Redirecting...';
      
      // Enrich booking with guest info and pricing for confirmation page
      bookingStore.enrichBookingWithDetails(
        {
          firstName: guestData.value.firstName,
          lastName: guestData.value.lastName,
          email: guestData.value.email,
          phone: guestData.value.phone,
          specialRequests: guestData.value.specialRequests
        },
        {
          method: 'card',
          cardLastFour: null,
          status: 'paid'
        }
      );
      
      router.push({
        name: 'TripConfirmation',
        params: { id: entityId.value },
        query: {
          bookingId: bookingId.value ?? undefined,
          payment_intent: paymentIntentId.value ?? undefined
        }
      });
    } else if (status === 'processing') {
      paymentMessage.value = 'Payment is processing. We will email you once confirmed.';
    }
  } catch (err) {
    error.value = err.message || 'Failed to place order';
    paymentError.value = error.value;
    console.error('Order error:', err);
  } finally {
    submitting.value = false;
  }
};

const goBack = () => {
  router.back();
};

onMounted(() => {
  if (!bookingStore.bookingInProgress) {
    error.value = 'No booking in progress';
    setTimeout(() => {
      router.push({ name: 'Home' });
    }, 2000);
    return;
  }

  if (authStore.user) {
    guestData.value.firstName = authStore.user.firstName || '';
    guestData.value.lastName = authStore.user.lastName || '';
    guestData.value.email = authStore.user.email || '';
    guestData.value.phone = authStore.user.phone || '';
  }

  initializePaymentIntent().catch((err) => {
    console.error('Failed to initialize payment intent:', err);
    paymentError.value = err.message || 'Unable to start payment. Please try again later.';
  });
});
</script>