<template>
  <aside class="sidebar-container bg-base-100 flex flex-col transition-all duration-300 shadow-sm">
    <!-- Logo Section -->
    <div class="p-6 border-b border-base-300">
      <svg viewBox="0 0 165 25" class="h-10 w-auto text-primary fill-current" xmlns="http://www.w3.org/2000/svg">
                          <path d="M6.576 9.84c.272.64.832.96 1.68.96.96 0 1.728-.48 2.304-1.44.4-.656.6-1.488.6-2.496 0-1.488-.392-2.576-1.176-3.264-.592-.512-1.344-.768-2.256-.768-.208 0-.424.016-.648.048l-1.608.24v14.088c0 .432.152.792.456 1.08.304.288.664.432 1.08.432h.312v.24H.024v-.24h.312c.416 0 .768-.144 1.056-.432.304-.288.464-.648.48-1.08V4.416c-.016-.416-.144-.744-.384-.984-.224-.24-.56-.36-1.008-.36-.112 0-.256.008-.432.024L0 2.88a48.92 48.92 0 0 0 4.272-.552c1.504-.272 2.488-.44 2.952-.504.48-.064.96-.096 1.44-.096.496 0 1.088.056 1.776.168.688.096 1.392.328 2.112.696.72.352 1.304.88 1.752 1.584.464.768.696 1.64.696 2.616 0 .96-.176 1.808-.528 2.544-.672 1.408-1.824 2.304-3.456 2.688a4.935 4.935 0 0 1-2.16.048c-.656-.096-1.208-.32-1.656-.672-.448-.352-.728-.84-.84-1.464l.216-.096ZM14.258.768c.256-.016.664-.024 1.224-.024.544 0 1.264.12 2.16.36a8.226 8.226 0 0 1 2.52 1.08 10.68 10.68 0 0 1 2.112 1.8c1.184 1.312 2.112 2.888 2.784 4.728.96-2.736 2.208-4.832 3.744-6.288C29.97 1.32 31.258.768 32.666.768h.192v.24c-.8.016-1.632.44-2.496 1.272-.848.816-1.592 1.808-2.232 2.976a24.864 24.864 0 0 0-1.632 3.648c-.432 1.264-.688 2.36-.768 3.288v5.016c0 .4.144.752.432 1.056.304.304.672.456 1.104.456h.312v.24h-7.296v-.24h.312c.416 0 .768-.144 1.056-.432.304-.304.464-.664.48-1.08v-5.04c-.192-1.728-.768-3.56-1.728-5.496-1.104-2.24-2.344-3.832-3.72-4.776-.8-.56-1.608-.856-2.424-.888v-.24ZM57.914 23.04a8.789 8.789 0 0 1-3.985.936c-1.248 0-2.384-.192-3.407-.576-2.209-.816-4.088-2.408-5.64-4.776-.4-.576-1.145-1.68-2.232-3.312-1.088-1.632-1.792-2.672-2.112-3.12-.56-.88-1.008-1.36-1.344-1.44v-.24h.479c.816 0 1.569-.184 2.256-.552 1.04-.576 1.56-1.64 1.56-3.192 0-1.136-.32-2.008-.96-2.616a3.183 3.183 0 0 0-2.304-.936h-1.824v13.992c0 .416.152.776.456 1.08.304.288.665.432 1.08.432h.313v.24h-7.297v-.24h.313c.4 0 .752-.144 1.056-.432.303-.288.463-.64.48-1.056V3.912a1.482 1.482 0 0 0-.48-1.056 1.399 1.399 0 0 0-1.056-.456h-.313v-.24h8.232c2.064 0 3.664.472 4.8 1.416.897.736 1.345 1.776 1.345 3.12s-.44 2.44-1.32 3.288c-.864.832-1.856 1.248-2.977 1.248-.096 0-.2-.008-.312-.024.657.352 1.2.744 1.633 1.176.447.432.831.84 1.151 1.224.337.384.616.728.84 1.032.24.288.553.68.937 1.176.384.48.784.984 1.2 1.512.416.528.871 1.072 1.367 1.632a17.45 17.45 0 0 0 1.68 1.56c.624.496 1.256.904 1.896 1.224 1.36.688 2.857 1.04 4.489 1.056v.24Zm14.698.312c-1.744.88-3.512 1.024-5.304.432-1.904-.608-3.544-1.904-4.92-3.888-.736-1.056-1.368-2.28-1.896-3.672l-.768-2.04H54.06l-1.272 3.024c-.08.208-.12.376-.12.504s.024.272.072.432c.064.144.184.28.36.408.192.112.4.168.624.168h.192v.24H48.78v-.24h.216c.368 0 .728-.104 1.08-.312.368-.224.672-.552.912-.984L56.34 5.208c-.624-1.408-1.288-2.376-1.992-2.904-.592-.448-1.16-.672-1.704-.672-.528 0-1.024.08-1.488.24l-.048-.24C52.26 1.04 53.364.744 54.42.744c1.312 0 2.488.512 3.528 1.536 1.024 1.008 1.928 2.464 2.712 4.368l3.48 8.592c1.312 3.248 2.656 5.48 4.032 6.696.56.496 1.136.84 1.728 1.032.592.192 1.088.288 1.488.288a4.93 4.93 0 0 0 1.2-.144l.024.24Zm-18.144-10.2h4.896l-1.68-4.44c-.08-.224-.192-.528-.336-.912a19.3 19.3 0 0 1-.288-.84l-2.592 6.192ZM64.046.576C64.958.192 65.75 0 66.422 0c.672 0 1.264.064 1.776.192a6.103 6.103 0 0 1 1.56.648 7.062 7.062 0 0 1 1.512 1.08 12.832 12.832 0 0 1 2.28 2.88l4.8 9 6.96-11.976h.216l2.496 14.112c.384 2.176 1.048 3.904 1.992 5.184.944 1.296 2.24 1.944 3.888 1.944h.312v.24c-.88.24-1.744.36-2.592.36-1.728 0-3.304-.68-4.728-2.04-1.376-1.296-2.312-3.192-2.808-5.688L82.91 8.952l-4.512 7.824c-.608 1.008-.92 1.896-.936 2.664h-.216L71.27 8.112l-1.176 9.6c-.048.368.112.664.48.888a.809.809 0 0 0 .456.144h.384v.216h-5.016v-.24h.384c.416 0 .784-.12 1.104-.36.336-.256.544-.6.624-1.032L70.142 5.88a17.564 17.564 0 0 0-2.472-3.408c-1.04-1.12-2.128-1.68-3.264-1.68a1.9 1.9 0 0 0-.312.024l-.048-.24ZM99.273 18.72h.312v.24h-7.296v-.24h.312c.416 0 .768-.144 1.056-.432.304-.304.464-.664.48-1.08V3.144l-2.28.096c-1.504.064-2.256.624-2.256 1.68 0 .432.144.8.432 1.104.304.288.72.432 1.248.432h.144V6.6c-.32.112-.592.168-.816.168-.72 0-1.272-.256-1.656-.768a1.91 1.91 0 0 1-.408-1.2c0-.704.28-1.28.84-1.728.544-.464 1.392-.736 2.544-.816.208-.016.88-.048 2.016-.096 2.592-.096 4.472-.256 5.64-.48l.048.24c-.688.112-1.176.344-1.464.696-.272.352-.416.872-.432 1.56v13.032c.016.416.168.776.456 1.08.304.288.664.432 1.08.432Zm3.341-6.576c-.64 1.024-.96 2.072-.96 3.144 0 1.168.424 2.232 1.272 3.192a4.722 4.722 0 0 0 1.752 1.272c.72.304 1.504.456 2.352.456.848 0 1.608-.16 2.28-.48 1.344-.656 2.016-1.728 2.016-3.216 0-.56-.192-1.152-.576-1.776-.368-.624-.928-1.184-1.68-1.68l-4.08-2.736c-1.584-.992-2.376-2.272-2.376-3.84 0-.16.008-.32.024-.48.096-1.232.584-2.232 1.464-3 .896-.784 2.088-1.176 3.576-1.176.912 0 2.016.08 3.312.24h1.488l-.312 3.528h-.216c-.016-.832-.336-1.496-.96-1.992-.624-.512-1.448-.768-2.472-.768-1.248 0-2.152.376-2.712 1.128-.272.384-.408.8-.408 1.248 0 .448.136.832.408 1.152.272.304.704.64 1.296 1.008l4.488 2.928c.912.592 1.624 1.288 2.136 2.088.608.944.912 1.912.912 2.904 0 .64-.136 1.32-.408 2.04-.256.704-.688 1.36-1.296 1.968-.592.608-1.36 1.104-2.304 1.488-.944.384-1.992.576-3.144.576-1.136 0-2.168-.2-3.096-.6-1.712-.72-2.8-1.976-3.264-3.768a4.93 4.93 0 0 1-.144-1.2c0-1.344.472-2.616 1.416-3.816l.216.168Z"/></svg>
        
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

        <button 
          @click="toggleTheme" 
          class="btn btn-circle btn-sm btn-ghost transition-colors theme-toggle-btn"
        >
          <i v-if="isDark" class="fas fa-sun text-lg spin-slow text-primary"></i>
          <i v-else class="fas fa-moon text-lg text-primary"></i> 
        </button>
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
import { useTheme } from '@/composables/useTheme';
import { computed, onMounted, ref, watch } from 'vue';

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const { isDark, toggleTheme } = useTheme();
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

  .theme-toggle-btn {
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

.spin-slow {
  animation: spin 8s linear infinite;
}
@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

</style>
