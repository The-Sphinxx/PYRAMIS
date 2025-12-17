<template>
  <div v-if="trip" class="min-h-screen bg-base-100 pb-12 pt-4">
    <div class="container mx-auto px-4 lg:px-[120px]">
      <!-- Breadcrumbs -->
      <div class="text-sm breadcrumbs mb-4 font-cairo text-base-content/70">
        <ul>
          <li><router-link to="/">Home</router-link></li>
          <li><router-link to="/trips/list">Trips</router-link></li>
          <li class="font-bold text-primary">{{ trip.title }}</li>
        </ul>
      </div>

      <!-- Hero Section (Carousel) -->
      <div class="mb-8">
        <Carousel :images="galleryImages" />
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        
        <!-- Left Column: Details -->
        <div class="lg:col-span-2 space-y-10">
          
          <!-- Header Info -->
          <div>
            <h1 class="text-3xl md:text-4xl font-bold font-cairo text-primary mb-3">{{ trip.title }}</h1>
            
            <div class="flex flex-wrap items-center gap-6 text-sm text-base-content/80 font-cairo pb-6 border-b border-base-200">
                <!-- Duration -->
                <span class="flex items-center gap-2">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-primary" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    {{ trip.duration }}
                </span> 
                
                <!-- Transport -->
                 <span class="flex items-center gap-1.5" v-if="trip.amenities?.transport">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <rect x="1" y="3" width="15" height="13"></rect>
                        <polygon points="16 8 20 8 23 11 23 16 16 16 16 8"></polygon>
                        <circle cx="5.5" cy="18.5" r="2.5"></circle>
                        <circle cx="18.5" cy="18.5" r="2.5"></circle>
                    </svg>
                    Transport
                </span>
                <!-- Accommodation -->
                <span class="flex items-center gap-1.5" v-if="trip.amenities?.accommodation">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M3 21h18M5 21V7l8-4 8 4v14M8 21V12a4 4 0 0 1 4-4 4 4 0 0 1 4 4v9"></path>
                    </svg>
                    Accommodation
                </span>
                
                <!-- Meals -->
                <span class="flex items-center gap-1.5" v-if="trip.amenities?.meals">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M18 8h1a4 4 0 0 1 0 8h-1"></path>
                        <path d="M2 8h16v9a4 4 0 0 1-4 4H6a4 4 0 0 1-4-4V8z"></path>
                        <line x1="6" y1="1" x2="6" y2="4"></line>
                        <line x1="10" y1="1" x2="10" y2="4"></line>
                        <line x1="14" y1="1" x2="14" y2="4"></line>
                    </svg>
                   <span>{{ trip.amenities.meals }} Meals</span>
                </span>                

                 <!-- Rating -->
                 <span class="flex items-center gap-1">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-orange-400 fill-orange-400" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z" />
                    </svg>
                     <span class="font-bold text-base-content">{{ trip.rating }}</span>
                    <span class="text-base-content/50">({{ trip.reviews }} reviews)</span>
                </span>
            </div>
          </div>

          <!-- Overview Section -->
          <div class="space-y-4">
              <div class="flex items-center gap-2">
                 <div class="h-8 w-1 bg-primary rounded-full"></div>
                 <h2 class="text-2xl font-bold font-cairo text-base-content">Overview</h2>
              </div>
              
              <div class="bg-base-100 p-6 rounded-2xl border border-base-200/60 shadow-sm">
                <p class="text-base-content/80 leading-relaxed font-cairo mb-6 text-lg">{{ trip.description }}</p>

                <div v-if="trip.highlights">
                     <h3 class="font-bold text-base-content mb-3 font-cairo text-lg">Highlights</h3>
                     <div class="grid grid-cols-1 md:grid-cols-2 gap-y-3 gap-x-8">
                        <div v-for="(item, idx) in trip.highlights" :key="idx" class="flex items-start gap-3">
                            <span class="mt-1.5 w-2 h-2 rounded-full bg-primary flex-shrink-0"></span>
                            <span class="text-base-content/80 font-cairo">{{ item }}</span>
                        </div>
                    </div>
                </div>
              </div>
          </div>

          <!-- Itinerary Section -->
          <div class="space-y-6" v-if="trip.itinerary">
             <div class="flex items-center gap-2">
                 <div class="h-8 w-1 bg-primary rounded-full"></div>
                 <h2 class="text-2xl font-bold font-cairo text-base-content">Itinerary</h2>
             </div>

             <div class="space-y-6">
                <div v-for="day in trip.itinerary" :key="day.day" class="relative pl-8 border-l-2 border-primary/20 pb-6 last:pb-0">
                     <div class="absolute -left-[9px] top-0 w-4 h-4 rounded-full bg-primary ring-4 ring-base-100"></div>
                     
                     <h3 class="text-lg font-bold font-cairo text-base-content mb-4">Day {{ day.day }}: {{ day.title }}</h3>

                     <div class="space-y-3 bg-base-200/50 p-4 rounded-lg">
                        <!-- Day Main Activity (First Item) -->
                        <div class="flex gap-3">
                            <span class="text-xs font-bold text-primary w-16 flex-shrink-0 pt-0.5">{{ day.time }}</span>
                            <div>
                                <!-- <h4 class="text-sm font-bold text-base-content">{{ day.title }}</h4> -->
                                <p class="text-xs text-base-content/60">{{ day.description }}</p>
                            </div>
                        </div>

                        <!-- Other Activities -->
                        <div v-for="(act, i) in day.activities" :key="i" class="flex gap-3">
                            <span class="text-xs font-bold text-primary w-16 flex-shrink-0 pt-0.5">{{ act.time }}</span>
                            <div>
                                <h4 class="text-sm font-bold text-base-content">{{ act.title }}</h4>
                                <p class="text-xs text-base-content/60">{{ act.desc }}</p>
                            </div>
                        </div>
                     </div>
                </div>
             </div>
          </div>

          <!-- Hotel Section -->
          <div class="space-y-4" v-if="trip.hotel">
            <div class="flex items-center gap-2">
                 <div class="h-8 w-1 bg-primary rounded-full"></div>
                 <h2 class="text-2xl font-bold font-cairo text-base-content">Hotel</h2>
            </div>
            
            <!-- Header Above Image -->
             <div class="flex items-center justify-between mb-4">
                 <h3 class="text-2xl font-bold font-cairo text-base-content">{{ trip.hotel.name }}</h3>
                 <div class="flex items-center gap-1.5 bg-orange-100 text-orange-600 px-3 py-1 rounded-full text-sm font-bold">
                     <span class="text-lg">★</span> {{ trip.hotel.rating }}
                 </div>
             </div>
            
            <div class="bg-base-100 rounded-2xl border border-base-200 overflow-hidden shadow-sm">
                 <!-- Full Width Image -->
                 <div class="w-full h-64 relative">
                     <img :src="trip.hotel.image" class="w-full h-full object-cover" />
                 </div>
                 
                 <!-- Content & Tags Below -->
                 <div class="p-6">
                     <div class="flex flex-wrap gap-2">
                            <span v-for="feat in trip.hotel.features" :key="feat" class="badge badge-lg badge-outline text-base-content/70 border-base-300 p-4">{{ feat }}</span>
                    </div>
                 </div>
            </div>
          </div>

          <!-- Reviews Section (Carousel) -->
           <div class="space-y-4" v-if="trip.reviewsList && trip.reviewsList.length">
               <!-- Used component handles title and layout -->
                <ReviewsCarousel :user-reviews="reviewsAdapted" />
           </div>

        </div>

        <!-- Right Column: Booking Card (Sticky) -->
        <div class="lg:col-span-1">
            <div class="sticky top-24 space-y-6">
                
                <!-- Price Card -->
                <div class="bg-base-100 rounded-2xl shadow-xl border border-base-200 p-6 overflow-hidden relative">
                    <!-- Removed Dollar Icon Background -->
                    
                    <div class="mb-6 relative z-10">
                        <span class="text-sm text-base-content/60 font-cairo block mb-1">Starting from</span>
                        <div class="flex items-baseline gap-1">
                             <h2 class="text-4xl font-bold text-primary font-cairo">{{ trip.price.toLocaleString() }} $</h2>
                             <span class="text-sm text-base-content/50">/ person</span>
                        </div>
                    </div>

                    <!-- Booking Form -->
                    <div class="space-y-5 relative z-10">
                        <!-- Date Selection -->
                        <div v-if="trip.availableDates && trip.availableDates.length">
                            <label class="label text-sm font-bold text-base-content/70 font-cairo pb-1">Select Date</label>
                            <div class="flex flex-col gap-2">
                                <div 
                                    v-for="(date, idx) in trip.availableDates" 
                                    :key="idx"
                                    class="bg-base-200/50 rounded-lg p-3 flex justify-between items-center cursor-pointer border transition-all hover:border-primary/50"
                                    :class="selectedDate === date ? 'border-primary bg-primary/5 ring-1 ring-primary' : 'border-transparent'"
                                    @click="selectedDate = date"
                                >
                                    <span class="font-bold text-sm">{{ date }}</span>
                                    <div class="w-4 h-4 rounded-full border border-base-content/20 flex items-center justify-center">
                                        <div v-if="selectedDate === date" class="w-2.5 h-2.5 rounded-full bg-primary"></div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Guests -->
                        <div> 
                            <label class="label text-sm font-bold text-base-content/70 font-cairo pb-1">Guests</label>
                            <div class="bg-base-200/50 rounded-lg p-1 flex items-center justify-between border border-transparent hover:border-base-content/10">
                                   <div class="p-2">
                                     <span class="block text-xs font-bold text-base-content/80">Adults</span>
                                     <span class="text-xs text-base-content/40">Age 13+</span>
                                   </div>
                                   <div class="flex items-center gap-3 pr-2">
                                       <button class="btn btn-circle btn-xs btn-ghost bg-white shadow-sm" @click="guests.adults > 1 ? guests.adults-- : null" :disabled="guests.adults <= 1">-</button>
                                       <span class="font-bold w-4 text-center">{{ guests.adults }}</span>
                                       <button class="btn btn-circle btn-xs btn-ghost bg-white shadow-sm" @click="guests.adults < (trip.maxPeople || 10) ? guests.adults++ : null">+</button>
                                   </div>
                            </div>
                             <div class="bg-base-200/50 rounded-lg p-1 flex items-center justify-between border border-transparent hover:border-base-content/10 mt-2">
                                   <div class="p-2">
                                     <span class="block text-xs font-bold text-base-content/80">Children</span>
                                     <span class="text-xs text-base-content/40">Age 2-12</span>
                                   </div>
                                   <div class="flex items-center gap-3 pr-2">
                                       <button class="btn btn-circle btn-xs btn-ghost bg-white shadow-sm" @click="guests.children > 0 ? guests.children-- : null" :disabled="guests.children <= 0">-</button>
                                       <span class="font-bold w-4 text-center">{{ guests.children }}</span>
                                       <button class="btn btn-circle btn-xs btn-ghost bg-white shadow-sm" @click="guests.children++">+</button>
                                   </div>
                            </div>
                        </div>

                        <!-- Total -->
                        <div class="pt-4 border-t border-dashed border-base-300 flex justify-between items-center">
                            <span class="font-bold text-base-content/80">Total</span>
                            <span class="font-bold text-2xl text-primary">{{ totalPrice.toLocaleString() }} $</span>
                        </div>

                        <!-- Kept shadow and blur as requested -->
                        <button class="btn btn-primary w-full text-white font-bold font-cairo h-12 shadow-lg shadow-primary/30 hover:shadow-primary/50 transition-all transform hover:-translate-y-0.5">
                            Proceed to Booking
                        </button>
                        
                        <div class="flex items-center justify-center gap-2 text-[10px] text-base-content/40">
                             <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
                             Free cancellation up to 48 hours before
                        </div>
                    </div>
                </div>

                <!-- Assistance Card -->
                <!-- <div class="bg-primary/5 rounded-xl p-4 border border-primary/10 flex items-center gap-4">
                     <div class="w-10 h-10 rounded-full bg-primary/20 flex items-center justify-center text-primary">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18.364 5.636l-3.536 3.536m0 5.656l3.536 3.536M9.172 9.172L5.636 5.636m3.536 9.192l-3.536 3.536M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-5 0a4 4 0 11-8 0 4 4 0 018 0z" /></svg>
                     </div>
                     <div>
                         <p class="text-xs font-bold text-base-content/80">Need Help?</p>
                         <p class="text-xs text-base-content/60">Contact our expert support team 24/7.</p>
                     </div>
                </div> -->
            </div>
        </div>
      </div>

      <!-- Bottom Section: You May Also Like -->
      <div class="mt-20">
        <div class="flex items-center gap-4 mb-8">
             <div class="h-px bg-base-200 flex-1"></div>
             <h2 class="text-2xl font-bold text-primary font-cairo uppercase tracking-wider">You May Also Like</h2>
             <div class="h-px bg-base-200 flex-1"></div>
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
            <TripCard v-for="relatedTrip in relatedTrips" :key="relatedTrip.id" :trip="relatedTrip" />
        </div>
      </div>

    </div>
  </div>
  
  <div v-else class="min-h-screen flex items-center justify-center">
     <span class="loading loading-spinner loading-lg text-primary"></span>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import { useRoute } from 'vue-router';
