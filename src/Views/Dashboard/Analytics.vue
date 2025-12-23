<template>
  <div class="min-h-screen bg-base-200 space-y-4 md:space-y-6">
    <!-- Page Header -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
      <div>
        <h1 class="text-2xl md:text-3xl font-bold text-base-content">Analytics Dashboard</h1>
        <p class="text-base-content/60 mt-1 text-sm md:text-base">Comprehensive insights and performance metrics</p>
      </div>
      <div class="flex flex-col sm:flex-row gap-2 w-full sm:w-auto">
        <select class="select select-bordered select-sm w-full sm:w-auto">
          <option>Last 12 Months</option>
          <option>Last 6 Months</option>
          <option>Last 3 Months</option>
          <option>This Month</option>
        </select>
        <button class="btn btn-sm btn-primary gap-2 w-full sm:w-auto">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"/>
          </svg>
          <span class="hidden sm:inline">Export Report</span>
          <span class="sm:hidden">Export</span>
        </button>
      </div>
    </div>
 
    <!-- Stats Cards -->
    <StatsCard :stats="stats" />

    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-20">
      <span class="loading loading-spinner loading-lg text-primary"></span>
    </div>

    <!-- Charts Grid -->
    <div v-else class="space-y-6">
      <!-- Row 1: Mixed Chart - Full Width -->
      <div class="card bg-base-100 shadow-xl border border-base-300 hover-lift">
        <div class="card-body">
          <div class="flex items-start justify-between mb-2">
            <div>
              <h3 class="card-title text-lg">Monthly Bookings & Revenue Trend</h3>
              <p class="text-sm text-base-content/60 mt-1">Combined view of booking volume and revenue performance</p>
            </div>
            <div class="badge badge-primary badge-outline">Last 12 Months</div>
          </div>
          <div class="h-64 md:h-80 lg:h-96 mt-4 w-full max-w-full overflow-hidden">
            <Bar :data="mixedChartData" :options="mixedChartOptions" />
          </div>
        </div>
      </div>

      <!-- Row 2: Area Chart + Radar Chart -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Area Chart - Revenue Analysis -->
        <div class="card bg-base-100 shadow-xl border border-base-300 hover-lift">
          <div class="card-body">
            <div class="flex items-start justify-between mb-2">
              <div>
                <h3 class="card-title text-lg">Revenue vs Costs Analysis</h3>
                <p class="text-sm text-base-content/60 mt-1">Breakdown of revenue, service fees, and taxes</p>
              </div>
              <div class="badge badge-success badge-outline">Trending Up</div>
            </div>
            <div class="h-64 md:h-80 mt-4 w-full max-w-full overflow-hidden">
              <Line :data="areaChartData" :options="areaChartOptions" />
            </div>
          </div>
        </div>

        <!-- Radar Chart - Service Performance -->
        <div class="card bg-base-100 shadow-xl border border-base-300 hover-lift">
          <div class="card-body">
            <div class="flex items-start justify-between mb-2">
              <div>
                <h3 class="card-title text-lg">Service Performance Metrics</h3>
                <p class="text-sm text-base-content/60 mt-1">Multi-dimensional comparison across services</p>
              </div>
              <div class="badge badge-info badge-outline">Normalized</div>
            </div>
            <div class="h-64 md:h-80 mt-4 flex items-center justify-center w-full max-w-full overflow-hidden">
              <Radar :data="radarChartData" :options="radarChartOptions" />
            </div>
          </div>
        </div>
      </div>

      <!-- Row 3: Bubble Chart + Polar Area Chart -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Bubble Chart - Revenue per Type -->
        <div class="card bg-base-100 shadow-xl border border-base-300 hover-lift">
          <div class="card-body">
            <div class="flex items-start justify-between mb-2">
              <div>
                <h3 class="card-title text-lg">Revenue Distribution by Service</h3>
                <p class="text-sm text-base-content/60 mt-1">Bubble size represents total revenue</p>
              </div>
              <div class="badge badge-secondary badge-outline">Interactive</div>
            </div>
            <div class="h-64 md:h-80 mt-4 w-full max-w-full overflow-hidden">
              <Bubble :data="bubbleChartData" :options="bubbleChartOptions" />
            </div>
          </div>
        </div>

        <!-- Polar Area Chart - Booking Status -->
        <div class="card bg-base-100 shadow-xl border border-base-300 hover-lift">
          <div class="card-body">
            <div class="flex items-start justify-between mb-2">
              <div>
                <h3 class="card-title text-lg">Booking Status Distribution</h3>
                <p class="text-sm text-base-content/60 mt-1">Current status of all bookings</p>
              </div>
              <div class="badge badge-accent badge-outline">Real-time</div>
            </div>
            <div class="h-64 md:h-80 mt-4 flex items-center justify-center w-full max-w-full overflow-hidden">
              <PolarArea :data="polarAreaChartData" :options="polarAreaChartOptions" />
            </div>
          </div>
        </div>
      </div>

      <!-- Row 4: Stacked Bar Chart + Doughnut Chart -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Stacked Bar Chart - Revenue Breakdown -->
        <div class="card bg-base-100 shadow-xl border border-base-300 hover-lift">
          <div class="card-body">
            <div class="flex items-start justify-between mb-2">
              <div>
                <h3 class="card-title text-lg">Revenue Breakdown by Component</h3>
                <p class="text-sm text-base-content/60 mt-1">Base price, fees, and taxes per service type</p>
              </div>
              <div class="badge badge-warning badge-outline">Stacked</div>
            </div>
            <div class="h-80 mt-4">
              <Bar :data="stackedBarChartData" :options="stackedBarChartOptions" />
            </div>
          </div>
        </div>

        <!-- Doughnut Chart - Payment Methods -->
        <div class="card bg-base-100 shadow-xl border border-base-300 hover-lift">
          <div class="card-body">
            <div class="flex items-start justify-between mb-2">
              <div>
                <h3 class="card-title text-lg">Payment Methods Distribution</h3>
                <p class="text-sm text-base-content/60 mt-1">Preferred payment options by customers</p>
              </div>
              <div class="badge badge-primary badge-outline">Popular</div>
            </div>
            <div class="h-80 mt-4 flex items-center justify-center">
              <Doughnut :data="paymentMethodsData" :options="doughnutChartOptions" />
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Bookings Table -->
      <div class="card bg-base-100 shadow-xl border border-base-300 hover-lift">
        <div class="card-body">
          <div class="flex justify-between items-center mb-4">
            <div>
              <h3 class="card-title text-lg">Recent Bookings</h3>
              <p class="text-sm text-base-content/60 mt-1">Latest transactions and reservations</p>
            </div>
            <router-link :to="{ name: 'Bookings' }" class="btn btn-sm btn-primary gap-2">
              View All
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
              </svg>
            </router-link>
          </div>
          <div class="overflow-x-auto">
            <table class="table w-full">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Item</th>
                  <th>Type</th>
                  <th>Date</th>
                  <th>Amount</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="booking in recentBookings" :key="booking.id" class="hover">
                  <td class="font-mono text-xs">{{ booking.id.substring(0, 8) }}...</td>
                  <td>
                    <div class="font-semibold">{{ booking.itemName || booking.title || 'Unknown Item' }}</div>
                  </td>
                  <td>
                    <span class="badge badge-outline capitalize">{{ booking.type }}</span>
                  </td>
                  <td class="text-sm">{{ formatDate(booking.bookingDate || booking.date) }}</td>
                  <td class="font-semibold">{{ formatCurrency(booking.pricing?.total || booking.price || 0) }}</td>
                  <td>
                    <span 
                      class="badge badge-sm"
                      :class="{
                        'badge-success': booking.status === 'confirmed',
                        'badge-warning': booking.status === 'pending',
                        'badge-error': booking.status === 'cancelled'
                      }"
                    >
                      {{ booking.status }}
                    </span>
                  </td>
                </tr>
                <tr v-if="recentBookings.length === 0">
                  <td colspan="6" class="text-center py-8 text-base-content/60">
                    No bookings data available
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import { Bar, Doughnut, Line, PolarArea, Radar, Bubble } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
  ArcElement,
  PointElement,
  LineElement,
  RadialLinearScale,
  Filler,
  BubbleController
} from 'chart.js';
import StatsCard from '@/components/Dashboard/StatsCard.vue';
import { bookingsAPI, usersAPI } from '@/Services/dashboardApi';

