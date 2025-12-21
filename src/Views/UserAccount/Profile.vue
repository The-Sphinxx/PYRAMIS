<template>
  <div class="min-h-screen bg-base-100">
    <!-- Header -->
    <ProfileHeader 
      :user="user"
      :tabs="tabs"
      :active-tab="activeTab"
      @edit-profile="handleEditProfile"
      @tab-change="handleTabChange"
    />

    <!-- Main Content -->
    <div class="page-container py-8">
      
      <!-- Overview Tab -->
      <div v-if="activeTab === 'overview'" class="space-y-12">
        <!-- Upcoming Bookings (Preview) -->
        <div>
          <div class="flex items-center gap-3 mb-6">
            <h2 class="text-2xl font-bold text-base-content">Upcoming Bookings</h2>
            <span class="badge badge-neutral badge-lg">
              {{ upcomingBookings.length }}
            </span>
          </div>

          <div v-if="upcomingBookings.length > 0" class="space-y-4">
            <BookingCard
              v-for="booking in upcomingBookings.slice(0, 2)"
              :key="booking.id"
              :booking="booking"
              @view-details="handleViewBookingDetails"
            />
          </div>
          <div v-else class="text-base-content/70 italic">No upcoming bookings.</div>
        </div>

        <!-- Recent Activity -->
        <div>
          <h2 class="text-2xl font-bold text-base-content mb-6">Recent Activity</h2>
          <div v-if="recentActivity.length > 0" class="card bg-base-100 shadow-lg overflow-hidden border border-base-300">
            <ActivityItem
              v-for="(activity, index) in recentActivity.slice(0, 5)"
              :key="activity.id"
              :activity="activity"
              :is-last="index === recentActivity.length - 1"
              @view="handleViewActivity"
            />
          </div>
           <div v-else class="text-base-content/70 italic">No recent activity.</div>

          <button 
            @click="activeTab = 'bookings'"
            class="btn btn-ghost text-primary gap-2 font-semibold mt-6"
          >
            View All Past Bookings
            <i class="fas fa-arrow-right"></i>
          </button>
        </div>
      </div>

      <!-- Bookings Tab -->
      <div v-if="activeTab === 'bookings'" class="space-y-8">
        <h2 class="text-2xl font-bold text-base-content mb-6">All Bookings</h2>
        <div v-if="bookings.length > 0" class="space-y-4">
            <BookingCard
              v-for="booking in bookings"
              :key="booking.id"
              :booking="booking"
              @view-details="handleViewBookingDetails"
            />
        </div>
        <div v-else class="text-center py-12 text-base-content/70">
          <i class="fas fa-calendar-times text-4xl mb-4"></i>
          <p>No bookings found.</p>
        </div>
      </div>

      <!-- Saved Tab -->
      <div v-if="activeTab === 'saved'" class="space-y-8">
         <h2 class="text-2xl font-bold text-base-content mb-6">Saved Items</h2>
         <div v-if="savedItems.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div v-for="item in savedItems" :key="item.id" class="card bg-base-100 shadow-xl image-full h-64">
              <figure><img :src="item.image" :alt="item.title" class="w-full h-full object-cover" /></figure>
              <div class="card-body">
                <h2 class="card-title text-white">{{ item.title }}</h2>
                <div class="flex items-center gap-1 text-white/90 text-sm">
                   <i class="fas fa-map-marker-alt"></i> {{ item.location }}
                </div>
                 <div class="flex items-center gap-1 text-yellow-400 text-sm">
                   <i class="fas fa-star"></i> {{ item.rating }}
                </div>
                <div class="card-actions justify-end mt-auto">
                  <button class="btn btn-primary btn-sm">View Details</button>
                </div>
              </div>
            </div>
         </div>
         <div v-else class="text-center py-12 text-base-content/70">
          <i class="fas fa-bookmark text-4xl mb-4"></i>
          <p>No saved items yet.</p>
        </div>
      </div>

      <!-- Wallet Tab -->
      <div v-if="activeTab === 'wallet'" class="space-y-8">
         <h2 class="text-2xl font-bold text-base-content mb-6">My Wallet</h2>
         
         <!-- Wallet Card -->
         <div v-if="wallet" class="card bg-gradient-to-r from-primary to-secondary text-primary-content shadow-xl max-w-md mx-auto mb-8">
            <div class="card-body">
              <h2 class="card-title text-white/80 text-sm uppercase tracking-wider">Total Balance</h2>
              <div class="text-4xl font-bold mb-4">{{ wallet.currency }} {{ wallet.balance }}</div>
              <div class="flex justify-between items-end">
                 <div>
                   <div class="text-white/80 text-xs">Loyalty Points</div>
                   <div class="font-bold text-xl">{{ wallet.points }} pts</div>
                 </div>
                 <div class="text-white/60 text-xs">ID: {{ wallet.id }}</div>
              </div>
            </div>
         </div>
         <div v-else class="text-center py-4">Loading Wallet...</div>

         <!-- Transactions -->
         <h3 class="text-xl font-bold text-base-content mb-4">Transaction History</h3>
         <div v-if="wallet && wallet.transactions && wallet.transactions.length > 0" class="overflow-x-auto">
            <table class="table table-zebra w-full border border-base-300 rounded-lg overflow-hidden">
              <thead class="bg-base-200">
                <tr>
                  <th>Type</th>
                  <th>Date</th>
                  <th>Description</th>
                  <th class="text-right">Amount</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="t in wallet.transactions" :key="t.id">
                  <td>
                    <span :class="t.type === 'deposit' ? 'text-success' : 'text-error'" class="font-bold uppercase text-xs">
                      {{ t.type }}
                    </span>
                  </td>
                  <td>{{ t.date }}</td>
                  <td>{{ t.description }}</td>
                  <td class="text-right font-mono font-bold">{{ t.type === 'deposit' ? '+' : '-' }}{{ t.amount }}</td>
                </tr>
              </tbody>
            </table>
         </div>
          <div v-else class="text-base-content/70 italic text-center">No transactions found.</div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axios from 'axios';
