<template>
  <div class="card bg-base-100 shadow-xl sticky top-24">
    <div class="card-body font-cairo">
      <h2 class="card-title text-2xl mb-4">Price Summary</h2>

      <!-- Main Price Breakdown -->
      <div class="space-y-3">
        <div class="flex justify-between text-base">
          <span class="text-base-content/70">{{ priceBreakdownLabel }}</span>
          <span class="font-semibold">{{ formatPrice(costs.subtotal) }}</span>
        </div>

        <!-- Service Fee -->
        <div class="flex justify-between text-base">
          <span class="text-base-content/70">Service fee</span>
          <span class="font-semibold">{{ formatPrice(costs.serviceFee) }}</span>
        </div>

        <!-- Taxes -->
        <div class="flex justify-between text-base">
          <span class="text-base-content/70">Taxes (14% VAT)</span>
          <span class="font-semibold">{{ formatPrice(costs.taxes) }}</span>
        </div>

        <!-- Add-ons (if provided) -->
        <div v-if="addOns > 0" class="flex justify-between text-base">
          <span class="text-base-content/70">Add-Ons</span>
          <span class="font-semibold">{{ formatPrice(addOns) }}</span>
        </div>
      </div>

      <div class="divider my-2"></div>

      <!-- Total -->
      <div class="flex justify-between items-center">
        <span class="text-xl font-bold">Total:</span>
        <span class="text-2xl font-bold text-primary">
          {{ formatPrice(totalWithAddOns) }}
        </span>
      </div>

      <div class="divider my-2"></div>

      <!-- Benefits -->
      <div class="space-y-2">
        <div class="flex items-center gap-2 text-success text-sm">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 flex-shrink-0" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
          </svg>
          <span class="font-medium">Free cancellation before check in</span>
        </div>

        <div class="flex items-center gap-2 text-success text-sm">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 flex-shrink-0" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
          </svg>
          <span class="font-medium">Instant confirmation</span>
        </div>

        <div class="flex items-start gap-2 text-base-content/70 text-sm">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 flex-shrink-0 mt-0.5" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
          </svg>
          <span>No hidden fees</span>
        </div>

        <div class="flex items-start gap-2 text-base-content/70 text-sm">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 flex-shrink-0 mt-0.5" viewBox="0 0 20 20" fill="currentColor">
            <path d="M2 3a1 1 0 011-1h2.153a1 1 0 01.986.836l.74 4.435a1 1 0 01-.54 1.06l-1.548.773a11.037 11.037 0 006.105 6.105l.774-1.548a1 1 0 011.059-.54l4.435.74a1 1 0 01.836.986V17a1 1 0 01-1 1h-2C7.82 18 2 12.18 2 5V3z" />
          </svg>
          <span>24/7 customer support</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { formatPrice as formatPriceUtil } from '@/Utils/bookingCalculator.js';

const props = defineProps({
  costs: {
    type: Object,
    required: true,
    validator: (value) => {
      return value.subtotal !== undefined &&
             value.serviceFee !== undefined &&
             value.taxes !== undefined &&
             value.total !== undefined;
    }
  },
  bookingType: {
    type: String,
    required: true,
    validator: (value) => ['attraction', 'hotel', 'car', 'trip'].includes(value)
  },
  bookingData: {
    type: Object,
    required: true
  },
  basePrice: {
    type: [String, Number],
    required: true
  },
  addOns: {
    type: Number,
    default: 0
  }
});

const priceBreakdownLabel = computed(() => {
  const { duration } = props.costs;
  
  switch (props.bookingType) {
    case 'attraction':
      const guests = props.bookingData.guests || 1;
      return `${formatPrice(props.costs.basePrice)} × ${guests} ${guests === 1 ? 'person' : 'persons'}`;
      
    case 'hotel':
      const rooms = props.bookingData.rooms || 1;
      const nights = props.bookingData.nights || duration;
      return `${formatPrice(props.costs.basePrice)} × ${nights} ${nights === 1 ? 'night' : 'nights'} × ${rooms} ${rooms === 1 ? 'room' : 'rooms'}`;
      
    case 'car':
      const days = props.bookingData.days || duration;
      return `${formatPrice(props.costs.basePrice)} × ${days} ${days === 1 ? 'day' : 'days'}`;
      
    case 'trip':
      const travelers = props.bookingData.travelers || 1;
      return `${formatPrice(props.costs.basePrice)} × ${travelers} ${travelers === 1 ? 'person' : 'persons'}`;
      
    default:
      return '';
  }
});

const totalWithAddOns = computed(() => {
  return props.costs.total + props.addOns;
});

function formatPrice(price) {
  return formatPriceUtil(price, '$');
}
</script>