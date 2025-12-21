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
         
         <!-- Loading State -->
         <div v-if="walletStore.loading" class="flex justify-center p-8">
             <span class="loading loading-spinner loading-lg text-primary"></span>
         </div>

         <!-- Empty State: Create Wallet -->
         <div v-else-if="!walletStore.wallet" class="text-center py-12 border-2 border-dashed border-base-300 rounded-xl">
             <i class="fas fa-wallet text-6xl text-base-content/20 mb-4"></i>
             <p class="text-lg font-bold mb-2">No Wallet Active</p>
             <p class="text-base-content/60 mb-6">Activate your wallet to earn loyalty points and manage payments.</p>
             <button @click="handleCreateWallet" class="btn btn-primary">
                 Activate Wallet
             </button>
         </div>

         <!-- Active Wallet -->
         <div v-else>
             <!-- Wallet Card -->
             <div class="card bg-gradient-to-r from-primary to-secondary text-primary-content shadow-xl max-w-md mx-auto mb-8 relative group">
                <div class="card-body">
                  <div class="absolute top-4 right-4">
                     <button @click="handleAddFunds" class="btn btn-sm btn-circle btn-ghost bg-white/20 hover:bg-white/40 text-white" title="Add Funds (Test)">
                        <i class="fas fa-plus"></i>
                     </button>
                  </div>
                  <h2 class="card-title text-white/80 text-sm uppercase tracking-wider">Total Balance</h2>
                  <div class="text-4xl font-bold mb-4">{{ walletStore.wallet.currency }} {{ walletStore.wallet.balance }}</div>
                  <div class="flex justify-between items-end">
                     <div>
                       <div class="text-white/80 text-xs">Loyalty Points</div>
                       <div class="font-bold text-xl">{{ walletStore.wallet.points }} pts</div>
                     </div>
                     <div class="text-white/60 text-xs text-right">
                         <div>ID: {{ walletStore.wallet.id }}</div>
                         <div class="text-[10px] opacity-70 mt-1">Synced to user: {{ walletStore.wallet.userId }}</div>
                     </div>
                  </div>
                </div>
             </div>

             <!-- Transactions -->
             <h3 class="text-xl font-bold text-base-content mb-4">Transaction History</h3>
             <div v-if="walletStore.wallet.transactions && walletStore.wallet.transactions.length > 0" class="overflow-x-auto">
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
                    <tr v-for="t in walletStore.wallet.transactions" :key="t.id">
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
              <div v-else class="text-base-content/70 italic text-center py-4">No transactions found.</div>
         </div>
      </div>

    </div>

    <!-- Edit Profile Modal -->
    <EditProfileModal
      :is-open="isEditModalOpen"
      :user="authStore.user"
      @close="isEditModalOpen = false"
      @save="handleSaveProfile"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axios from 'axios';
import ProfileHeader from '@/components/profile/ProfileHeader.vue';
import BookingCard from '@/components/profile/BookingCard.vue';
import ActivityItem from '@/components/profile/ActivityItem.vue';
import EditProfileModal from '@/components/profile/EditProfileModal.vue';

import { useAuthStore } from '@/stores/authStore';
import { useWalletStore } from '@/stores/walletStore';

const authStore = useAuthStore();
const walletStore = useWalletStore();
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
const isEditModalOpen = ref(false);

const tabs = [
  { id: 'overview', name: 'Overview', icon: 'fas fa-th-large' },
  { id: 'bookings', name: 'Bookings', icon: 'fas fa-calendar-alt' },
  { id: 'saved', name: 'Saved', icon: 'fas fa-bookmark' },
  { id: 'wallet', name: 'Wallet', icon: 'fas fa-wallet' }
];

const upcomingBookings = computed(() => {
  // Filter bookings for future dates
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return bookings.value.filter(b => {
      const bookingDate = new Date(b.date || b.startDate); // Adjust based on your booking validation
      return bookingDate >= today;
  });
});

// Handlers
// Handlers
const handleEditProfile = () => {
  isEditModalOpen.value = true;
};

const handleSaveProfile = async (updatedData) => {
  const result = await authStore.updateProfile(updatedData);
  if (result.success) {
    // Update local user ref to reflect changes immediately
    user.value = {
      ...user.value,
      name: updatedData.fullName,
      email: authStore.user.email, // Email usually doesn't change here
      avatar: updatedData.profileImage,
      firstName: updatedData.firstName,
      lastName: updatedData.lastName,
      phone: updatedData.phone,
      nationality: updatedData.nationality,
      dateOfBirth: updatedData.dateOfBirth,
      gender: updatedData.gender
    };
    isEditModalOpen.value = false;
  } else {
    alert('Failed to update profile: ' + result.error);
    isEditModalOpen.value = false; // Or keep open to retry
  }
};

const handleCreateWallet = async () => {
    if (!authStore.user) return;
    await walletStore.createWallet(authStore.user.id);
};

const handleAddFunds = async () => {
    await walletStore.addFunds(1000, 'Top Up (Demo)');
};

const handleTabChange = (tabId) => {
  activeTab.value = tabId;
};

const handleViewBookingDetails = (id) => {
  console.log('View booking details', id);
  // Router navigation
};

const handleViewActivity = (id) => {
  console.log('View activity', id);
};

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

    // 2. Fetch Bookings (Strictly for this user)
    // Only fetch if we have a valid userId
         // 2. Fetch Bookings (Strictly for this user)
    // Only fetch if we have a valid userId
    if (userId && userId !== '1') {
         // Query API
         const bookingsRes = await axios.get(`http://localhost:3000/bookings?userId=${userId}`);
         // DOUBLE CHECK: Client-side filter to guarantee isolation
         bookings.value = bookingsRes.data.filter(b => b.userId == userId);

          // 3. Fetch Saved Items
         const savedRes = await axios.get(`http://localhost:3000/savedItems?userId=${userId}`);
         savedItems.value = savedRes.data.filter(i => i.userId == userId);

         // 4. Fetch Wallet using Store
         await walletStore.fetchWallet(userId);

          // 5. Fetch Recent Activity
         const activityRes = await axios.get(`http://localhost:3000/recentActivity?userId=${userId}`);
         recentActivity.value = activityRes.data.filter(a => a.userId == userId);
    } else {
        // Clear data if no valid user logged in to prevent leakage
        bookings.value = [];
        savedItems.value = [];
        wallet.value = null;
        recentActivity.value = [];
    }

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