// Importing directly from db.json. 
// Note: In a real app, this should probably be an API call, but for this setup we import the file.
import dbData from '../../../db.json';
import TripCard from '@/components/Trips/TripCard.vue';
import Carousel from '@/components/Common/Carousel.vue';
import ReviewsCarousel from '@/components/Common/ReviewsCarousel.vue';

const route = useRoute();
const trip = ref(null);
const selectedDate = ref(null);
const guests = ref({ adults: 2, children: 0 });

// Access the nested Structure: dbData.trips is [[...], ...]
// We assume index 0 contains the trips list as seen in db.json analysis
const allTrips = computed(() => {
    if (dbData.trips && Array.isArray(dbData.trips) && dbData.trips.length > 0) {
        // If it's a nested array [[t1, t2]], return data.trips[0]
        if (Array.isArray(dbData.trips[0])) {
            return dbData.trips[0];
        }
        // If it's a flat array [t1, t2], return data.trips
        return dbData.trips;
    }
    return [];
});

const loadTrip = () => {
    const id = parseInt(route.params.id);
    trip.value = allTrips.value.find(t => t.id === id);
    
    if (trip.value) {
        // Set default date
        if (trip.value.availableDates && trip.value.availableDates.length) {
            selectedDate.value = trip.value.availableDates[0];
        }
        
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }
};

