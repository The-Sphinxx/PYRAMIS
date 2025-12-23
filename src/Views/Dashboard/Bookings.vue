<template>
  <div class="space-y-2">
    <!-- Stats Cards -->
    <StatsCard :stats="statsCardData" />

    <!-- Tabs Navigation -->
    <div class="flex items-center justify-between bg-base-100 p-4 rounded-lg shadow-sm border border-base-300 ">
      <div class="flex gap-2">
        <button
          v-for="tab in tabs"
          :key="tab.value"
          @click="activeTab = tab.value"
          class="px-6 py-2.5 font-medium transition-all duration-200 rounded-lg"
          :class="
            activeTab === tab.value
              ? 'text-primary bg-base-200 shadow-sm'
              : 'text-base-content/60 hover:text-base-content hover:bg-base-200/50'
          "
        >
          {{ tab.label }}
        </button>
      </div>

      <div class="flex items-center gap-3">
        <!-- Filter Button -->
        <button
          @click="showFilterModal = true"
          class="btn btn-outline btn-primary gap-2"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
              d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
          </svg>
          Filter
        </button>

        <!-- Add Booking Button -->

      </div>
    </div>


    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-12">
      <span class="loading loading-spinner loading-lg text-primary"></span>
    </div>

    <!-- Bookings Table -->
    <div v-else class="bg-base-100 rounded-lg shadow-sm border border-base-300 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="table w-full">
          <!-- Table Head -->
          <thead class="bg-base-100">
            <tr>
              <th class="text-base-content font-semibold">Booking ID</th>
              <th class="text-base-content font-semibold">Type</th>
              <th class="text-base-content font-semibold">Customer</th>
              <th class="text-base-content font-semibold">Date Range</th>
              <th class="text-base-content font-semibold">Status</th>
              <th class="text-base-content font-semibold">Payment</th>
              <th class="text-base-content font-semibold">Amount</th>
              <th class="text-base-content font-semibold">Actions</th>
            </tr>
          </thead>

          <!-- Table Body -->
          <tbody>
            <tr
              v-for="booking in paginatedBookings"
              :key="booking.id"
              class="hover:bg-base-200/50 transition-colors border-b border-base-300 last:border-0"
            >
              <!-- Booking ID -->
              <td class="font-mono text-sm text-base-content/80">
                {{ formatBookingId(booking.id) }}
              </td>

              <!-- Type -->
              <td class="capitalize font-medium">
                {{ booking.type || 'N/A' }}
              </td>

              <!-- Customer -->
              <td>
                <div class="font-medium text-base-content">
                  {{ getCustomerName(booking) }}
                </div>
                <div class="text-sm text-base-content/60">
                  {{ booking.guestInfo?.email || 'N/A' }}
                </div>
              </td>

              <!-- Date Range -->
              <td class="text-sm">
                {{ formatDateRange(booking) }}
              </td>

              <!-- Status -->
              <td>
                <select
                  :value="booking.status"
                  @change="handleStatusChange($event, booking)"
                  class="select select-bordered select-sm font-medium w-full max-w-xs"
                  :class="getStatusSelectClass(booking.status)"
                >
                  <option value="confirmed">Confirmed</option>
                  <option value="pending">Pending</option>
                  <option value="cancelled">Cancelled</option>
                  <option value="draft">Draft</option>
                </select>
              </td>

              <!-- Payment Status -->
              <td>
                <select
                  :value="booking.paymentStatus || 'unpaid'"
                  @change="handlePaymentStatusChange($event, booking)"
                  class="select select-bordered select-sm font-medium w-full max-w-xs"
                  :class="getPaymentStatusSelectClass(booking.paymentStatus)"
                >
                  <option value="paid">Paid</option>
                  <option value="unpaid">Unpaid</option>
                  <option value="pending">Pending</option>
                  <option value="refunded">Refunded</option>
                </select>
              </td>

              <!-- Amount -->
              <td class="font-semibold text-base-content">
                {{ formatCurrency(booking.pricing?.total || booking.price || 0) }}
              </td>

              <!-- Actions -->
              <td>
                <div class="flex items-center gap-2">
                 

                  <button
                    @click="handleEditBooking(booking)"
                    class="btn btn-ghost btn-sm text-primary hover:bg-primary/10"
                    title="Edit Booking"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                        d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                    </svg>
                  </button>

                  <button
                    @click="handleDeleteBooking(booking)"
                    class="btn btn-ghost btn-sm text-error hover:bg-error/10"
                    title="Delete Booking"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                        d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                  </button>
                   <button
                    @click="handleViewBooking(booking)"
                    class="btn btn-ghost btn-sm text-secondary hover:bg-secondary/10"
                    title="View Details"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                        d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                        d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                    </svg>
                  </button>
                </div>
              </td>
            </tr>

            <!-- Empty State -->
            <tr v-if="filteredBookings.length === 0">
              <td colspan="8" class="text-center py-12">
                <div class="flex flex-col items-center gap-3">
                  <svg class="w-16 h-16 text-base-content/20" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                      d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
                  </svg>
                  <p class="text-lg text-base-content/60">No bookings found</p>
                  <p class="text-sm text-base-content/40">Try adjusting your filters or add a new booking</p>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div v-if="filteredBookings.length > 0" class="p-4 border-t border-base-300">
        <div class="flex items-center justify-between">
          <div class="text-sm text-base-content/60">
            Showing data {{ startIndex + 1 }} to {{ endIndex }} of {{ filteredBookings.length }} entries
          </div>
          
          <div class="flex items-center gap-2">
            <button
              @click="currentPage--"
              :disabled="currentPage === 1"
              class="btn btn-sm btn-ghost"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
              </svg>
            </button>

            <button
              v-for="page in displayedPages"
              :key="page"
              @click="currentPage = page"
              class="btn btn-sm"
              :class="currentPage === page ? 'btn-primary' : 'btn-ghost'"
            >
              {{ page }}
            </button>

            <button
              v-if="totalPages > 5"
              class="btn btn-sm btn-ghost"
              disabled
            >
              ...
            </button>

            <button
              v-if="totalPages > 5 && currentPage < totalPages - 2"
              @click="currentPage = totalPages"
              class="btn btn-sm btn-ghost"
            >
              {{ totalPages }}
            </button>

            <button
              @click="currentPage++"
              :disabled="currentPage === totalPages"
              class="btn btn-sm btn-ghost"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Filter Modal -->
    <FilterModal 
      :is-open="showFilterModal"
      v-bind="filterConfig"
      @filter-change="applyFilters"
      @close="showFilterModal = false"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router'; // Added useRoute import
