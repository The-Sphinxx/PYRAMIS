<template>
  <!-- Modal Overlay & Content -->
  <Teleport to="body">
    <Transition name="modal">
      <div 
        v-if="isOpen" 
        class="modal-overlay fixed inset-0 z-50 flex items-center justify-center p-4"
        @click.self="closeModal"
      >
        <!-- Modal Container -->
        <div class="modal-container bg-base-100 rounded-lg shadow-2xl max-w-lg w-full max-h-[90vh] overflow-hidden">
          <!-- Modal Header -->
          <div class="modal-header px-6 py-5 flex items-center justify-between border-b border-base-300">
            <h2 class="text-2xl font-bold font-cairo text-base-content">
              Filters
              <span v-if="activeFiltersCount > 0" class="text-base font-normal text-base-content/60">
                ( {{ activeFiltersCount }} selected )
              </span>
            </h2>
            <button 
              @click="closeModal" 
              class="btn btn-ghost btn-sm btn-square"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <!-- Modal Body (Scrollable) -->
          <div class="modal-body overflow-y-auto px-6 py-6" style="max-height: calc(90vh - 180px);">
            <!-- Price Filter -->
            <div v-if="showPriceFilter" class="filter-section mb-8">
              <h3 class="filter-title text-base font-bold font-cairo text-base-content mb-4">
                {{ priceLabel || 'Filter By Price' }}
              </h3>
              <div class="price-slider w-full mb-4">
                <input
                  type="range"
                  :min="priceRange.min"
                  :max="priceRange.max"
                  v-model="tempFilters.maxPrice"
                  class="range range-primary w-full"
                  style="height: 6px;"
                />
              </div>
              <div class="price-inputs flex gap-3 w-full">
                <input
                  type="text"
                  :value="priceRange.min + '$'"
                  readonly
                  class="input input-bordered w-full text-center bg-base-100 font-medium text-base-content/70"
                />
                <input
                  type="text"
                  :value="tempFilters.maxPrice + '$'"
                  readonly
                  class="input input-bordered w-full text-center bg-base-100 font-medium text-base-content/70"
                />
              </div>
            </div>

            <!-- Categories Filter -->
            <div v-if="categoryOptions.length > 0" class="filter-section mb-6">
              <h3 class="filter-title text-base font-semibold font-cairo text-base-content mb-3">
                {{ categoryLabel || 'Category' }}
              </h3>
              <select 
                v-model="tempFilters.categorySelected"
                class="select select-bordered w-full font-cairo text-base-content/60"
              >
                <option value="">Select category</option>
                <option 
                  v-for="category in categoryOptions" 
                  :key="category.value"
                  :value="category.value"
                >
                  {{ category.label }}
                </option>
              </select>
            </div>

            <!-- Custom Filters (as Select Dropdowns) -->
            <div
              v-for="customFilter in customFilters"
              :key="customFilter.key"
              class="filter-section mb-6"
            >
              <h3 class="filter-title text-base font-semibold font-cairo text-base-content mb-3">
                {{ customFilter.title }}
              </h3>
              
              <!-- For single selection filters like transmission -->
              <select 
                v-if="customFilter.type === 'single'"
                v-model="tempFilters[customFilter.key + 'Selected']"
                class="select select-bordered w-full font-cairo text-base-content/60"
              >
                <option value="">Select {{ customFilter.title.toLowerCase() }}</option>
                <option 
                  v-for="option in customFilter.options" 
                  :key="option.value"
                  :value="option.value"
                >
                  {{ option.label }}
                </option>
              </select>

              <!-- For date/text inputs -->
              <input
                v-else-if="customFilter.inputType === 'date' || customFilter.inputType === 'text' || customFilter.inputType === 'number'"
                :type="customFilter.inputType"
                v-model="tempFilters[customFilter.key]"
                :placeholder="customFilter.placeholder || 'Enter ' + customFilter.title.toLowerCase()"
                class="input input-bordered w-full font-cairo text-base-content/60"
              />

              <!-- For multi-selection (checkboxes) - keep as is -->
              <div v-else class="space-y-2">
                <label
                  v-for="option in customFilter.options"
                  :key="option.value"
                  class="flex items-center gap-3 p-2 hover:bg-base-200 rounded-md cursor-pointer transition-colors"
                >
                  <input
                    type="checkbox"
                    :value="option.value"
                    v-model="tempFilters[customFilter.key]"
                    class="checkbox checkbox-primary checkbox-sm"
                  />
                  <span class="label-text font-cairo text-base-content font-medium">{{ option.label }}</span>
                </label>
              </div>
            </div>
          </div>

          <!-- Modal Footer -->
          <div class="modal-footer px-6 py-4 border-t border-base-300 flex gap-3">
            <button 
              @click="clearFilters" 
              class="btn btn-outline btn-primary flex-1 font-cairo font-bold"
            >
              Reset All
            </button>
            <button 
              @click="applyFilters" 
              class="btn btn-primary flex-1 font-cairo font-bold"
            >
              Apply Filter
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { ref, computed, watch, defineProps, defineEmits } from 'vue';

