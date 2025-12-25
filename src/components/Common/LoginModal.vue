<template>
  <!-- Modal Overlay -->
  <Teleport to="body">
    <transition name="modal">
      <div v-if="isOpen" class="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
        <div class="glass backdrop-blur-glass bg-base-200 rounded-lg max-w-md w-full border border-base-300/50 relative">
          <!-- Close Button -->
          <button
            @click="close"
            class="absolute top-4 right-4 text-base-content/60 hover:text-base-content"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>

          <!-- Content -->
          <div class="p-8">
            <h2 class="text-2xl font-bold text-base-content mb-2 font-cairo">{{ title }}</h2>
            <p class="text-base-content/60 mb-6">{{ message }}</p>

            <!-- Buttons -->
            <div class="space-y-3">
              <button
                @click="handleSignUp"
                class="w-full bg-primary hover:bg-primary-focus text-primary-content py-3 rounded-lg font-semibold transition-all"
              >
                Sign Up Now
              </button>
              <button
                @click="handleLogin"
                class="w-full bg-base-300 hover:bg-base-300/80 text-base-content py-3 rounded-lg font-semibold transition-all"
              >
                Already have an account? Login
              </button>
              <button
                @click="close"
                class="w-full text-base-content/60 hover:text-base-content py-3 font-semibold transition-all"
              >
                Continue as Guest
              </button>
            </div>

            <!-- Info -->
            <p class="text-xs text-base-content/40 text-center mt-4">
              Sign in to save your trip plans and manage your wishlist
            </p>
          </div>
        </div>
      </div>
    </transition>
  </Teleport>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const isOpen = ref(false);

const props = defineProps({
  title: {
    type: String,
    default: 'Save Your Trip'
  },
  message: {
    type: String,
    default: 'Sign in to add this trip to your wishlist and access your saved plans'
  }
});

const emit = defineEmits(['closed']);

const open = () => {
  isOpen.value = true;
};

const close = () => {
  isOpen.value = false;
  emit('closed');
};

const handleLogin = () => {
  close();
  router.push({ name: 'Login' });
};

const handleSignUp = () => {
  close();
  router.push({ name: 'SignUp' });
};

defineExpose({
  open,
  close
});
</script>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-to,
.modal-leave-from {
  opacity: 1;
}
</style>
