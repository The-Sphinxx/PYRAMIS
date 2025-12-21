<template>
  <div class="bg-base-100 rounded-lg shadow-lg p-6 font-cairo">
    <!-- Price Header -->
    <div class="mb-6 pb-4 border-b border-base-300">
      <div class="flex items-baseline gap-2">
        <span class="text-3xl font-bold text-base-content">{{ formattedBasePrice }}</span>
        <span class="text-base-content/60">{{ priceUnit }}</span>
      </div>
    </div>

    <!-- Attraction Booking Form -->
    <div v-if="type === 'attraction'" class="space-y-4">
      <DatePicker
        v-model="localBookingData.date"
        :min-date="today"
        label="Select Date"
        placeholder="mm/dd/yyyy"
        @update:modelValue="updateField('date', $event)"
      />

      <GuestSelector
        v-model="localBookingData.guests"
        label="Guests"
        icon="guests"
        singular-label="Guest"
        plural-label="Guests"
        @update:modelValue="updateField('guests', $event)"
      />
    </div>

    <!-- Hotel Booking Form -->
    <div v-else-if="type === 'hotel'" class="space-y-4">
      <DatePicker
        v-model="localBookingData.checkIn"
        :min-date="today"
        label="Check-in Date"
        placeholder="mm/dd/yyyy"
        @update:modelValue="updateField('checkIn', $event)"
      />

      <DatePicker
        v-model="localBookingData.checkOut"
        :min-date="checkOutMinDate"
        label="Check-out Date"
        placeholder="mm/dd/yyyy"
        @update:modelValue="updateField('checkOut', $event)"
      />

      <div class="grid grid-cols-2 gap-4">
        <GuestSelector
          v-model="localBookingData.rooms"
          label="Rooms"
          icon="room"
          singular-label="Room"
          plural-label="Rooms"
          :min="1"
          :max="10"
          @update:modelValue="updateField('rooms', $event)"
        />
        
        <GuestSelector
          v-model="localBookingData.guests"
          label="Guests"
          icon="guests"
          singular-label="Guest"
          plural-label="Guests"
          :min="1"
          :max="50"
          @update:modelValue="updateField('guests', $event)"
        />
      </div>
    </div>

    <!-- Car Booking Form -->
    <div v-else-if="type === 'car'" class="space-y-4">
      <DatePicker
        v-model="localBookingData.pickupDate"
        :min-date="today"
        label="Pickup Date"
        placeholder="mm/dd/yyyy"
        @update:modelValue="updateField('pickupDate', $event)"
      />

      <TimePicker
        v-model="localBookingData.pickupTime"
        label="Pickup Time"
        placeholder="--:-- --"
        @update:modelValue="updateField('pickupTime', $event)"
      />

      <DatePicker
        v-model="localBookingData.returnDate"
        :min-date="returnDateMinDate"
        label="Return Date"
        placeholder="mm/dd/yyyy"
        @update:modelValue="updateField('returnDate', $event)"
      />

      <GuestSelector
        v-model="localBookingData.passengers"
        label="Passengers"
        icon="passengers"
        singular-label="Passenger"
        plural-label="Passengers"
        :min="1"
        :max="20"
        @update:modelValue="updateField('passengers', $event)"
      />
    </div>

    <!-- Trip Booking Form -->
    <div v-else-if="type === 'trip'" class="space-y-4">
       <!-- Dates -->
       <div v-if="availableDates && availableDates.length">
            <label class="label text-sm font-bold text-base-content/70 font-cairo pb-1">Select Date</label>
            <div class="flex flex-col gap-2">
                <div 
                    v-for="(date, idx) in availableDates" 
                    :key="idx"
                    class="bg-base-200/50 rounded-lg p-3 flex justify-between items-center cursor-pointer border transition-all hover:border-primary/50"
                    :class="localBookingData.date === date ? 'border-primary bg-primary/5 ring-1 ring-primary' : 'border-transparent'"
                    @click="updateField('date', date)"
                >
                    <span class="font-bold text-sm">{{ date }}</span>
                    <div class="w-4 h-4 rounded-full border border-base-content/20 flex items-center justify-center">
                        <div v-if="localBookingData.date === date" class="w-2.5 h-2.5 rounded-full bg-primary"></div>
                    </div>
                </div>
            </div>
        </div>
        <div v-else>
             <!-- Fallback if no specific dates provided -->
              <DatePicker
                v-model="localBookingData.date"
                :min-date="today"
                label="Select Date"
                placeholder="mm/dd/yyyy"
                @update:modelValue="updateField('date', $event)"
              />
        </div>

        <!-- Guests (Adults/Children) -->
        <div> 
            <label class="label text-sm font-bold text-base-content/70 font-cairo pb-1">Guests</label>
            <div class="bg-base-200/50 rounded-lg p-1 flex items-center justify-between border border-transparent hover:border-base-content/10">
                    <div class="p-2">
                        <span class="block text-xs font-bold text-base-content/80">Adults</span>
                        <span class="text-xs text-base-content/40">Age 13+</span>
                    </div>
                    <div class="flex items-center gap-3 pr-2">
                        <button class="btn btn-circle btn-xs btn-ghost bg-white shadow-sm" type="button" @click="updateTripGuests('adults', -1)" :disabled="localBookingData.guests.adults <= 1">-</button>
                        <span class="font-bold w-4 text-center">{{ localBookingData.guests.adults }}</span>
                        <button class="btn btn-circle btn-xs btn-ghost bg-white shadow-sm" type="button" @click="updateTripGuests('adults', 1)" :disabled="localBookingData.guests.adults >= (maxPeople || 10)">+</button>
                    </div>
            </div>
                <div class="bg-base-200/50 rounded-lg p-1 flex items-center justify-between border border-transparent hover:border-base-content/10 mt-2">
                    <div class="p-2">
                        <span class="block text-xs font-bold text-base-content/80">Children</span>
                        <span class="text-xs text-base-content/40">Age 2-12</span>
                    </div>
                    <div class="flex items-center gap-3 pr-2">
                        <button class="btn btn-circle btn-xs btn-ghost bg-white shadow-sm" type="button" @click="updateTripGuests('children', -1)" :disabled="localBookingData.guests.children <= 0">-</button>
                        <span class="font-bold w-4 text-center">{{ localBookingData.guests.children }}</span>
                        <button class="btn btn-circle btn-xs btn-ghost bg-white shadow-sm" type="button" @click="updateTripGuests('children', 1)">+</button>
                    </div>
            </div>
        </div>
    </div>

    <!-- Cost Breakdown -->
    <div v-if="costs" class="mt-6 space-y-3 bg-base-200 rounded-lg p-4">
      <div class="flex justify-between text-sm text-base-content/70">
        <span>{{ costBreakdownLabel }}</span>
        <span>{{ formatPrice(costs.subtotal) }}</span>
      </div>

      <div class="flex justify-between text-sm text-base-content/70">
        <span>Service fee</span>
        <span>{{ formatPrice(costs.serviceFee) }}</span>
      </div>

      <div class="flex justify-between text-sm text-base-content/70">
        <span>Taxes</span>
        <span>{{ formatPrice(costs.taxes) }}</span>
      </div>

      <div class="h-px bg-base-300 my-2"></div>

      <div class="flex justify-between font-bold text-lg">
        <span>Total:</span>
        <span class="text-primary">{{ formatPrice(costs.total) }}</span>
      </div>
    </div>

    <!-- Validation Errors -->
    <div v-if="validationErrors.length > 0" class="mt-4 space-y-2">
      <div v-for="(error, index) in validationErrors" :key="index" class="alert alert-error text-sm py-2">
        <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-5 w-5" fill="none" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <span class="text-xs">{{ error }}</span>
      </div>
    </div>

    <!-- Book Now Button -->
    <button
      @click="handleBooking"
      :disabled="loading || !isFormValid"
      type="button"
      class="btn btn-primary w-full mt-6 text-white font-bold"
      :class="{ 'loading': loading }"
    >
      <span v-if="!loading">Book Now</span>
      <span v-else>Processing...</span>
    </button>

    <!-- Cancellation Policy -->
    <div class="mt-4 flex items-center justify-center gap-2 text-success text-sm">
      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
        <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
      </svg>
      <span class="font-medium">Free cancellation before check in</span>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';
