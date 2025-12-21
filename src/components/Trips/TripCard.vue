<template>
  <div class="card bg-base-100 shadow-xl hover:shadow-2xl transition-all duration-300 group overflow-hidden rounded-glass-radius border border-base-200">
    <!-- Image Section -->
    <figure class="relative h-48 overflow-hidden">
      <router-link :to="`/trips/details/${trip.id}`" class="w-full h-full block">
        <img 
          :src="trip.image" 
          :alt="trip.title" 
          class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110"
        />
        <div class="absolute inset-0 bg-gradient-to-t from-black/50 to-transparent opacity-60"></div>
      </router-link>
    </figure>

    <!-- Content Section -->
    <div class="card-body p-4">
      <!-- Title -->
      <router-link :to="`/trips/details/${trip.id}`">
        <h2 class="card-title text-lg font-bold font-cairo text-base-content mb-1 hover:text-primary transition-colors">
          {{ trip.title }}
        </h2>
      </router-link>

      <!-- Description -->
      <p class="text-sm text-base-content/70 font-cairo line-clamp-2 h-10 mb-4">
        {{ trip.description }}
      </p>

      <!-- Amenities Grid -->
      <div class="grid grid-cols-2 gap-y-2 gap-x-1 text-xs text-base-content/80 font-cairo mb-4">
        <!-- Transport -->
        <div class="flex items-center gap-1.5" :class="{ 'opacity-50': !trip.amenities.transport }">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <rect x="1" y="3" width="15" height="13"></rect>
                <polygon points="16 8 20 8 23 11 23 16 16 16 16 8"></polygon>
                <circle cx="5.5" cy="18.5" r="2.5"></circle>
                <circle cx="18.5" cy="18.5" r="2.5"></circle>
            </svg>
            <span>Transport</span>
        </div>

         <!-- Accommodation -->
         <div class="flex items-center gap-1.5" :class="{ 'opacity-50': !trip.amenities.accommodation }">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M3 21h18M5 21V7l8-4 8 4v14M8 21V12a4 4 0 0 1 4-4 4 4 0 0 1 4 4v9"></path>
            </svg>
            <span>Accommodation</span>
        </div>

        <!-- Meals -->
        <div class="flex items-center gap-1.5">
             <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M18 8h1a4 4 0 0 1 0 8h-1"></path>
                <path d="M2 8h16v9a4 4 0 0 1-4 4H6a4 4 0 0 1-4-4V8z"></path>
                <line x1="6" y1="1" x2="6" y2="4"></line>
                <line x1="10" y1="1" x2="10" y2="4"></line>
                <line x1="14" y1="1" x2="14" y2="4"></line>
            </svg>
           <span>{{ trip.amenities.meals }} Meals</span>
        </div>

        <!-- Duration -->
        <div class="flex items-center gap-1.5">
           <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-primary" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
             <path stroke-linecap="round" stroke-linejoin="round" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
           </svg>
           <span>{{ trip.duration }}</span>
        </div>
      </div>

      <!-- Rating & Price -->
      <div class="flex items-center justify-between mb-4 border-t border-base-200 pt-3">
        <div class="flex items-center gap-1">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-orange-400 fill-orange-400" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z" />
          </svg>
          <span class="text-sm font-bold text-base-content">{{ trip.rating }}</span>
        </div>
        <div class="text-right">
           <span class="text-lg font-bold text-primary font-cairo block leading-none">{{ trip.price.toLocaleString() }} $</span>
           <span class="text-[10px] text-base-content/60 font-cairo">Per Person</span>
        </div>
      </div>

      <!-- Buttons -->
      <div class="flex flex-col gap-2 mt-auto">
        <router-link :to="`/trips/details/${trip.id}`" class="btn btn-outline btn-primary btn-sm w-full font-cairo h-10 hover:!text-white">
          View Details
        </router-link>
        <router-link :to="`/trips/details/${trip.id}`" class="btn btn-primary btn-sm w-full font-cairo h-10 text-white shadow-lg shadow-primary/30">
          Book Now
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  trip: {
    type: Object,
    required: true
  }
});
</script>

<style scoped>
.font-cairo {
  font-family: 'Cairo', sans-serif;
}
.rounded-glass-radius {
    border-radius: 12px; /* Or whatever variable is being used globally */
}
</style>
