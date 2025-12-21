<template>
  <div class="space-y-6">
    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="card bg-base-100 shadow-sm border border-base-300 rounded-lg">
        <div class="card-body p-5">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-base-content/60">Total Bookings</p>
              <h3 class="text-2xl font-bold text-base-content mt-1">1,234</h3>
              <p class="text-xs text-success mt-1">+12.5% from last month</p>
            </div>
            <div class="w-12 h-12 rounded-lg bg-primary/10 flex items-center justify-center">
              <svg class="w-6 h-6 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
              </svg>
            </div>
          </div>
        </div>
      </div>

      <div class="card bg-base-100 shadow-sm border border-base-300 rounded-lg">
        <div class="card-body p-5">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-base-content/60">Revenue</p>
              <h3 class="text-2xl font-bold text-base-content mt-1">$45,678</h3>
              <p class="text-xs text-success mt-1">+8.2% from last month</p>
            </div>
            <div class="w-12 h-12 rounded-lg bg-success/10 flex items-center justify-center">
              <svg class="w-6 h-6 text-success" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
          </div>
        </div>
      </div>

      <div class="card bg-base-100 shadow-sm border border-base-300 rounded-lg">
        <div class="card-body p-5">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-base-content/60">Active Trips</p>
              <h3 class="text-2xl font-bold text-base-content mt-1">89</h3>
              <p class="text-xs text-warning mt-1">15 pending</p>
            </div>
            <div class="w-12 h-12 rounded-lg bg-warning/10 flex items-center justify-center">
              <svg class="w-6 h-6 text-warning" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7" />
              </svg>
            </div>
          </div>
        </div>
      </div>

      <div class="card bg-base-100 shadow-sm border border-base-300 rounded-lg">
        <div class="card-body p-5">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-base-content/60">Customer Satisfaction</p>
              <h3 class="text-2xl font-bold text-base-content mt-1">4.8/5</h3>
              <p class="text-xs text-success mt-1">+0.3 from last month</p>
            </div>
            <div class="w-12 h-12 rounded-lg bg-accent/10 flex items-center justify-center">
              <svg class="w-6 h-6 text-accent" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z" />
              </svg>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Bookings -->
    <div class="card bg-base-100 shadow-sm border border-base-300 rounded-lg">
      <div class="card-body p-4 md:p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-base md:text-lg font-semibold">Recent Bookings</h3>
          <button class="btn btn-sm btn-ghost rounded-md">View All</button>
        </div>
        
        <div class="overflow-x-auto">
          <table class="table w-full table-sm md:table-md">
            <thead>
              <tr>
                <th>Customer</th>
                <th>Destination</th>
                <th class="hidden md:table-cell">Date</th>
                <th>Status</th>
                <th class="hidden lg:table-cell">Amount</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="booking in recentBookings" :key="booking.id">
                <td>
                  <div class="flex items-center gap-3">
                    <div class="avatar">
                      <div class="w-8 h-8 md:w-10 md:h-10 rounded-full">
                        <img :src="booking.avatar" :alt="booking.customer" />
                      </div>
                    </div>
                    <span class="font-medium text-sm md:text-base">{{ booking.customer }}</span>
                  </div>
                </td>
                <td class="text-sm md:text-base">{{ booking.destination }}</td>
                <td class="hidden md:table-cell text-sm">{{ booking.date }}</td>
                <td>
                  <span class="badge badge-sm rounded-md" :class="{
                    'badge-success': booking.status === 'Confirmed',
                    'badge-warning': booking.status === 'Pending',
                    'badge-info': booking.status === 'In Progress'
                  }">
                    {{ booking.status }}
                  </span>
                </td>
                <td class="hidden lg:table-cell font-semibold">${{ booking.amount }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const recentBookings = ref([
  {
    id: 1,
    customer: 'Ahmed Hassan',
    avatar: 'https://i.pravatar.cc/150?img=1',
    destination: 'Pyramids of Giza',
    date: 'Dec 25, 2025',
    status: 'Confirmed',
    amount: 299
  },
  {
    id: 2,
    customer: 'Sarah Mohamed',
    avatar: 'https://i.pravatar.cc/150?img=5',
    destination: 'Luxor Temple',
    date: 'Dec 28, 2025',
    status: 'Pending',
    amount: 450
  },
  {
    id: 3,
    customer: 'Omar Ali',
    avatar: 'https://i.pravatar.cc/150?img=8',
    destination: 'Red Sea Diving',
    date: 'Jan 5, 2026',
    status: 'In Progress',
    amount: 650
  },
  {
    id: 4,
    customer: 'Layla Ibrahim',
    avatar: 'https://i.pravatar.cc/150?img=9',
    destination: 'Alexandria',
    date: 'Jan 10, 2026',
    status: 'Confirmed',
    amount: 380
  },
  {
    id: 5,
    customer: 'Youssef Kamal',
    avatar: 'https://i.pravatar.cc/150?img=12',
    destination: 'Aswan',
    date: 'Jan 15, 2026',
    status: 'Pending',
    amount: 520
  }
]);
</script>
