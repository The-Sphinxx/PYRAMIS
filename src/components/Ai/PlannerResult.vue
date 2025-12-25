<template>
  <div class="min-h-screen bg-base-100">
    <!-- Header -->
    <div class="bg-gradient-to-r from-primary to-primary-focus text-primary-content py-8 px-4 sm:px-6 lg:px-8 shadow-lg">
      <div class="max-w-7xl mx-auto">
        <button
          @click="goBack"
          class="mb-4 flex items-center text-primary-content hover:text-primary-content/80 transition"
        >
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
          </svg>
          Back to Planner
        </button>
        <h1 class="text-3xl font-bold font-cairo">Your Personalized Trip Plan</h1>
        <p class="text-primary-content/80 mt-2" v-if="tripData?.trip_overview">
          {{ tripData.trip_overview.destination }} • 
          {{ tripData.trip_overview.duration_days }} Days • 
          {{ tripData.trip_overview.budget_level }}
        </p>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        
        <!-- Left Column: Itinerary (Scrollable) -->
        <div class="lg:col-span-2 space-y-6 overflow-y-auto max-h-[calc(100vh-12rem)]">
          
          <!-- Trip Overview -->
          <div class="glass backdrop-blur-glass bg-base-200/50 rounded-lg p-6 border border-base-300/50">
            <h2 class="text-2xl font-bold text-base-content mb-4 font-cairo">📋 Trip Overview</h2>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <p class="text-sm text-base-content/60">Destination</p>
                <p class="font-semibold text-base-content">{{ tripData?.trip_overview?.destination }}</p>
              </div>
              <div>
                <p class="text-sm text-base-content/60">Duration</p>
                <p class="font-semibold text-base-content">{{ tripData?.trip_overview?.duration_days }} Days</p>
              </div>
              <div>
                <p class="text-sm text-base-content/60">Travelers</p>
                <p class="font-semibold text-base-content">{{ tripData?.trip_overview?.travelers }} People</p>
              </div>
              <div>
                <p class="text-sm text-base-content/60">Budget</p>
                <p class="font-semibold text-base-content capitalize">{{ tripData?.trip_overview?.budget_level }}</p>
              </div>
            </div>
          </div>

          <!-- Estimated Costs -->
          <div class="glass backdrop-blur-glass bg-success/10 rounded-lg p-6 border border-success/30">
            <h2 class="text-2xl font-bold text-base-content mb-4 font-cairo">💰 Estimated Costs</h2>
            <div class="space-y-3">
              <div class="flex justify-between">
                <span class="text-base-content">Accommodation</span>
                <span class="font-semibold">${{ tripData?.estimated_costs?.accommodation_total || 0 }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-base-content">Activities & Attractions</span>
                <span class="font-semibold">${{ tripData?.estimated_costs?.activities_total || 0 }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-base-content">Transportation</span>
                <span class="font-semibold">${{ tripData?.estimated_costs?.transportation_total || 0 }}</span>
              </div>
              <div class="border-t border-success/30 pt-3 flex justify-between text-lg">
                <span class="font-bold text-base-content">Grand Total</span>
                <span class="font-bold text-success">${{ tripData?.estimated_costs?.grand_total || 0 }}</span>
              </div>
              <div class="text-sm text-base-content/60 text-right">
                ${{ tripData?.estimated_costs?.per_person || 0 }} per person
              </div>
            </div>
          </div>

          <!-- Lodging -->
          <div v-if="tripData?.lodging_recommendations?.length" class="glass backdrop-blur-glass bg-base-200/50 rounded-lg p-6 border border-base-300/50">
            <h2 class="text-2xl font-bold text-base-content mb-4 font-cairo">🏨 Recommended Hotels</h2>
            <div class="space-y-4">
              <!-- Direct v-bind to HotelCard component -->
              <HotelCard
                v-for="hotelData in tripData.lodging_recommendations"
                :key="hotelData.hotel_id"
                :hotel="{
                  name: hotelData.name,
                  image: hotelData.images?.[0] || '/images/hotels/default.jpg',
                  price: hotelData.price_per_night,
                  location: hotelData.city,
                  rating: hotelData.rating,
                  reviews: hotelData.total_nights?.toString() || '0',
                  amenities: hotelData.amenities || []
                }"
              />
            </div>
          </div>

          <!-- Itinerary -->
          <div class="glass backdrop-blur-glass bg-base-200/50 rounded-lg p-6 border border-base-300/50">
            <h2 class="text-2xl font-bold text-base-content mb-4 font-cairo">📅 Day-by-Day Itinerary</h2>
            <div class="space-y-6">
              <div
                v-for="day in tripData?.itinerary"
                :key="day.day"
                class="border-l-4 border-primary pl-4"
              >
                <h3 class="text-xl font-bold text-base-content font-cairo">Day {{ day.day }}: {{ day.title }}</h3>
                <p class="text-sm text-base-content/60">{{ day.date }}</p>
                
                <div class="mt-4 space-y-3">
                  <div
                    v-for="(activity, idx) in day.activities"
                    :key="idx"
                    class="bg-base-300/30 rounded-lg p-3"
                  >
                    <div class="flex items-start justify-between">
                      <div class="flex-1">
                        <div class="flex items-center">
                          <span class="text-xs font-semibold text-primary bg-primary/20 px-2 py-1 rounded mr-2">
                            {{ activity.time }}
                          </span>
                          <span class="text-xs bg-base-300 text-base-content px-2 py-1 rounded">
                            {{ activity.activity_type }}
                          </span>
                        </div>
                        <h4 class="mt-2 font-semibold text-base-content">{{ activity.name }}</h4>
                        <p class="text-sm text-base-content/60 mt-1">{{ activity.description }}</p>
                        <div class="mt-2 flex items-center text-sm text-base-content/60">
                          <span>⏱️ {{ activity.duration_hours }}h</span>
                          <span class="mx-2">•</span>
                          <span>💵 ${{ activity.price }}</span>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Meals -->
                <div v-if="day.meals" class="mt-4 bg-warning/10 rounded-lg p-3 text-sm border border-warning/30">
                  <p class="font-semibold text-base-content mb-2">🍽️ Meals</p>
                  <p><strong>Breakfast:</strong> {{ day.meals.breakfast }}</p>
                  <p><strong>Lunch:</strong> {{ day.meals.lunch }}</p>
                  <p><strong>Dinner:</strong> {{ day.meals.dinner }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Tips -->
          <div v-if="tripData?.travel_tips?.length" class="glass backdrop-blur-glass bg-info/10 rounded-lg p-6 border border-info/30">
            <h2 class="text-2xl font-bold text-base-content mb-4 font-cairo">💡 Travel Tips</h2>
            <ul class="space-y-2">
              <li
                v-for="(tip, idx) in tripData.travel_tips"
                :key="idx"
                class="flex items-start"
              >
                <span class="text-info mr-2">✓</span>
                <span class="text-base-content">{{ tip }}</span>
              </li>
            </ul>
          </div>
        </div>

        <!-- Right Column: Map -->
        <div class="lg:col-span-1">
          <div class="sticky top-4">
            <div class="glass backdrop-blur-glass bg-base-200/50 rounded-lg p-4 border border-base-300/50">
              <h2 class="text-xl font-bold text-base-content mb-4 font-cairo">🗺️ Trip Map</h2>
              <div id="map" class="h-[600px] rounded-lg border border-base-300"></div>
              <div class="mt-4 text-xs text-base-content/60">
                <p class="flex items-center mb-2">
                  <span class="w-4 h-4 bg-primary rounded-full mr-2"></span>
                  Hotels
                </p>
                <p class="flex items-center mb-2">
                  <span class="w-4 h-4 bg-error rounded-full mr-2"></span>
                  Attractions
                </p>
                <p class="flex items-center">
                  <span class="w-4 h-4 bg-success rounded-full mr-2"></span>
                  Activities
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import HotelCard from '@/components/Hotels/HotelCard.vue';

const route = useRoute();
const router = useRouter();

const tripData = ref(null);
let map = null;
const markers = [];
const polyline = ref(null);

const goBack = () => {
  router.push({ name: 'AiCollectData' });
};

const initializeMap = () => {
  map = L.map('map').setView([26.8206, 30.8025], 6);

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap',
    maxZoom: 19
  }).addTo(map);

  delete L.Icon.Default.prototype._getIconUrl;
  L.Icon.Default.mergeOptions({
    iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
    iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png',
    shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
  });
};

