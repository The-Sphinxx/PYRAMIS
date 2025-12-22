<template>
  <div class="min-h-screen relative overflow-hidden">
    <!-- Background Slideshow -->
    <div class="absolute inset-0 z-0">
      <transition-group name="fade" mode="out-in">
        <div
          v-for="(image, index) in backgroundImages"
          :key="`${image}-${index}`"
          v-if="index === currentImageIndex"
          class="absolute inset-0 bg-cover bg-center"
          :style="{ backgroundImage: `url(${image})` }"
        ></div>
      </transition-group>
      <div class="absolute inset-0 bg-black/20"></div>
    </div>

    <!-- Content -->
    <div class="relative z-10 min-h-screen flex items-center p-4">
      <div class="bg-base-200/80 backdrop-blur-sm w-full max-w-md p-8 rounded-lg shadow-xl">
        <!-- Logo -->
        <div class="text-center mb-8">
          <h1 class="text-4xl font-bold text-primary font-cairo">PYRAMIS</h1>
        </div>

        <!-- Step Indicator -->
        <div class="flex items-center justify-center mb-6 gap-2">
          <div class="w-8 h-8 rounded-full flex items-center justify-center text-sm font-semibold transition-all"
               :class="currentStep === 'email' || currentStep === 'token' || currentStep === 'password' || currentStep === 'success' ? 'bg-primary text-white' : 'bg-gray-300 text-gray-600'">
            1
          </div>
          <div class="w-12 h-0.5 transition-all"
               :class="currentStep === 'token' || currentStep === 'password' || currentStep === 'success' ? 'bg-primary' : 'bg-gray-300'"></div>
          <div class="w-8 h-8 rounded-full flex items-center justify-center text-sm font-semibold transition-all"
               :class="currentStep === 'token' || currentStep === 'password' || currentStep === 'success' ? 'bg-primary text-white' : 'bg-gray-300 text-gray-600'">
            2
          </div>
          <div class="w-12 h-0.5 transition-all"
               :class="currentStep === 'password' || currentStep === 'success' ? 'bg-primary' : 'bg-gray-300'"></div>
          <div class="w-8 h-8 rounded-full flex items-center justify-center text-sm font-semibold transition-all"
               :class="currentStep === 'password' || currentStep === 'success' ? 'bg-primary text-white' : 'bg-gray-300 text-gray-600'">
            3
          </div>
        </div>

        <!-- Step 1: Enter Email -->
        <div v-if="currentStep === 'email'">
          <h2 class="text-2xl font-semibold text-center mb-2">Forgot Password</h2>
          <p class="text-center text-base-content/70 text-sm mb-6">Enter the email associated with your account</p>

          <form @submit.prevent="handleSendToken">
            <div class="mb-4">
              <label class="block text-sm font-medium text-base-content mb-2">Email</label>
              <input 
                type="email" 
                v-model="formData.email"
                placeholder="Enter your email"
                class="input input-bordered w-full bg-white/90 focus:bg-white"
                :class="{ 'input-error': errors.email }"
              />
              <span v-if="errors.email" class="text-error text-xs mt-1">{{ errors.email }}</span>
            </div>

            <!-- Error Message -->
            <div v-if="errorMessage" class="alert alert-error mb-4 rounded-md">
              <span>{{ errorMessage }}</span>
            </div>

            <!-- Submit Button -->
            <button 
              type="submit" 
              class="btn btn-primary w-full text-white mb-4"
              :disabled="isLoading"
            >
              <span v-if="isLoading" class="loading loading-spinner"></span>
              <span v-else>Send Reset Code</span>
            </button>
          </form>

          <!-- Back to login -->
          <p class="text-center text-sm text-base-content">
            Remember your password? 
            <a href="#" @click.prevent="navigateToLogin" class="text-primary hover:text-primary-focus font-medium underline">Back to Sign In</a>
          </p>
        </div>

        <!-- Step 2: Enter Code -->
        <div v-if="currentStep === 'token'">
          <h2 class="text-2xl font-semibold text-center mb-2">Enter Code</h2>
          <p class="text-center text-base-content/70 text-sm mb-6">Enter the 6-digit code sent to your email</p>

          <form @submit.prevent="handleVerifyToken">
            <div class="mb-4">
              <input 
                type="text" 
                v-model="formData.resetToken"
                placeholder="000000"
                class="input input-bordered w-full bg-white/90 focus:bg-white text-center text-2xl tracking-widest font-semibold"
                :class="{ 'input-error': errors.resetToken }"
                maxlength="6"
              />
              <span v-if="errors.resetToken" class="text-error text-xs mt-1">{{ errors.resetToken }}</span>
            </div>

            <!-- Resend option -->
            <div class="text-center mb-4">
              <button 
                type="button" 
                @click="handleResendToken"
                class="text-sm text-primary hover:text-primary-focus underline"
                :disabled="resendCooldown > 0"
                :class="{ 'opacity-50 cursor-not-allowed': resendCooldown > 0 }"
              >
                {{ resendCooldown > 0 ? `Resend after ${resendCooldown}s` : 'Resend Code' }}
              </button>
            </div>

            <!-- Error Message -->
            <div v-if="errorMessage" class="alert alert-error mb-4 rounded-md">
              <span>{{ errorMessage }}</span>
            </div>

            <!-- Submit Button -->
            <button 
              type="submit" 
              class="btn btn-primary w-full text-white"
              :disabled="isLoading"
            >
              <span v-if="isLoading" class="loading loading-spinner"></span>
              <span v-else>Verify Code</span>
            </button>
          </form>
        </div>

        <!-- Step 3: Reset Password -->
        <div v-if="currentStep === 'password'">
          <h2 class="text-2xl font-semibold text-center mb-2">Create New Password</h2>
          <p class="text-center text-base-content/70 text-sm mb-6">Enter your new password below</p>

          <form @submit.prevent="handleResetPassword">
            <!-- New Password -->
            <div class="mb-4">
              <div class="flex justify-between items-center mb-2">
                <label class="text-sm font-medium text-base-content">New Password</label>
                <button
                  type="button"
                  @click="showPassword = !showPassword"
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
                :type="showPassword ? 'text' : 'password'" 
                v-model="formData.newPassword"
                placeholder="At least 8 characters" 
                class="input input-bordered w-full bg-white/90 focus:bg-white"
                :class="{ 'input-error': errors.newPassword }"
              />
              <span v-if="errors.newPassword" class="text-error text-xs mt-1">{{ errors.newPassword }}</span>
            </div>

            <!-- Confirm Password -->
            <div class="mb-4">
              <label class="block text-sm font-medium text-base-content mb-2">Confirm Password</label>
              <input 
                :type="showPassword ? 'text' : 'password'" 
                v-model="formData.confirmPassword"
                placeholder="At least 8 characters" 
                class="input input-bordered w-full bg-white/90 focus:bg-white"
                :class="{ 'input-error': errors.confirmPassword }"
              />
              <span v-if="errors.confirmPassword" class="text-error text-xs mt-1">{{ errors.confirmPassword }}</span>
            </div>

            <!-- Error Message -->
            <div v-if="errorMessage" class="alert alert-error mb-4 rounded-md">
              <span>{{ errorMessage }}</span>
            </div>

            <!-- Submit Button -->
            <button 
              type="submit" 
              class="btn btn-primary w-full text-white"
              :disabled="isLoading"
            >
              <span v-if="isLoading" class="loading loading-spinner"></span>
              <span v-else>Reset Password</span>
            </button>
          </form>
        </div>

        <!-- Step 4: Success -->
        <div v-if="currentStep === 'success'" class="text-center">
          <div class="flex justify-center mb-4">
            <div class="w-20 h-20 rounded-full bg-success/20 flex items-center justify-center">
              <svg width="60" height="60" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="text-success">
                <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path>
                <polyline points="22 4 12 14.01 9 11.01"></polyline>
              </svg>
            </div>
          </div>
          <h2 class="text-2xl font-semibold mb-2">Congratulations!</h2>
          <p class="text-base-content/70 text-sm mb-6">Your password has been reset successfully</p>

          <button 
            @click="navigateToLogin"
            class="btn btn-primary w-full text-white"
          >
            Go to Sign In
          </button>
        </div>

        <!-- Auth Links -->
        <div v-if="currentStep !== 'success'" class="text-center mt-6 text-sm">
          <a href="#" @click.prevent="navigateToLogin" class="text-primary hover:text-primary-focus underline">Sign In</a>
          <span class="mx-2 text-base-content/50">|</span>
          <a href="#" @click.prevent="navigateToSignUp" class="text-primary hover:text-primary-focus underline">Sign Up</a>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { authApi } from '@/Services/api';
