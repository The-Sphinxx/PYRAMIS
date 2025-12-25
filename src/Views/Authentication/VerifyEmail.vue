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
        <div class="text-center mb-6">
          <svg viewBox="0 0 115 25" class="h-10 w-auto mx-auto fill-current text-primary" xmlns="http://www.w3.org/2000/svg">
            <path d="M6.576 9.84c.272.64.832.96 1.68.96.96 0 1.728-.48 2.304-1.44.4-.656.6-1.488.6-2.496 0-1.488-.392-2.576-1.176-3.264-.592-.512-1.344-.768-2.256-.768-.208 0-.424.016-.648.048l-1.608.24v14.088c0 .432.152.792.456 1.08.304.288.664.432 1.08.432h.312v.24H.024v-.24h.312c.416 0 .768-.144 1.056-.432.304-.288.464-.648.48-1.08V4.416c-.016-.416-.144-.744-.384-.984-.224-.24-.56-.36-1.008-.36-.112 0-.256.008-.432.024L0 2.88a48.92 48.92 0 0 0 4.272-.552c1.504-.272 2.488-.44 2.952-.504.48-.064.96-.096 1.44-.096.496 0 1.088.056 1.776.168.688.096 1.392.328 2.112.696.72.352 1.304.88 1.752 1.584.464.768.696 1.64.696 2.616 0 .96-.176 1.808-.528 2.544-.672 1.408-1.824 2.304-3.456 2.688a4.935 4.935 0 0 1-2.16.048c-.656-.096-1.208-.32-1.656-.672-.448-.352-.728-.84-.84-1.464l.216-.096ZM14.258.768c.256-.016.664-.024 1.224-.024.544 0 1.264.12 2.16.36a8.226 8.226 0 0 1 2.52 1.08 10.68 10.68 0 0 1 2.112 1.8c1.184 1.312 2.112 2.888 2.784 4.728.96-2.736 2.208-4.832 3.744-6.288C29.97 1.32 31.258.768 32.666.768h.192v.24c-.8.016-1.632.44-2.496 1.272-.848.816-1.592 1.808-2.232 2.976a24.864 24.864 0 0 0-1.632 3.648c-.432 1.264-.688 2.36-.768 3.288v5.016c0 .4.144.752.432 1.056.304.304.672.456 1.104.456h.312v.24h-7.296v-.24h.312c.416 0 .768-.144 1.056-.432.304-.304.464-.664.48-1.08v-5.04c-.192-1.728-.768-3.56-1.728-5.496-1.104-2.24-2.344-3.832-3.72-4.776-.8-.56-1.608-.856-2.424-.888v-.24ZM57.914 23.04a8.789 8.789 0 0 1-3.985.936c-1.248 0-2.384-.192-3.407-.576-2.209-.816-4.088-2.408-5.64-4.776-.4-.576-1.145-1.68-2.232-3.312-1.088-1.632-1.792-2.672-2.112-3.12-.56-.88-1.008-1.36-1.344-1.44v-.24h.479c.816 0 1.569-.184 2.256-.552 1.04-.576 1.56-1.64 1.56-3.192 0-1.136-.32-2.008-.96-2.616a3.183 3.183 0 0 0-2.304-.936h-1.824v13.992c0 .416.152.776.456 1.08.304.288.665.432 1.08.432h.313v.24h-7.297v-.24h.313c.4 0 .752-.144 1.056-.432.303-.288.463-.64.48-1.056V3.912a1.482 1.482 0 0 0-.48-1.056 1.399 1.399 0 0 0-1.056-.456h-.313v-.24h8.232c2.064 0 3.664.472 4.8 1.416.897.736 1.345 1.776 1.345 3.12s-.44 2.44-1.32 3.288c-.864.832-1.856 1.248-2.977 1.248-.096 0-.2-.008-.312-.024.657.352 1.2.744 1.633 1.176.447.432.831.84 1.151 1.224.337.384.616.728.84 1.032.24.288.553.68.937 1.176.384.48.784.984 1.2 1.512.416.528.871 1.072 1.367 1.632a17.45 17.45 0 0 0 1.68 1.56c.624.496 1.256.904 1.896 1.224 1.36.688 2.857 1.04 4.489 1.056v.24Zm14.698.312c-1.744.88-3.512 1.024-5.304.432-1.904-.608-3.544-1.904-4.92-3.888-.736-1.056-1.368-2.28-1.896-3.672l-.768-2.04H54.06l-1.272 3.024c-.08.208-.12.376-.12.504s.024.272.072.432c.064.144.184.28.36.408.192.112.4.168.624.168h.192v.24H48.78v-.24h.216c.368 0 .728-.104 1.08-.312.368-.224.672-.552.912-.984L56.34 5.208c-.624-1.408-1.288-2.376-1.992-2.904-.592-.448-1.16-.672-1.704-.672-.528 0-1.024.08-1.488.24l-.048-.24C52.26 1.04 53.364.744 54.42.744c1.312 0 2.488.512 3.528 1.536 1.024 1.008 1.928 2.464 2.712 4.368l3.48 8.592c1.312 3.248 2.656 5.48 4.032 6.696.56.496 1.136.84 1.728 1.032.592.192 1.088.288 1.488.288a4.93 4.93 0 0 0 1.2-.144l.024.24Zm-18.144-10.2h4.896l-1.68-4.44c-.08-.224-.192-.528-.336-.912a19.3 19.3 0 0 1-.288-.84l-2.592 6.192ZM64.046.576C64.958.192 65.75 0 66.422 0c.672 0 1.264.064 1.776.192a6.103 6.103 0 0 1 1.56.648 7.062 7.062 0 0 1 1.512 1.08 12.832 12.832 0 0 1 2.28 2.88l4.8 9 6.96-11.976h.216l2.496 14.112c.384 2.176 1.048 3.904 1.992 5.184.944 1.296 2.24 1.944 3.888 1.944h.312v.24c-.88.24-1.744.36-2.592.36-1.728 0-3.304-.68-4.728-2.04-1.376-1.296-2.312-3.192-2.808-5.688L82.91 8.952l-4.512 7.824c-.608 1.008-.92 1.896-.936 2.664h-.216L71.27 8.112l-1.176 9.6c-.048.368.112.664.48.888a.809.809 0 0 0 .456.144h.384v.216h-5.016v-.24h.384c.416 0 .784-.12 1.104-.36.336-.256.544-.6.624-1.032L70.142 5.88a17.564 17.564 0 0 0-2.472-3.408c-1.04-1.12-2.128-1.68-3.264-1.68a1.9 1.9 0 0 0-.312.024l-.048-.24ZM99.273 18.72h.312v.24h-7.296v-.24h.312c.416 0 .768-.144 1.056-.432.304-.304.464-.664.48-1.08V3.144l-2.28.096c-1.504.064-2.256.624-2.256 1.68 0 .432.144.8.432 1.104.304.288.72.432 1.248.432h.144V6.6c-.32.112-.592.168-.816.168-.72 0-1.272-.256-1.656-.768a1.91 1.91 0 0 1-.408-1.2c0-.704.28-1.28.84-1.728.544-.464 1.392-.736 2.544-.816.208-.016.88-.048 2.016-.096 2.592-.096 4.472-.256 5.64-.48l.048.24c-.688.112-1.176.344-1.464.696-.272.352-.416.872-.432 1.56v13.032c.016.416.168.776.456 1.08.304.288.664.432 1.08.432Zm3.341-6.576c-.64 1.024-.96 2.072-.96 3.144 0 1.168.424 2.232 1.272 3.192a4.722 4.722 0 0 0 1.752 1.272c.72.304 1.504.456 2.352.456.848 0 1.608-.16 2.28-.48 1.344-.656 2.016-1.728 2.016-3.216 0-.56-.192-1.152-.576-1.776-.368-.624-.928-1.184-1.68-1.68l-4.08-2.736c-1.584-.992-2.376-2.272-2.376-3.84 0-.16.008-.32.024-.48.096-1.232.584-2.232 1.464-3 .896-.784 2.088-1.176 3.576-1.176.912 0 2.016.08 3.312.24h1.488l-.312 3.528h-.216c-.016-.832-.336-1.496-.96-1.992-.624-.512-1.448-.768-2.472-.768-1.248 0-2.152.376-2.712 1.128-.272.384-.408.8-.408 1.248 0 .448.136.832.408 1.152.272.304.704.64 1.296 1.008l4.488 2.928c.912.592 1.624 1.288 2.136 2.088.608.944.912 1.912.912 2.904 0 .64-.136 1.32-.408 2.04-.256.704-.688 1.36-1.296 1.968-.592.608-1.36 1.104-2.304 1.488-.944.384-1.992.576-3.144.576-1.136 0-2.168-.2-3.096-.6-1.712-.72-2.8-1.976-3.264-3.768a4.93 4.93 0 0 1-.144-1.2c0-1.344.472-2.616 1.416-3.816l.216.168Z"/>
          </svg>
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
import { getBackgrounds } from '@/Services/systemService';

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();

const email = computed(() => route.query.email || authStore.user?.email || '');
const code = ref('');
const isLoading = ref(false);
const isResending = ref(false);
const resendCooldown = ref(0);
const errorMessage = ref('');

const backgroundImages = ref([]);

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

onMounted(async () => {
  const backgrounds = await getBackgrounds('VerifyEmail');
  backgroundImages.value = backgrounds.map(b => b.url);
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