// Register Chart.js components
ChartJS.register(
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
  ArcElement,
  PointElement,
  LineElement,
  RadialLinearScale,
  Filler,
  BubbleController
);

// State
const bookings = ref([]);
const users = ref([]);
const loading = ref(false);

// Theme colors from tailwind.config.js
const chartColors = {
  primary: '#C86A41',
  secondary: '#2A6F7A',
  accent: '#D5B77A',
  success: '#36B37E',
  warning: '#F2A540',
  error: '#E45858',
  info: '#3ABFF8',
  neutral: '#3F3F41',
};

const colorPalette = [
  'rgba(200, 106, 65, 0.8)',   // primary
  'rgba(242, 165, 64, 0.8)',   // warning
  'rgba(54, 179, 126, 0.8)',   // success
  'rgba(228, 88, 88, 0.8)',    // error
  'rgba(59, 191, 248, 0.8)',   // info
  'rgba(42, 111, 122, 0.8)',   // secondary
  'rgba(213, 183, 122, 0.8)',  // accent
];

// Fetch Data
const fetchData = async () => {
  loading.value = true;
  try {
    const [bookingsRes, usersRes] = await Promise.all([
      bookingsAPI.getAll(),
      usersAPI.getAll()
    ]);
    bookings.value = bookingsRes.data;
    users.value = usersRes.data;
  } catch (error) {
    console.error('Error fetching analytics data:', error);
  } finally {
    loading.value = false;
  }
};

