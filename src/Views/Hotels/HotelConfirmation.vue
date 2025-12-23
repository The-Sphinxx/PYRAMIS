<template>
  <div class="page-container py-8 font-cairo" v-if="hotel">
    <!-- Step Indicator -->
    <StepIndicator :current-step="3" booking-type="hotel" class="mb-12" />

    <div class="max-w-4xl mx-auto">
      <!-- Success Message -->
      <div class="text-center mb-12 animate-fade-in-up">
        <div class="w-16 h-16 rounded-lg border-2 border-emerald-400 text-emerald-400 flex items-center justify-center mx-auto mb-6 text-2xl">
          <i class="fas fa-check"></i>
        </div>
        <h1 class="text-4xl font-bold text-base-content mb-3">Booking Confirmed !</h1>
        <p class="text-base-content/70 text-lg">Your reservation has been successfully processed</p>
      </div>

      <!-- Confirmation Card -->
      <div class="bg-base-100 rounded-2xl shadow-lg border border-base-200 overflow-hidden mb-12">
        <!-- Hotel Image -->
        <div class="h-[300px] w-full relative">
          <img 
            :src="hotel.images?.[0] || '/images/placeholder-hotel.jpg'" 
            :alt="hotel.name" 
            class="w-full h-full object-cover"
          />
          <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
          <div class="absolute bottom-6 left-6 text-white">
             <!-- Optional text overlay if needed -->
          </div>
        </div>

        <div class="p-8">
            <!-- Hotel Info -->
            <p class="text-base-content/70 text-sm mb-4 leading-relaxed line-clamp-2">
                {{ hotel.overview || hotel.description }}
            </p>

            <div class="flex items-center gap-2 text-sm text-base-content/80 mb-8">
                <i class="fas fa-map-marker-alt text-[#C86A3F]"></i>
                <span>{{ hotel.name }}, {{ hotel.city }}, Egypt</span>
            </div>

            <div class="divider"></div>

            <!-- Booking Details Section -->
             <div class="mb-8">
                <div class="flex items-center gap-3 mb-6">
                    <div class="w-1 h-8 bg-[#C86A3F]"></div>
                    <h3 class="text-xl font-bold text-base-content">Booking Details</h3>
                </div>
                
                <div class="flex flex-wrap gap-4">
                     <div v-for="(amenity, index) in displayAmenities" :key="index" class="bg-base-200/40 px-6 py-3 rounded-lg flex items-center gap-3 min-w-[200px]">
                        <div class="w-8 h-8 rounded bg-[#FBEFE9] flex items-center justify-center text-[#C86A3F] text-sm">
                            <i :class="getAmenityIcon(amenity)"></i>
                        </div>
                        <span class="text-sm font-medium text-base-content/70">{{ amenity }}</span>
                     </div>
                </div>
             </div>

             <!-- Location Section -->
             <div class="mb-8" v-if="hotel.latitude && hotel.longitude">
                <Location 
                  :name="hotel.name"
                  :city="hotel.city"
                  :latitude="hotel.latitude"
                  :longitude="hotel.longitude"
                  :current-id="hotel.id"
                  type="hotel"
                  :show-nearby="false"
                />
             </div>

             <!-- Back Home Button -->
             <button @click="router.push('/')" class="btn bg-[#C86A3F] hover:bg-[#B05D35] text-white w-full text-lg font-bold py-4 h-auto shadow-md border-none rounded-lg">
                Back To Home Page
             </button>
        </div>
      </div>
      
      <!-- Enhance Your Stay Title -->
      <div class="text-center mb-12">
        <h2 class="text-2xl font-bold text-base-content">Enhance Your Stay</h2>
      </div>

       <!-- Enhance Your Stay Grid (Mock Data) -->
       <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div v-for="item in enhanceItems" :key="item.id" class="bg-base-100 rounded-xl shadow-sm border border-base-200 overflow-hidden group hover:shadow-md transition-all">
              <div class="h-48 relative overflow-hidden">
                  <img :src="item.image" :alt="item.title" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"/>
              </div>
              <div class="p-4">
                  <div class="flex justify-between items-start mb-2">
                      <h3 class="font-bold text-lg">{{ item.title }}</h3>
                      <span class="text-[#C86A3F] font-bold">{{ item.price }}</span>
                  </div>
                  <p class="text-xs text-base-content/60 mb-3 line-clamp-2">{{ item.description }}</p>
                  
                  <div class="flex items-center gap-1 text-xs text-yellow-500 mb-3">
                      <i class="fas fa-star"></i>
                      <span class="text-base-content/70 font-medium">{{ item.rating }} ({{ item.reviews }} reviews)</span>
                  </div>

                  <div class="flex flex-wrap gap-2 mb-4">
                      <span v-for="tag in item.tags" :key="tag" class="badge badge-sm bg-base-200 border-none text-[10px] text-base-content/70 flex gap-1">
                          <i v-if="tag.icon" :class="tag.icon"></i>
                          {{ tag.label }}
                      </span>
                  </div>

                  <button class="btn btn-sm w-full bg-[#C86A3F] hover:bg-[#B05D35] text-white border-none">
                      {{ item.type === 'car' ? 'Rent Now' : 'Book Now' }}
                  </button>
              </div>
          </div>
       </div>

    </div>
  </div>

  <div v-else class="page-container py-12 text-center text-base-content/70">
     <span class="loading loading-spinner loading-lg text-primary"></span>
     <p class="mt-4">Loading confirmation details...</p>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useHotelsStore } from '@/stores/hotelsStore';
