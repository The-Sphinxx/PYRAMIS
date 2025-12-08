<template>
  <!-- Filter Button -->
  <button 
    @click="openModal" 
    class="btn btn-primary gap-2 font-cairo font-bold"
  >
    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
      <path fill-rule="evenodd" d="M3 3a1 1 0 011-1h12a1 1 0 011 1v3a1 1 0 01-.293.707L12 11.414V15a1 1 0 01-.293.707l-2 2A1 1 0 018 17v-5.586L3.293 6.707A1 1 0 013 6V3z" clip-rule="evenodd" />
    </svg>
    Filters
    <span v-if="activeFiltersCount > 0" class="badge badge-secondary">
      {{ activeFiltersCount }}
    </span>
  </button>

  <!-- Modal Overlay & Content -->
  <Teleport to="body">
    <Transition name="modal">
      <div 
        v-if="isOpen" 
        class="modal-overlay fixed inset-0 z-50 flex items-center justify-center p-4"
        @click.self="closeModal"
      >
        <!-- Modal Container -->
        <div class="modal-container bg-base-100 rounded-2xl shadow-2xl max-w-lg w-full max-h-[90vh] overflow-hidden border-2 border-base-300">
          <!-- Modal Header -->
          <div class="modal-header bg-base-200 px-6 py-4 border-b-2 border-base-300 flex items-center justify-between">
            <h2 class="text-2xl font-bold font-cairo text-primary">
              Filters
            </h2>
            <button 
              @click="closeModal" 
              class="btn btn-sm btn-ghost btn-circle"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <!-- Modal Body (Scrollable) -->
          <div class="modal-body overflow-y-auto p-6" style="max-height: calc(90vh - 180px);">
            <!-- Price Filter -->
            <div v-if="showPriceFilter" class="filter-section">
              <h3 class="filter-title text-lg font-bold font-cairo text-primary mb-4">
                Filter By Price
              </h3>
              <div class="price-slider w-full">
                <input
                  type="range"
                  :min="priceRange.min"
                  :max="priceRange.max"
                  v-model="tempFilters.maxPrice"
                  class="range range-primary w-full mb-4"
                />
                <div class="price-inputs flex gap-3 w-full">
                  <input
                    type="number"
                    :value="priceRange.min"
                    readonly
                    class="input input-bordered input-sm w-full text-center bg-base-200 font-bold"
                  />
                  <input
                    type="number"
                    v-model="tempFilters.maxPrice"
                    :max="priceRange.max"
                    class="input input-bordered input-sm w-full text-center font-bold"
                  />
                </div>
              </div>
            </div>

            <!-- Categories Filter -->
            <div v-if="categoryOptions.length > 0" class="filter-section">
              <h3 class="filter-title text-lg font-bold font-cairo text-primary mb-4">
                Categories
              </h3>
              <div class="checkbox-group flex flex-col gap-3">
                <label
                  v-for="category in categoryOptions"
                  :key="category.value"
                  class="checkbox-label cursor-pointer flex items-center gap-3 hover:bg-base-200 p-2 rounded-lg transition-all"
                >
                  <input
                    type="checkbox"
                    :value="category.value"
                    v-model="tempFilters.categories"
                    class="checkbox checkbox-primary checkbox-sm"
                  />
                  <span class="label-text font-cairo text-base-content font-medium">{{ category.label }}</span>
                </label>
              </div>
            </div>

            <!-- Custom Filters -->
            <div
              v-for="customFilter in customFilters"
              :key="customFilter.key"
              class="filter-section"
            >
              <h3 class="filter-title text-lg font-bold font-cairo text-primary mb-4">
                {{ customFilter.title }}
              </h3>
              <div class="checkbox-group flex flex-col gap-3">
                <label
                  v-for="option in customFilter.options"
                  :key="option.value"
                  class="checkbox-label cursor-pointer flex items-center gap-3 hover:bg-base-200 p-2 rounded-lg transition-all"
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
          <div class="modal-footer bg-base-200 px-6 py-4 border-t-2 border-base-300 flex gap-3">
            <button 
              @click="clearFilters" 
              class="btn btn-outline btn-primary flex-1 font-cairo font-bold"
            >
              Clear Filters
            </button>
            <button 
              @click="applyFilters" 
              class="btn btn-primary flex-1 font-cairo font-bold"
            >
              Apply Filters
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
  showPriceFilter: {
    type: Boolean,
    default: true
  },
  priceRange: {
    type: Object,
    default: () => ({ min: 200, max: 1000 })
  },
  categoryOptions: {
    type: Array,
    default: () => []
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

const emit = defineEmits(['filter-change']);

const isOpen = ref(false);
const tempFilters = ref({
  maxPrice: props.priceRange.max,
  categories: [],
  ...props.initialFilters
});

// Initialize custom filter arrays
props.customFilters.forEach(filter => {
  if (!tempFilters.value[filter.key]) {
    tempFilters.value[filter.key] = [];
  }
});

// Count active filters
const activeFiltersCount = computed(() => {
  let count = 0;
  
  // Check categories
  if (tempFilters.value.categories?.length > 0) {
    count += tempFilters.value.categories.length;
  }
  
  // Check custom filters
  props.customFilters.forEach(filter => {
    if (tempFilters.value[filter.key]?.length > 0) {
      count += tempFilters.value[filter.key].length;
    }
  });
  
  return count;
});

const openModal = () => {
  isOpen.value = true;
  document.body.style.overflow = 'hidden';
};

const closeModal = () => {
  isOpen.value = false;
  document.body.style.overflow = '';
};

const applyFilters = () => {
  emit('filter-change', { ...tempFilters.value });
  closeModal();
};

const clearFilters = () => {
  tempFilters.value = {
    maxPrice: props.priceRange.max,
    categories: [],
  };
  
  props.customFilters.forEach(filter => {
    tempFilters.value[filter.key] = [];
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
</script>

<style scoped>
.modal-overlay {
  background-color: rgba(0, 0, 0, 0.6);
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
    transform: translateY(20px) scale(0.95);
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
  transform: translateY(20px) scale(0.95);
}

/* Filter sections */
.filter-section {
  margin-bottom: 2rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid rgba(200, 106, 65, 0.2);
}

.filter-section:last-child {
  border-bottom: none;
  margin-bottom: 0;
  padding-bottom: 0;
}

/* Scrollbar styling */
.modal-body::-webkit-scrollbar {
  width: 8px;
}

.modal-body::-webkit-scrollbar-track {
  background: var(--base-200);
  border-radius: 4px;
}

.modal-body::-webkit-scrollbar-thumb {
  background: var(--primary);
  border-radius: 4px;
}

.modal-body::-webkit-scrollbar-thumb:hover {
  background: var(--primary-focus);
}
</style>