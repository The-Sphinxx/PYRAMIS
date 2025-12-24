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
           :use-stripe-elements="true"
           @submit="handleConfirm"
        >
          <template #payment-element>
            <div class="space-y-3">
              <div id="payment-element" class="rounded-xl border border-base-300 bg-base-200/60 p-4"></div>
              <p class="text-xs text-base-content/60">Secured by Stripe. Your card details never hit our servers.</p>
              <p v-if="paymentError" class="text-sm text-error">{{ paymentError }}</p>
              <p v-if="paymentMessage" class="text-sm text-success">{{ paymentMessage }}</p>
            </div>
          </template>
        </GuestInfoForm>

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
import { loadStripe } from '@stripe/stripe-js';
import { useAuthStore } from '@/stores/authStore';
import { useBookingStore } from '@/stores/bookingStore';
import { useRoute, useRouter } from 'vue-router';
import { useHotelsStore } from '@/stores/hotelsStore';
import StepIndicator from '@/components/Common/StepIndicator.vue';
import GuestInfoForm from '@/components/Common/GuestInfoForm.vue';
import PriceSummary from '@/components/Common/PriceSummary.vue';
import { calculateBookingCosts } from '@/Utils/bookingCalculator.js';
import hotelsApi from '@/Services/hotelsApi';

const route = useRoute();
const router = useRouter();
const hotelStore = useHotelsStore();
const authStore = useAuthStore();
const bookingStore = useBookingStore();

const hotel = ref(null);
const submitting = ref(false);
const paymentError = ref('');
const paymentMessage = ref('');
const clientSecret = ref('');
const bookingId = ref(null);
const paymentIntentId = ref(null);
const stripeInstance = ref(null);
const currentPublishableKey = ref('');
const elements = ref(null);
const paymentElement = ref(null);

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
  paymentElement.value.mount('#payment-element');
};

const initializePaymentIntent = async () => {
  const user = authStore.user;
  const bookingUserId = user ? user.id : `guest_${Date.now()}`;
  const totalAmount = Number(costs.value.total);

  if (!hotel.value || !bookingData.value.checkIn || !bookingData.value.checkOut || !totalAmount) {
    throw new Error('Booking details are incomplete');
  }

  const intentPayload = await hotelsApi.createBookingIntent({
    userId: bookingUserId,
    entityId: Number(hotel.value.id),
    bookingType: 'Hotel',
    startDate: new Date(bookingData.value.checkIn).toISOString(),
    endDate: new Date(bookingData.value.checkOut).toISOString(),
    totalPrice: totalAmount
  });

  await mountPaymentElement(intentPayload);
};

const handleConfirm = async () => {
  submitting.value = true;
  paymentError.value = '';
  paymentMessage.value = '';

  try {
    // Check if user selected "Pay on Arrival"
    if (guest.value.paymentMethod === 'arrival') {
      // Skip Stripe payment, go directly to confirmation
      bookingStore.enrichBookingWithDetails(
        {
          firstName: guest.value.firstName,
          lastName: guest.value.lastName,
          email: guest.value.email,
          phone: guest.value.phone,
          specialRequests: guest.value.specialRequests
        },
        {
          method: 'arrival',
          cardLastFour: null,
          status: 'pending'
        }
      );
      
      router.push({
        name: 'HotelConfirmation',
        params: { id: route.params.id }
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

    const returnUrl = `${window.location.origin}/hotels/confirmation/${route.params.id}?bookingId=${bookingId.value ?? ''}`;

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
          firstName: guest.value.firstName,
          lastName: guest.value.lastName,
          email: guest.value.email,
          phone: guest.value.phone,
          specialRequests: guest.value.specialRequests
        },
        {
          method: 'card',
          cardLastFour: null,
          status: 'paid'
        }
      );
            router.push({
        name: 'HotelConfirmation',
        params: { id: route.params.id },
        query: {
          bookingId: bookingId.value ?? undefined,
          payment_intent: paymentIntentId.value ?? undefined
        }
      });
    } else if (status === 'processing') {
      paymentMessage.value = 'Payment is processing. We will email you once confirmed.';
    }
  } catch (error) {
    console.error('Booking failed:', error);
    paymentError.value = error.message || 'Failed to confirm booking. Please try again.';
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

  if (hotel.value) {
    try {
      await initializePaymentIntent();
    } catch (error) {
      console.error('Failed to initialize payment intent:', error);
      paymentError.value = error.message || 'Unable to start payment. Please try again later.';
    }
  }
});
</script>