const addMarkers = () => {
  if (!map || !tripData.value) return;

  const allCoordinates = [];

  tripData.value.lodging_recommendations?.forEach(hotel => {
    if (hotel.latitude && hotel.longitude) {
      const hotelIcon = L.divIcon({
        className: 'custom-marker',
        html: '<div style="background-color: #C86A41; width: 24px; height: 24px; border-radius: 50%; border: 2px solid white;"></div>',
        iconSize: [24, 24]
      });

      const marker = L.marker([hotel.latitude, hotel.longitude], { icon: hotelIcon })
        .addTo(map)
        .bindPopup(`<strong>🏨 ${hotel.name}</strong><br>${hotel.city}<br>$${hotel.price_per_night}/night`);
      
      markers.push(marker);
      allCoordinates.push([hotel.latitude, hotel.longitude]);
    }
  });

  tripData.value.itinerary?.forEach(day => {
    day.activities?.forEach(activity => {
      if (activity.latitude && activity.longitude) {
        const color = activity.activity_type === 'attraction' ? '#E45858' : '#36B37E';
        const activityIcon = L.divIcon({
          className: 'custom-marker',
          html: `<div style="background-color: ${color}; width: 20px; height: 20px; border-radius: 50%; border: 2px solid white;"></div>`,
          iconSize: [20, 20]
        });

        const marker = L.marker([activity.latitude, activity.longitude], { icon: activityIcon })
          .addTo(map)
          .bindPopup(`<strong>${activity.name}</strong><br>${activity.description?.substring(0, 50)}...<br>$${activity.price}`);
        
        markers.push(marker);
        allCoordinates.push([activity.latitude, activity.longitude]);
      }
    });
  });

  if (allCoordinates.length > 1) {
    polyline.value = L.polyline(allCoordinates, {
      color: '#C86A41',
      weight: 3,
      opacity: 0.6,
      dashArray: '10, 10'
    }).addTo(map);
  }

  if (allCoordinates.length > 0) {
    const bounds = L.latLngBounds(allCoordinates);
    map.fitBounds(bounds, { padding: [50, 50] });
  }
};

onMounted(async () => {
  try {
    if (localStorage.getItem('tripPlanData')) {
      tripData.value = JSON.parse(localStorage.getItem('tripPlanData'));
    } else {
      router.push({ name: 'AiCollectData' });
      return;
    }

    await nextTick();
    initializeMap();
    addMarkers();
  } catch (error) {
    console.error('Error:', error);
    router.push({ name: 'AiCollectData' });
  }
});
</script>

<style scoped>
.overflow-y-auto::-webkit-scrollbar {
  width: 8px;
}

.overflow-y-auto::-webkit-scrollbar-track {
  background: transparent;
}

.overflow-y-auto::-webkit-scrollbar-thumb {
  background: rgba(200, 106, 65, 0.4);
  border-radius: 4px;
}

.overflow-y-auto::-webkit-scrollbar-thumb:hover {
  background: rgba(200, 106, 65, 0.6);
}
</style>
