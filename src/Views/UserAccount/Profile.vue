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
        <!-- Upcoming Bookings -->
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
            <div v-for="item in savedItems" :key="item.id" class="card bg-base-100 shadow-xl image-full h-64 relative">
              <button
                class="btn btn-square btn-sm absolute right-3 top-3 bg-base-100/80 z-50 hover:bg-error hover:text-white"
                @click.stop="handleRemoveWishlist(item)"
                title="Remove from wishlist"
              >
                <i class="fas fa-heart-broken text-error"></i>
              </button>
              <figure><img :src="item.imageUrl || item.image" :alt="item.title" class="w-full h-full object-cover" /></figure>
              <div class="card-body">
                <h2 class="card-title text-white">{{ item.title }}</h2>
                <div class="flex items-center gap-1 text-white/90 text-sm">
                   <i class="fas fa-map-marker-alt"></i> {{ item.location }}
                </div>
                 <div class="flex items-center gap-1 text-yellow-400 text-sm">
                   <i class="fas fa-star"></i> {{ item.rating }}
                </div>
                <div class="card-actions justify-end mt-auto">
                  <button class="btn btn-primary btn-sm" @click="handleViewDetails(item)">View Details</button>
                </div>
              </div>
            </div>
         </div>
         <div v-else class="text-center py-12 text-base-content/70">
          <i class="fas fa-bookmark text-4xl mb-4"></i>
          <p>No saved items yet.</p>
        </div>
      </div>

      <!-- Support Tab -->
      <div v-if="activeTab === 'support'" class="h-[600px] flex bg-base-100 border border-base-300 rounded-lg overflow-hidden shadow-sm">
        <div class="w-80 border-r border-base-300 flex-none">
          <TicketList 
            :is-user-view="true" 
            :user-id="authStore.user?.id || '1'" 
          />
        </div>
        <div class="flex-1 min-w-0">
          <TicketChat 
            :is-user-view="true" 
            :user-id="authStore.user?.id || '1'" 
            :user-name="user.name"
          />
        </div>
      </div>



    </div>

    <!-- Edit Profile Modal -->
    <EditProfileModal
      :is-open="isEditModalOpen"
      :user="user"
      @close="isEditModalOpen = false"
      @save="handleSaveProfile"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import ProfileHeader from '@/components/profile/ProfileHeader.vue';
import BookingCard from '@/components/profile/BookingCard.vue';
import ActivityItem from '@/components/profile/ActivityItem.vue';
import EditProfileModal from '@/components/profile/EditProfileModal.vue';
import TicketList from '@/components/Support/TicketList.vue';
import TicketChat from '@/components/Support/TicketChat.vue';

import api from '@/Services/api';
import { useAuthStore } from '@/stores/authStore';
import { useWishlistStore } from '@/stores/wishlistStore';

const authStore = useAuthStore();
const wishlistStore = useWishlistStore();
const router = useRouter();
const activeTab = ref('overview');

const user = ref({
  name: 'Guest User',
  email: '',
  avatar: 'https://images.unsplash.com/photo-1599566150163-29194dcaad36?w=150',
  membershipType: 'Member'
});
const bookings = ref([]);
const savedItems = computed(() => wishlistStore.wishlistItems);
const recentActivity = ref([]);
const isEditModalOpen = ref(false);

const tabs = [
  { id: 'overview', name: 'Overview', icon: 'fas fa-th-large' },
  { id: 'bookings', name: 'Bookings', icon: 'fas fa-calendar-alt' },
  { id: 'saved', name: 'Saved', icon: 'fas fa-bookmark' },
  { id: 'support', name: 'Support', icon: 'fas fa-headset' }
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

const handleRemoveWishlist = async (item) => {
  try {
    const itemIdToRemove = item.itemId ?? item.id;
    const typeValue = item.itemType ?? item.itemtype ?? item.type ?? 'Trip';
    const type = typeof typeValue === 'number'
      ? ['Trip', 'Hotel', 'Car', 'Attraction'][typeValue] || 'Trip'
      : typeValue;

    await wishlistStore.removeFromWishlist(type, itemIdToRemove);
  } catch (error) {
    console.error('Failed to remove wishlist item', error);
  }
};

const handleViewDetails = (item) => {
  const itemIdValue = item.itemId ?? item.id;
  const typeValue = item.itemType ?? item.itemtype ?? item.type ?? 0;
  
  // Convert numeric type to string if needed
  const typeMap = ['trips', 'hotels', 'cars', 'attractions'];
  let routePath = '';
  
  if (typeof typeValue === 'number') {
    routePath = `/${typeMap[typeValue]}/details/${itemIdValue}`;
  } else {
    const typeStr = typeValue.toLowerCase();
    routePath = `/${typeStr}s/details/${itemIdValue}`;
  }
  
  router.push(routePath);
};

onMounted(async () => {
  if (!authStore.user) {
    authStore.initAuth();
  }

  try {
    const { data } = await api.get('/Profile');

    user.value = {
      id: data.id,
      name: data.fullName || `${data.firstName} ${data.lastName}`.trim() || 'Guest User',
      firstName: data.firstName,
      lastName: data.lastName,
      email: data.email,
      avatar: data.profileImage || 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150',
      profileImage: data.profileImage,
      phone: data.phone,
      nationality: data.nationality,
      gender: data.gender,
      dateOfBirth: data.dateOfBirth,
      membershipType: data.membershipType || 'Standard Member'
    };

    bookings.value = data.bookings || [];
    savedItems.value = data.wishlist || [];
    recentActivity.value = []; // Not yet provided by backend
  } catch (error) {
    console.error('Failed to load profile data:', error);
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
