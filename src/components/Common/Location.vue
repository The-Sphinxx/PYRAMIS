<template>
  <div class="w-full space-y-6">
    <!-- Title -->
    <div class="flex items-center gap-3">
      <div class="w-1 h-8 bg-primary rounded-full"></div>
      <h2 class="text-2xl md:text-3xl font-bold text-base-content font-cairo">
        Location
      </h2>
    </div>

    <!-- Map Container -->
    <div class="card bg-base-200 shadow-lg overflow-hidden rounded-lg">
      <div class="relative w-full h-[400px] md:h-[500px]">
        <!-- OpenStreetMap iframe -->
        <iframe
          :src="osmMapUrl"
          width="100%"
          height="100%"
          frameborder="0"
          scrolling="no"
          marginheight="0"
          marginwidth="0"
          class="w-full h-full"
          @load="mapLoaded = true"
          style="filter: grayscale(0%) contrast(100%) brightness(100%);"
        />
        
        <!-- Loading Overlay -->
        <div 
          v-if="!mapLoaded"
          class="absolute inset-0 flex flex-col items-center justify-center bg-base-200"
        >
          <span class="loading loading-spinner loading-lg text-primary mb-4"></span>
          <p class="text-base-content/60 font-cairo">Loading map...</p>
        </div>

        <!-- Location Pin Overlay -->
        <div class="absolute inset-0 flex flex-col items-center justify-center pointer-events-none">
          <div class="flex flex-col items-center animate-bounce">
            <svg 
              class="w-16 h-16 text-primary drop-shadow-lg" 
              fill="currentColor" 
              viewBox="0 0 24 24"
            >
              <path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z"/>
            </svg>
          </div>
          
          <div class="mt-4 bg-base-100/95 backdrop-blur-sm px-6 py-3 rounded-full shadow-xl pointer-events-auto">
            <h3 class="text-lg md:text-xl font-bold text-base-content text-center font-cairo">
              {{ name }}
            </h3>
            <p class="text-sm text-base-content/70 text-center font-cairo">
              {{ city }}, Egypt
            </p>
          </div>
        </div>

        <!-- Directions Button -->
        <div class="absolute top-4 right-4">
          <button 
            @click="openInMaps"
            class="btn btn-primary gap-2 shadow-lg"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
            </svg>
            <span>Directions</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Nearby Landmarks -->
    <div v-if="!loadingNearby && nearbyLandmarks.length > 0" class="space-y-4">
      <h3 class="text-xl md:text-2xl font-bold text-base-content font-cairo">
        Nearby {{ nearbyTitle }}
      </h3>
      
      <!-- Landmarks Grid -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div 
          v-for="landmark in nearbyLandmarks"
          :key="landmark.id"
          class="flex items-center gap-4 p-4 bg-base-100 rounded-lg transition-all duration-300"
        >
          <!-- Icon Circle -->
          <div class="w-14 h-14 bg-accent/20 rounded-full flex items-center justify-center flex-shrink-0">
            <component :is="getIconForItem(landmark)" class="w-7 h-7 text-primary" />
          </div>
          
          <!-- Text Content -->
          <div class="flex-1 min-w-0">
            <h4 class="font-bold text-base-content mb-1 font-cairo truncate">
              {{ landmark.name }}
            </h4>
            <p class="text-sm text-base-content/60 font-cairo">
              {{ landmark.formattedDistance }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-else-if="loadingNearby" class="flex justify-center py-8">
      <span class="loading loading-spinner loading-md text-primary"></span>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';

// Import all stores
import { useAttractionStore } from '@/stores/attractionStore';
import { useHotelsStore } from '@/stores/hotelsStore';
import { useTripsStore } from '@/stores/tripsStore';
import { useCarsStore } from '@/stores/carsStore';


const PyramidIcon = {
  template: `<svg viewBox="0 0 24 24" fill="currentColor">
    <path d="M12 2L1 21h22L12 2zm0 3.84L19.5 19h-15L12 5.84z"/>
    <path d="M12 8L6.5 17h11L12 8z"/>
  </svg>`
};

const MuseumIcon = {
  template: `<svg viewBox="0 0 24 24" fill="currentColor">
    <path d="M4 21V10l8-6 8 6v11h-5v-7h-6v7H4z"/>
    <rect x="7" y="11" width="2" height="2"/>
    <rect x="11" y="11" width="2" height="2"/>
    <rect x="15" y="11" width="2" height="2"/>
  </svg>`
};

const HotelIcon = {
  template: `<svg viewBox="0 0 24 24" fill="currentColor">
    <path d="M7 13c1.66 0 3-1.34 3-3S8.66 7 7 7s-3 1.34-3 3 1.34 3 3 3zm12-6h-8v7H3V5H1v15h2v-3h18v3h2v-9c0-2.21-1.79-4-4-4z"/>
  </svg>`
};

const CarIcon = {
  template: `<svg viewBox="0 0 24 24" fill="currentColor">
    <path d="M18.92 6.01C18.72 5.42 18.16 5 17.5 5h-11c-.66 0-1.21.42-1.42 1.01L3 12v8c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-1h12v1c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-8l-2.08-5.99zM6.5 16c-.83 0-1.5-.67-1.5-1.5S5.67 13 6.5 13s1.5.67 1.5 1.5S7.33 16 6.5 16zm11 0c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zM5 11l1.5-4.5h11L19 11H5z"/>
  </svg>`
};

const TripIcon = {
  template: `<svg viewBox="0 0 24 24" fill="currentColor">
    <path d="M20 6h-3V4c0-1.11-.89-2-2-2H9c-1.11 0-2 .89-2 2v2H4c-1.11 0-2 .89-2 2v11c0 1.11.89 2 2 2h16c1.11 0 2-.89 2-2V8c0-1.11-.89-2-2-2zM9 4h6v2H9V4zm11 15H4v-2h16v2zm0-5H4V8h3v2h2V8h6v2h2V8h3v6z"/>
  </svg>`
};

const TreeIcon = {
  template: `<svg viewBox="0 0 24 24" fill="currentColor">
    <path d="M12 2C9.24 2 7 4.24 7 7c0 1.77.94 3.32 2.35 4.19C8.5 12.34 8 13.63 8 15v5h8v-5c0-1.37-.5-2.66-1.35-3.81C15.06 10.32 16 8.77 16 7c0-2.76-2.24-5-4-5z"/>
    <rect x="10" y="20" width="4" height="2"/>
  </svg>`
};

const FamilyIcon = {
  template: `<svg viewBox="0 0 24 24" fill="currentColor">
    <path d="M16 4c0-1.11.89-2 2-2s2 .89 2 2-.89 2-2 2-2-.89-2-2zm4 18v-6h2.5l-2.54-7.63C19.68 7.55 18.92 7 18.06 7h-.12c-.86 0-1.62.55-1.9 1.37L13.5 16H16v6h4zM6 4c0-1.11.89-2 2-2s2 .89 2 2-.89 2-2 2-2-.89-2-2zm3 8.5h-1L6.87 8.87C6.61 8.16 5.89 7.68 5.11 7.68h-.22C4.11 7.68 3.39 8.16 3.13 8.87L2 12.5H1V22h4v-9.5z"/>
  </svg>`
};

const ReligiousIcon = {
  template: `<svg viewBox="0 0 24 24" fill="currentColor">
    <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z"/>
    <circle cx="12" cy="12" r="3"/>
  </svg>`
};

const CultureIcon = {
  template: `<svg viewBox="0 0 24 24" fill="currentColor">
    <path d="M12 2l-5.5 9h11L12 2zm0 3.84L13.93 9h-3.87L12 5.84zM17.5 13c-1.93 0-3.5 1.57-3.5 3.5s1.57 3.5 3.5 3.5 3.5-1.57 3.5-3.5-1.57-3.5-3.5-3.5zm-11 0C4.57 13 3 14.57 3 16.5S4.57 20 6.5 20 10 18.43 10 16.5 8.43 13 6.5 13z"/>
  </svg>`
};

const LandmarkIcon = {
  template: `<svg viewBox="0 0 24 24" fill="currentColor">
    <path d="M12 2L2 7v2h20V7L12 2zm-9 7v11h6v-5h6v5h6V9H3zm8 9H9v-3h2v3zm6 0h-2v-3h2v3z"/>
  </svg>`
};

const BuildingIcon = {
  template: `<svg viewBox="0 0 24 24" fill="currentColor">
    <path d="M17 11V3H7v4H3v14h8v-4h2v4h8V11h-4zM7 19H5v-2h2v2zm0-4H5v-2h2v2zm0-4H5V9h2v2zm4 4H9v-2h2v2zm0-4H9V9h2v2zm0-4H9V5h2v2zm4 8h-2v-2h2v2zm0-4h-2V9h2v2zm0-4h-2V5h2v2zm4 12h-2v-2h2v2zm0-4h-2v-2h2v2z"/>
  </svg>`
};


const props = defineProps({
  name: { type: String, required: true },
  city: { type: String, required: true },
  latitude: { type: Number, required: true },
  longitude: { type: Number, required: true },
  currentId: { type: [String, Number], required: true },
  type: { 
    type: String, 
    required: true,
    validator: (value) => ['attraction', 'hotel', 'trip', 'car'].includes(value)
  },
  showNearby: { type: Boolean, default: true }
});

const router = useRouter();
const attractionStore = useAttractionStore();
const hotelStore = useHotelsStore();
const tripStore = useTripsStore();
const carStore = useCarsStore();

const mapLoaded = ref(false);
const nearbyLandmarks = ref([]);
const loadingNearby = ref(false);

const osmMapUrl = computed(() => {
  const zoom = 14;
  const bbox = `${props.longitude - 0.015},${props.latitude - 0.01},${props.longitude + 0.015},${props.latitude + 0.01}`;
  return `https://www.openstreetmap.org/export/embed.html?bbox=${bbox}&layer=mapnik`;
});

const nearbyTitle = computed(() => {
  const titles = {
    attraction: 'Landmarks',
    hotel: 'Places',
    trip: 'Attractions',
    car: 'Locations'
  };
  return titles[props.type] || 'Places';
});

const openInMaps = () => {
  window.open(`https://www.google.com/maps?q=${props.latitude},${props.longitude}`, '_blank');
};

const calculateDistance = (lat1, lon1, lat2, lon2) => {
  const R = 6371;
  const dLat = toRad(lat2 - lat1);
  const dLon = toRad(lon2 - lon1);
  const a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    Math.cos(toRad(lat1)) * Math.cos(toRad(lat2)) *
    Math.sin(dLon / 2) * Math.sin(dLon / 2);
  const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
  return Math.round(R * c * 10) / 10;
};

const toRad = (degrees) => degrees * (Math.PI / 180);

const formatDistance = (distance) => {
  if (distance < 1) return `${Math.round(distance * 1000)} m`;
  return `${distance} km`;
};

const getIconForItem = (item) => {
  
  if (props.type === 'hotel' || item.type === 'hotel') {
    return HotelIcon;
  }
  
  
  if (props.type === 'car' || item.type === 'car') {
    return CarIcon;
  }
  
 
  if (props.type === 'trip' || item.type === 'trip') {
    return TripIcon;
  }
  
 
  if (item.categories) {
    if (item.categories.includes('HISTORICAL')) return PyramidIcon;
    if (item.categories.includes('MUSEUMS')) return MuseumIcon;
    if (item.categories.includes('NATURE')) return TreeIcon;
    if (item.categories.includes('FAMILY')) return FamilyIcon;
    if (item.categories.includes('RELIGIOUS')) return ReligiousIcon;
    if (item.categories.includes('CULTURE')) return CultureIcon;
    if (item.categories.includes('TOP PICKS')) return LandmarkIcon;
  }
  
  return BuildingIcon;
};

const fetchNearbyLandmarks = async () => {
  loadingNearby.value = true;
  
  try {
    let allItems = [];
    
    
    switch (props.type) {
      case 'attraction':
        if (attractionStore.attractions.length === 0) {
          await attractionStore.fetchAttractions();
        }
        allItems = attractionStore.attractions.map(item => ({ ...item, type: 'attraction' }));
        break;
        
      case 'hotel':
        if (hotelStore.hotels.length === 0) {
          await hotelStore.fetchHotels();
        }
        allItems = hotelStore.hotels.map(item => ({ ...item, type: 'hotel' }));
        break;
        
      case 'trip':
        if (tripStore.trips.length === 0) {
          await tripStore.fetchTrips();
        }
        allItems = tripStore.trips.map(item => ({ ...item, type: 'trip' }));
        break;
        
      case 'car':
        if (carStore.cars.length === 0) {
          await carStore.fetchCars();
        }
        allItems = carStore.cars.map(item => ({ ...item, type: 'car' }));
        break;
    }
    
    
    const itemsWithDistance = allItems
      .filter(item => item.id !== parseInt(props.currentId))
      .filter(item => item.latitude && item.longitude)
      .map(item => {
        const distance = calculateDistance(
          props.latitude,
          props.longitude,
          item.latitude,
          item.longitude
        );
        return {
          ...item,
          distance,
          formattedDistance: formatDistance(distance)
        };
      })
      .sort((a, b) => a.distance - b.distance)
      .slice(0, 3);
    
    nearbyLandmarks.value = itemsWithDistance;
  } catch (error) {
    console.error('Error fetching nearby landmarks:', error);
    nearbyLandmarks.value = [];
  } finally {
    loadingNearby.value = false;
  }
};


onMounted(() => {
  if (props.showNearby) {
    fetchNearbyLandmarks();
  }
});
</script>

<style scoped>
@keyframes bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}

.animate-bounce {
  animation: bounce 2s ease-in-out infinite;
}

.truncate {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>