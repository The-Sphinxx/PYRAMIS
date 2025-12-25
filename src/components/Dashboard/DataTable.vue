<template>
  <div class="w-full font-cairo bg-base-100 p-2 md:p-4 rounded-lg shadow-sm">
    <!-- Table Header (Title + Actions) -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-3 mb-4 md:mb-6">
      <h1 class="text-2xl md:text-3xl font-bold text-base-content">{{ title }}</h1>
      
      <div class="flex flex-col sm:flex-row items-stretch sm:items-center gap-2 sm:gap-3 w-full md:w-auto">
        <!-- Filter Button -->
        <button
          v-if="showFilter"
          @click="$emit('filter')"
          class="btn btn-outline btn-primary gap-2 w-full sm:w-auto"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
              d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
          </svg>
          Filter
        </button>

        <!-- Add New Button -->
        <button
          v-if="addButtonText"
          @click="$emit('add')"
          class="btn btn-primary gap-2 w-full sm:w-auto"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          {{ addButtonText }}
        </button>
      </div>
    </div>

    <!-- Table Container -->
    <div class="bg-base-100 rounded-lg shadow-md overflow-hidden">
      <div class="overflow-x-auto">
        <table class="table w-full ">
          <!-- Table Head -->
          <thead class="bg-base-100">
            <tr>
              <th
                v-for="(column, index) in columns"
                :key="index"
                :class="[
                  'text-base-content font-semibold text-sm',
                  column.headerClass || ''
                ]"
              >
                {{ column.label }}
              </th>
            </tr>
          </thead>

          <!-- Table Body -->
          <tbody>
            <tr
              v-for="(row, rowIndex) in paginatedData"
              :key="rowIndex"
              class="hover:bg-base-200/50 transition-colors border-b border-base-300 last:border-0"
            >
              <td
                v-for="(column, colIndex) in columns"
                :key="colIndex"
                :class="column.cellClass || ''"
              >
                <!-- Custom Slot Content -->
                <slot
                  v-if="column.slot"
                  :name="column.slot"
                  :row="row"
                  :value="getNestedValue(row, column.field)"
                />

                <!-- Image Column -->
                <div v-else-if="column.type === 'image'" class="flex items-center gap-3">
                  <div class="avatar">
                    <div class="w-12 h-12 rounded-lg">
                      <img :src="getNestedValue(row, column.field)" :alt="row.name || 'Image'" />
                    </div>
                  </div>
                  <div v-if="column.showNameWithImage">
                    <div class="font-semibold text-base-content">{{ row.name }}</div>
                    <div v-if="row.location" class="text-sm text-base-content/60 flex items-center gap-1">
                      <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                          d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                          d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                      </svg>
                      {{ row.location }}
                    </div>
                    <div v-if="row.year" class="text-xs text-base-content/50">{{ row.year }}</div>
                  </div>
                </div>

                <!-- Rating Column -->
                <div v-else-if="column.type === 'rating'" class="flex items-center gap-2">
                  <div class="flex items-center gap-1">
                    <svg class="w-5 h-5 text-warning" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                    </svg>
                    <span class="font-semibold text-warning">{{ getNestedValue(row, column.field) }}</span>
                  </div>
                  <span v-if="row.reviewsCount" class="text-sm text-base-content/60">
                    ({{ row.reviewsCount }} reviews)
                  </span>
                </div>

                <!-- Toggle Column -->
                <label v-else-if="column.type === 'toggle'" class="swap swap-rotate">
                  <input
                    type="checkbox"
                    :checked="getNestedValue(row, column.field)"
                    @change="handleToggleChange($event, row, column)"
                    class="toggle toggle-primary"
                    :disabled="column.disabled"
                  />
                </label>

                <!-- Status Badge Column -->
                <span
                  v-else-if="column.type === 'status'"
                  class="badge badge-lg font-medium transition-transform duration-200"
                  :class="[
                    getStatusClass(getNestedValue(row, column.field)),
                    column.clickable ? 'cursor-pointer hover:scale-105 active:scale-95' : ''
                  ]"
                  @click="column.clickable ? handleStatusBadgeClick(row, column) : null"
                >
                  {{ getNestedValue(row, column.field) }}
                </span>

                <!-- Status Dropdown Column -->
                <select
                  v-else-if="column.type === 'status-dropdown'"
                  :value="getNestedValue(row, column.field)"
                  @change="handleStatusChange($event, row, column)"
                  class="select select-bordered select-sm font-medium"
                  :class="getStatusSelectClass(getNestedValue(row, column.field))"
                  :disabled="column.disabled"
                >
                  <option
                    v-for="option in column.options"
                    :key="option"
                    :value="option"
                  >
                    {{ option }}
                  </option>
                </select>

                <!-- Action Buttons Column -->
                <div v-else-if="column.type === 'actions'" class="flex items-center gap-2">
                  <button
                    v-if="showActions.edit"
                    @click="$emit('edit', row)"
                    class="btn btn-ghost btn-sm text-primary hover:bg-primary/10"
                    title="Edit"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                        d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                    </svg>
                  </button>
                  
                  <button
                    v-if="showActions.delete"
                    @click="$emit('delete', row)"
                    class="btn btn-ghost btn-sm text-error hover:bg-error/10"
                    title="Delete"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                        d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                  </button>
                  
                  <button
                    v-if="showActions.view"
                    @click="$emit('view', row)"
                    class="btn btn-ghost btn-sm text-secondary hover:bg-secondary/10"
                    title="View"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                        d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                        d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                    </svg>
                  </button>
                </div>

                <!-- Default Text Column -->
                <span v-else :class="column.textClass || 'text-base-content'">
                  {{ formatValue(getNestedValue(row, column.field), column) }}
                </span>
              </td>
            </tr>

            <!-- Empty State -->
            <tr v-if="!paginatedData.length">
              <td :colspan="columns.length" class="text-center py-12">
                <div class="flex flex-col items-center gap-3">
                  <svg class="w-16 h-16 text-base-content/20" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                      d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
                  </svg>
                  <p class="text-lg text-base-content/60">{{ emptyMessage }}</p>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div v-if="data.length > 0 && showPagination" class="p-4 border-t border-base-300">
        <Pagination
          v-model:currentPage="currentPage"
          v-model:perPage="itemsPerPage"
          :total="serverSide ? totalItems : data.length"
          @page-change="handlePageChange"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import Pagination from '../Common/Pagination.vue';