const props = defineProps({
  isOpen: {
    type: Boolean,
    required: true
  },
  showPriceFilter: {
    type: Boolean,
    default: true
  },
  priceRange: {
    type: Object,
    default: () => ({ min: 100, max: 1000 })
  },
  priceLabel: {
    type: String,
    default: 'Filter By Price'
  },
  categoryOptions: {
    type: Array,
    default: () => []
  },
  categoryLabel: {
    type: String,
    default: 'Category'
  },
  customFilters: {
    type: Array,
    default: () => []
  },
  initialFilters: {
    type: Object,
    default: () => ({})
  }
});

const emit = defineEmits(['filter-change', 'close']);

const tempFilters = ref({
  maxPrice: props.priceRange.max,
  categorySelected: '',
  ...props.initialFilters
});

// Initialize custom filter fields
props.customFilters.forEach(filter => {
  if (filter.type === 'single') {
    tempFilters.value[filter.key + 'Selected'] = '';
  } else if (filter.inputType) {
    tempFilters.value[filter.key] = '';
  } else {
    tempFilters.value[filter.key] = [];
  }
});

// Count active filters
const activeFiltersCount = computed(() => {
  let count = 0;
  
  // Check category
  if (tempFilters.value.categorySelected) {
    count++;
  }
  
  // Check custom filters
  props.customFilters.forEach(filter => {
    if (filter.type === 'single') {
      if (tempFilters.value[filter.key + 'Selected']) {
        count++;
      }
    } else if (filter.inputType) {
      if (tempFilters.value[filter.key]) {
        count++;
      }
    } else if (Array.isArray(tempFilters.value[filter.key])) {
      count += tempFilters.value[filter.key].length;
    }
  });
  
  return count;
});

const closeModal = () => {
  emit('close');
  document.body.style.overflow = '';
};

const applyFilters = () => {
  const filtersToApply = { ...tempFilters.value };
  
  // Convert single selections to arrays for consistency
  if (filtersToApply.categorySelected) {
    filtersToApply.categories = [filtersToApply.categorySelected];
  }
  
  props.customFilters.forEach(filter => {
    if (filter.type === 'single' && filtersToApply[filter.key + 'Selected']) {
      filtersToApply[filter.key] = [filtersToApply[filter.key + 'Selected']];
    }
  });
  
  emit('filter-change', filtersToApply);
  closeModal();
};

const clearFilters = () => {
  tempFilters.value = {
    maxPrice: props.priceRange.max,
    categorySelected: '',
  };
  
  props.customFilters.forEach(filter => {
    if (filter.type === 'single') {
      tempFilters.value[filter.key + 'Selected'] = '';
    } else if (filter.inputType) {
      tempFilters.value[filter.key] = '';
    } else {
      tempFilters.value[filter.key] = [];
    }
  });
  
  emit('filter-change', { ...tempFilters.value });
};

// Sync with external filter changes
watch(() => props.initialFilters, (newFilters) => {
  tempFilters.value = {
    ...tempFilters.value,
    ...newFilters
  };
}, { deep: true });

// Handle body overflow when modal opens/closes
watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    document.body.style.overflow = 'hidden';
  } else {
    document.body.style.overflow = '';
  }
});
</script>

<style scoped>
.modal-overlay {
  background-color: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  animation: fadeIn 0.2s ease-out;
}

.modal-container {
  animation: slideUp 0.3s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px) scale(0.98);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

/* Modal transitions */
.modal-enter-active,
.modal-leave-active {
  transition: all 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .modal-container,
.modal-leave-to .modal-container {
  transform: translateY(20px) scale(0.98);
}

/* Scrollbar styling */
.modal-body::-webkit-scrollbar {
  width: 6px;
}

.modal-body::-webkit-scrollbar-track {
  background: transparent;
}

.modal-body::-webkit-scrollbar-thumb {
  background: var(--bc, #2F2F2F);
  opacity: 0.2;
  border-radius: 3px;
}

.modal-body::-webkit-scrollbar-thumb:hover {
  opacity: 0.3;
}

/* Range Slider Custom Styling */
.range {
  -webkit-appearance: none;
  appearance: none;
  background: transparent;
  cursor: pointer;
}

.range::-webkit-slider-track {
  background: linear-gradient(to right, var(--p) 0%, var(--p) 50%, #E5DBC7 50%, #E5DBC7 100%);
  height: 6px;
  border-radius: 3px;
}

.range::-webkit-slider-thumb {
  -webkit-appearance: none;
  appearance: none;
  width: 22px;
  height: 22px;
  border-radius: 50%;
  background: var(--p);
  cursor: pointer;
  border: 3px solid #fff;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.15);
  transition: all 0.2s ease;
}

.range::-webkit-slider-thumb:hover {
  transform: scale(1.1);
  box-shadow: 0 3px 6px rgba(0, 0, 0, 0.2);
}

.range::-moz-range-track {
  background: linear-gradient(to right, var(--p) 0%, var(--p) 50%, #E5DBC7 50%, #E5DBC7 100%);
  height: 6px;
  border-radius: 3px;
}

.range::-moz-range-thumb {
  width: 22px;
  height: 22px;
  border-radius: 50%;
  background: var(--p);
  cursor: pointer;
  border: 3px solid #fff;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.15);
}
</style>
