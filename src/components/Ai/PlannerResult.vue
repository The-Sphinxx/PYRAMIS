<template>
  <div class="min-h-screen bg-base-100">
    <!-- Header -->
    <header class="bg-base-200 border-b border-base-300 py-4">
      <div class="page-container">
        <div class="flex justify-between items-center">
          <div class="flex items-center gap-2">
            <svg class="w-8 h-8 text-primary" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"/>
            </svg>
            <span class="text-2xl font-bold text-primary font-cairo">PYRAMIS</span>
          </div>
          <div class="flex gap-4">
            <button class="btn btn-ghost btn-sm text-base-content hover:text-primary">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                <circle cx="12" cy="7" r="4"/>
              </svg>
              Login
            </button>
            <button class="btn btn-primary btn-sm">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/>
                <circle cx="9" cy="7" r="4"/>
                <path d="M22 21v-2a4 4 0 0 0-3-3.87"/>
                <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
              </svg>
              Sign Up
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <div class="page-container py-8">
      <!-- Hero Section -->
      <div class="glass-morphism p-8 mb-6 text-center">
        <h1 class="text-3xl font-bold text-primary mb-2 font-cairo">{{ tripData.title }}</h1>
        <p class="text-base-content opacity-80 font-cairo">{{ tripData.summary }}</p>
      </div>

      <!-- Main Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Left Column -->
        <div class="lg:col-span-2 space-y-6">
          
          <!-- Booking Details -->
          <div class="glass-morphism p-6">
            <h2 class="text-xl font-bold text-base-content mb-4 pb-2 border-b-4 border-primary inline-block font-cairo">
              Booking Details
            </h2>
            
            <div class="space-y-3 mt-4">
              <div v-for="day in [1, 2, 3, 4]" :key="day" class="bg-base-200 p-4 rounded-lg flex items-center gap-3">
                <svg class="w-5 h-5 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
                  <line x1="16" y1="2" x2="16" y2="6"/>
                  <line x1="8" y1="2" x2="8" y2="6"/>
                  <line x1="3" y1="10" x2="21" y2="10"/>
                </svg>
                <div>
                  <div class="text-sm text-base-content opacity-70 font-cairo">Duration</div>
                  <div class="text-base-content font-cairo">2 Passengers</div>
                </div>
              </div>
            </div>
          </div>

          <!-- Day Tabs & Content -->
          <div class="glass-morphism overflow-hidden">
            <div class="flex border-b border-base-300 overflow-x-auto">
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
                        <div v-if="activity.locations" class="text-sm text-base-content opacity-70 mb-2 font-cairo">
                          {{ activity.locations }}
                        </div>
                        <div v-if="activity.price > 0" class="text-sm text-accent font-cairo">
                          💰 {{ activity.price }} EGP
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Notes -->
                <div v-if="dayData.notes" class="mt-6 bg-base-200 p-4 rounded-lg">
                  <h3 class="font-bold text-base-content mb-2 font-cairo">Notes</h3>
                  <p class="text-sm text-base-content opacity-80 font-cairo">{{ dayData.notes }}</p>
                </div>

                <!-- Map Section -->
                <div class="mt-6 bg-accent bg-opacity-20 rounded-lg p-8 text-center">
                  <svg class="w-16 h-16 mx-auto mb-3 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"/>
                    <circle cx="12" cy="10" r="3"/>
                  </svg>
                  <div class="font-bold text-base-content text-lg font-cairo">Sharm El Shiekh</div>
                  <div class="text-sm text-base-content opacity-70 font-cairo">Sinai, Egypt</div>
                </div>
              </div>
            </div>
          </div>

          <!-- Historical Background -->
          <div class="glass-morphism p-6">
            <h2 class="text-xl font-bold text-base-content mb-4 pb-2 border-b-4 border-primary inline-block font-cairo">
              📜 Historical Background
            </h2>
            <p class="text-base-content opacity-90 leading-relaxed mt-4 font-cairo">{{ tripData.historicalBackground }}</p>
          </div>

          <!-- Lodging -->
          <div class="glass-morphism p-6">
            <h2 class="text-xl font-bold text-base-content mb-4 pb-2 border-b-4 border-primary inline-block flex items-center gap-2 font-cairo">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/>
                <polyline points="9 22 9 12 15 12 15 22"/>
              </svg>
              Accommodation
            </h2>
            <div v-for="(lodging, idx) in tripData.lodgingRecommendations" :key="idx" class="bg-base-200 p-4 rounded-lg mt-4">
              <div class="flex justify-between items-start mb-2">
                <h3 class="font-bold text-base-content text-lg font-cairo">{{ lodging.hotel.name }}</h3>
                <span class="badge badge-primary font-cairo">⭐ {{ lodging.hotel.rating }}</span>
              </div>
              <p class="text-base-content opacity-70 text-sm mb-2 flex items-center gap-1 font-cairo">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"/>
                  <circle cx="12" cy="10" r="3"/>
                </svg>
                {{ lodging.hotel.city }}
              </p>
              <p class="text-base-content font-bold mb-3 font-cairo">{{ lodging.hotel.pricePerNight }} per night</p>
              <div class="grid grid-cols-2 gap-2">
                <div v-for="(amenity, i) in lodging.hotel.amenities" :key="i" class="text-sm text-base-content opacity-80 font-cairo">• {{ amenity }}</div>
              </div>
            </div>
          </div>

          <!-- Car Rental -->
          <div class="glass-morphism p-6">
            <h2 class="text-xl font-bold text-base-content mb-4 pb-2 border-b-4 border-primary inline-block flex items-center gap-2 font-cairo">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path d="M19 17h2c.6 0 1-.4 1-1v-3c0-.9-.7-1.7-1.5-1.9C18.7 10.6 16 10 16 10s-1.3-1.4-2.2-2.3c-.5-.4-1.1-.7-1.8-.7H5c-.6 0-1.1.4-1.4.9l-1.4 2.9A3.7 3.7 0 0 0 2 12v4c0 .6.4 1 1 1h2"/>
                <circle cx="7" cy="17" r="2"/>
                <circle cx="17" cy="17" r="2"/>
              </svg>
              Transportation
            </h2>
            <div v-for="(carRec, idx) in tripData.carRecommendations" :key="idx" class="bg-base-200 p-4 rounded-lg mt-4">
              <h3 class="font-bold text-base-content text-lg mb-2 font-cairo">{{ carRec.car.name }}</h3>
              <p class="text-base-content opacity-80 text-sm mb-2 font-cairo">{{ carRec.car.overview }}</p>
              <div class="flex gap-4 text-sm text-base-content font-cairo">
                <span>🔧 {{ carRec.car.transmission }}</span>
                <span>⛽ {{ carRec.car.fuelType }}</span>
                <span class="font-bold">{{ carRec.car.price }} total</span>
              </div>
            </div>
          </div>

        </div>

        <!-- Right Sidebar -->
        <div class="lg:col-span-1 space-y-6">
          
          <!-- Overview Cards -->
          <div class="glass-morphism p-4">
            <div class="space-y-3">
              <div class="bg-base-200 p-3 rounded-lg flex items-center gap-2">
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
              
              <div class="bg-base-200 p-3 rounded-lg flex items-center gap-2">
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
              
              <div class="bg-base-200 p-3 rounded-lg flex items-center gap-2">
                <svg class="w-5 h-5 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <line x1="12" y1="1" x2="12" y2="23"/>
                  <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
                </svg>
                <div>
                  <div class="text-sm text-base-content opacity-70 font-cairo">Total Cost</div>
                  <div class="font-bold text-base-content font-cairo">${{ tripData.estimatedCosts.grandTotal }}</div>
                </div>
              </div>
              
              <div class="bg-base-200 p-3 rounded-lg flex items-center gap-2">
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
          <div class="glass-morphism p-4">
            <h3 class="text-lg font-bold text-base-content mb-3 pb-2 border-b-4 border-primary inline-block font-cairo">
              🏛️ Key Attractions
            </h3>
            <div class="space-y-3 mt-4">
              <div v-for="(attr, idx) in tripData.attractionRecommendations" :key="idx" class="bg-base-200 p-3 rounded-lg">
                <div class="flex justify-between items-start mb-1">
                  <h4 class="font-bold text-base-content text-sm font-cairo">{{ attr.attraction.name }}</h4>
                  <span class="badge badge-primary badge-sm font-cairo">⭐ {{ attr.attraction.rating }}</span>
                </div>
                <p class="text-xs text-base-content opacity-70 mb-1 font-cairo">{{ attr.attraction.city }}</p>
                <p class="text-xs text-base-content font-bold font-cairo">{{ attr.attraction.price }}</p>
              </div>
            </div>
          </div>

          <!-- Cost Breakdown -->
          <div class="glass-morphism p-4">
            <h3 class="text-lg font-bold text-base-content mb-3 pb-2 border-b-4 border-primary inline-block font-cairo">
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
              <div class="divider my-2"></div>
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
          <div class="glass-morphism p-4">
            <h3 class="text-lg font-bold text-base-content mb-3 pb-2 border-b-4 border-primary inline-block font-cairo">
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
export default {
  name: 'EgyptianTripPlanner',
  data() {
    return {
      selectedDay: 1,
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
  methods: {
    formatDate(dateStr) {
      const date = new Date(dateStr);
      return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
    }
  }
};
</script>

<style scoped>
/* DaisyUI + Tailwind CSS with custom theme */
</style>