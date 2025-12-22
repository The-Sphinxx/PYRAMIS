<template>
  <div class="min-h-screen bg-base-200 p-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
      <div>
        <h1 class="text-3xl font-bold text-base-content">Analytics Dashboard</h1>
        <p class="text-sm text-base-content/60 mt-1">
          Real-time insights and performance metrics
        </p>
      </div>
      <div class="flex gap-2 items-center">
        <button 
          @click="fetchData" 
          class="btn btn-sm btn-circle btn-ghost"
          :class="{ 'loading': loading }"
        >
          <svg v-if="!loading" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
        </button>
        <div class="text-xs text-base-content/60">
          Updated: {{ new Date().toLocaleTimeString() }}
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <StatsCard :stats="stats" />

    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-20">
      <span class="loading loading-spinner loading-lg text-primary"></span>
    </div>

    <!-- Charts Grid -->
    <div v-else class="space-y-6 mt-6">
      <!-- Row 1: Mixed Chart - Full Width -->
      <div class="card bg-base-100 shadow-xl border border-base-300">
        <div class="card-body">
          <h3 class="card-title text-lg">Monthly Bookings & Revenue Trend</h3>
          <p class="text-sm text-base-content/60">Combined view of booking volume and revenue performance</p>
          <div class="h-96 mt-4">
            <Bar :data="mixedChartData" :options="mixedChartOptions" />
          </div>
        </div>
      </div>

      <!-- Row 2: Area Chart + Radar Chart -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Area Chart - Revenue Analysis -->
        <div class="card bg-base-100 shadow-xl border border-base-300">
          <div class="card-body">
            <h3 class="card-title text-lg">Revenue vs Costs Analysis</h3>
            <p class="text-sm text-base-content/60">Breakdown of revenue, service fees, and taxes</p>
            <div class="h-80 mt-4">
              <Line :data="areaChartData" :options="areaChartOptions" />
            </div>
          </div>
        </div>

        <!-- Radar Chart - Service Performance -->
        <div class="card bg-base-100 shadow-xl border border-base-300">
          <div class="card-body">
            <h3 class="card-title text-lg">Service Performance Metrics</h3>
            <p class="text-sm text-base-content/60">Multi-dimensional comparison across services</p>
            <div class="h-80 mt-4 flex items-center justify-center">
              <Radar :data="radarChartData" :options="radarChartOptions" />
            </div>
          </div>
        </div>
      </div>

      <!-- Row 3: Bubble Chart + Polar Area Chart -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Bubble Chart - Revenue per Type -->
        <div class="card bg-base-100 shadow-xl border border-base-300">
          <div class="card-body">
            <h3 class="card-title text-lg">Revenue Distribution by Service</h3>
            <p class="text-sm text-base-content/60">Bubble size represents total revenue</p>
            <div class="h-80 mt-4">
              <Bubble :data="bubbleChartData" :options="bubbleChartOptions" />
            </div>
          </div>
        </div>

        <!-- Polar Area Chart - Booking Status -->
        <div class="card bg-base-100 shadow-xl border border-base-300">
          <div class="card-body">
            <h3 class="card-title text-lg">Booking Status Distribution</h3>
            <p class="text-sm text-base-content/60">Current status of all bookings</p>
            <div class="h-80 mt-4 flex items-center justify-center">
              <PolarArea :data="polarAreaChartData" :options="polarAreaChartOptions" />
            </div>
          </div>
        </div>
      </div>

      <!-- Row 4: Stacked Bar Chart + Doughnut Chart -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Stacked Bar Chart - Revenue Breakdown -->
        <div class="card bg-base-100 shadow-xl border border-base-300">
          <div class="card-body">
            <h3 class="card-title text-lg">Revenue Breakdown by Component</h3>
            <p class="text-sm text-base-content/60">Base price, fees, and taxes per service type</p>
            <div class="h-80 mt-4">
              <Bar :data="stackedBarChartData" :options="stackedBarChartOptions" />
            </div>
          </div>
        </div>

        <!-- Doughnut Chart - Payment Methods -->
        <div class="card bg-base-100 shadow-xl border border-base-300">
          <div class="card-body">
            <h3 class="card-title text-lg">Payment Methods Distribution</h3>
            <p class="text-sm text-base-content/60">Preferred payment options by customers</p>
            <div class="h-80 mt-4 flex items-center justify-center">
              <Doughnut :data="paymentMethodsData" :options="doughnutChartOptions" />
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Bookings Table -->
      <div class="card bg-base-100 shadow-xl border border-base-300">
        <div class="card-body">
          <div class="flex justify-between items-center mb-4">
            <div>
              <h3 class="card-title text-lg">Recent Bookings</h3>
              <p class="text-sm text-base-content/60">Latest transactions and reservations</p>
            </div>
            <router-link to="/dashboard/bookings" class="btn btn-sm btn-primary">
              View All
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

