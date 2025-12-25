<template>
  <div class="min-h-screen bg-base-100">
    <!-- Skeleton Loading State -->
    <div v-if="isLoading" class="page-container py-8">
      <div class="bg-base-100 backdrop-blur-sm p-8 mb-6 animate-pulse border border-base-300 rounded-xl shadow-xl">
        <div class="h-8 bg-base-200 rounded w-3/4 mx-auto mb-4"></div>
        <div class="h-4 bg-base-200 rounded w-1/2 mx-auto"></div>
      </div>
      
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div class="lg:col-span-2 space-y-6">
          <div class="bg-base-100 backdrop-blur-sm p-6 animate-pulse border border-base-300 rounded-xl shadow-xl">
            <div class="h-6 bg-base-200 rounded w-1/3 mb-4"></div>
            <div class="space-y-3">
              <div class="h-20 bg-base-200 rounded"></div>
              <div class="h-20 bg-base-200 rounded"></div>
            </div>
          </div>
        </div>
        <div class="lg:col-span-1">
          <div class="bg-base-100 backdrop-blur-sm p-4 animate-pulse border border-base-300 rounded-xl shadow-xl">
            <div class="space-y-3">
              <div class="h-16 bg-base-200 rounded"></div>
              <div class="h-16 bg-base-200 rounded"></div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div v-else class="page-container py-8">
      <!-- Hero Section -->
      <div class="bg-base-100 backdrop-blur-sm p-8 mb-6 text-center shadow-xl border border-base-300 rounded-xl my-8">
        <h1 class="text-4xl font-bold text-primary mb-3 font-cairo">{{ tripData.title }}</h1>
        <p class="text-base-content opacity-80 text-lg font-cairo">{{ tripData.summary }}</p>
      </div>

      <!-- Main Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Left Column -->
        <div class="lg:col-span-2 space-y-6 mb-8">
          
          <!-- Day Tabs & Content -->
          <div class="bg-base-100 backdrop-blur-sm overflow-hidden shadow-xl border border-base-300 rounded-xl">
            <div class="flex border-b border-base-300 overflow-x-auto bg-base-100">
              <button
                v-for="dayData in tripData.itinerary"
                :key="dayData.day"
                @click="selectedDay = dayData.day"
                :class="[
                  'flex-1 py-4 px-6 font-semibold transition-colors whitespace-nowrap font-cairo',
                  selectedDay === dayData.day
                    ? 'text-primary border-b-4 border-primary bg-primary bg-opacity-10'
                    : 'text-base-content opacity-70 hover:bg-base-200'
                ]"
              >
                DAY {{ String(dayData.day).padStart(2, '0') }}
              </button>
            </div>

            <!-- Day Content -->
            <div v-for="dayData in tripData.itinerary" :key="dayData.day">
              <div v-if="dayData.day === selectedDay" class="p-6">
                <h3 class="text-2xl font-bold mb-4 text-primary font-cairo">{{ formatDate(dayData.date) }}</h3>
                
                <div class="space-y-6">
                  <div v-for="(activity, idx) in dayData.activities" :key="idx" class="border-l-4 border-primary pl-4 pb-4">
                    <div class="flex items-start gap-3 mb-2">
                      <svg class="w-5 h-5 text-secondary mt-1 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <circle cx="12" cy="12" r="10"/>
                        <polyline points="12 6 12 12 16 14"/>
                      </svg>
                      <div class="flex-1">
                        <div class="font-bold text-base-content mb-1 font-cairo">
                          {{ activity.time }} - {{ activity.name }}
                        </div>
                        <div class="text-sm text-base-content opacity-70 mb-2 font-cairo">
                          Duration: {{ activity.duration }}
                        </div>
                        <div v-if="activity.price > 0" class="text-sm text-accent font-cairo">
                          💰 {{ activity.price }} {{ tripData.estimatedCosts.currency }}
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Meals -->
                <div class="mt-6 bg-base-200 border border-base-300 p-4 rounded-lg shadow-sm">
                  <h4 class="font-bold text-primary mb-3 font-cairo">🍽️ Meals for Day {{ dayData.day }}</h4>
                  <div class="space-y-2 text-sm">
                    <p class="font-cairo text-base-content"><strong class="text-primary">Breakfast:</strong> {{ dayData.meals.breakfast }}</p>
                    <p class="font-cairo text-base-content"><strong class="text-primary">Lunch:</strong> {{ dayData.meals.lunch }}</p>
                    <p class="font-cairo text-base-content"><strong class="text-primary">Dinner:</strong> {{ dayData.meals.dinner }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <!-- Map Section -->
          <div class="bg-base-100 backdrop-blur-sm p-4 shadow-xl border border-base-300 rounded-xl">
            <h3 class="text-lg font-bold text-primary mb-3 pb-2 border-b-2 border-primary font-cairo">
              🗺️ Trip Map
            </h3>
            <div class="mt-4 rounded-lg overflow-hidden shadow-lg">
              <div id="map" class="w-full h-80"></div>
            </div>
          </div>

          <!-- Historical Background -->
          <div class="bg-base-100 backdrop-blur-sm p-6 shadow-xl border border-base-300 rounded-xl">
            <h2 class="text-2xl font-bold text-primary mb-4 pb-3 border-b-4 border-primary inline-block font-cairo">
              📜 Historical Background
            </h2>
            <p class="text-base-content opacity-90 leading-relaxed mt-4 font-cairo">{{ tripData.historicalBackground }}</p>
          </div>

          <!-- Lodging -->
          <div class="bg-base-100 backdrop-blur-sm p-6 shadow-xl border border-base-300 rounded-xl">
            <h2 class="text-2xl font-bold text-primary mb-4 pb-3 border-b-4 border-primary flex items-center gap-2 font-cairo">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/>
                <polyline points="9 22 9 12 15 12 15 22"/>
              </svg>
              Accommodation Recommendations
            </h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
              <HotelCard 
                v-for="(lodging, idx) in tripData.lodgingRecommendations" 
                :key="idx" 
                :hotel="formatHotelForCard(lodging.hotel)"
              />
            </div>
          </div>

          <!-- Car Rental -->
          <div v-if="tripData.carRecommendations && tripData.carRecommendations.length > 0" class="bg-base-100 backdrop-blur-sm p-6 shadow-xl border border-base-300 rounded-xl">
            <h2 class="text-2xl font-bold text-primary mb-4 pb-3 border-b-4 border-primary flex items-center gap-2 font-cairo">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path d="M19 17h2c.6 0 1-.4 1-1v-3c0-.9-.7-1.7-1.5-1.9C18.7 10.6 16 10 16 10s-1.3-1.4-2.2-2.3c-.5-.4-1.1-.7-1.8-.7H5c-.6 0-1.1.4-1.4.9l-1.4 2.9A3.7 3.7 0 0 0 2 12v4c0 .6.4 1 1 1h2"/>
                <circle cx="7" cy="17" r="2"/>
                <circle cx="17" cy="17" r="2"/>
              </svg>
              Transportation Options
            </h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
              <CarCard 
                v-for="(carRec, idx) in tripData.carRecommendations" 
                :key="idx" 
                :car="formatCarForCard(carRec.car)"
              />
            </div>
          </div>

        </div>

        <!-- Right Sidebar -->
        <div class="lg:col-span-1 space-y-6">
          
          <!-- Overview Cards -->
          <div class="bg-base-100 backdrop-blur-sm p-4 shadow-xl border border-base-300 rounded-xl">
            <div class="space-y-3">
              <div class="bg-base-200 border border-base-300 p-3 rounded-lg flex items-center gap-2">
                <svg class="w-5 h-5 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
                  <line x1="16" y1="2" x2="16" y2="6"/>
                  <line x1="8" y1="2" x2="8" y2="6"/>
                  <line x1="3" y1="10" x2="21" y2="10"/>
                </svg>
                <div>
                  <div class="text-sm text-base-content opacity-70 font-cairo">Duration</div>
                  <div class="font-bold text-base-content font-cairo">{{ tripData.durationDays }} Days</div>
                </div>
              </div>
              
              <div class="bg-base-200 border border-base-300 p-3 rounded-lg flex items-center gap-2">
                <svg class="w-5 h-5 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
                  <circle cx="9" cy="7" r="4"/>
                  <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
                  <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
                </svg>
                <div>
                  <div class="text-sm text-base-content opacity-70 font-cairo">Travelers</div>
                  <div class="font-bold text-base-content font-cairo">{{ tripData.tripOverview.travelers }} People</div>
                </div>
              </div>
              
              <div class="bg-base-200 border border-base-300 p-3 rounded-lg flex items-center gap-2">
                <svg class="w-5 h-5 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <line x1="12" y1="1" x2="12" y2="23"/>
                  <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
                </svg>
                <div>
                  <div class="text-sm text-base-content opacity-70 font-cairo">Total Cost</div>
                  <div class="font-bold text-base-content font-cairo">${{ tripData.estimatedCosts.grandTotal }}</div>
                </div>
              </div>
              
              <div class="bg-base-200 border border-base-300 p-3 rounded-lg flex items-center gap-2">
                <svg class="w-5 h-5 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <circle cx="12" cy="12" r="10"/>
                  <polygon points="16.24 7.76 14.12 14.12 7.76 16.24 9.88 9.88 16.24 7.76"/>
                </svg>
                <div>
                  <div class="text-sm text-base-content opacity-70 font-cairo">Dates</div>
                  <div class="font-bold text-base-content text-xs font-cairo">{{ formatDate(tripData.tripOverview.startDate) }}</div>
                </div>
              </div>
            </div>
          </div>

          

          <!-- Attractions -->
          <div class="bg-base-100 backdrop-blur-sm p-4 shadow-xl border border-base-300 rounded-xl">
            <h3 class="text-lg font-bold text-primary mb-3 pb-2 border-b-4 border-primary inline-block font-cairo">
              🏛️ Key Attractions
            </h3>
            <div class="space-y-3 mt-4">
              <AttractionCard 
                v-for="(attr, idx) in tripData.attractionRecommendations.slice(0, 3)" 
                :key="idx" 
                v-bind="formatAttractionForCard(attr.attraction)"
              />
            </div>
          </div>

          <!-- Cost Breakdown -->
          <div class="bg-base-100 backdrop-blur-sm p-4 shadow-xl border border-base-300 rounded-xl">
            <h3 class="text-lg font-bold text-primary mb-3 pb-2 border-b-4 border-primary inline-block font-cairo">
              💰 Cost Breakdown
            </h3>
            <div class="space-y-2 text-sm mt-4">
              <div class="flex justify-between text-base-content font-cairo">
                <span>🏨 Accommodation</span>
                <span class="font-bold">${{ tripData.estimatedCosts.breakdown.stay }}</span>
              </div>
              <div class="flex justify-between text-base-content font-cairo">
                <span>🚗 Transportation</span>
                <span class="font-bold">${{ tripData.estimatedCosts.breakdown.car }}</span>
              </div>
              <div class="flex justify-between text-base-content font-cairo">
                <span>🎫 Activities</span>
                <span class="font-bold">${{ tripData.estimatedCosts.breakdown.activities }}</span>
              </div>
              <div class="divider my-2 border-base-300"></div>
              <div class="flex justify-between text-base-content font-bold text-base font-cairo">
                <span>Total</span>
                <span class="text-primary">${{ tripData.estimatedCosts.grandTotal }}</span>
              </div>
              <div class="text-center text-xs text-base-content opacity-70 mt-2 font-cairo">
                ${{ tripData.estimatedCosts.totalPerDay }} per day
              </div>
            </div>
          </div>

          <!-- Travel Tips -->
          <div class="bg-base-100 backdrop-blur-sm p-4 shadow-xl border border-base-300 rounded-xl">
            <h3 class="text-lg font-bold text-primary mb-3 pb-2 border-b-4 border-primary inline-block font-cairo">
              💡 Travel Tips
            </h3>
            <ul class="space-y-2 mt-4">
              <li v-for="(tip, idx) in tripData.travelTips" :key="idx" class="text-sm text-base-content opacity-90 flex gap-2 font-cairo">
                <span class="text-primary font-bold">▸</span>
                <span>{{ tip }}</span>
              </li>
            </ul>
          </div>

        </div>
      </div>
    </div>
  </div>
</template>

<script>
import HotelCard from '@/components/Hotels/HotelCard.vue';
import CarCard from '@/components/Cars/CarCard.vue';
import AttractionCard from '@/components/Attractions/AttractionCard.vue';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';

// Fix for default marker icons in Leaflet
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
  iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
});

