<template>
  <div class="min-h-screen relative overflow-hidden">
    <!-- Background Slideshow -->
    <div class="absolute inset-0 z-0">
      <transition-group name="fade" mode="out-in">
        <div
          v-for="(image, index) in backgroundImages"
          :key="image"
          v-show="currentImageIndex === index"
          class="absolute inset-0 bg-cover bg-center transition-opacity duration-1000"
          :style="{ backgroundImage: `url(${image})` }"
        ></div>
      </transition-group>
      <!-- Overlay -->
      <div class="absolute inset-0 bg-black/20"></div>
    </div>

    <!-- Content -->
    <div class="relative z-10 min-h-screen flex items-center p-4">
      <div class="bg-base-200/80 backdrop-blur-sm w-full max-w-md p-8 rounded-lg shadow-xl">
        <!-- Logo -->
        <div class="text-center mb-8">
          <h1 class="text-4xl font-bold text-primary font-cairo">PYRAMIS</h1>
        </div>

        <!-- Form -->
        <form @submit.prevent="handleSignUp">
          <!-- Full Name Field -->
          <div class="mb-4">
            <label class="block text-sm font-medium text-base-content mb-2">
              Full Name
            </label>
            <input
              v-model="formData.fullName"
              type="text"
              placeholder="Enter your full name"
              class="input input-bordered w-full bg-white/90 focus:bg-white"
              required
            />
          </div>

          <!-- Email Field -->
          <div class="mb-4">
            <label class="block text-sm font-medium text-base-content mb-2">
              Email
            </label>
            <input
              v-model="formData.email"
              type="email"
              placeholder="Enter your email"
              class="input input-bordered w-full bg-white/90 focus:bg-white"
              required
            />
          </div>

          <!-- Password Field -->
          <div class="mb-4">
            <div class="flex justify-between items-center mb-2">
              <label class="text-sm font-medium text-base-content">
                Your password
              </label>
              <button
                type="button"
                @click="togglePassword"
                class="text-sm text-primary hover:text-primary-focus flex items-center gap-1"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke-width="1.5"
                  stroke="currentColor"
                  class="w-4 h-4"
                >
                  <path
                    v-if="showPassword"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.523 10.523 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.242 4.242L9.88 9.88"
                  />
                  <path
                    v-else
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178z"
                  />
                  <path
                    v-if="!showPassword"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                  />
                </svg>
                {{ showPassword ? 'Hide' : 'Show' }}
              </button>
            </div>
            <input
              v-model="formData.password"
              :type="showPassword ? 'text' : 'password'"
              placeholder="Enter your password"
              class="input input-bordered w-full bg-white/90 focus:bg-white"
              required
              minlength="6"
            />
          </div>

          <!-- Confirm Password Field -->
          <div class="mb-6">
            <div class="flex justify-between items-center mb-2">
              <label class="text-sm font-medium text-base-content">
                Confirm Password
              </label>
              <button
                type="button"
                @click="toggleConfirmPassword"
                class="text-sm text-primary hover:text-primary-focus flex items-center gap-1"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke-width="1.5"
                  stroke="currentColor"
                  class="w-4 h-4"
                >
                  <path
                    v-if="showConfirmPassword"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.523 10.523 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.242 4.242L9.88 9.88"
                  />
                  <path
                    v-else
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178z"
                  />
                  <path
                    v-if="!showConfirmPassword"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                  />
                </svg>
                {{ showConfirmPassword ? 'Hide' : 'Show' }}
              </button>
            </div>
            <input
              v-model="formData.confirmPassword"
              :type="showConfirmPassword ? 'text' : 'password'"
              placeholder="Confirm your password"
              class="input input-bordered w-full bg-white/90 focus:bg-white"
              required
            />
          </div>

          <!-- Error Message -->
          <div v-if="error" class="alert alert-error mb-4 rounded-md">
            <span>{{ error }}</span>
          </div>

          <!-- Sign Up Button -->
          <button
            type="submit"
            class="btn btn-primary w-full text-white mb-4"
            :disabled="loading"
          >
            <span v-if="loading" class="loading loading-spinner"></span>
            <span v-else>Sign Up</span>
          </button>

          <!-- Sign In Link -->
          <p class="text-start text-sm text-base-content mb-4">
            Already have an account?
            <router-link to="/authentication/login" class="text-primary hover:text-primary-focus font-medium underline">
              Sign In
            </router-link>
          </p>

          <!-- Divider -->
          <div class="divider text-base-content/60">OR</div>

          <!-- Google Sign Up -->
          <button
            type="button"
            @click="handleGoogleSignUp"
            class="btn btn-outline w-full bg-white hover:bg-gray-50"
          >
            <svg class="w-5 h-5" viewBox="0 0 24 24">
              <path
                fill="#4285F4"
                d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
              />
              <path
                fill="#34A853"
                d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
              />
              <path
                fill="#FBBC05"
                d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
              />
              <path
                fill="#EA4335"
                d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
              />
            </svg>
            <span class="text-base-content">Continue with Google</span>
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';

const router = useRouter();
const authStore = useAuthStore();

// Background Images
const backgroundImages = ref([
  '/images/backk.png',
  '/images/pyramids.jpg',
  '/images/museum.jpg',
  '/images/camelriding.png',
]);

const currentImageIndex = ref(0);
let slideInterval = null;

// Form Data
const formData = ref({
  fullName: '',
  email: '',
  password: '',
  confirmPassword: '',
});

const showPassword = ref(false);
const showConfirmPassword = ref(false);
const loading = ref(false);
const error = ref(null);

// Methods
const togglePassword = () => {
  showPassword.value = !showPassword.value;
};

const toggleConfirmPassword = () => {
  showConfirmPassword.value = !showConfirmPassword.value;
};

const handleSignUp = async () => {
  error.value = null;

  // Validation
  if (formData.value.password !== formData.value.confirmPassword) {
    error.value = 'Passwords do not match';
    return;
  }

  if (formData.value.password.length < 6) {
    error.value = 'Password must be at least 6 characters';
    return;
  }

  loading.value = true;

  const result = await authStore.register({
    fullName: formData.value.fullName,
    email: formData.value.email,
    password: formData.value.password,
  });

  loading.value = false;

  if (result.success) {
    router.push('/');
  } else {
    error.value = result.error;
  }
};

const handleGoogleSignUp = async () => {
  loading.value = true;
  error.value = null;

  const result = await authStore.loginWithGoogle();

  loading.value = false;

  if (result.success) {
    router.push('/');
  } else {
    error.value = result.error;
  }
};

// Slideshow
const startSlideshow = () => {
  slideInterval = setInterval(() => {
    currentImageIndex.value = (currentImageIndex.value + 1) % backgroundImages.value.length;
  }, 5000);
};

onMounted(() => {
  startSlideshow();
});

onUnmounted(() => {
  if (slideInterval) {
    clearInterval(slideInterval);
  }
});
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 1s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>