const props = defineProps({
  title: {
    type: String,
    required: true
  },
  columns: {
    type: Array,
    required: true
    // Example:
    // [
    //   { label: 'Name', field: 'name', type: 'text' },
    //   { label: 'Status', field: 'status', type: 'status' },
    //   { label: 'Actions', type: 'actions' }
    // ]
  },
  data: {
    type: Array,
    default: () => []
  },
  showFilter: {
    type: Boolean,
    default: true
  },
  addButtonText: {
    type: String,
    default: ''
  },
  showActions: {
    type: Object,
    default: () => ({
      edit: true,
      delete: true,
      view: true
    })
  },
  emptyMessage: {
    type: String,
    default: 'No data available'
  },
  perPage: {
    type: Number,
    default: 8
  },
  resource: {
    type: String,
    default: '' // e.g., 'attractions', 'hotels', 'cars', etc.
  },
  loading: {
    type: Boolean,
    default: false
  },
  showPagination: {
    type: Boolean,
    default: true
  },
  serverSide: {
    type: Boolean,
    default: false
  },
  totalItems: {
    type: Number,
    default: 0
  }
});

const emit = defineEmits([
  'filter', 
  'add', 
  'edit', 
  'delete', 
  'view', 
  'toggle', 
  'status-change',
  'update:data',
  'refresh',
  'status-click',
  'page-change'
]);

const currentPage = ref(1);
const itemsPerPage = ref(props.perPage);

const paginatedData = computed(() => {
  if (props.serverSide) return props.data;
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return props.data.slice(start, end);
});

const handlePageChange = ({ page, perPage }) => {
  currentPage.value = page;
  itemsPerPage.value = perPage;
  
  if (props.serverSide) {
    emit('page-change', { page, perPage });
  }
};

// Handle status/dropdown changes
const handleStatusChange = async (event, row, column) => {
  const newValue = event.target.value;
  const oldValue = getNestedValue(row, column.field);
  
  if (newValue === oldValue) return;
  
  // Optimistic update
  const rowIndex = props.data.findIndex(item => item.id === row.id);
  if (rowIndex !== -1) {
    const updatedData = [...props.data];
    setNestedValue(updatedData[rowIndex], column.field, newValue);
    emit('update:data', updatedData);
  }
  
  // Emit event with details
  emit('status-change', {
    row,
    field: column.field,
    oldValue,
    newValue,
    success: true
  });
};

