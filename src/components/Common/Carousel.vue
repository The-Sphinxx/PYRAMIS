<template>
  <div class="carousel-container">
    <!-- Main Image Display -->
    <div class="relative w-full h-[500px] overflow-hidden rounded-lg" style="position: relative;">
      <!-- Image -->
      <transition-group name="slide" tag="div" class="absolute inset-0">
        <img
          v-for="(image, index) in images"
          v-show="index === currentIndex"
          :key="index"
          :src="image"
          :alt="`Slide ${index + 1}`"
          class="absolute w-full h-full object-cover pointer-events-none transition-opacity duration-500"
          :class="index === currentIndex ? 'opacity-100 z-10' : 'opacity-0 z-0'"
        />
      </transition-group>

      <!-- Navigation Arrows -->
      <button
        @click.stop="previousSlide"
        class="absolute left-4 top-1/2 -translate-y-1/2 w-12 h-12 flex items-center justify-center rounded-full bg-white/90 hover:bg-white border-0 shadow-lg transition-all cursor-pointer"
        style="z-index: 50;"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-neutral" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
        </svg>
      </button>

      <button
        @click.stop="nextSlide"
        class="absolute right-4 top-1/2 -translate-y-1/2 w-12 h-12 flex items-center justify-center rounded-full bg-white/90 hover:bg-white border-0 shadow-lg transition-all cursor-pointer"
        style="z-index: 50;"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-neutral" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
        </svg>
      </button>

      <!-- Slide Indicators (dots) -->
      <div class="absolute bottom-4 left-1/2 -translate-x-1/2 flex gap-2 z-10">
        <button
          v-for="(image, index) in images"
          :key="index"
          @click="goToSlide(index)"
          class="w-2 h-2 rounded-full transition-all duration-300"
          :class="currentIndex === index ? 'bg-white w-8' : 'bg-white/50 hover:bg-white/75'"
        ></button>
      </div>
    </div>

    <!-- Thumbnails -->
    <div class="flex gap-4 mt-4 overflow-x-auto pb-2 pt-2 ps-1">
      <button
        v-for="(image, index) in images"
        :key="index"
        @click="goToSlide(index)"
        class="flex-shrink-0 transition-all duration-300 relative p-1 rounded-lg"
        :class="currentIndex === index 
          ? 'bg-primary scale-105' 
          : 'bg-transparent opacity-60 hover:opacity-100'"
      >
        <div class="w-32 h-24 overflow-hidden rounded-md">
          <img
            :src="image"
            :alt="`Thumbnail ${index + 1}`"
            class="w-full h-full object-cover"
          />
        </div>
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, watch } from 'vue';

const props = defineProps({
  images: {
    type: Array,
    required: true,
    default: () => []
  },
  autoPlayInterval: {
    type: Number,
    default: 3000 // 6 seconds
  },
  autoPlay: {
    type: Boolean,
    default: true
  }
});

const currentIndex = ref(0);
let autoPlayTimer = null;

const nextSlide = () => {
  currentIndex.value = (currentIndex.value + 1) % props.images.length;
};

const previousSlide = () => {
  currentIndex.value = currentIndex.value === 0 
    ? props.images.length - 1 
    : currentIndex.value - 1;
};

const goToSlide = (index) => {
  currentIndex.value = index;
  resetAutoPlay();
};

const startAutoPlay = () => {
  if (props.autoPlay && props.images.length > 1) {
    autoPlayTimer = setInterval(() => {
      nextSlide();
    }, props.autoPlayInterval);
  }
};

const stopAutoPlay = () => {
  if (autoPlayTimer) {
    clearInterval(autoPlayTimer);
    autoPlayTimer = null;
  }
};

const resetAutoPlay = () => {
  stopAutoPlay();
  startAutoPlay();
};

onMounted(() => {
  startAutoPlay();
});

onUnmounted(() => {
  stopAutoPlay();
});

// Watch for images changes
watch(() => props.images, () => {
  currentIndex.value = 0;
  resetAutoPlay();
});
</script>

<style scoped>
.carousel-container {
  width: 100%;
  user-select: none;
}

/* Smooth opacity transition - no flash */
img {
  transition: opacity 0.5s ease-in-out;
}

/* Custom scrollbar for thumbnails */
.overflow-x-auto::-webkit-scrollbar {
  height: 6px;
}

.overflow-x-auto::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.1);
  border-radius: 10px;
}

.overflow-x-auto::-webkit-scrollbar-thumb {
  background: #C86A41;
  border-radius: 10px;
}

.overflow-x-auto::-webkit-scrollbar-thumb:hover {
  background: #B85F3A;
}
</style>