import { bookingsAPI } from '@/Services/dashboardApi';
import StatsCard from '@/components/Dashboard/StatsCard.vue';
import FilterModal from '@/components/Dashboard/FilterModal.vue';
import { bookingFilterConfig } from '@/Utils/dashboardFilterConfigs';

// State
const bookings = ref([]);
const loading = ref(false);
const router = useRouter();
const route = useRoute(); // Added missing route definition
const activeTab = ref('all');
const currentPage = ref(1);
const itemsPerPage = ref(8);
const showFilterModal = ref(false);
const activeFilters = ref({});

const filterConfig = bookingFilterConfig;

// Tabs configuration
const tabs = [
  { label: 'All Types', value: 'all' },
  { label: 'Hotels', value: 'hotel' },
  { label: 'Trips', value: 'trip' },
  { label: 'Car Rental', value: 'car' },
  { label: 'Attractions', value: 'attraction' }
];

// Fetch bookings data
const fetchBookings = async () => {
  loading.value = true;
  try {
    const response = await bookingsAPI.getAll();
    bookings.value = response.data || [];
  } catch (error) {
    console.error('Error fetching bookings:', error);
    bookings.value = [];
  } finally {
    loading.value = false;
  }
};

// Filtered bookings based on active tab and filters
const filteredBookings = computed(() => {
  let result = bookings.value;

  // Global Search from Sidebar
  if (route.query.q) {
    const search = route.query.q.toLowerCase();
    result = result.filter(b => 
      (b.id || '').toString().toLowerCase().includes(search) ||
      getCustomerName(b).toLowerCase().includes(search) ||
      (b.guestInfo?.email || '').toLowerCase().includes(search) ||
      (b.type || '').toLowerCase().includes(search) ||
      (b.status || '').toLowerCase().includes(search) || // Status text match
      (b.paymentStatus || '').toLowerCase().includes(search) // Payment Status text match
    );
  }

  // Apply tab filter
  if (activeTab.value !== 'all') {
    result = result.filter(booking => {
      const bookingType = (booking.type || '').toLowerCase();
      return bookingType === activeTab.value || bookingType.includes(activeTab.value);
    });
  }

  // Apply additional filters
  if (activeFilters.value.maxPrice && activeFilters.value.maxPrice < filterConfig.priceRange.max) {
    result = result.filter(b => (b.pricing?.total || b.price || 0) <= activeFilters.value.maxPrice);
  }

  if (activeFilters.value.bookingTypeSelected) {
    const searchType = activeFilters.value.bookingTypeSelected.toLowerCase();
    result = result.filter(b => (b.type || '').toLowerCase().includes(searchType));
  }

  if (activeFilters.value.statusSelected) {
    result = result.filter(b => (b.status || '').toLowerCase() === activeFilters.value.statusSelected.toLowerCase());
  }

  if (activeFilters.value.paymentStatusSelected) {
    result = result.filter(b => (b.paymentStatus || 'unpaid').toLowerCase() === activeFilters.value.paymentStatusSelected.toLowerCase());
  }

  if (activeFilters.value.dateFrom) {
    const fromDate = new Date(activeFilters.value.dateFrom);
    result = result.filter(b => new Date(b.bookingDate || b.date) >= fromDate);
  }

  if (activeFilters.value.dateTo) {
    const toDate = new Date(activeFilters.value.dateTo);
    result = result.filter(b => new Date(b.bookingDate || b.date) <= toDate);
  }

  if (activeFilters.value.bookingId) {
    const searchId = activeFilters.value.bookingId.toLowerCase();
    result = result.filter(b => {
      return (b.id || '').toString().toLowerCase().includes(searchId);
    });
  }

  return result;
});