import { getBackgrounds } from '@/Services/systemService';

const router = useRouter();

// Background Images
const backgroundImages = ref([]);


const currentImageIndex = ref(0);
let slideInterval = null;

const currentStep = ref('email'); // email, token, password, success
const formData = reactive({
  email: '',
  resetToken: '',
  newPassword: '',
  confirmPassword: ''
});

const errors = reactive({
  email: '',
  resetToken: '',
  newPassword: '',
  confirmPassword: ''
});

const showPassword = ref(false);
const isLoading = ref(false);
const errorMessage = ref('');
const resendCooldown = ref(0);
let cooldownInterval = null;

const navigateToLogin = () => {
  router.push('/auth/login');
};

const navigateToSignUp = () => {
  router.push('/auth/sign-up');
};

const startCooldown = () => {
  resendCooldown.value = 60;
  cooldownInterval = setInterval(() => {
    resendCooldown.value--;
    if (resendCooldown.value <= 0) {
      clearInterval(cooldownInterval);
    }
  }, 1000);
};

const handleSendToken = async () => {
  errorMessage.value = '';
  errors.email = '';
  
  if (!formData.email) {
    errors.email = 'Email is required';
    return;
  }
  
  if (!/\S+@\S+\.\S+/.test(formData.email)) {
    errors.email = 'Email is invalid';
    return;
  }
  
  isLoading.value = true;
  
  try {
    await authApi.forgotPassword(formData.email);
    isLoading.value = false;
    currentStep.value = 'token';
    startCooldown();
  } catch (error) {
    isLoading.value = false;
    errorMessage.value = error.response?.data?.message || 'Unable to send reset code';
  }
};