import DatePicker from '@/components/Common/DatePicker.vue';
import TimePicker from '@/components/Common/TimePicker.vue';
import GuestSelector from '@/components/Common/GuestSelector.vue';
import { calculateBookingCosts, validateBookingData, extractPrice, formatPrice as formatPriceUtil } from '@/Utils/bookingCalculator.js';

const props = defineProps({
  type: {
    type: String,
    required: true,
    validator: (value) => ['attraction', 'hotel', 'car', 'trip'].includes(value)
  },
  basePrice: {
    type: [String, Number],
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  },
  availableDates: {
    type: Array,
    default: () => []
  },
  maxPeople: {
    type: Number,
    default: 10
  }
});

const emit = defineEmits(['submit']);

// Local booking data
const localBookingData = ref(getDefaultData());

// Validation errors
const validationErrors = ref([]);

// Min dates
const today = new Date();
today.setHours(0, 0, 0, 0);

const checkOutMinDate = computed(() => {
  if (localBookingData.value.checkIn) {
    const checkIn = new Date(localBookingData.value.checkIn);
    const nextDay = new Date(checkIn);
    nextDay.setDate(checkIn.getDate() + 1);
    return nextDay;
  }
  return today;
});

const returnDateMinDate = computed(() => {
  if (localBookingData.value.pickupDate) {
    const pickup = new Date(localBookingData.value.pickupDate);
    // Return same day is usually allowed for cars, or +1 depending on policy.
    // Let's allow same day for now.
    return pickup;
  }
  return today;
});

