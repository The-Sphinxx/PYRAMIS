<template>
  <div class="min-h-screen relative overflow-hidden">
    <!-- Background Slideshow -->
    <div class="absolute inset-0 z-0">
      <transition-group name="fade" mode="out-in">
        <div
          v-for="(image, index) in backgroundImages"
          :key="image"
          v-show="currentImageIndex === index"
          class="absolute inset-0 bg-cover bg-center bg-no-repeat transition-opacity duration-1000"
          :style="{ backgroundImage: `url(${image})` }"
        ></div>
      </transition-group>
      <!-- Overlay -->
      <div class="absolute inset-0 bg-black/20"></div>
    </div>

    <!-- Content -->
    <div class="relative z-10 min-h-screen flex items-center p-4 ms-[64px]">
      <div class="bg-base-200/80 backdrop-blur-sm w-full max-w-md p-8 rounded-lg shadow-xl">
        <!-- Logo -->
        <div class="text-center mb-8">
          <svg viewBox="0 0 115 25" class="h-10 w-auto mx-auto fill-current text-primary" xmlns="http://www.w3.org/2000/svg">
            <path d="M6.576 9.84c.272.64.832.96 1.68.96.96 0 1.728-.48 2.304-1.44.4-.656.6-1.488.6-2.496 0-1.488-.392-2.576-1.176-3.264-.592-.512-1.344-.768-2.256-.768-.208 0-.424.016-.648.048l-1.608.24v14.088c0 .432.152.792.456 1.08.304.288.664.432 1.08.432h.312v.24H.024v-.24h.312c.416 0 .768-.144 1.056-.432.304-.288.464-.648.48-1.08V4.416c-.016-.416-.144-.744-.384-.984-.224-.24-.56-.36-1.008-.36-.112 0-.256.008-.432.024L0 2.88a48.92 48.92 0 0 0 4.272-.552c1.504-.272 2.488-.44 2.952-.504.48-.064.96-.096 1.44-.096.496 0 1.088.056 1.776.168.688.096 1.392.328 2.112.696.72.352 1.304.88 1.752 1.584.464.768.696 1.64.696 2.616 0 .96-.176 1.808-.528 2.544-.672 1.408-1.824 2.304-3.456 2.688a4.935 4.935 0 0 1-2.16.048c-.656-.096-1.208-.32-1.656-.672-.448-.352-.728-.84-.84-1.464l.216-.096ZM14.258.768c.256-.016.664-.024 1.224-.024.544 0 1.264.12 2.16.36a8.226 8.226 0 0 1 2.52 1.08 10.68 10.68 0 0 1 2.112 1.8c1.184 1.312 2.112 2.888 2.784 4.728.96-2.736 2.208-4.832 3.744-6.288C29.97 1.32 31.258.768 32.666.768h.192v.24c-.8.016-1.632.44-2.496 1.272-.848.816-1.592 1.808-2.232 2.976a24.864 24.864 0 0 0-1.632 3.648c-.432 1.264-.688 2.36-.768 3.288v5.016c0 .4.144.752.432 1.056.304.304.672.456 1.104.456h.312v.24h-7.296v-.24h.312c.416 0 .768-.144 1.056-.432.304-.304.464-.664.48-1.08v-5.04c-.192-1.728-.768-3.56-1.728-5.496-1.104-2.24-2.344-3.832-3.72-4.776-.8-.56-1.608-.856-2.424-.888v-.24ZM57.914 23.04a8.789 8.789 0 0 1-3.985.936c-1.248 0-2.384-.192-3.407-.576-2.209-.816-4.088-2.408-5.64-4.776-.4-.576-1.145-1.68-2.232-3.312-1.088-1.632-1.792-2.672-2.112-3.12-.56-.88-1.008-1.36-1.344-1.44v-.24h.479c.816 0 1.569-.184 2.256-.552 1.04-.576 1.56-1.64 1.56-3.192 0-1.136-.32-2.008-.96-2.616a3.183 3.183 0 0 0-2.304-.936h-1.824v13.992c0 .416.152.776.456 1.08.304.288.665.432 1.08.432h.313v.24h-7.297v-.24h.313c.4 0 .752-.144 1.056-.432.303-.288.463-.64.48-1.056V3.912a1.482 1.482 0 0 0-.48-1.056 1.399 1.399 0 0 0-1.056-.456h-.313v-.24h8.232c2.064 0 3.664.472 4.8 1.416.897.736 1.345 1.776 1.345 3.12s-.44 2.44-1.32 3.288c-.864.832-1.856 1.248-2.977 1.248-.096 0-.2-.008-.312-.024.657.352 1.2.744 1.633 1.176.447.432.831.84 1.151 1.224.337.384.616.728.84 1.032.24.288.553.68.937 1.176.384.48.784.984 1.2 1.512.416.528.871 1.072 1.367 1.632a17.45 17.45 0 0 0 1.68 1.56c.624.496 1.256.904 1.896 1.224 1.36.688 2.857 1.04 4.489 1.056v.24Zm14.698.312c-1.744.88-3.512 1.024-5.304.432-1.904-.608-3.544-1.904-4.92-3.888-.736-1.056-1.368-2.28-1.896-3.672l-.768-2.04H54.06l-1.272 3.024c-.08.208-.12.376-.12.504s.024.272.072.432c.064.144.184.28.36.408.192.112.4.168.624.168h.192v.24H48.78v-.24h.216c.368 0 .728-.104 1.08-.312.368-.224.672-.552.912-.984L56.34 5.208c-.624-1.408-1.288-2.376-1.992-2.904-.592-.448-1.16-.672-1.704-.672-.528 0-1.024.08-1.488.24l-.048-.24C52.26 1.04 53.364.744 54.42.744c1.312 0 2.488.512 3.528 1.536 1.024 1.008 1.928 2.464 2.712 4.368l3.48 8.592c1.312 3.248 2.656 5.48 4.032 6.696.56.496 1.136.84 1.728 1.032.592.192 1.088.288 1.488.288a4.93 4.93 0 0 0 1.2-.144l.024.24Zm-18.144-10.2h4.896l-1.68-4.44c-.08-.224-.192-.528-.336-.912a19.3 19.3 0 0 1-.288-.84l-2.592 6.192ZM64.046.576C64.958.192 65.75 0 66.422 0c.672 0 1.264.064 1.776.192a6.103 6.103 0 0 1 1.56.648 7.062 7.062 0 0 1 1.512 1.08 12.832 12.832 0 0 1 2.28 2.88l4.8 9 6.96-11.976h.216l2.496 14.112c.384 2.176 1.048 3.904 1.992 5.184.944 1.296 2.24 1.944 3.888 1.944h.312v.24c-.88.24-1.744.36-2.592.36-1.728 0-3.304-.68-4.728-2.04-1.376-1.296-2.312-3.192-2.808-5.688L82.91 8.952l-4.512 7.824c-.608 1.008-.92 1.896-.936 2.664h-.216L71.27 8.112l-1.176 9.6c-.048.368.112.664.48.888a.809.809 0 0 0 .456.144h.384v.216h-5.016v-.24h.384c.416 0 .784-.12 1.104-.36.336-.256.544-.6.624-1.032L70.142 5.88a17.564 17.564 0 0 0-2.472-3.408c-1.04-1.12-2.128-1.68-3.264-1.68a1.9 1.9 0 0 0-.312.024l-.048-.24ZM99.273 18.72h.312v.24h-7.296v-.24h.312c.416 0 .768-.144 1.056-.432.304-.304.464-.664.48-1.08V3.144l-2.28.096c-1.504.064-2.256.624-2.256 1.68 0 .432.144.8.432 1.104.304.288.72.432 1.248.432h.144V6.6c-.32.112-.592.168-.816.168-.72 0-1.272-.256-1.656-.768a1.91 1.91 0 0 1-.408-1.2c0-.704.28-1.28.84-1.728.544-.464 1.392-.736 2.544-.816.208-.016.88-.048 2.016-.096 2.592-.096 4.472-.256 5.64-.48l.048.24c-.688.112-1.176.344-1.464.696-.272.352-.416.872-.432 1.56v13.032c.016.416.168.776.456 1.08.304.288.664.432 1.08.432Zm3.341-6.576c-.64 1.024-.96 2.072-.96 3.144 0 1.168.424 2.232 1.272 3.192a4.722 4.722 0 0 0 1.752 1.272c.72.304 1.504.456 2.352.456.848 0 1.608-.16 2.28-.48 1.344-.656 2.016-1.728 2.016-3.216 0-.56-.192-1.152-.576-1.776-.368-.624-.928-1.184-1.68-1.68l-4.08-2.736c-1.584-.992-2.376-2.272-2.376-3.84 0-.16.008-.32.024-.48.096-1.232.584-2.232 1.464-3 .896-.784 2.088-1.176 3.576-1.176.912 0 2.016.08 3.312.24h1.488l-.312 3.528h-.216c-.016-.832-.336-1.496-.96-1.992-.624-.512-1.448-.768-2.472-.768-1.248 0-2.152.376-2.712 1.128-.272.384-.408.8-.408 1.248 0 .448.136.832.408 1.152.272.304.704.64 1.296 1.008l4.488 2.928c.912.592 1.624 1.288 2.136 2.088.608.944.912 1.912.912 2.904 0 .64-.136 1.32-.408 2.04-.256.704-.688 1.36-1.296 1.968-.592.608-1.36 1.104-2.304 1.488-.944.384-1.992.576-3.144.576-1.136 0-2.168-.2-3.096-.6-1.712-.72-2.8-1.976-3.264-3.768a4.93 4.93 0 0 1-.144-1.2c0-1.344.472-2.616 1.416-3.816l.216.168Z"/>
          </svg>
        </div>

        <!-- Form -->
        <form @submit.prevent="handleSignIn">
          <!-- Email Field -->
          <div class="mb-4">
            <label class="block text-sm font-medium text-base-content mb-2">
              User name or email address
            </label>
            <input
              v-model="formData.email"
              type="text"
              placeholder="Enter your email address"
              class="input input-bordered w-full bg-white/90 focus:bg-white text-gray-900"
              :class="{ 'input-error': errors.email }"
              @blur="validateEmailField"
              @input="clearEmailError"
            />
            <span v-if="errors.email" class="text-error text-xs mt-1 block">{{ errors.email }}</span>
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
              class="input input-bordered w-full bg-white/90 focus:bg-white text-gray-900"
              :class="{ 'input-error': errors.password }"
              @blur="validatePasswordField"
              @input="clearPasswordError"
            />
            <span v-if="errors.password" class="text-error text-xs mt-1 block">{{ errors.password }}</span>
          </div>

          <!-- Remember Me & Forget Password -->
          <div class="flex justify-between items-center mb-6">
            <label class="flex items-center gap-2 cursor-pointer">
              <input
                v-model="formData.rememberMe"
                type="checkbox"
                class="checkbox checkbox-primary checkbox-sm"
              />
              <span class="text-sm text-base-content underline">Remember Me</span>
            </label>
            <router-link
              to="/auth/forget-password"
              class="text-sm text-primary hover:text-primary-focus underline"
            >
              Forget your password?
            </router-link>
          </div>

          <!-- Error Message -->
          <div v-if="error" class="alert alert-error mb-4 rounded-md">
            <span>{{ error }}</span>
          </div>

          <!-- Sign In Button -->
          <button
            type="submit"
            class="btn btn-primary w-full text-white mb-4"
            :disabled="loading"
          >
            <span v-if="loading" class="loading loading-spinner"></span>
            <span v-else>Sign In</span>
          </button>

          <!-- Sign Up Link -->
          <p class="text-start text-sm text-base-content mb-2">
            Don't have an account?
            <router-link to="/auth/sign-up" class="text-primary hover:text-primary-focus font-medium underline">
              Sign up
            </router-link>
          </p>

          <!-- Continue as Guest -->
          <p class="text-start">
            <router-link to="/home" class="text-sm text-base-content hover:text-primary-focus underline">
              Continue as a guest
            </router-link>
          </p>

          <!-- Divider -->
          <div class="divider text-base-content/60">OR</div>

          <!-- Google Sign In -->
          <button
            type="button"
            @click="handleGoogleSignIn"
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
import { useRouter, useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { validateEmail } from '@/Utils/validators';
import { useToast } from '@/composables/useToast';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const { toast } = useToast();
const googleClientId = import.meta.env.VITE_GOOGLE_CLIENT_ID || '769814768658-2sqboqvdrghp4qompmtfn76bdd05bfht.apps.googleusercontent.com';

// Background Images
import { getBackgrounds } from '@/Services/systemService';
const backgroundImages = ref([]);

const currentImageIndex = ref(0);
let slideInterval = null;

// Form Data
const formData = ref({
  email: '',
  password: '',
  rememberMe: false,
});

const showPassword = ref(false);
const loading = ref(false);
const error = ref(null);
const errors = ref({});

// Methods
const togglePassword = () => {
  showPassword.value = !showPassword.value;
};

const validateEmailField = () => {
  const emailValidation = validateEmail(formData.value.email);
  if (!emailValidation.valid) {
    errors.value.email = emailValidation.message;
  } else {
    errors.value.email = '';
  }
};

const clearEmailError = () => {
  errors.value.email = '';
};

const validatePasswordField = () => {
  if (!formData.value.password) {
    errors.value.password = 'Password is required';
  } else {
    errors.value.password = '';
  }
};

const clearPasswordError = () => {
  errors.value.password = '';
};

const handleSignIn = async () => {
  loading.value = true;
  error.value = null;
  errors.value = {};

  // Validate Email
  const emailValidation = validateEmail(formData.value.email);
  if (!emailValidation.valid) {
    errors.value.email = emailValidation.message;
    toast.error(emailValidation.message);
    loading.value = false;
    return;
  }

  // Validate Password
  if (!formData.value.password) {
    errors.value.password = 'Password is required';
    toast.error('Password is required');
    loading.value = false;
    return;
  }

  const result = await authStore.login({
    email: formData.value.email,
    password: formData.value.password,
    rememberMe: formData.value.rememberMe,
  });

  loading.value = false;

  if (result.success) {
    toast.success('Login successful! Welcome back.');
    const role = authStore.user?.role || '';
    if (role === 'Admin' || role === 'SuperAdmin') {
      router.push('/dashboard/overview');
    } else {
      const redirectPath = route.query.redirect || '/';
      router.push(redirectPath);
    }
  } else {
    error.value = result.error;
    toast.error(result.error || 'Login failed. Please check your credentials.');
  }
};

const ensureGoogleSdk = () => {
  if (window.google?.accounts?.id) return Promise.resolve();
  return new Promise((resolve, reject) => {
    const script = document.createElement('script');
    script.src = 'https://accounts.google.com/gsi/client';
    script.async = true;
    script.defer = true;
    script.onload = () => resolve();
    script.onerror = () => reject(new Error('Failed to load Google SDK'));
    document.head.appendChild(script);
  });
};

const getGoogleIdToken = async () => {
  await ensureGoogleSdk();

  return new Promise((resolve, reject) => {
    if (!window.google?.accounts?.id) {
      reject(new Error('Google SDK not available'));
      return;
    }

    let resolved = false;

    window.google.accounts.id.initialize({
      client_id: googleClientId,
      callback: (response) => {
        resolved = true;
        if (response.credential) {
          resolve(response.credential);
        } else {
          reject(new Error('No credential returned from Google'));
        }
      },
    });

    window.google.accounts.id.prompt((notification) => {
      if (notification.isNotDisplayed() || notification.isSkippedMoment()) {
        if (!resolved) {
          reject(new Error('Google sign-in was closed or blocked'));
        }
      }
    });
  });
};

const handleGoogleSignIn = async () => {
  loading.value = true;
  error.value = null;

  try {
    const idToken = await getGoogleIdToken();
    const result = await authStore.loginWithGoogle(idToken);

    loading.value = false;

    if (result.success) {
      const role = authStore.user?.role || '';
      if (role === 'Admin' || role === 'SuperAdmin') {
        router.push('/dashboard/overview');
      } else {
        const redirectPath = route.query.redirect || '/';
        router.push(redirectPath);
      }
    } else {
      error.value = result.error;
      toast.error(result.error || 'Google sign-in failed');
    }
  } catch (err) {
    loading.value = false;
    error.value = err.message || 'Google sign-in failed';
    toast.error(err.message || 'Google sign-in failed');
  }
};

// Slideshow
const startSlideshow = () => {
  slideInterval = setInterval(() => {
    currentImageIndex.value = (currentImageIndex.value + 1) % backgroundImages.value.length;
  }, 5000);
};

onMounted(async () => {
  const backgrounds = await getBackgrounds('Login');
  backgroundImages.value = backgrounds.map(b => b.url);
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