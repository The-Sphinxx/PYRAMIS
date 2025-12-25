<template>
  <div class="w-full">
    <h2 class="text-3xl font-bold mb-6 text-base-content border-l-4 border-primary pl-4">
      Guest Reviews
    </h2>

    <div v-if="!userReviews || userReviews.length === 0" class="alert alert-info">
      <span>No reviews available yet</span>
    </div>

    <div v-else class="relative">
      <!-- Carousel Container -->
      <div 
        ref="carouselContainer"
        class="overflow-hidden cursor-grab active:cursor-grabbing select-none"
        @mousedown="startDrag"
        @mousemove="onDrag"
        @mouseup="endDrag"
        @mouseleave="endDrag"
        @touchstart="startDrag"
        @touchmove="onDrag"
        @touchend="endDrag"
      >
        <div 
          ref="carouselTrack"
          class="flex"
          :class="{ 'transition-transform duration-500 ease-in-out': !isDragging }"
          :style="{ 
            transform: isDragging 
              ? `translateX(calc(-${currentIndex * (100 / visibleCards)}% + ${dragOffset}px))` 
              : `translateX(-${currentIndex * (100 / visibleCards)}%)`
          }"
        >
          <div 
            v-for="review in userReviews" 
            :key="review.id"
            class="flex-shrink-0 px-1 md:px-2"
            :style="{ width: `${100 / visibleCards}%` }"
          >
            <!-- Review Card -->
            <div class="bg-base-100 rounded-lg shadow-lg p-4 md:p-6 h-full flex flex-col">
              <!-- User Info & Rating -->
              <div class="flex items-center gap-3 mb-3">
                <div class="avatar flex-shrink-0">
                  <div class="w-12 h-12 md:w-14 md:h-14 rounded-full">
                    <img 
                      :src="review.userImage || '/images/users/default.jpg'" 
                      :alt="review.userName"
                      class="object-cover"
                    />
                  </div>
                </div>
                <div class="flex-1 min-w-0 mr-1">
                  <h3 class="font-bold text-sm md:text-base truncate leading-tight">
                    {{ review.userName }}
                  </h3>
                  <p class="text-xs text-base-content/60 leading-tight">
                    {{ review.date }}
                  </p>
                </div>
                <!-- Rating on the right -->
                <div class="flex items-center gap-1 flex-shrink-0">
                  <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 md:w-6 md:h-6 fill-warning" viewBox="0 0 24 24">
                    <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"/>
                  </svg>
                  <span class="text-lg md:text-xl font-bold text-primary whitespace-nowrap">
                    {{ review.rating.toFixed(1) }}
                  </span>
                </div>
              </div>

              <!-- Review Text -->
              <p class="text-sm md:text-base text-base-content/80 flex-1 leading-relaxed">
                {{ review.comment }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Dots Indicator -->
      <div class="flex justify-center gap-2 mt-6">
        <button
          v-for="(dot, index) in totalDots"
          :key="index"
          @click="goToSlide(index)"
          class="w-3 h-3 rounded-full transition-all"
          :class="currentIndex === index ? 'bg-primary w-8' : 'bg-base-300'"
        ></button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';

const props = defineProps({
  userReviews: {
    type: Array,
    required: false,
    default: () => []
  },
  autoPlay: {
    type: Boolean,
    default: true
  },
  autoPlayInterval: {
    type: Number,
    default: 5000
  }
});

const currentIndex = ref(0);
const screenWidth = ref(window.innerWidth);
const carouselContainer = ref(null);
const carouselTrack = ref(null);

// Drag state
const isDragging = ref(false);
const startX = ref(0);
const dragOffset = ref(0);

// Responsive visible cards
const visibleCards = computed(() => {
  if (screenWidth.value >= 1536) return 3; 
  if (screenWidth.value >= 1280) return 2; 
  if (screenWidth.value >= 1024) return 2; 
  if (screenWidth.value >= 768) return 2; 
  return 1; 
});

// Calculate max index
const maxIndex = computed(() => {
  return Math.max(0, props.userReviews.length - visibleCards.value);
});

// Total dots for pagination
const totalDots = computed(() => {
  return maxIndex.value + 1;
});

// Drag functions
const getPositionX = (event) => {
  return event.type.includes('mouse') ? event.pageX : event.touches[0].clientX;
};

const startDrag = (event) => {
  isDragging.value = true;
  startX.value = getPositionX(event);
  dragOffset.value = 0;
  stopAutoPlay();
};

const onDrag = (event) => {
  if (!isDragging.value) return;
  
  event.preventDefault();
  const currentPosition = getPositionX(event);
  dragOffset.value = currentPosition - startX.value;
};

const endDrag = () => {
  if (!isDragging.value) return;
  
  const movedBy = dragOffset.value;
  const threshold = carouselContainer.value.offsetWidth / visibleCards.value * 0.3;
  
  if (movedBy < -threshold && currentIndex.value < maxIndex.value) {
    currentIndex.value++;
  } else if (movedBy > threshold && currentIndex.value > 0) {
    currentIndex.value--;
  }
  
  isDragging.value = false;
  dragOffset.value = 0;
  startAutoPlay();
};

// Navigation functions
const next = () => {
  if (currentIndex.value < maxIndex.value) {
    currentIndex.value++;
  } else if (props.autoPlay) {
    currentIndex.value = 0; 
  }
};

const prev = () => {
  if (currentIndex.value > 0) {
    currentIndex.value--;
  }
};

const goToSlide = (index) => {
  currentIndex.value = index;
};

// Auto-play functionality
let autoPlayTimer = null;

const startAutoPlay = () => {
  if (props.autoPlay && props.userReviews.length > visibleCards.value) {
    autoPlayTimer = setInterval(next, props.autoPlayInterval);
  }
};

const stopAutoPlay = () => {
  if (autoPlayTimer) {
    clearInterval(autoPlayTimer);
    autoPlayTimer = null;
  }
};

// Handle window resize
const handleResize = () => {
  screenWidth.value = window.innerWidth;
  // Reset to first slide if current index is out of bounds
  if (currentIndex.value > maxIndex.value) {
    currentIndex.value = maxIndex.value;
  }
};

// Lifecycle hooks
onMounted(() => {
  window.addEventListener('resize', handleResize, { passive: true });
  startAutoPlay();
});

onUnmounted(() => {
  window.removeEventListener('resize', handleResize);
  stopAutoPlay();
});
</script>

<style scoped>

.transition-transform {
  transition: transform 0.5s ease-in-out;
}
.cursor-grab {
  cursor: grab;
}

.cursor-grab:active {
  cursor: grabbing;
}

.select-none {
  user-select: none;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
}
</style>