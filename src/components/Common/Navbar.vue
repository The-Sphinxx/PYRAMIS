<template>
  <nav class="bg-base-100 shadow-md sticky top-0 z-50">
    <div class="page-container">
      <div class="flex items-center justify-between h-20">
        <router-link :to="{ name: 'Home' }" class="text-3xl font-bold text-primary font-cairo">{{ metadata.siteName || 'PYRAMIS' }}</router-link>
        
        <div class="hidden md:flex items-center space-x-8">
          <router-link :to="{ name: 'Home' }" class="font-cairo font-semibold text-primary hover:text-primary/80" active-class="text-primary">
            Home
          </router-link>
          <router-link :to="{ name: 'AttractionsList' }" class="font-cairo text-base-content hover:text-primary" active-class="text-primary">
            Attractions
          </router-link>
          <router-link :to="{ name: 'HotelsList' }" class="font-cairo text-base-content hover:text-primary" active-class="text-primary">
            Hotels
          </router-link>
          <router-link :to="{ name: 'TripsList' }" class="font-cairo text-base-content hover:text-primary" active-class="text-primary">
            Trips
          </router-link>
          <router-link :to="{ name: 'CarsList' }" class="font-cairo text-base-content hover:text-primary" active-class="text-primary">
            Car Rental
          </router-link>
          <router-link :to="{ name: 'AiCollectData' }" class="font-cairo text-base-content hover:text-primary" active-class="text-primary">
            Ai Trip Planner
          </router-link>
        </div>

        <div class="hidden md:flex items-center gap-3">
          <template v-if="!authStore.isAuthenticated">
            <router-link :to="{ name: 'Login' }" class="btn btn-ghost btn-sm font-cairo border border-base-300">
              <i class="fas fa-user mr-2"></i>Login
            </router-link>
            <router-link :to="{ name: 'SignUp' }" class="btn btn-primary btn-sm font-cairo">Sign Up</router-link>
          </template>
          <template v-else>
            <router-link :to="{ name: 'Profile' }" class="btn btn-ghost btn-sm font-cairo border border-base-300">
              <i class="fas fa-user mr-2"></i>Profile
            </router-link>
            <button @click="handleLogout" class="btn btn-primary btn-sm font-cairo">Logout</button>
          </template>
          <button @click="toggleTheme" class="btn btn-circle btn-sm btn-ghost" :title="isDark ? 'Switch to Light Mode' : 'Switch to Dark Mode'">
            <i v-if="isDark" class="fas fa-sun text-lg"></i>
            <i v-else class="fas fa-moon text-lg"></i>
          </button>
        </div>

        <button class="md:hidden btn btn-ghost btn-circle" @click="mobileMenuOpen = !mobileMenuOpen" aria-label="Toggle menu">
          <i class="fas fa-bars text-xl"></i>
        </button>
      </div>

      <div v-if="mobileMenuOpen" class="md:hidden py-4 border-t border-base-300">
        <div class="flex flex-col space-y-4">
          <router-link :to="{ name: 'Home' }" class="font-cairo font-semibold text-primary" @click="closeMobile">Home</router-link>
          <router-link :to="{ name: 'AttractionsList' }" class="font-cairo text-base-content" @click="closeMobile">Attractions</router-link>
          <router-link :to="{ name: 'HotelsList' }" class="font-cairo text-base-content" @click="closeMobile">Hotels</router-link>
          <router-link :to="{ name: 'TripsList' }" class="font-cairo text-base-content" @click="closeMobile">Trips</router-link>
          <router-link :to="{ name: 'CarsList' }" class="font-cairo text-base-content" @click="closeMobile">Car Rental</router-link>
          <router-link :to="{ name: 'AiCollectData' }" class="font-cairo text-base-content" @click="closeMobile">Ai Trip Planner</router-link>
          <div class="flex gap-2 pt-4">
            <template v-if="!authStore.isAuthenticated">
              <router-link :to="{ name: 'Login' }" class="btn btn-ghost btn-sm font-cairo flex-1" @click="closeMobile">Login</router-link>
              <router-link :to="{ name: 'SignUp' }" class="btn btn-primary btn-sm font-cairo flex-1" @click="closeMobile">Sign Up</router-link>
            </template>
            <template v-else>
              <router-link :to="{ name: 'Profile' }" class="btn btn-ghost btn-sm font-cairo flex-1" @click="closeMobile">Profile</router-link>
              <button class="btn btn-primary btn-sm font-cairo flex-1" @click="() => { handleLogout(); closeMobile(); }">Logout</button>
            </template>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { useTheme } from '@/composables/useTheme';

const mobileMenuOpen = ref(false);
const { isDark, toggleTheme } = useTheme();
const router = useRouter();
const authStore = useAuthStore();

const metadata = ref({
  siteName: 'PYRAMIS'
});

const handleLogout = () => {
  authStore.logout();
  router.push({ name: 'Home' });
};

const closeMobile = () => {
  mobileMenuOpen.value = false;
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700;900&display=swap');

.font-cairo { font-family: 'Cairo', sans-serif; }
</style>