export default {
  name: 'EgyptianTripPlanner',
  components: {
    HotelCard,
    CarCard,
    AttractionCard
  },
  data() {
    return {
      selectedDay: 1,
      isLoading: true,
      map: null,
      tripData: {
        title: "Cairo 4-Day Adventure",
        summary: "Discover the timeless wonders of Cairo over 4 days, from ancient Egyptian treasures at the Museum and Citadel to vibrant bazaars and luxurious Nile-side stays.",
        historicalBackground: "Cairo, founded in 969 AD by the Fatimid dynasty, is one of the world's oldest cities with over 7,000 years of history. Known as the 'City of a Thousand Minarets,' it blends Pharaonic, Coptic Christian, and Islamic heritage. Home to iconic sites like the Egyptian Museum's Tutankhamun treasures and the medieval Salah El-Din Citadel, Cairo was a hub of the Arab world during the Ottoman era and remains Egypt's pulsating cultural heart.",
        durationDays: 4,
        tripOverview: {
          startDate: "2025-12-25",
          endDate: "2025-12-28",
          travelers: 2
        },
        estimatedCosts: {
          totalPerDay: 339.5,
          grandTotal: 1358,
          breakdown: {
            stay: 885,
            car: 400,
            activities: 73
          },
          currency: "USD"
        },
        lodgingRecommendations: [
          {
            hotel: {
              name: "Four Seasons Hotel Cairo at Nile Plaza",
              city: "Cairo, Nile Plaza (Lat: 30.0385, Lon: 31.2305)",
              rating: 4.9,
              pricePerNight: "295 $",
              amenities: [
                "Rooftop Swimming Pool",
                "Full-Service Spa",
                "Fitness Center",
                "7 Restaurants and Bars",
                "Free Wi-Fi"
              ]
            }
          }
        ],
        attractionRecommendations: [
          {
            attraction: {
              name: "Salah El-Din Citadel",
              city: "Mokattam Hill, Cairo",
              rating: 4.7,
              price: "28 EGP"
            }
          },
          {
            attraction: {
              name: "Egyptian Museum",
              city: "Tahrir Square, Cairo",
              rating: 4.7,
              price: "45 EGP"
            }
          },
          {
            attraction: {
              name: "Khan El Khalili Bazaar",
              city: "Islamic Cairo",
              rating: 4.6,
              price: "0 EGP"
            }
          }
        ],
        carRecommendations: [
          {
            car: {
              name: "Toyota Corolla",
              overview: "Reliable compact sedan suitable for 4 passengers.",
              transmission: "automatic",
              fuelType: "Petrol",
              price: "100 $"
            }
          }
        ],
        itinerary: [
          {
            day: 1,
            date: "2025-12-25",
            activities: [
              { time: "09:00 AM", name: "Arrive in Cairo, Discover Khan el-Khalili & Egyptian Museum, Nile Tower", duration: "3h", price: 45, locations: "Khan el - Khalili → The Egyptian Museum in Cairo → Nile → Cairo Tower → Stay in Cairo" },
              { time: "10:00 AM", name: "Experience Pharaonic Village, Hanging Church, National Museum", duration: "2h", price: 0, locations: "Pharaonic Village → Hanging Church → The National Museum of Egyptian Civilization → Cairo Citadel → Stay in Cairo" },
              { time: "11:00 AM", name: "Morning Exploration of City Of The Dead and The Cave Church", duration: "2h", price: 0, locations: "City Of The Dead Cairo Egypt → The Cave Church → Stay in Cairo" },
              { time: "12:00 PM", name: "Marvel at Giza Necropolis & Grand Egyptian Museum, Return to Hotel", duration: "3h", price: 28, locations: "Giza Necropolis → Grand Egyptian Museum" }
            ],
            meals: {
              breakfast: "Ful Medames and fresh bread at hotel",
              lunch: "Koshari street food near museum",
              dinner: "Grilled kofta and molokhia at Nile-view restaurant"
            },
            notes: "Wear comfortable shoes for walking at Khan el-Khalili and museums. Bring water and sunscreen."
          },
          {
            day: 2,
            date: "2025-12-26",
            activities: [
              { time: "08:00 AM", name: "Salah El-Din Citadel tour", duration: "3h", price: 28, locations: "Salah El-Din Citadel → Muhammad Ali Mosque" },
              { time: "12:00 PM", name: "Muhammad Ali Mosque visit", duration: "1h", price: 0, locations: "Muhammad Ali Mosque → Panoramic Views" },
              { time: "03:00 PM", name: "Panoramic city views", duration: "1h", price: 0, locations: "Cairo Citadel → Stay in Cairo" }
            ],
            meals: {
              breakfast: "Egyptian breakfast platter with cheeses and olives",
              lunch: "Shawarma wraps from local vendor",
              dinner: "Seafood mezze at hotel rooftop"
            },
            notes: "The Citadel offers stunning panoramic views of Cairo. Best visited in the morning to avoid crowds."
          },
          {
            day: 3,
            date: "2025-12-27",
            activities: [
              { time: "10:00 AM", name: "Khan El Khalili Bazaar shopping", duration: "4h", price: 0, locations: "Khan El Khalili Bazaar → El Fishawi Cafe" },
              { time: "03:00 PM", name: "El Fishawi Cafe coffee break", duration: "1h", price: 5, locations: "El Fishawi Cafe → Islamic Cairo" },
              { time: "05:00 PM", name: "Drive around Islamic Cairo", duration: "2h", price: 0, locations: "Islamic Cairo → Stay in Cairo" }
            ],
            meals: {
              breakfast: "Honey pancakes and tea at hotel",
              lunch: "Falafel and tahini in bazaar",
              dinner: "Lamb mansaf with rice"
            },
            notes: "Bargain respectfully at the bazaar. Keep small bills for easier transactions."
          },
          {
            day: 4,
            date: "2025-12-28",
            activities: [
              { time: "09:00 AM", name: "Nile River cruise or walk", duration: "2h", price: 20, locations: "Nile River → Hotel" },
              { time: "11:00 AM", name: "Free time shopping or spa", duration: "3h", price: 0, locations: "Hotel → Shopping District" },
              { time: "03:00 PM", name: "Departure prep", duration: "2h", price: 0, locations: "Hotel → Airport" }
            ],
            meals: {
              breakfast: "Om Ali pastry dessert for breakfast",
              lunch: "Hummus and pita with salads",
              dinner: "Light mezze before departure"
            },
            notes: "Check out by 12 PM. Allow extra time for airport security during peak hours."
          }
        ],
        travelTips: [
          "Bargain hard at Khan El Khalili but stay polite.",
          "Visit museums early to beat crowds and heat.",
          "Drink bottled water only and use sunscreen.",
          "Hire a guide for Citadel history insights.",
          "Try street food from busy spots for freshness."
        ]
      }
    };
  },
  mounted() {
    this.loadTripData();
  },
  methods: {
    loadTripData() {
      // Get data from sessionStorage
      const storedData = sessionStorage.getItem('tripPlanData');
      
      if (storedData) {
        try {
          const parsedData = JSON.parse(storedData);
          this.tripData = parsedData;
          console.log('Loaded trip data:', this.tripData);
          // Keep storage for page refreshes - only clear when leaving the site
        } catch (error) {
          console.error('Error parsing trip data:', error);
        }
      } else {
        console.warn('No trip data found in sessionStorage - using default data');
      }
      
      this.isLoading = false;
      
      // Initialize map after data is loaded and DOM is ready
      this.$nextTick(() => {
        if (this.tripData && document.getElementById('map')) {
          this.initMap();
        }
      });
    },
    
    initMap() {
      // Safety check - don't initialize if no map container
      const mapContainer = document.getElementById('map');
      if (!mapContainer) {
        console.warn('Map container not found');
        return;
      }
      
      // Initialize OpenStreetMap
      if (this.map) {
        try {
          this.map.remove();
          this.map = null;
        } catch (error) {
          console.warn('Error removing old map:', error);
          this.map = null;
        }
      }
      
      // Get coordinates from first attraction or hotel, or default to Cairo
      let lat = 30.0444;
      let lon = 31.2357;
      let cityName = 'Cairo';
      
      // Try to get destination city from trip overview or first location
      if (this.tripData.tripOverview?.destination) {
        cityName = this.tripData.tripOverview.destination;
      } else if (this.tripData.attractionRecommendations && this.tripData.attractionRecommendations.length > 0) {
        const firstAttr = this.tripData.attractionRecommendations[0].attraction;
        cityName = firstAttr.city || cityName;
        if (firstAttr.latitude && firstAttr.longitude) {
          lat = firstAttr.latitude;
          lon = firstAttr.longitude;
        }
      } else if (this.tripData.lodgingRecommendations && this.tripData.lodgingRecommendations.length > 0) {
        const firstHotel = this.tripData.lodgingRecommendations[0].hotel;
        cityName = firstHotel.city || cityName;
        if (firstHotel.latitude && firstHotel.longitude) {
          lat = firstHotel.latitude;
          lon = firstHotel.longitude;
        }
      }
      
      this.map = L.map('map').setView([lat, lon], 12);
      
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors',
        maxZoom: 19
      }).addTo(this.map);
      
      // Custom icons
      const attractionIcon = L.icon({
        iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',
        shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41]
      });
      
      const hotelIcon = L.icon({
        iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-blue.png',
        shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41]
      });
      
      // Add city marker with custom styled popup
      const cityPopup = `
        <div style="min-width: 200px; font-family: 'Cairo', sans-serif;">
          <div style="background: linear-gradient(135deg, #c86a41 0%, #a5533a 100%); color: white; padding: 12px; margin: -15px -20px 10px -20px; border-radius: 8px 8px 0 0;">
            <h3 style="margin: 0; font-size: 18px; font-weight: bold;">📍 ${cityName}</h3>
            <p style="margin: 5px 0 0 0; font-size: 12px; opacity: 0.9;">Your Destination</p>
          </div>
          <div style="padding: 5px 0;">
            <p style="margin: 5px 0; font-size: 13px; color: #666;">
              <strong>${this.tripData.durationDays || 0} days</strong> of adventure awaits
            </p>
          </div>
        </div>
      `;
      
      L.marker([lat, lon])
        .addTo(this.map)
        .bindPopup(cityPopup, { maxWidth: 300 })
        .openPopup();
      
      // Add markers for attractions with enhanced popups
      if (this.tripData.attractionRecommendations) {
        this.tripData.attractionRecommendations.forEach(attr => {
          if (attr.attraction.latitude && attr.attraction.longitude) {
            const attraction = attr.attraction;
            const attractionPopup = `
              <div style="min-width: 250px; font-family: 'Cairo', sans-serif;">
                <div style="background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; padding: 12px; margin: -15px -20px 10px -20px; border-radius: 8px 8px 0 0;">
                  <h3 style="margin: 0; font-size: 16px; font-weight: bold;">🏛️ ${attraction.name}</h3>
                </div>
                ${attraction.images && attraction.images.length > 0 ? `
                  <img src="${attraction.images[0]}" alt="${attraction.name}" 
                       style="width: 100%; height: 120px; object-fit: cover; margin: 10px 0; border-radius: 6px;" 
                       onerror="this.style.display='none'"/>
                ` : ''}
                <div style="padding: 5px 0;">
                  <p style="margin: 5px 0; font-size: 13px; color: #666;">
                    <strong>📍 Location:</strong> ${attraction.city || 'N/A'}
                  </p>
                  ${attraction.rating ? `
                    <p style="margin: 5px 0; font-size: 13px; color: #666;">
                      <strong>⭐ Rating:</strong> ${attraction.rating}/5
                    </p>
                  ` : ''}
                  ${attraction.price ? `
                    <p style="margin: 5px 0; font-size: 13px; color: #666;">
                      <strong>💰 Price:</strong> ${attraction.price}
                    </p>
                  ` : ''}
                  ${attr.visitReason ? `
                    <p style="margin: 8px 0 5px 0; font-size: 12px; color: #888; font-style: italic; border-top: 1px solid #eee; padding-top: 8px;">
                      ${attr.visitReason}
                    </p>
                  ` : ''}
                </div>
              </div>
            `;
            
            L.marker([attraction.latitude, attraction.longitude], { icon: attractionIcon })
              .addTo(this.map)
              .bindPopup(attractionPopup, { maxWidth: 300 });
          }
        });
      }
      
      // Add markers for hotels with enhanced popups
      if (this.tripData.lodgingRecommendations) {
        this.tripData.lodgingRecommendations.forEach(lodging => {
          if (lodging.hotel.latitude && lodging.hotel.longitude) {
            const hotel = lodging.hotel;
            const hotelPopup = `
              <div style="min-width: 250px; font-family: 'Cairo', sans-serif;">
                <div style="background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); color: white; padding: 12px; margin: -15px -20px 10px -20px; border-radius: 8px 8px 0 0;">
                  <h3 style="margin: 0; font-size: 16px; font-weight: bold;">🏨 ${hotel.name}</h3>
                </div>
                ${hotel.images && hotel.images.length > 0 ? `
                  <img src="${hotel.images[0]}" alt="${hotel.name}" 
                       style="width: 100%; height: 120px; object-fit: cover; margin: 10px 0; border-radius: 6px;" 
                       onerror="this.style.display='none'"/>
                ` : ''}
                <div style="padding: 5px 0;">
                  <p style="margin: 5px 0; font-size: 13px; color: #666;">
                    <strong>📍 Location:</strong> ${hotel.city || 'N/A'}
                  </p>
                  ${hotel.rating ? `
                    <p style="margin: 5px 0; font-size: 13px; color: #666;">
                      <strong>⭐ Rating:</strong> ${hotel.rating}/5
                    </p>
                  ` : ''}
                  ${hotel.pricePerNight ? `
                    <p style="margin: 5px 0; font-size: 13px; color: #666;">
                      <strong>💰 Price:</strong> ${hotel.pricePerNight}/night
                    </p>
                  ` : ''}
                  ${hotel.amenities && hotel.amenities.length > 0 ? `
                    <p style="margin: 8px 0 5px 0; font-size: 12px; color: #666; border-top: 1px solid #eee; padding-top: 8px;">
                      <strong>✨ Amenities:</strong><br/>
                      ${hotel.amenities.slice(0, 3).join(' • ')}
                    </p>
                  ` : ''}
                  ${lodging.reason ? `
                    <p style="margin: 8px 0 5px 0; font-size: 12px; color: #888; font-style: italic; border-top: 1px solid #eee; padding-top: 8px;">
                      ${lodging.reason}
                    </p>
                  ` : ''}
                </div>
              </div>
            `;
            
            L.marker([hotel.latitude, hotel.longitude], { icon: hotelIcon })
              .addTo(this.map)
              .bindPopup(hotelPopup, { maxWidth: 300 });
          }
        });
      }
      
      // Fit map bounds to show all markers if multiple locations exist
      const allMarkers = [];
      if (this.tripData.attractionRecommendations) {
        this.tripData.attractionRecommendations.forEach(attr => {
          if (attr.attraction.latitude && attr.attraction.longitude) {
            allMarkers.push([attr.attraction.latitude, attr.attraction.longitude]);
          }
        });
      }
      if (this.tripData.lodgingRecommendations) {
        this.tripData.lodgingRecommendations.forEach(lodging => {
          if (lodging.hotel.latitude && lodging.hotel.longitude) {
            allMarkers.push([lodging.hotel.latitude, lodging.hotel.longitude]);
          }
        });
      }
      
      if (allMarkers.length > 1) {
        const bounds = L.latLngBounds(allMarkers);
        this.map.fitBounds(bounds, { padding: [50, 50] });
      }
    },
    
    formatDate(dateStr) {
      const date = new Date(dateStr);
      return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
    },
    
    formatHotelForCard(hotel) {
      return {
        id: hotel.id || 0,
        name: hotel.name || 'Unknown Hotel',
        location: hotel.city || 'Unknown',
        rating: hotel.rating || 0,
        reviews: hotel.reviewsCount || 0,
        price: typeof hotel.pricePerNight === 'string' ? hotel.pricePerNight.replace(/[^0-9.]/g, '') : (hotel.pricePerNight || 0),
        image: hotel.images && hotel.images.length > 0 ? hotel.images[0] : '/public/images/Hotels/default.jpg',
        amenities: hotel.amenities ? hotel.amenities.slice(0, 4) : []
      };
    },
    
    formatCarForCard(car) {
      return {
        id: car.id || 0,
        name: car.name || 'Unknown Car',
        overview: car.overview || car.description || 'No description available',
        transmission: car.transmission || 'Automatic',
        fuelType: car.fuelType || 'Petrol',
        price: typeof car.price === 'string' ? car.price.replace(/[^0-9.]/g, '') : (car.price || 0),
        images: car.images && car.images.length > 0 ? car.images : ['/images-car/placeholder.jpg'],
        reviews: {
          overallRating: car.reviewSummary?.overallRating || 0,
          totalReviews: car.reviewSummary?.totalReviews || 0
        }
      };
    },
    
    formatAttractionForCard(attraction) {
      return {
        id: attraction.id || 0,
        title: attraction.name || 'Unknown Attraction',
        image: attraction.images && attraction.images.length > 0 ? attraction.images[0] : '/images/Attractions/default.png',
        price: attraction.price || '0 EGP',
        location: attraction.city || 'Unknown',
        rating: attraction.rating || 0,
        reviews: attraction.reviews?.totalReviews || 0
      };
    }
  },
  beforeUnmount() {
    if (this.map) {
      try {
        this.map.remove();
        this.map = null;
      } catch (error) {
        console.warn('Error cleaning up map:', error);
      }
    }
    // Optionally clear sessionStorage when leaving the results page
    // sessionStorage.removeItem('tripPlanData');
  }
};
</script>

<style scoped>
.glass-morphism {
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.page-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 1rem;
}

.font-cairo {
  font-family: 'Cairo', sans-serif;
}

#map {
  min-height: 300px;
  border-radius: 0.5rem;
}
</style>