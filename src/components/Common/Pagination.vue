<template>
  <div class="flex flex-col sm:flex-row items-center justify-between gap-4 font-cairo w-full">
    <!-- Info Section -->
    <div class="text-sm text-base-content/70 order-2 sm:order-1">
      <span v-if="showInfo">
        Showing {{ startItem }} to {{ endItem }} of {{ total }} results
      </span>
    </div>

    <!-- Pagination Controls -->
    <div class="join order-1 sm:order-2">
      <!-- Previous Button -->
      <button
        @click="goToPage(currentPage - 1)"
        :disabled="currentPage === 1"
        class="join-item btn btn-sm"
        :class="buttonClass"
      >
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
        </svg>
      </button>

      <!-- Page Numbers -->
      <template v-for="page in visiblePages" :key="page">
        <button
          v-if="page !== '...'"
          @click="goToPage(page)"
          class="join-item btn btn-sm hidden sm:inline-flex"
          :class="[
            buttonClass,
            currentPage === page ? 'btn-primary' : ''
          ]"
        >
          {{ page }}
        </button>
        <button
          v-else
          disabled
          class="join-item btn btn-sm btn-disabled hidden sm:inline-flex"
        >
          ...
        </button>
      </template>

      <!-- Current page indicator for mobile -->
      <button class="join-item btn btn-sm btn-primary sm:hidden">
        {{ currentPage }}
      </button>

      <!-- Next Button -->
      <button
        @click="goToPage(currentPage + 1)"
        :disabled="currentPage === totalPages"
        class="join-item btn btn-sm"
        :class="buttonClass"
      >
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
        </svg>
      </button>
    </div>

    <!-- Per Page Selector -->
    <div v-if="showPerPageSelector" class="flex items-center gap-2 order-3">
      <label class="text-sm text-base-content/70 hidden sm:inline">Show:</label>
      <select
        v-model="internalPerPage"
        @change="handlePerPageChange"
        class="select select-bordered select-sm"
      >
        <option
          v-for="option in perPageOptions"
          :key="option"
          :value="option"
        >
          {{ option }}
        </option>
      </select>
      <span class="text-sm text-base-content/70 hidden sm:inline">per page</span>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';

const props = defineProps({
  currentPage: {
    type: Number,
    default: 1
  },
  total: {
    type: Number,
    required: true
  },
  perPage: {
    type: Number,
    default: 10
  },
  maxVisiblePages: {
    type: Number,
    default: 5
  },
  showInfo: {
    type: Boolean,
    default: true
  },
  showLabels: {
    type: Boolean,
    default: false
  },
  showFirstLast: {
    type: Boolean,
    default: true
  },
  showPerPageSelector: {
    type: Boolean,
    default: true
  },
  perPageOptions: {
    type: Array,
    default: () => [10, 25, 50, 100]
  },
  buttonClass: {
    type: String,
    default: ''
  }
});

const emit = defineEmits(['update:currentPage', 'update:perPage', 'page-change']);

const internalPerPage = ref(props.perPage);

const totalPages = computed(() => {
  return Math.ceil(props.total / props.perPage);
});

const startItem = computed(() => {
  return props.total === 0 ? 0 : (props.currentPage - 1) * props.perPage + 1;
});

const endItem = computed(() => {
  const end = props.currentPage * props.perPage;
  return end > props.total ? props.total : end;
});

const visiblePages = computed(() => {
  const pages = [];
  const total = totalPages.value;
  const current = props.currentPage;
  const max = props.maxVisiblePages;

  if (total <= max) {
    for (let i = 1; i <= total; i++) {
      pages.push(i);
    }
  } else {
    const half = Math.floor(max / 2);
    let start = current - half;
    let end = current + half;

    if (start < 1) {
      start = 1;
      end = max;
    }

    if (end > total) {
      end = total;
      start = total - max + 1;
    }

    if (start > 1) {
      pages.push(1);
      if (start > 2) {
        pages.push('...');
      }
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    if (end < total) {
      if (end < total - 1) {
        pages.push('...');
      }
      pages.push(total);
    }
  }

  return pages;
});

const goToPage = (page) => {
  if (page < 1 || page > totalPages.value || page === props.currentPage) {
    return;
  }
  
  emit('update:currentPage', page);
  emit('page-change', {
    page,
    perPage: props.perPage,
    startItem: (page - 1) * props.perPage + 1,
    endItem: Math.min(page * props.perPage, props.total)
  });
};

const handlePerPageChange = () => {
  emit('update:perPage', internalPerPage.value);
  emit('update:currentPage', 1);
  emit('page-change', {
    page: 1,
    perPage: internalPerPage.value,
    startItem: 1,
    endItem: Math.min(internalPerPage.value, props.total)
  });
};

watch(() => props.perPage, (newVal) => {
  internalPerPage.value = newVal;
});
</script>