const galleryImages = computed(() => {
    if (!trip.value) return [];
    return trip.value.images || (trip.value.image ? [trip.value.image] : []);
});

// Adapt props for ReviewsCarousel
const reviewsAdapted = computed(() => {
    if (!trip.value || !trip.value.reviewsList) return [];
    return trip.value.reviewsList.map(r => ({
        id: r.id,
        userName: r.user,
        userImage: null, // db.json doesn't have user image
        date: r.date,
        rating: r.rating || 5,
        comment: r.text
    }));
});

const relatedTrips = computed(() => {
    if (!trip.value || !allTrips.value.length) return [];
    // Get 4 random trips that are not the current one
    return allTrips.value
        .filter(t => t.id !== trip.value.id)
        .sort(() => 0.5 - Math.random())
        .slice(0, 4);
});

const totalPrice = computed(() => {
    if (!trip.value) return 0;
    const adultPrice = trip.value.price * guests.value.adults;
    const childPrice = (trip.value.price * 0.5) * guests.value.children; // Assuming 50% for children
    return adultPrice + childPrice;
});

onMounted(() => {
    loadTrip();
});

watch(() => route.params.id, () => {
    loadTrip();
});
</script>

<style scoped>
.font-cairo {
    font-family: 'Cairo', sans-serif;
}

.scrollbar-none::-webkit-scrollbar {
    display: none;
}
.scrollbar-none {
    -ms-overflow-style: none;
    scrollbar-width: none;
}
</style>

