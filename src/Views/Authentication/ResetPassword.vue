<template>
  <div class="min-h-screen relative overflow-hidden">
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
      <div class="absolute inset-0 bg-black/20"></div>
    </div>

    <div class="relative z-10 min-h-screen flex items-center p-4 justify-center">
      <div class="bg-base-200/80 backdrop-blur-sm w-full max-w-md p-8 rounded-lg shadow-xl">
        <div class="text-center mb-8">
          <h1 class="text-4xl font-bold text-primary font-cairo">PYRAMIS</h1>
        </div>

        <h2 class="text-2xl font-semibold text-center mb-2">Reset Password</h2>
        <p class="text-center text-base-content/70 text-sm mb-6">Enter the code you received and your new password</p>

        <form @submit.prevent="handleReset">
          <div class="mb-4">
            <label class="block text-sm font-medium text-base-content mb-2">Email</label>
            <input
              v-model="formData.email"
              type="email"
              placeholder="you@example.com"
              class="input input-bordered w-full bg-white/90 focus:bg-white text-gray-900"
              :disabled="Boolean(route.query.email)"
            />
            <span v-if="errors.email" class="text-error text-xs mt-1">{{ errors.email }}</span>
          </div>

          <div class="mb-4">
            <label class="block text-sm font-medium text-base-content mb-2">Reset Token</label>
            <input
              v-model="formData.token"
              type="text"
              placeholder="Paste the 6-digit code"
              class="input input-bordered w-full bg-white/90 focus:bg-white text-gray-900"
              maxlength="64"
            />
            <span v-if="errors.token" class="text-error text-xs mt-1">{{ errors.token }}</span>
          </div>

          <div class="mb-4">
            <div class="flex justify-between items-center mb-2">
              <label class="text-sm font-medium text-base-content">New Password</label>
              <button
                type="button"
                @click="showPassword = !showPassword"
                class="text-sm text-primary hover:text-primary-focus flex items-center gap-1"
              >
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-4 h-4">
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
              :type="showPassword ? 'text' : 'password'"
              v-model="formData.newPassword"
              placeholder="At least 8 characters"
              class="input input-bordered w-full bg-white/90 focus:bg-white text-gray-900"
            />
            <span v-if="errors.newPassword" class="text-error text-xs mt-1">{{ errors.newPassword }}</span>
          </div>

          <div class="mb-6">
            <label class="block text-sm font-medium text-base-content mb-2">Confirm Password</label>
            <input
              :type="showPassword ? 'text' : 'password'"
              v-model="formData.confirmPassword"
              placeholder="Repeat your password"
              class="input input-bordered w-full bg-white/90 focus:bg-white text-gray-900"
            />
            <span v-if="errors.confirmPassword" class="text-error text-xs mt-1">{{ errors.confirmPassword }}</span>
          </div>

          <div v-if="errorMessage" class="alert alert-error mb-4 rounded-md">
            <span>{{ errorMessage }}</span>
          </div>
          <div v-if="successMessage" class="alert alert-success mb-4 rounded-md">
            <span>{{ successMessage }}</span>
          </div>

          <button type="submit" class="btn btn-primary w-full text-white" :disabled="isLoading">
            <span v-if="isLoading" class="loading loading-spinner"></span>
            <span v-else>Reset Password</span>
          </button>
        </form>

        <div class="text-center mt-6 text-sm">
          <router-link to="/auth/login" class="text-primary hover:text-primary-focus underline">Back to Sign In</router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, onUnmounted } from 'vue';
import { getBackgrounds } from '@/Services/systemService';
import { useRoute, useRouter } from 'vue-router';
import { authApi } from '@/Services/api';

const router = useRouter();
const route = useRoute();

const backgroundImages = ref([]);

const currentImageIndex = ref(0);
let slideInterval = null;

const formData = reactive({
  email: route.query.email || '',
  token: route.query.token || '',
  newPassword: '',
  confirmPassword: '',
});

const errors = reactive({
  email: '',
  token: '',
  newPassword: '',
  confirmPassword: '',
});

const showPassword = ref(false);
const isLoading = ref(false);
const errorMessage = ref('');
const successMessage = ref('');

const validate = () => {
  errors.email = '';
  errors.token = '';
  errors.newPassword = '';
  errors.confirmPassword = '';

  if (!formData.email) errors.email = 'Email is required';
  if (!/\S+@\S+\.\S+/.test(formData.email)) errors.email = 'Invalid email';
  if (!formData.token) errors.token = 'Reset token is required';
  if (!formData.newPassword || formData.newPassword.length < 8) errors.newPassword = 'Password must be at least 8 characters';
  if (formData.newPassword !== formData.confirmPassword) errors.confirmPassword = 'Passwords do not match';

  return !errors.email && !errors.token && !errors.newPassword && !errors.confirmPassword;
};

const handleReset = async () => {
  errorMessage.value = '';
  successMessage.value = '';
  if (!validate()) return;

  isLoading.value = true;
  try {
    await authApi.resetPassword(formData.email, formData.token, formData.newPassword);
    successMessage.value = 'Password reset successfully. You can now sign in.';
    setTimeout(() => router.push('/auth/login'), 800);
  } catch (error) {
    errorMessage.value = error.response?.data?.message || 'Failed to reset password';
  } finally {
    isLoading.value = false;
  }
};

const startSlideshow = () => {
  slideInterval = setInterval(() => {
    currentImageIndex.value = (currentImageIndex.value + 1) % backgroundImages.value.length;
  }, 5000);
};

onMounted(async () => {
  const backgrounds = await getBackgrounds('ResetPassword');
  backgroundImages.value = backgrounds.map(b => b.url);
  startSlideshow();
});

onUnmounted(() => {
  if (slideInterval) clearInterval(slideInterval);
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