// Stats Cards
const stats = computed(() => {
  const now = new Date();
  const currentMonth = now.getMonth();
  const currentYear = now.getFullYear();
  
  const currentMonthBookings = bookings.value.filter(b => {
    const bookingDate = new Date(b.bookingDate || b.date);
    return bookingDate.getMonth() === currentMonth && bookingDate.getFullYear() === currentYear;
  });
  
  const prevMonth = currentMonth === 0 ? 11 : currentMonth - 1;
  const prevYear = currentMonth === 0 ? currentYear - 1 : currentYear;
  const prevMonthBookings = bookings.value.filter(b => {
    const bookingDate = new Date(b.bookingDate || b.date);
    return bookingDate.getMonth() === prevMonth && bookingDate.getFullYear() === prevYear;
  });
  
  const totalRevenue = bookings.value.reduce((sum, b) => sum + (b.pricing?.total || b.price || 0), 0);
  const currentRevenue = currentMonthBookings.reduce((sum, b) => sum + (b.pricing?.total || b.price || 0), 0);
  const prevRevenue = prevMonthBookings.reduce((sum, b) => sum + (b.pricing?.total || b.price || 0), 0);
  
  const calculateTrend = (current, previous) => {
    if (previous === 0) return current > 0 ? 100 : 0;
    return Math.round(((current - previous) / previous) * 100);
  };

  return [
    {
      label: 'Total Revenue',
      value: formatCurrency(totalRevenue),
      icon: 'total',
      trend: calculateTrend(currentRevenue, prevRevenue)
    },
    {
      label: 'Total Bookings',
      value: bookings.value.length,
      icon: 'available',
      trend: calculateTrend(currentMonthBookings.length, prevMonthBookings.length)
    },
    {
      label: 'Active Users',
      value: users.value.length,
      icon: 'active',
      trend: 15
    },
    {
      label: 'Avg. Transaction',
      value: formatCurrency(bookings.value.length > 0 ? totalRevenue / bookings.value.length : 0),
      icon: 'featured',
      trend: calculateTrend(
        currentMonthBookings.length > 0 ? currentRevenue / currentMonthBookings.length : 0,
        prevMonthBookings.length > 0 ? prevRevenue / prevMonthBookings.length : 0
      )
    }
  ];
});

