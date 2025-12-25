<template>
  <div class="flex items-center justify-center gap-4 md:gap-12 font-cairo">
    <!-- Step 1: Review -->
    <div class="flex flex-col items-center gap-2">
      <div class="flex items-center gap-2 md:gap-3">
        <div 
          class="flex items-center justify-center w-8 h-8 md:w-10 md:h-10 rounded-lg font-bold transition-all text-sm"
          :class="getStepCircleClass(1)"
        >
          {{ currentStep > 1 ? '✓' : '1' }}
        </div>
        <span 
          class="text-xs md:text-sm font-semibold whitespace-nowrap"
          :class="getLabelClass(1)"
        >
          Review
        </span>
      </div>
      <!-- Underline for Active Step -->
      <div 
        v-if="currentStep === 1"
        class="h-1 w-full bg-primary rounded-full"
      ></div>
    </div>

    <!-- Step 2: Checkout -->
    <div class="flex flex-col items-center gap-2">
      <div class="flex items-center gap-2 md:gap-3">
        <div 
          class="flex items-center justify-center w-8 h-8 md:w-10 md:h-10 rounded-lg font-bold transition-all text-sm"
          :class="getStepCircleClass(2)"
        >
          {{ currentStep > 2 ? '✓' : '2' }}
        </div>
        <span 
          class="text-xs md:text-sm font-semibold whitespace-nowrap"
          :class="getLabelClass(2)"
        >
          Checkout
        </span>
      </div>
      <!-- Underline for Active Step -->
      <div 
        v-if="currentStep === 2"
        class="h-1 w-full bg-primary rounded-full"
      ></div>
    </div>

    <!-- Step 3: Confirmation -->
    <div class="flex flex-col items-center gap-2">
      <div class="flex items-center gap-2 md:gap-3">
        <div 
          class="flex items-center justify-center w-8 h-8 md:w-10 md:h-10 rounded-lg font-bold transition-all text-sm"
          :class="getStepCircleClass(3)"
        >
          {{ currentStep > 3 ? '✓' : '3' }}
        </div>
        <span 
          class="text-xs md:text-sm font-semibold whitespace-nowrap"
          :class="getLabelClass(3)"
        >
          Confirmation
        </span>
      </div>
      <!-- Underline for Active Step -->
      <div 
        v-if="currentStep === 3"
        class="h-1 w-full bg-primary rounded-full"
      ></div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  currentStep: {
    type: Number,
    required: true,
    validator: (value) => value >= 1 && value <= 3
  },
  bookingType: {
    type: String,
    required: true,
    validator: (value) => ['attraction', 'hotel', 'car', 'trip'].includes(value)
  }
});

const getStepCircleClass = (step) => {
  if (props.currentStep > step) return 'bg-success text-white'; 
  if (props.currentStep === step) return 'bg-primary text-white'; 
  return 'bg-base-300 text-base-content/50'; 
};

const getLabelClass = (step) => {
  if (props.currentStep > step) return 'text-success'; 
  if (props.currentStep === step) return 'text-primary'; 
  return 'text-base-content/50'; 
};
</script>