// Computed stats for cards
const statsCardData = computed(() => {
  const now = new Date();
  const currentMonth = now.getMonth();
  const currentYear = now.getFullYear();
  
  // Get current month bookings
  const currentMonthBookings = bookings.value.filter(b => {
    const bookingDate = new Date(b.bookingDate || b.date);
    return bookingDate.getMonth() === currentMonth && bookingDate.getFullYear() === currentYear;
  });
  
  // Get previous month bookings
  const prevMonth = currentMonth === 0 ? 11 : currentMonth - 1;
  const prevYear = currentMonth === 0 ? currentYear - 1 : currentYear;
  const prevMonthBookings = bookings.value.filter(b => {
    const bookingDate = new Date(b.bookingDate || b.date);
    return bookingDate.getMonth() === prevMonth && bookingDate.getFullYear() === prevYear;
  });
  
  // Current period stats
  const total = bookings.value.length;
  const currentTotal = currentMonthBookings.length;
  const prevTotal = prevMonthBookings.length;
  
  const pendingAction = bookings.value.filter(b => {
    const status = (b.status || '').toLowerCase();
    return status === 'pending' || status === 'draft';
  }).length;
  
  const currentPending = currentMonthBookings.filter(b => {
    const status = (b.status || '').toLowerCase();
    return status === 'pending' || status === 'draft';
  }).length;
  
  const prevPending = prevMonthBookings.filter(b => {
    const status = (b.status || '').toLowerCase();
    return status === 'pending' || status === 'draft';
  }).length;
  
  const totalRevenue = bookings.value.reduce((sum, b) => {
    return sum + (b.pricing?.total || b.price || 0);
  }, 0);
  
  const currentRevenue = currentMonthBookings.reduce((sum, b) => {
    return sum + (b.pricing?.total || b.price || 0);
  }, 0);
  
  const prevRevenue = prevMonthBookings.reduce((sum, b) => {
    return sum + (b.pricing?.total || b.price || 0);
  }, 0);
  
  const resolved = bookings.value.filter(b => {
    const status = (b.status || '').toLowerCase();
    return status === 'confirmed' || status === 'completed';
  }).length;
  
  const currentResolved = currentMonthBookings.filter(b => {
    const status = (b.status || '').toLowerCase();
    return status === 'confirmed' || status === 'completed';
  }).length;
  
  const prevResolved = prevMonthBookings.filter(b => {
    const status = (b.status || '').toLowerCase();
    return status === 'confirmed' || status === 'completed';
  }).length;
  
  // Calculate trends (percentage change)
  const calculateTrend = (current, previous) => {
    if (previous === 0) return current > 0 ? 100 : 0;
    return Math.round(((current - previous) / previous) * 100);
  };
  
  return [
    {
      label: 'Total Booking',
      value: total,
      icon: 'total',
      trend: calculateTrend(currentTotal, prevTotal)
    },
    {
      label: 'Pending Action',
      value: pendingAction,
      icon: 'active',
      trend: calculateTrend(currentPending, prevPending)
    },
    {
      label: 'Revenue',
      value: formatCurrency(totalRevenue),
      icon: 'featured',
      trend: calculateTrend(currentRevenue, prevRevenue)
    },
    {
      label: 'Resolved',
      value: resolved,
      icon: 'available',
      trend: calculateTrend(currentResolved, prevResolved)
    }
  ];
});