// Chart 1: Mixed Chart - Bookings & Revenue
const mixedChartData = computed(() => {
  const monthlyData = {};
  const now = new Date();
  
  // Initialize last 12 months
  for (let i = 11; i >= 0; i--) {
    const date = new Date(now.getFullYear(), now.getMonth() - i, 1);
    const key = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
    monthlyData[key] = { revenue: 0, count: 0 };
  }

  bookings.value.forEach(b => {
    const date = new Date(b.bookingDate || b.date);
    const key = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
    if (monthlyData[key]) {
      monthlyData[key].revenue += (b.pricing?.total || b.price || 0);
      monthlyData[key].count++;
    }
  });

  const labels = Object.keys(monthlyData).map(key => {
    const [year, month] = key.split('-');
    return new Date(year, month - 1).toLocaleDateString('en-US', { month: 'short', year: 'numeric' });
  });

  return {
    labels,
    datasets: [
      {
        type: 'bar',
        label: 'Number of Bookings',
        data: Object.values(monthlyData).map(d => d.count),
        backgroundColor: colorPalette[0],
        borderColor: chartColors.primary,
        borderWidth: 2,
        yAxisID: 'y',
        order: 2
      },
      {
        type: 'line',
        label: 'Revenue ($)',
        data: Object.values(monthlyData).map(d => d.revenue),
        borderColor: chartColors.success,
        backgroundColor: 'rgba(54, 179, 126, 0.1)',
        borderWidth: 3,
        tension: 0.4,
        fill: false,
        yAxisID: 'y1',
        order: 1
      }
    ]
  };
});

const mixedChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  interaction: {
    mode: 'index',
    intersect: false,
  },
  plugins: {
    legend: {
      position: 'top',
      labels: {
        usePointStyle: true,
        padding: 15,
        font: { size: 12 }
      }
    },
    tooltip: {
      backgroundColor: 'rgba(0, 0, 0, 0.8)',
      padding: 12,
      titleFont: { size: 14 },
      bodyFont: { size: 13 },
    }
  },
  scales: {
    y: {
      type: 'linear',
      display: true,
      position: 'left',
      beginAtZero: true,
      title: {
        display: true,
        text: 'Bookings Count'
      },
      grid: {
        color: 'rgba(0, 0, 0, 0.05)'
      }
    },
    y1: {
      type: 'linear',
      display: true,
      position: 'right',
      beginAtZero: true,
      title: {
        display: true,
        text: 'Revenue ($)'
      },
      grid: {
        drawOnChartArea: false,
      },
      ticks: {
        callback: (value) => formatCurrency(value)
      }
    },
    x: {
      grid: {
        display: false
      }
    }
  }
};

// Chart 2: Area Chart - Revenue Analysis
const areaChartData = computed(() => {
  const monthlyData = {};
  const now = new Date();
  
  for (let i = 11; i >= 0; i--) {
    const date = new Date(now.getFullYear(), now.getMonth() - i, 1);
    const key = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
    monthlyData[key] = { revenue: 0, fees: 0, taxes: 0 };
  }

  bookings.value.forEach(b => {
    const date = new Date(b.bookingDate || b.date);
    const key = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
    if (monthlyData[key]) {
      monthlyData[key].revenue += (b.pricing?.subtotal || 0);
      monthlyData[key].fees += (b.pricing?.serviceFee || 0);
      monthlyData[key].taxes += (b.pricing?.taxes || 0);
    }
  });

  const labels = Object.keys(monthlyData).map(key => {
    const [year, month] = key.split('-');
    return new Date(year, month - 1).toLocaleDateString('en-US', { month: 'short' });
  });

  return {
    labels,
    datasets: [
      {
        label: 'Base Revenue',
        data: Object.values(monthlyData).map(d => d.revenue),
        borderColor: chartColors.primary,
        backgroundColor: colorPalette[0],
        fill: true,
        tension: 0.4
      },
      {
        label: 'Service Fees',
        data: Object.values(monthlyData).map(d => d.fees),
        borderColor: chartColors.warning,
        backgroundColor: colorPalette[1],
        fill: true,
        tension: 0.4
      },
      {
        label: 'Taxes',
        data: Object.values(monthlyData).map(d => d.taxes),
        borderColor: chartColors.info,
        backgroundColor: colorPalette[4],
        fill: true,
        tension: 0.4
      }
    ]
  };
});

const areaChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top',
      labels: {
        usePointStyle: true,
        padding: 15,
        font: { size: 12 }
      }
    },
    tooltip: {
      mode: 'index',
      intersect: false,
      backgroundColor: 'rgba(0, 0, 0, 0.8)',
      padding: 12,
      callbacks: {
        label: (context) => `${context.dataset.label}: ${formatCurrency(context.parsed.y)}`
      }
    }
  },
  scales: {
    y: {
      stacked: true,
      beginAtZero: true,
      ticks: {
        callback: (value) => formatCurrency(value)
      },
      grid: {
        color: 'rgba(0, 0, 0, 0.05)'
      }
    },
    x: {
      stacked: true,
      grid: {
        display: false
      }
    }
  }
};

// Chart 3: Radar Chart - Service Performance
const radarChartData = computed(() => {
  const serviceMetrics = {
    trip: { count: 0, revenue: 0, avgRating: 0 },
    hotel: { count: 0, revenue: 0, avgRating: 0 },
    car: { count: 0, revenue: 0, avgRating: 0 },
    attraction: { count: 0, revenue: 0, avgRating: 0 }
  };

  bookings.value.forEach(b => {
    const type = (b.type || 'other').toLowerCase();
    if (serviceMetrics[type]) {
      serviceMetrics[type].count++;
      serviceMetrics[type].revenue += (b.pricing?.total || b.price || 0);
    }
  });

  // Normalize metrics to 0-100 scale
  const maxCount = Math.max(...Object.values(serviceMetrics).map(m => m.count), 1);
  const maxRevenue = Math.max(...Object.values(serviceMetrics).map(m => m.revenue), 1);

  const normalize = (value, max) => (value / max) * 100;

  return {
    labels: ['Trip', 'Hotel', 'Car', 'Attraction'],
    datasets: [
      {
        label: 'Booking Volume',
        data: [
          normalize(serviceMetrics.trip.count, maxCount),
          normalize(serviceMetrics.hotel.count, maxCount),
          normalize(serviceMetrics.car.count, maxCount),
          normalize(serviceMetrics.attraction.count, maxCount)
        ],
        borderColor: chartColors.primary,
        backgroundColor: colorPalette[0],
        pointBackgroundColor: chartColors.primary,
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: chartColors.primary
      },
      {
        label: 'Revenue Performance',
        data: [
          normalize(serviceMetrics.trip.revenue, maxRevenue),
          normalize(serviceMetrics.hotel.revenue, maxRevenue),
          normalize(serviceMetrics.car.revenue, maxRevenue),
          normalize(serviceMetrics.attraction.revenue, maxRevenue)
        ],
        borderColor: chartColors.success,
        backgroundColor: colorPalette[2],
        pointBackgroundColor: chartColors.success,
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: chartColors.success
      }
    ]
  };
});

const radarChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top',
      labels: {
        usePointStyle: true,
        padding: 15,
        font: { size: 12 }
      }
    },
    tooltip: {
      backgroundColor: 'rgba(0, 0, 0, 0.8)',
      padding: 12,
      callbacks: {
        label: (context) => `${context.dataset.label}: ${Math.round(context.parsed.r)}%`
      }
    }
  },
  scales: {
    r: {
      beginAtZero: true,
      max: 100,
      ticks: {
        stepSize: 20,
        callback: (value) => value + '%'
      },
      grid: {
        color: 'rgba(0, 0, 0, 0.1)'
      }
    }
  }
};

// Chart 4: Bubble Chart - Revenue per Type
const bubbleChartData = computed(() => {
  const serviceData = {};

  bookings.value.forEach(b => {
    const type = (b.type || 'other').toLowerCase();
    if (!serviceData[type]) {
      serviceData[type] = { count: 0, totalRevenue: 0 };
    }
    serviceData[type].count++;
    serviceData[type].totalRevenue += (b.pricing?.total || b.price || 0);
  });

  const bubbles = Object.entries(serviceData).map(([type, data], index) => ({
    x: data.count,
    y: data.totalRevenue / data.count, // Average revenue
    r: Math.sqrt(data.totalRevenue) / 20, // Bubble size based on total revenue
    label: type.charAt(0).toUpperCase() + type.slice(1),
    color: colorPalette[index % colorPalette.length]
  }));

  return {
    datasets: bubbles.map(bubble => ({
      label: bubble.label,
      data: [{ x: bubble.x, y: bubble.y, r: bubble.r }],
      backgroundColor: bubble.color,
      borderColor: bubble.color.replace('0.8', '1'),
      borderWidth: 2
    }))
  };
});

const bubbleChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top',
      labels: {
        usePointStyle: true,
        padding: 15,
        font: { size: 12 }
      }
    },
    tooltip: {
      backgroundColor: 'rgba(0, 0, 0, 0.8)',
      padding: 12,
      callbacks: {
        label: (context) => {
          return [
            `${context.dataset.label}`,
            `Bookings: ${context.parsed.x}`,
            `Avg Revenue: ${formatCurrency(context.parsed.y)}`,
            `Total: ${formatCurrency(Math.pow(context.raw.r * 20, 2))}`
          ];
        }
      }
    }
  },
  scales: {
    x: {
      beginAtZero: true,
      title: {
        display: true,
        text: 'Number of Bookings'
      },
      grid: {
        color: 'rgba(0, 0, 0, 0.05)'
      }
    },
    y: {
      beginAtZero: true,
      title: {
        display: true,
        text: 'Average Revenue ($)'
      },
      ticks: {
        callback: (value) => formatCurrency(value)
      },
      grid: {
        color: 'rgba(0, 0, 0, 0.05)'
      }
    }
  }
};

// Chart 5: Polar Area - Booking Status
const polarAreaChartData = computed(() => {
  const statusCount = bookings.value.reduce((acc, b) => {
    const status = (b.status || 'unknown').toLowerCase();
    acc[status] = (acc[status] || 0) + 1;
    return acc;
  }, {});

  const labels = Object.keys(statusCount).map(s => s.charAt(0).toUpperCase() + s.slice(1));
  const data = Object.values(statusCount);

  return {
    labels,
    datasets: [{
      label: 'Bookings',
      data,
      backgroundColor: [
        colorPalette[2], // confirmed - success
        colorPalette[1], // pending - warning
        colorPalette[3], // cancelled - error
        'rgba(156, 163, 175, 0.8)' // unknown - gray
      ],
      borderColor: '#fff',
      borderWidth: 3
    }]
  };
});

const polarAreaChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'bottom',
      labels: {
        usePointStyle: true,
        padding: 15,
        font: { size: 12 }
      }
    },
    tooltip: {
      backgroundColor: 'rgba(0, 0, 0, 0.8)',
      padding: 12
    }
  },
  scales: {
    r: {
      beginAtZero: true,
      grid: {
        color: 'rgba(0, 0, 0, 0.1)'
      }
    }
  }
};