// Helper function to convert HSL to RGB
const hslToRgb = (hslString) => {
  if (!hslString) return 'rgba(0, 0, 0, 0.8)';
  
  const [h, s, l] = hslString.trim().split(' ').map(v => parseFloat(v));
  const sDecimal = s / 100;
  const lDecimal = l / 100;
  
  const c = (1 - Math.abs(2 * lDecimal - 1)) * sDecimal;
  const x = c * (1 - Math.abs((h / 60) % 2 - 1));
  const m = lDecimal - c / 2;
  
  let r = 0, g = 0, b = 0;
  if (h < 60) { r = c; g = x; b = 0; }
  else if (h < 120) { r = x; g = c; b = 0; }
  else if (h < 180) { r = 0; g = c; b = x; }
  else if (h < 240) { r = 0; g = x; b = c; }
  else if (h < 300) { r = x; g = 0; b = c; }
  else { r = c; g = 0; b = x; }
  
  r = Math.round((r + m) * 255);
  g = Math.round((g + m) * 255);
  b = Math.round((b + m) * 255);
  
  return `rgb(${r}, ${g}, ${b})`;
};

// Get theme colors
const getThemeColor = (colorName) => {
  if (typeof window === 'undefined') return 'rgba(0, 0, 0, 0.8)';
  const root = document.documentElement;
  const hsl = getComputedStyle(root).getPropertyValue(`--${colorName}`).trim();
  return hslToRgb(hsl);
};

// Color palettes
const chartColors = computed(() => ({
  primary: getThemeColor('p'),
  secondary: getThemeColor('s'),
  accent: getThemeColor('a'),
  success: getThemeColor('su'),
  warning: getThemeColor('wa'),
  error: getThemeColor('er'),
  info: getThemeColor('in'),
  neutral: getThemeColor('n'),
}));

const colorPalette = computed(() => {
  const colors = chartColors.value;
  return [
    colors.primary.replace('rgb', 'rgba').replace(')', ', 0.8)'),
    colors.warning.replace('rgb', 'rgba').replace(')', ', 0.8)'),
    colors.success.replace('rgb', 'rgba').replace(')', ', 0.8)'),
    colors.error.replace('rgb', 'rgba').replace(')', ', 0.8)'),
    colors.info.replace('rgb', 'rgba').replace(')', ', 0.8)'),
    colors.secondary.replace('rgb', 'rgba').replace(')', ', 0.8)'),
    colors.accent.replace('rgb', 'rgba').replace(')', ', 0.8)'),
  ];
});

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
        backgroundColor: colorPalette.value[0],
        borderColor: chartColors.value.primary,
        borderWidth: 2,
        yAxisID: 'y',
        order: 2
      },
      {
        type: 'line',
        label: 'Revenue ($)',
        data: Object.values(monthlyData).map(d => d.revenue),
        borderColor: chartColors.value.success,
        backgroundColor: chartColors.value.success.replace('rgb', 'rgba').replace(')', ', 0.1)'),
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
        borderColor: chartColors.value.primary,
        backgroundColor: colorPalette.value[0],
        fill: true,
        tension: 0.4
      },
      {
        label: 'Service Fees',
        data: Object.values(monthlyData).map(d => d.fees),
        borderColor: chartColors.value.warning,
        backgroundColor: colorPalette.value[1],
        fill: true,
        tension: 0.4
      },
      {
        label: 'Taxes',
        data: Object.values(monthlyData).map(d => d.taxes),
        borderColor: chartColors.value.info,
        backgroundColor: colorPalette.value[4],
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
        borderColor: chartColors.value.primary,
        backgroundColor: colorPalette.value[0],
        pointBackgroundColor: chartColors.value.primary,
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: chartColors.value.primary
      },
      {
        label: 'Revenue Performance',
        data: [
          normalize(serviceMetrics.trip.revenue, maxRevenue),
          normalize(serviceMetrics.hotel.revenue, maxRevenue),
          normalize(serviceMetrics.car.revenue, maxRevenue),
          normalize(serviceMetrics.attraction.revenue, maxRevenue)
        ],
        borderColor: chartColors.value.success,
        backgroundColor: colorPalette.value[2],
        pointBackgroundColor: chartColors.value.success,
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: chartColors.value.success
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
    color: colorPalette.value[index % colorPalette.value.length]
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
        colorPalette.value[2], // confirmed - success
        colorPalette.value[1], // pending - warning
        colorPalette.value[3], // cancelled - error
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
        backgroundColor: colorPalette.value[0],
        borderColor: chartColors.value.primary,
        borderWidth: 2
      },
      {
        label: 'Service Fees',
        data: Object.values(revenueByType).map(r => r.fees),
        backgroundColor: colorPalette.value[1],
        borderColor: chartColors.value.warning,
        borderWidth: 2
      },
      {
        label: 'Taxes',
        data: Object.values(revenueByType).map(r => r.taxes),
        backgroundColor: colorPalette.value[4],
        borderColor: chartColors.value.info,
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
      backgroundColor: colorPalette.value,
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
.card {
  transition: all 0.3s ease;
}

.card:hover {
  transform: translateY(-2px);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
}
</style>