// Pagination
const totalPages = computed(() => {
  return Math.ceil(filteredBookings.value.length / itemsPerPage.value);
});

const startIndex = computed(() => {
  return (currentPage.value - 1) * itemsPerPage.value;
});

const endIndex = computed(() => {
  const end = startIndex.value + itemsPerPage.value;
  return Math.min(end, filteredBookings.value.length);
});

const paginatedBookings = computed(() => {
  return filteredBookings.value.slice(startIndex.value, endIndex.value);
});

const displayedPages = computed(() => {
  const pages = [];
  const maxVisible = 5;
  
  if (totalPages.value <= maxVisible) {
    for (let i = 1; i <= totalPages.value; i++) {
      pages.push(i);
    }
  } else {
    if (currentPage.value <= 3) {
      for (let i = 1; i <= 4; i++) {
        pages.push(i);
      }
    } else if (currentPage.value >= totalPages.value - 2) {
      for (let i = totalPages.value - 3; i <= totalPages.value; i++) {
        pages.push(i);
      }
    } else {
      for (let i = currentPage.value - 1; i <= currentPage.value + 1; i++) {
        pages.push(i);
      }
    }
  }
  
  return pages;
});

// Helper functions
const formatBookingId = (id) => {
  if (!id) return 'N/A';
  if (id.length <= 4) return id;
  return id.substring(0, 4);
};

const getCustomerName = (booking) => {
  if (booking.guestInfo?.firstName && booking.guestInfo?.lastName) {
    return `${booking.guestInfo.firstName} ${booking.guestInfo.lastName}`;
  }
  if (booking.guestInfo?.firstName) {
    return booking.guestInfo.firstName;
  }
  if (booking.userName) {
    return booking.userName;
  }
  return 'N/A';
};

const formatDateRange = (booking) => {
  // Try different date field combinations
  let startDate, endDate;
  
  if (booking.bookingData?.checkIn && booking.bookingData?.checkOut) {
    startDate = booking.bookingData.checkIn;
    endDate = booking.bookingData.checkOut;
  } else if (booking.bookingData?.pickupDate && booking.bookingData?.returnDate) {
    startDate = booking.bookingData.pickupDate;
    endDate = booking.bookingData.returnDate;
  } else if (booking.bookingData?.startDate && booking.bookingData?.endDate) {
    startDate = booking.bookingData.startDate;
    endDate = booking.bookingData.endDate;
  } else if (booking.date) {
    return formatDate(booking.date);
  } else {
    return 'N/A';
  }
  
  return `${formatDate(startDate)} - ${formatDate(endDate)}`;
};

const formatDate = (dateString) => {
  if (!dateString) return 'N/A';
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', { day: 'numeric', month: 'short' });
};

const formatCurrency = (value) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    maximumFractionDigits: 0
  }).format(value);
};

const formatPaymentStatus = (booking) => {
  if (booking.paymentStatus) {
    return booking.paymentStatus.charAt(0).toUpperCase() + booking.paymentStatus.slice(1);
  }
  if (booking.paymentInfo?.method === 'arrival') {
    return 'Unpaid';
  }
  if (booking.paymentInfo?.method === 'card') {
    return 'Paid';
  }
  return 'Unpaid';
};