import ProfileHeader from '@/components/profile/ProfileHeader.vue';
import BookingCard from '@/components/profile/BookingCard.vue';
import ActivityItem from '@/components/profile/ActivityItem.vue';

import { useAuthStore } from '@/stores/authStore';

const authStore = useAuthStore();
const activeTab = ref('overview');

const user = ref({
  name: 'Guest User',
  email: '',
  avatar: 'https://images.unsplash.com/photo-1599566150163-29194dcaad36?w=150',
  membershipType: 'Member'
});
const bookings = ref([]);
const savedItems = ref([]);
const wallet = ref(null);
const recentActivity = ref([]);

// ... tabs ...

// ... upcomingBookings ...

// ... handlers ...

onMounted(async () => {
  // Ensure auth is initialized (try to load from local storage if fresh reload)
  if (!authStore.user) {
    authStore.initAuth();
  }

  const currentUser = authStore.user;
  const userId = currentUser ? currentUser.id : '1'; // Fallback to 1 if not logged in

  try {
    // 1. Fetch User Info (Dynamic)
    // If we have currentUser, we can use that, or fetch fresh from API
    if (currentUser) {
         user.value = {
            name: currentUser.fullName || (currentUser.firstName + ' ' + currentUser.lastName),
            email: currentUser.email,
            avatar: currentUser.profileImage || 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150',
            membershipType: currentUser.membershipType || 'Standard Member'
        };
    } else {
        // Fallback fetch for user 1
        const userRes = await axios.get(`http://localhost:3000/users/${userId}`);
        if (userRes.data) {
            const u = userRes.data;
             user.value = {
                name: (u.firstName + ' ' + u.lastName) || u.fullName,
                email: u.email,
                avatar: u.profileImage || 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150',
                membershipType: u.membershipType || 'Standard Member'
            };
        }
    }

    // 2. Fetch Bookings
    const bookingsRes = await axios.get(`http://localhost:3000/bookings?userId=${userId}`);
    bookings.value = bookingsRes.data;

    // 3. Fetch Saved Items
    const savedRes = await axios.get(`http://localhost:3000/savedItems?userId=${userId}`);
    savedItems.value = savedRes.data;

    // 4. Fetch Wallet
    const walletRes = await axios.get(`http://localhost:3000/wallet?userId=${userId}`);
     if (walletRes.data && walletRes.data.length > 0) {
        wallet.value = walletRes.data[0];
    } else if (walletRes.data.id && walletRes.data.userId) {
         // handle object return
         wallet.value = walletRes.data;
    }

     // 5. Fetch Recent Activity
    const activityRes = await axios.get(`http://localhost:3000/recentActivity?userId=${userId}`);
    recentActivity.value = activityRes.data;

  } catch (error) {
    console.error("Failed to load profile data:", error);
  }
});
</script>

<style scoped>
.page-container {
  width: 100%;
  padding-left: 120px;
  padding-right: 120px;
}

@media (max-width: 1280px) {
  .page-container {
    padding-left: 80px;
    padding-right: 80px;
  }
}

@media (max-width: 1024px) {
  .page-container {
    padding-left: 60px;
    padding-right: 60px;
  }
}

@media (max-width: 768px) {
  .page-container {
    padding-left: 24px;
    padding-right: 24px;
  }
}

@media (max-width: 640px) {
  .page-container {
    padding-left: 16px;
    padding-right: 16px;
  }
}

.card {
  border-radius: 8px;
}

.btn {
  border-radius: 6px;
}

.badge {
  border-radius: 6px;
}
</style>