// Handle toggle changes
const handleToggleChange = async (event, row, column) => {
  const newValue = event.target.checked;
  const oldValue = getNestedValue(row, column.field);
  
  if (newValue === oldValue) return;
  
  // Optimistic update
  const rowIndex = props.data.findIndex(item => item.id === row.id);
  if (rowIndex !== -1) {
    const updatedData = [...props.data];
    setNestedValue(updatedData[rowIndex], column.field, newValue);
    emit('update:data', updatedData);
  }
  
  // Emit event with details
  emit('toggle', {
    row,
    field: column.field,
    oldValue,
    newValue,
    success: true
  });
};

// Handle status badge click
const handleStatusBadgeClick = (row, column) => {
  emit('status-click', {
    row,
    field: column.field,
    value: getNestedValue(row, column.field)
  });
};

// Get nested object value (e.g., 'user.name')
const getNestedValue = (obj, path) => {
  if (!path) return '';
  return path.split('.').reduce((acc, part) => acc?.[part], obj);
};

// Set nested object value
const setNestedValue = (obj, path, value) => {
  const keys = path.split('.');
  const lastKey = keys.pop();
  const target = keys.reduce((acc, key) => acc[key] = acc[key] || {}, obj);
  target[lastKey] = value;
};

// Format value based on column type
const formatValue = (value, column) => {
  if (value === null || value === undefined) return '-';
  
  if (column.format) {
    return column.format(value);
  }
  
  if (column.type === 'price' || column.type === 'currency') {
    return `From ${value}$`;
  }
  
  if (column.type === 'capacity') {
    return `${value.current} / ${value.total} ${column.suffix || ''}`;
  }
  
  if (column.type === 'date-range') {
    return `${value.start} - ${value.end}`;
  }
  
  return value;
};

// Get status badge class
const getStatusClass = (status) => {
  const statusMap = {
    // Success states (Green)
    'Active': 'badge-success',
    'Available': 'badge-success',
    'Confirmed': 'badge-success',
    'Paid': 'badge-success',
    'Upcoming': 'badge-success',
    'Completed': 'badge-success',
    
    // Error states (Red)
    'Cancelled': 'badge-error',
    'Suspended': 'badge-error',
    'Disabled': 'badge-error',
    'Sold Out': 'badge-error',
    'Refunded': 'badge-error',
    
    // Warning states (Yellow)
    'Pending': 'badge-warning',
    'Maintenance': 'badge-warning',
    
    // Neutral states (Gray)
    'Draft': 'badge-ghost',
    'Unpaid': 'badge-ghost',
    'Rented': 'badge-ghost',
    'Ongoing': 'badge-ghost'
  };
  
  return statusMap[status] || 'badge-ghost';
};

// Get status select class for dropdown
const getStatusSelectClass = (status) => {
  const statusMap = {
    // Success states (Green)
    'Active': 'text-success border-success/30 focus:border-success',
    'Available': 'text-success border-success/30 focus:border-success',
    'Confirmed': 'text-success border-success/30 focus:border-success',
    'Paid': 'text-success border-success/30 focus:border-success',
    'Upcoming': 'text-success border-success/30 focus:border-success',
    'Completed': 'text-success border-success/30 focus:border-success',
    
    // Error states (Red)
    'Cancelled': 'text-error border-error/30 focus:border-error',
    'Suspended': 'text-error border-error/30 focus:border-error',
    'Disabled': 'text-error border-error/30 focus:border-error',
    'Sold Out': 'text-error border-error/30 focus:border-error',
    'Refunded': 'text-error border-error/30 focus:border-error',
    
    // Warning states (Yellow)
    'Pending': 'text-warning border-warning/30 focus:border-warning',
    'Maintenance': 'text-warning border-warning/30 focus:border-warning',
    
    // Neutral states (Gray)
    'Draft': 'text-base-content border-base-300 focus:border-base-content',
    'Unpaid': 'text-base-content border-base-300 focus:border-base-content',
    'Rented': 'text-base-content border-base-300 focus:border-base-content',
    'Ongoing': 'text-base-content border-base-300 focus:border-base-content'
  };
  
  return statusMap[status] || 'text-base-content border-base-300';
};
</script>

<style scoped>
.table :where(thead, tbody) :where(tr:not(:last-child)),
.table :where(thead, tbody) :where(tr:first-child:last-child) {
  border-bottom-width: 1px;
}

.table :where(th, td) {
  padding: 1rem 1.5rem;
  vertical-align: middle;
}

.badge-success {
  @apply bg-success/20 text-success border-success/30;
}

.badge-error {
  @apply bg-error/20 text-error border-error/30;
}

.badge-warning {
  @apply bg-warning/20 text-warning border-warning/30;
}

.badge-ghost {
  @apply bg-base-300 text-base-content border-base-300;
}
</style>