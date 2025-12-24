<template>
  <aside class="sidebar-container bg-base-100 flex flex-col transition-all duration-300 shadow-sm">
    <!-- Logo Section -->
    <div class="p-6 border-b border-base-300">
      <h1 class="sidebar-logo text-4xl font-bold text-primary font-cairo">
        <span class="full-text">PYRAMIS</span>
        <span class="icon-text">P</span>
      </h1>
    </div>

    <!-- Search Bar -->
    <div class="p-4 sidebar-search">
      <div class="relative">
        <input 
          type="text" 
          placeholder="Search" 
          v-model="searchQuery"
          @input="handleSearch"
          class="input input-bordered w-full pl-10 bg-base-200 rounded-md text-sm"
        />
        <svg 
          class="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-base-content opacity-50"
          fill="none" 
          stroke="currentColor" 
          viewBox="0 0 24 24"
        >
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
      </div>
    </div>

    <!-- Navigation Menu -->
    <nav class="flex-1 p-4 space-y-1 overflow-y-auto">
      <router-link
        v-for="item in menuItems"
        :key="item.name"
        :to="item.path"
        class="nav-item flex items-center gap-3 px-4 py-3 rounded-lg transition-all duration-200 hover:bg-base-200"
        :class="{ 'bg-accent/20 text-primary': isActive(item.path) }"
      >
        <!-- Icon SVG -->
        <svg 
          class="w-5 h-5 flex-shrink-0" 
          :class="{ 'text-primary': isActive(item.path) }"
          fill="none" 
          stroke="currentColor" 
          viewBox="0 0 24 24"
        >
          <path 
            stroke-linecap="round" 
            stroke-linejoin="round" 
            stroke-width="2" 
            :d="item.iconPath" 
          />
        </svg>
        <span class="nav-text font-medium">{{ item.name }}</span>
      </router-link>
    </nav>

    <!-- User Profile Section -->
    <div class="p-4 border-t border-base-300">
      <div class="flex items-center gap-3 mb-4 user-profile">
        <div class="avatar">
          <div class="w-12 h-12 rounded-full">
            <img :src="user?.profileImage || 'https://i.pravatar.cc/150?img=33'" alt="User Avatar" />
          </div>
        </div>
        <div class="flex-1 user-info">
          <p class="font-semibold text-sm">{{ user?.fullName || 'Guest' }}</p>
          <span class="badge badge-success badge-sm rounded-md">{{ user?.role === 'admin' ? 'Admin' : 'User' }}</span>
        </div>
      </div>
      
      <button 
        @click="logout"
        class="btn btn-ghost w-full justify-start gap-3 text-error hover:bg-error/10 rounded-md btn-sm"
      >
        <svg class="w-5 h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
        </svg>
        <span class="nav-text">Log out</span>
      </button>
    </div>
  </aside>
</template>

<script setup>
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { computed, onMounted, ref, watch } from 'vue';

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const searchQuery = ref('');

onMounted(() => {
  authStore.initAuth();
  // Initialize search from URL
  if (route.query.q) {
    searchQuery.value = route.query.q;
  }
});

const user = computed(() => authStore.user);

// Watch for search input and update URL
let timeout;
const handleSearch = () => {
  clearTimeout(timeout);
  timeout = setTimeout(() => {
    router.replace({ 
      query: { 
        ...route.query, 
        q: searchQuery.value || undefined 
      } 
    });
  }, 300);
};

// Clear search when changing main routes (optional, but good UX if context changes completely)
watch(() => route.path, () => {
  searchQuery.value = '';
});

const menuItems = [
  {
    name: 'Overview',
    path: '/dashboard/overview',
    iconPath: 'M4 5a1 1 0 011-1h4a1 1 0 011 1v7a1 1 0 01-1 1H5a1 1 0 01-1-1V5zM14 5a1 1 0 011-1h4a1 1 0 011 1v7a1 1 0 01-1 1h-4a1 1 0 01-1-1V5zM4 16a1 1 0 011-1h4a1 1 0 011 1v3a1 1 0 01-1 1H5a1 1 0 01-1-1v-3zM14 16a1 1 0 011-1h4a1 1 0 011 1v3a1 1 0 01-1 1h-4a1 1 0 01-1-1v-3z'
  },
  {
    name: 'Analytics',
    path: '/dashboard/analytics',
    iconPath: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z'
  },
  {
    name: 'Bookings',
    path: '/dashboard/bookings',
    iconPath: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2'
  },
  {
    name: 'Users',
    path: '/dashboard/users-manage',
    iconPath: 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z'
  },
  {
    name: 'Trips',
    path: '/dashboard/trips-manage',
    iconPath: 'M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7'
  },
  {
    name: 'Hotels',
    path: '/dashboard/hotels-manage',
    iconPath: 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4'
  },
  {
    name: 'Cars',
    path: '/dashboard/cars-manage',
    iconPath: 'M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4'
  },
  {
    name: 'Attractions',
    path: '/dashboard/attractions-manage',
    iconPath: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4'
  },
  {
    name: 'Customer Support',
    path: '/dashboard/support',
    iconPath: 'M18.364 5.636l-3.536 3.536m0 5.656l3.536 3.536M9.172 9.172L5.636 5.636m3.536 9.192l-3.536 3.536M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-5 0a4 4 0 11-8 0 4 4 0 018 0z'
  },
  {
    name: 'Admins',
    path: '/dashboard/admins-manage',
    iconPath: 'M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z'
  },
  {
    name: 'Settings',
    path: '/dashboard/settings',
    iconPath: 'M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z'
  }
];

const isActive = (path) => {
  return route.path === path;
};

const logout = () => {
  authStore.logout();
  router.push('/auth/login');
};
</script>

<style scoped>
/* Desktop - Full sidebar */
.sidebar-container {
  width: 288px;
  height: 100%;
  border-radius: 1rem; /* rounded-2xl */
}

.icon-text {
  display: none;
}

.full-text {
  display: inline;
}

/* Tablet and below - Icon-only sidebar */
@media (max-width: 768px) {
  .sidebar-container {
    width: 80px;
  }

  .sidebar-search {
    display: none;
  }

  .nav-text {
    display: none;
  }

  .user-info {
    display: none;
  }

  .nav-item {
    justify-content: center;
    padding: 0.75rem;
  }

  .user-profile {
    justify-content: center;
  }

  .user-profile .avatar {
    margin: 0;
  }

  .icon-text {
    display: inline;
  }

  .full-text {
    display: none;
  }

  .sidebar-logo {
    text-align: center;
  }
}
</style>
