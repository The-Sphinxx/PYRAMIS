<template>
  <div class="w-full max-w-[570px] bg-base-100 border border-base-300/50 shadow-lg hover:shadow-2xl hover:-translate-y-2 transition-all duration-300 rounded-2xl overflow-hidden font-sans group">
    <!-- Hotel Image -->
    <div class="relative h-[200px] overflow-hidden">
      <button
        class="btn btn-circle btn-sm absolute right-3 top-3 z-10 bg-base-100/80"
        :class="{ 'text-error': isWishlisted }"
        @click.stop="toggleWishlist"
        title="Save to wishlist"
      >
        <i :class="isWishlisted ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
      </button>
      <img 
        :src="hotel.image" 
        :alt="hotel.name" 
        class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110"
      />
      <!-- Gradient Overlay matching AttractionCard -->
      <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-black/20 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
    </div>

    <!-- Hotel Details -->
    <div class="p-4">
      <div class="flex justify-between items-center mb-3">
        <h3 class="text-lg font-bold m-0 text-base-content leading-[1.4] line-clamp-2 min-h-[3.25rem] flex-1">
          {{ hotel.name }}
        </h3>
        <span class="text-xl font-bold text-primary ml-2">${{ hotel.price }}</span>
      </div>

      <div class="flex items-center gap-1.5 mb-2 text-base-content/70 text-[13px]">
        <svg class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path>
          <circle cx="12" cy="10" r="3"></circle>
        </svg>
        <span>{{ hotel.location }}</span>
      </div>

      <div class="flex items-center gap-1.5 mb-3">
        <svg class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="currentColor">
          <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"></path>
        </svg>
        <span class="text-[13px] font-semibold text-base-content">{{ hotel.rating }} ({{ hotel.reviews }} reviews)</span>
      </div>

      <div class="flex gap-4 mb-4 overflow-x-auto no-scrollbar">
        <div v-for="amenity in hotel.amenities" :key="amenity" class="flex items-center gap-1 text-base-content/80 text-xs whitespace-nowrap bg-base-200 px-2 py-1 rounded-lg">
          <!-- Wifi Icon -->
          <svg v-if="amenity === 'Wifi'" class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M5 12.55a11 11 0 0 1 14.08 0"></path>
            <path d="M1.42 9a16 16 0 0 1 21.16 0"></path>
            <path d="M8.53 16.11a6 6 0 0 1 6.95 0"></path>
            <line x1="12" y1="20" x2="12.01" y2="20"></line>
          </svg>
          <!-- Pool Icon -->
          <svg v-else-if="amenity === 'Pool'" class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="13" width="18" height="8" rx="1"></rect>
            <path d="M12 13V7"></path>
            <path d="M7 7h10"></path>
          </svg>
          <!-- Gym Icon -->
          <svg v-else-if="amenity === 'Gym'" class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M6.5 6.5l11 11"></path>
            <path d="M21 21H3"></path>
            <path d="M3 10l7-7 7 7 4 4"></path>
          </svg>
          <!-- Spa Icon -->
          <svg v-else-if="amenity === 'Spa'" class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9z"></path>
          </svg>
          <!-- Restaurant Icon -->
          <svg v-else-if="amenity === 'Restaurant'" class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M3 2v7c0 1.5 1.3 3 3 3v11"></path>
            <path d="M8 2v11"></path>
            <path d="M8 12h6"></path>
            <path d="M18 2v20"></path>
          </svg>
          <!-- Bar Icon -->
          <svg v-else-if="amenity === 'Bar'" class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M5 3h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2z"></path>
            <path d="M9 7h6"></path>
            <path d="M9 11h6"></path>
            <path d="M9 15h3"></path>
          </svg>
          <!-- Parking Icon -->
          <svg v-else-if="amenity === 'Parking'" class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="3" width="18" height="18" rx="2"></rect>
            <path d="M9 17V7h4a3 3 0 0 1 0 6H9"></path>
          </svg>
          <!-- Shuttle Icon -->
          <svg v-else-if="amenity === 'Shuttle'" class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="6" width="18" height="13" rx="2"></rect>
            <path d="M3 10h18"></path>
            <circle cx="8" cy="19" r="1"></circle>
            <circle cx="16" cy="19" r="1"></circle>
          </svg>
          <!-- Beach Icon -->
          <svg v-else-if="amenity === 'Beach'" class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M17.5 19H9a7 7 0 1 1 6.71-9h1.79a4.5 4.5 0 1 1 0 9z"></path>
          </svg>
          <span>{{ amenity }}</span>
        </div>
      </div>

      <!-- Book Now Button -->
      <router-link class="block text-center w-full px-4 py-2.5 bg-primary text-primary-content border-none rounded-lg text-sm font-semibold cursor-pointer transition-all duration-300 hover:bg-primary-focus hover:shadow-lg active:scale-95" :to="`/hotels/details/${hotel.id}`">
        View Details
      </router-link>
    </div>
  </div>
