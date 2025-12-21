<template>
  <div class="form-control w-full">
    <label v-if="label" class="label">
      <span class="label-text font-cairo flex items-center gap-2">
        <svg 
          v-if="icon === 'guests'" 
          xmlns="http://www.w3.org/2000/svg" 
          class="h-5 w-5 text-primary" 
          viewBox="0 0 20 20" 
          fill="currentColor"
        >
          <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
        </svg>
        <svg 
          v-else-if="icon === 'room'" 
          xmlns="http://www.w3.org/2000/svg" 
          class="h-5 w-5 text-primary" 
          viewBox="0 0 20 20" 
          fill="currentColor"
        >
          <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 011 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z" />
        </svg>
        <svg 
          v-else-if="icon === 'travelers'" 
          xmlns="http://www.w3.org/2000/svg" 
          class="h-5 w-5 text-primary" 
          viewBox="0 0 20 20" 
          fill="currentColor"
        >
          <path fill-rule="evenodd" d="M6.267 3.455a3.066 3.066 0 001.745-.723 3.066 3.066 0 013.976 0 3.066 3.066 0 001.745.723 3.066 3.066 0 012.812 2.812c.051.643.304 1.254.723 1.745a3.066 3.066 0 010 3.976 3.066 3.066 0 00-.723 1.745 3.066 3.066 0 01-2.812 2.812 3.066 3.066 0 00-1.745.723 3.066 3.066 0 01-3.976 0 3.066 3.066 0 00-1.745-.723 3.066 3.066 0 01-2.812-2.812 3.066 3.066 0 00-.723-1.745 3.066 3.066 0 010-3.976 3.066 3.066 0 00.723-1.745 3.066 3.066 0 012.812-2.812zm7.44 5.252a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
        </svg>
        <svg 
          v-else-if="icon === 'passengers'" 
          xmlns="http://www.w3.org/2000/svg" 
          class="h-5 w-5 text-primary" 
          viewBox="0 0 20 20" 
          fill="currentColor"
        >
          <path d="M8 16.5a1.5 1.5 0 11-3 0 1.5 1.5 0 013 0zM15 16.5a1.5 1.5 0 11-3 0 1.5 1.5 0 013 0z" />
          <path d="M3 4a1 1 0 00-1 1v10a1 1 0 001 1h1.05a2.5 2.5 0 014.9 0H10a1 1 0 001-1V5a1 1 0 00-1-1H3zM14 7a1 1 0 00-1 1v6.05A2.5 2.5 0 0115.95 16H17a1 1 0 001-1v-5a1 1 0 00-.293-.707l-2-2A1 1 0 0015 7h-1z" />
        </svg>
        {{ label }}
      </span>
    </label>
    
    <div class="flex items-center justify-between gap-3 input input-bordered bg-base-100 p-2 h-12">
      <button
        @click="decrement"
        :disabled="count <= min"
        type="button"
        class="btn btn-ghost btn-sm btn-circle flex-shrink-0"
        :class="{ 'btn-disabled opacity-40': count <= min }"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z" clip-rule="evenodd" />
        </svg>
      </button>

      <div class="text-center font-cairo flex-1 min-w-0">
        <span class="text-base font-semibold whitespace-nowrap overflow-hidden text-ellipsis">
          {{ count }} {{ countLabel }}
        </span>
      </div>

      <button
        @click="increment"
        :disabled="count >= max"
        type="button"
        class="btn btn-ghost btn-sm btn-circle flex-shrink-0"
        :class="{ 'btn-disabled opacity-40': count >= max }"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
        </svg>
      </button>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  modelValue: {
    type: Number,
    default: 1
  },
  label: {
    type: String,
    default: ''
  },
  icon: {
    type: String,
    default: 'guests',
    validator: (value) => ['guests', 'room', 'travelers', 'passengers'].includes(value)
  },
  min: {
    type: Number,
    default: 1
  },
  max: {
    type: Number,
    default: 50
  },
  singularLabel: {
    type: String,
    default: 'Guest'
  },
  pluralLabel: {
    type: String,
    default: 'Guests'
  }
});

const emit = defineEmits(['update:modelValue']);

const count = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
});

const countLabel = computed(() => {
  return count.value === 1 ? props.singularLabel : props.pluralLabel;
});

const increment = () => {
  if (count.value < props.max) {
    count.value++;
  }
};

const decrement = () => {
  if (count.value > props.min) {
    count.value--;
  }
};
</script>