const getStatusClass = (status) => {
  const statusLower = (status || '').toLowerCase();
  
  const statusMap = {
    'confirmed': 'badge-success',
    'completed': 'badge-success',
    'pending': 'badge-warning',
    'cancelled': 'badge-error',
    'draft': 'badge-ghost',
  };
  
  return statusMap[statusLower] || 'badge-ghost';
};

const getPaymentStatusClass = (status) => {
  const statusLower = (status || '').toLowerCase();
  
  const statusMap = {
    'paid': 'badge-success',
    'refunded': 'badge-error',
    'unpaid': 'badge-ghost',
    'pending': 'badge-warning',
  };
  
  return statusMap[statusLower] || 'badge-ghost';
};

// Select class functions for dropdowns
const getStatusSelectClass = (status) => {
  const statusLower = (status || '').toLowerCase();
  
  const statusMap = {
    'confirmed': 'text-success border-success/30 focus:border-success bg-success/5',
    'completed': 'text-success border-success/30 focus:border-success bg-success/5',
    'pending': 'text-warning border-warning/30 focus:border-warning bg-warning/5',
    'cancelled': 'text-error border-error/30 focus:border-error bg-error/5',
    'draft': 'text-base-content border-base-300 focus:border-base-content bg-base-200',
  };
  
  return statusMap[statusLower] || 'text-base-content border-base-300';
};

const getPaymentStatusSelectClass = (status) => {
  const statusLower = (status || '').toLowerCase();
  
  const statusMap = {
    'paid': 'text-success border-success/30 focus:border-success bg-success/5',
    'refunded': 'text-error border-error/30 focus:border-error bg-error/5',
    'unpaid': 'text-base-content border-base-300 focus:border-base-content bg-base-200',
    'pending': 'text-warning border-warning/30 focus:border-warning bg-warning/5',
  };
  
  return statusMap[statusLower] || 'text-base-content border-base-300';
};

// Action handlers
const handleStatusChange = async (event, booking) => {
  const newStatus = event.target.value;
  const oldStatus = booking.status;
  
  if (newStatus === oldStatus) return;
  
  try {
    // Update booking status via API
    await bookingsAPI.patch(booking.id, { status: newStatus });
    
    // Refresh bookings data
    await fetchBookings();
    
    console.log(`Booking ${booking.id} status changed from ${oldStatus} to ${newStatus}`);
  } catch (error) {
    console.error('Error updating booking status:', error);
    // Revert on error
    await fetchBookings();
  }
};

const handlePaymentStatusChange = async (event, booking) => {
  const newPaymentStatus = event.target.value;
  const oldPaymentStatus = booking.paymentStatus || 'unpaid';
  
  if (newPaymentStatus === oldPaymentStatus) return;
  
  try {
    // Update payment status via API
    await bookingsAPI.patch(booking.id, { paymentStatus: newPaymentStatus });
    
    // Refresh bookings data
    await fetchBookings();
    
    console.log(`Booking ${booking.id} payment status changed from ${oldPaymentStatus} to ${newPaymentStatus}`);
  } catch (error) {
    console.error('Error updating payment status:', error);
    // Revert on error
    await fetchBookings();
  }
};

const handleAddBooking = () => {
  console.log('Add new booking');
  // TODO: Implement add booking modal
};

const handleViewBooking = (booking) => {
  router.push({ name: 'DashboardDetails', params: { type: 'bookings', id: booking.id } });
};

const handleEditBooking = (booking) => {
  console.log('Edit booking:', booking);
  // TODO: Implement edit booking modal
};

const handleDeleteBooking = async (booking) => {
  if (confirm(`Are you sure you want to delete booking ${formatBookingId(booking.id)}?`)) {
    try {
      await bookingsAPI.delete(booking.id);
      await fetchBookings();
      console.log('Booking deleted successfully');
    } catch (error) {
      console.error('Error deleting booking:', error);
    }
  }
};

// Filter handler
const applyFilters = (filters) => {
  activeFilters.value = filters;
  showFilterModal.value = false;
  currentPage.value = 1; // Reset to first page when filtering
};

// Lifecycle
onMounted(() => {
  fetchBookings();
});
</script>

<style scoped>
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

.table :where(th, td) {
  padding: 1rem 1.5rem;
  vertical-align: middle;
}
</style>