</template>

<script>
import { ref } from 'vue';
import { useAuthStore } from '@/stores/authStore';
import wishlistApi from '@/Services/wishlistApi';

export default {
  name: 'HotelCard',
  props: {
    hotel: {
      type: Object,
      required: true,
      default: () => ({
        name: 'Luxor Palace',
        image: 'https://example.com/hotel-image.jpg',
        price: 200,
        location: 'Cairo',
        rating: 4.8,
        reviews: '5,205',
        amenities: ['Wifi', 'Pool', 'Gym']
      })
    }
  },
  setup(props) {
    const authStore = useAuthStore();
    const isWishlisted = ref(!!props.hotel?.isWishlisted);
    const isSaving = ref(false);

    const toggleWishlist = async () => {
      if (isSaving.value) return;
      if (!authStore.user?.id) {
        // Future: redirect to login or show toast
        return;
      }

      isSaving.value = true;
      try {
        const payload = {
          itemId: props.hotel.id,
          itemType: 'Hotel',
          title: props.hotel.name,
          imageUrl: props.hotel.image,
          location: props.hotel.location || '',
          rating: props.hotel.rating || null
        };

        if (isWishlisted.value) {
          await wishlistApi.remove('Hotel', props.hotel.id);
          isWishlisted.value = false;
        } else {
          await wishlistApi.add(payload);
          isWishlisted.value = true;
        }
      } finally {
        isSaving.value = false;
      }
    };

    return {
      isWishlisted,
      toggleWishlist
    };
  },
  methods: {
    handleBooking() {
      this.$emit('book', this.hotel);
    }
  }
}
</script>

<style scoped>
.hotel-card {
  width: 100%;
  max-width: 570px;
  background: white;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

.hotel-image {
  width: 100%;
  height: 200px;
  overflow: hidden;
}

.hotel-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.hotel-details {
  padding: 16px;
}

.hotel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.hotel-name {
  font-size: 18px;
  font-weight: 700;
  margin: 0;
  color: #1a1a1a;
  line-height: 1.4;
  min-height: 50px;
  display: -webkit-box;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.hotel-price {
  font-size: 20px;
  font-weight: 700;
  color: #c17a3a;
}

.hotel-location {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-bottom: 8px;
  color: #666;
  font-size: 13px;
}

.hotel-rating {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-bottom: 12px;
}

.icon {
  width: 16px;
  height: 16px;
}

.icon.star {
  color: #c17a3a;
}

.rating-text {
  font-size: 13px;
  font-weight: 600;
  color: #1a1a1a;
}

.hotel-amenities {
  display: flex;
  gap: 16px;
  margin-bottom: 16px;
}

.amenity {
  display: flex;
  align-items: center;
  gap: 4px;
  color: #666;
  font-size: 12px;
}

.amenity .icon {
  color: #c17a3a;
}

.book-now-btn {
  width: 100%;
  padding: 10px 16px;
  background: #c17a3a;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.3s ease;
}

.book-now-btn:hover {
  background: #a66830;
}

.book-now-btn:active {
  transform: scale(0.98);
}
</style>