// Chart 6: Stacked Bar - Revenue Breakdown
const stackedBarChartData = computed(() => {
  const revenueByType = {};

  bookings.value.forEach(b => {
    const type = (b.type || 'other').toLowerCase();
    if (!revenueByType[type]) {
      revenueByType[type] = { base: 0, fees: 0, taxes: 0 };
    }
    revenueByType[type].base += (b.pricing?.subtotal || 0);
    revenueByType[type].fees += (b.pricing?.serviceFee || 0);
    revenueByType[type].taxes += (b.pricing?.taxes || 0);
  });

  const labels = Object.keys(revenueByType).map(type => 
    type.charAt(0).toUpperCase() + type.slice(1)
  );

  return {
    labels,
    datasets: [
      {
        label: 'Base Price',
        data: Object.values(revenueByType).map(r => r.base),
        backgroundColor: colorPalette[0],
        borderColor: chartColors.primary,
        borderWidth: 2
      },
      {
        label: 'Service Fees',
        data: Object.values(revenueByType).map(r => r.fees),
        backgroundColor: colorPalette[1],
        borderColor: chartColors.warning,
        borderWidth: 2
      },
      {
        label: 'Taxes',
        data: Object.values(revenueByType).map(r => r.taxes),
        backgroundColor: colorPalette[4],
        borderColor: chartColors.info,
        borderWidth: 2
      }
    ]
  };
});

const stackedBarChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top',
      labels: {
        usePointStyle: true,
        padding: 15,
        font: { size: 12 }
      }
    },
    tooltip: {
      mode: 'index',
      backgroundColor: 'rgba(0, 0, 0, 0.8)',
      padding: 12,
      callbacks: {
        label: (context) => `${context.dataset.label}: ${formatCurrency(context.parsed.y)}`
      }
    }
  },
  scales: {
    x: {
      stacked: true,
      grid: {
        display: false
      }
    },
    y: {
      stacked: true,
      beginAtZero: true,
      ticks: {
        callback: (value) => formatCurrency(value)
      },
      grid: {
        color: 'rgba(0, 0, 0, 0.05)'
      }
    }
  }
};

// Chart 7: Doughnut - Payment Methods
const paymentMethodsData = computed(() => {
  const methods = bookings.value.reduce((acc, b) => {
    const method = b.paymentInfo?.method || 'unknown';
    acc[method] = (acc[method] || 0) + 1;
    return acc;
  }, {});

  const labels = Object.keys(methods).map(m => 
    m === 'arrival' ? 'Pay on Arrival' : 
    m === 'card' ? 'Credit Card' :
    m.charAt(0).toUpperCase() + m.slice(1)
  );

  return {
    labels,
    datasets: [{
      label: 'Bookings',
      data: Object.values(methods),
      backgroundColor: colorPalette,
      borderColor: '#fff',
      borderWidth: 3
    }]
  };
});

const doughnutChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'bottom',
      labels: {
        usePointStyle: true,
        padding: 15,
        font: { size: 12 }
      }
    },
    tooltip: {
      backgroundColor: 'rgba(0, 0, 0, 0.8)',
      padding: 12
    }
  }
};

// Recent Bookings
const recentBookings = computed(() => {
  return [...bookings.value]
    .sort((a, b) => {
      const dateA = new Date(a.bookingDate || a.date);
      const dateB = new Date(b.bookingDate || b.date);
      return dateB - dateA;
    })
    .slice(0, 5);
});

// Helper Functions
const formatCurrency = (value) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    maximumFractionDigits: 0
  }).format(value);
};

const formatDate = (dateString) => {
  if (!dateString) return 'N/A';
  return new Date(dateString).toLocaleDateString();
};

// Lifecycle
onMounted(() => {
  fetchData();
});
</script>

<style scoped>
.hover-lift {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.hover-lift:hover {
  transform: translateY(-4px);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.15), 0 10px 10px -5px rgba(0, 0, 0, 0.08);
}

.card-body {
  position: relative;
}

.badge {
  font-weight: 600;
  font-size: 0.75rem;
}
</style>