const handleResendToken = async () => {
  if (resendCooldown.value > 0) return;
  await handleSendToken();
};

const handleVerifyToken = () => {
  errorMessage.value = '';
  errors.resetToken = '';
  
  if (!formData.resetToken) {
    errors.resetToken = 'Reset Token is required';
    return;
  }
  
  if (formData.resetToken.length !== 6) {
    errors.resetToken = 'Reset Token must be 6 digits';
    return;
  }
  
  currentStep.value = 'password';
};

const handleResetPassword = async () => {
  errorMessage.value = '';
  errors.newPassword = '';
  errors.confirmPassword = '';
  
  if (!formData.newPassword) {
    errors.newPassword = 'New Password is required';
    return;
  }
  
  if (formData.newPassword.length < 8) {
    errors.newPassword = 'Password must be at least 8 characters';
    return;
  }
  
  if (formData.newPassword !== formData.confirmPassword) {
    errors.confirmPassword = 'Passwords do not match';
    return;
  }
  
  isLoading.value = true;
  
  try {
    await authApi.resetPassword(
      formData.email,
      formData.resetToken,
      formData.newPassword
    );
    isLoading.value = false;
    currentStep.value = 'success';
  } catch (error) {
    isLoading.value = false;
    errorMessage.value = error.response?.data?.message || 'Reset failed';
  }
};

// Slideshow
const startSlideshow = () => {
  slideInterval = setInterval(() => {
    currentImageIndex.value = (currentImageIndex.value + 1) % backgroundImages.value.length;
  }, 5000);
};

onMounted(async () => {
  try {
    const backgrounds = await getBackgrounds('ForgetPassword');
    backgroundImages.value = backgrounds.map(b => b.url);
  } catch (error) {
    console.error('Failed to load backgrounds', error);
  } finally {
    startSlideshow();
  }
});

onUnmounted(() => {
  if (slideInterval) {
    clearInterval(slideInterval);
  }
  if (cooldownInterval) {
    clearInterval(cooldownInterval);
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