import StepIndicator from '@/components/Common/StepIndicator.vue';
import Location from '@/components/Common/Location.vue';

const route = useRoute();
const router = useRouter();
const hotelStore = useHotelsStore();

const hotel = ref(null);

const displayAmenities = computed(() => {
  if (!hotel.value?.amenities) return [];
  return hotel.value.amenities.slice(0, 3);
});

const getAmenityIcon = (amenity) => {
  const lower = amenity.toLowerCase();
  if (lower.includes('pool')) return 'fas fa-swimming-pool';
  if (lower.includes('spa') || lower.includes('wellness')) return 'fas fa-spa';
  if (lower.includes('gym') || lower.includes('fitness')) return 'fas fa-dumbbell';
  if (lower.includes('wifi')) return 'fas fa-wifi';
  if (lower.includes('park')) return 'fas fa-parking';
  if (lower.includes('transport') || lower.includes('shuttle')) return 'fas fa-shuttle-van';
  return 'fas fa-check-circle'; // Default
};

// Mock Data for "Enhance Your Stay" - using generic images as placeholders
const enhanceItems = [
    {
        id: 1,
        type: 'car',
        title: 'Luxury SUV',
        price: '200$',
        description: 'Perfect for family trips and long journeys with premium comfort',
        rating: 4.9,
        reviews: '4,344',
        image: 'https://images.unsplash.com/photo-1503376763036-066120622c74?auto=format&fit=crop&q=80&w=800',
        tags: [
            { label: 'Auto', icon: 'fas fa-cog' },
            { label: '7 Seats', icon: 'fas fa-chair' },
            { label: 'GPS', icon: 'fas fa-map-marked-alt' },
            { label: 'A/C', icon: 'fas fa-snowflake' }
        ]
    },
    {
        id: 2,
        type: 'trip',
        title: 'Fayoum',
        price: '200$/Person',
        description: 'A quick adventure: lakes, waterfalls, deserts, and sandboarding.',
        rating: 4.9,
        reviews: '4,344',
        image: 'https://images.unsplash.com/photo-1667839603054-051877478049?auto=format&fit=crop&q=80&w=800', 
        tags: [
            { label: 'Transport', icon: 'fas fa-car' },
            { label: '1 Day', icon: 'far fa-clock' }
        ]
    },
    {
        id: 3,
        type: 'car',
        title: 'Luxury SUV',
        price: '200$',
        description: 'Perfect for family trips and long journeys with premium comfort',
        rating: 4.9,
        reviews: '4,344',
        image: 'https://images.unsplash.com/photo-1549399542-7e3f8b79c341?auto=format&fit=crop&q=80&w=800',
        tags: [
            { label: 'Auto', icon: 'fas fa-cog' },
            { label: '7 Seats', icon: 'fas fa-chair' },
            { label: 'GPS', icon: 'fas fa-map-marked-alt' },
            { label: 'A/C', icon: 'fas fa-snowflake' }
        ]
    }
];

onMounted(async () => {
    const hotelId = route.params.id;
    if (hotelId) {
        if (hotelStore.hotels.length === 0) {
            await hotelStore.fetchHotels();
        }
        hotel.value = hotelStore.getHotelById(hotelId);
    }
});
</script>