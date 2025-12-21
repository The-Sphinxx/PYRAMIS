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

    <div class="relative z-10 min-h-screen flex items-center p-4">
      <div class="bg-base-200/80 backdrop-blur-sm w-full max-w-md p-8 rounded-lg shadow-xl">
        <div class="text-center mb-6">
          <h1 class="text-4xl font-bold text-primary font-cairo">PYRAMIS</h1>
        </div>

        <h2 class="text-2xl font-semibold text-center mb-2">Verify your email</h2>
        <p class="text-center text-base-content/70 text-sm mb-6">
          Enter the 6-digit code we sent to {{ email }}. Check your inbox and spam folder.
        </p>

        <form @submit.prevent="handleVerify">
          <div class="mb-4">
            <label class="block text-sm font-medium text-base-content mb-2">Verification Code</label>
            <input
              v-model="code"
              type="text"
              maxlength="6"
              placeholder="000000"
              class="input input-bordered w-full bg-white/90 focus:bg-white text-center text-2xl tracking-widest font-semibold"
            />
            <span v-if="errorMessage" class="text-error text-xs mt-1">{{ errorMessage }}</span>
          </div>

          <div class="flex justify-between items-center mb-4 text-sm">
            <span class="text-base-content/70">Didn't get the code?</span>
            <button
              type="button"
              class="text-primary hover:text-primary-focus underline"
              :disabled="resendCooldown > 0 || isResending"
              :class="{ 'opacity-50 cursor-not-allowed': resendCooldown > 0 }"
              @click="handleResend"
            >
              {{ resendCooldown > 0 ? `Resend in ${resendCooldown}s` : 'Resend code' }}
            </button>
          </div>

          <button type="submit" class="btn btn-primary w-full text-white" :disabled="isLoading">
            <span v-if="isLoading" class="loading loading-spinner"></span>
            <span v-else>Verify Email</span>
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
import { ref, onMounted, onUnmounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { authApi } from '@/Services/api';

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();

const email = computed(() => route.query.email || authStore.user?.email || '');
const code = ref('');
const isLoading = ref(false);
const isResending = ref(false);
const resendCooldown = ref(0);
const errorMessage = ref('');

const backgroundImages = ref([
  '/images/backk.png',
  '/images/pyramids.jpg',
  '/images/museum.jpg',
  '/images/camelriding.png',
]);

const currentImageIndex = ref(0);
let slideInterval = null;
let cooldownInterval = null;

const handleVerify = async () => {
  errorMessage.value = '';
  if (!email.value) {
    errorMessage.value = 'Email is required';
    return;
  }
  if (!code.value || code.value.length !== 6) {
    errorMessage.value = 'Enter the 6-digit code';
    return;
  }

  isLoading.value = true;
  try {
    await authApi.verifyEmail(email.value, code.value);
    router.push('/');
  } catch (error) {
    errorMessage.value = error.response?.data?.message || 'Verification failed';
  } finally {
    isLoading.value = false;
  }
};

const startCooldown = () => {
  resendCooldown.value = 60;
  cooldownInterval = setInterval(() => {
    resendCooldown.value -= 1;
    if (resendCooldown.value <= 0 && cooldownInterval) {
      clearInterval(cooldownInterval);
    }
  }, 1000);
};

const handleResend = async () => {
  if (!email.value) {
    errorMessage.value = 'Email is required to resend.';
    return;
  }
  errorMessage.value = '';
  isResending.value = true;
  try {
    await authApi.resendOtp(email.value);
    startCooldown();
  } catch (error) {
    errorMessage.value = error.response?.data?.message || 'Could not resend code';
  } finally {
    isResending.value = false;
  }
};

const startSlideshow = () => {
  slideInterval = setInterval(() => {
    currentImageIndex.value = (currentImageIndex.value + 1) % backgroundImages.value.length;
  }, 5000);
};

onMounted(() => {
  startSlideshow();
});

onUnmounted(() => {
  if (slideInterval) clearInterval(slideInterval);
  if (cooldownInterval) clearInterval(cooldownInterval);
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