// Get default data based on type
function getDefaultData() {
  const baseData = { date: null };

  switch (props.type) {
    case 'attraction':
      return { ...baseData, guests: 2 };
    case 'hotel':
      return { checkIn: null, checkOut: null, rooms: 1, guests: 2, nights: 0 };
    case 'car':
      return { pickupDate: null, returnDate: null, pickupTime: null, passengers: 2, days: 0 };
    case 'trip':
      return { ...baseData, guests: { adults: 2, children: 0 } };
    default:
      return baseData;
  }
}

// Computed
const formattedBasePrice = computed(() => {
  return formatPrice(extractPrice(props.basePrice));
});

const priceUnit = computed(() => {
  const units = {
    attraction: '/person',
    hotel: '/night',
    car: '/day',
    trip: '/person'
  };
  return units[props.type] || '';
});

const costs = computed(() => {
  return calculateBookingCosts(props.type, props.basePrice, localBookingData.value);
});

const costBreakdownLabel = computed(() => {
  const { duration } = costs.value;
  const count = getCountValue();

  switch (props.type) {
    case 'attraction':
      return `${formattedBasePrice.value} × ${count} ${count === 1 ? 'person' : 'persons'}`;
    case 'hotel':
      return `${formattedBasePrice.value} × ${duration} ${duration === 1 ? 'night' : 'nights'} × ${localBookingData.value.rooms} ${localBookingData.value.rooms === 1 ? 'room' : 'rooms'}`;
    case 'car':
      return `${formattedBasePrice.value} × ${duration} ${duration === 1 ? 'day' : 'days'}`;
    case 'trip':
      if (localBookingData.value.guests && typeof localBookingData.value.guests === 'object') {
           const { adults, children } = localBookingData.value.guests;
           const parts = [];
           if (adults) parts.push(`${adults} Adults`);
           if (children) parts.push(`${children} Children`);
           return parts.join(', ');
      }
      return `${formattedBasePrice.value} × ${count} ${count === 1 ? 'person' : 'persons'}`;
    default:
      return '';
  }
});

const isFormValid = computed(() => {
  const validation = validateBookingData(props.type, localBookingData.value);
  return validation.isValid;
});

// Methods
function getCountValue() {
  switch (props.type) {
    case 'attraction':
      return localBookingData.value.guests || 1;
    case 'hotel':
      return localBookingData.value.rooms || 1;
    case 'car':
      return 1;
    case 'trip':
      if (localBookingData.value.guests && typeof localBookingData.value.guests === 'object') {
          return (localBookingData.value.guests.adults || 0) + (localBookingData.value.guests.children || 0);
      }
      return localBookingData.value.travelers || 1;
    default:
      return 1;
  }
}

function updateField(field, value) {
  localBookingData.value[field] = value;

  // Auto-calculate nights for hotels
  if (props.type === 'hotel' && (field === 'checkIn' || field === 'checkOut')) {
    const { checkIn, checkOut } = localBookingData.value;
    if (checkIn && checkOut) {
      const nights = Math.ceil(
        (new Date(checkOut) - new Date(checkIn)) / (1000 * 60 * 60 * 24)
      );
      localBookingData.value.nights = Math.max(0, nights);
    }
  }

  // Auto-calculate days for cars
  if (props.type === 'car' && (field === 'pickupDate' || field === 'returnDate')) {
    const { pickupDate, returnDate } = localBookingData.value;
    if (pickupDate && returnDate) {
      const days = Math.ceil(
        (new Date(returnDate) - new Date(pickupDate)) / (1000 * 60 * 60 * 24)
      );
      localBookingData.value.days = Math.max(0, days);
    }
  }

  // Clear validation errors when user makes changes
  validationErrors.value = [];
}

function formatPrice(price) {
  return formatPriceUtil(price, '$');
}

function handleBooking() {
  // Validate
  const validation = validateBookingData(props.type, localBookingData.value);
  
  if (!validation.isValid) {
    validationErrors.value = validation.errors;
    return;
  }

  validationErrors.value = [];

  // Emit booking data with costs
  emit('submit', {
    bookingData: { ...localBookingData.value },
    costs: costs.value
  });
}

// Watch for external price changes
watch(() => props.basePrice, () => {
  validationErrors.value = [];
});

function updateTripGuests(type, change) {
    if (!localBookingData.value.guests) {
        localBookingData.value.guests = { adults: 2, children: 0 };
    }
    
    // Initialize if missing
    if (type === 'adults' && localBookingData.value.guests.adults === undefined) localBookingData.value.guests.adults = 2;
    if (type === 'children' && localBookingData.value.guests.children === undefined) localBookingData.value.guests.children = 0;

    if (type === 'adults') {
        const newVal = localBookingData.value.guests.adults + change;
        if (newVal >= 1 && newVal <= props.maxPeople) {
            localBookingData.value.guests.adults = newVal;
        }
    } else if (type === 'children') {
         const newVal = localBookingData.value.guests.children + change;
         if (newVal >= 0) {
             localBookingData.value.guests.children = newVal;
         }
    }
}
</script>