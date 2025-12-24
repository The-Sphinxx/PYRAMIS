<template>
  <div class="min-h-screen bg-gradient-to-br from-amber-50 to-orange-50">
    <!-- Header -->
    <header class="bg-white border-b-4 border-amber-500 py-4 shadow-md">
      <div class="max-w-7xl mx-auto px-4">
        <div class="flex justify-between items-center">
          <div class="flex items-center gap-2">
            <svg class="w-8 h-8 text-amber-600" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"/>
            </svg>
            <span class="text-2xl font-bold text-amber-600 font-cairo">PYRAMIS</span>
          </div>
          <div class="flex gap-4">
            <button class="px-4 py-2 text-gray-700 hover:text-amber-600 transition-colors">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                <circle cx="12" cy="7" r="4"/>
              </svg>
            </button>
            <button class="px-6 py-2 bg-amber-600 text-white rounded-lg hover:bg-amber-700 transition-colors font-semibold">
              Sign Up
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <div class="max-w-5xl mx-auto px-4 py-8">
      <!-- Hero Section -->
      <div class="glass-morphism rounded-2xl p-8 mb-6 text-center shadow-xl border border-amber-200">
        <h1 class="text-4xl font-bold text-amber-600 mb-3 font-cairo">
          Plan Your Trip to Egypt
        </h1>
        <p class="text-gray-600 text-lg font-cairo">
          Start your adventure in the land of the Pharaohs - Design your perfect journey
        </p>
      </div>

      <!-- Progress Steps -->
      <div class="flex justify-between mb-8 bg-white rounded-xl p-6 shadow-lg">
        <div v-for="step in [1, 2, 3]" :key="step" class="flex items-center flex-1">
          <div :class="[
            'flex items-center justify-center w-12 h-12 rounded-full font-bold transition-all',
            currentStep >= step ? 'bg-amber-600 text-white' : 'bg-gray-200 text-gray-500'
          ]">
            {{ step }}
          </div>
          <div v-if="step < 3" :class="[
            'flex-1 h-1 mx-2',
            currentStep > step ? 'bg-amber-600' : 'bg-gray-200'
          ]"></div>
        </div>
      </div>

      <!-- Form Steps -->
      <div class="glass-morphism rounded-2xl p-8 shadow-xl border border-amber-200">
        
        <!-- Step 1: Basic Info -->
        <div v-if="currentStep === 1" class="space-y-6">
          <h2 class="text-2xl font-bold text-gray-800 mb-6 pb-3 border-b-4 border-amber-600 inline-block font-cairo">
            📍 Basic Information
          </h2>

          <div>
            <label class="block text-gray-700 font-semibold mb-2 font-cairo">
              Choose Your Destination
            </label>
            <select 
              v-model="formData.destination"
              class="w-full p-4 border-2 border-amber-300 rounded-xl focus:border-amber-600 focus:outline-none text-lg font-cairo"
            >
              <option v-for="city in egyptianCities" :key="city" :value="city">{{ city }}</option>
            </select>
          </div>

          <div>
            <label class="block text-gray-700 font-semibold mb-2 font-cairo">
              Trip Title
            </label>
            <input 
              type="text"
              v-model="formData.tripTitle"
              placeholder="Example: Cairo 4-Day Adventure"
              class="w-full p-4 border-2 border-amber-300 rounded-xl focus:border-amber-600 focus:outline-none text-lg font-cairo"
            />
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-gray-700 font-semibold mb-2 font-cairo">
                Start Date
              </label>
              <input 
                type="date"
                v-model="formData.startDate"
                class="w-full p-4 border-2 border-amber-300 rounded-xl focus:border-amber-600 focus:outline-none"
              />
            </div>
            <div>
              <label class="block text-gray-700 font-semibold mb-2 font-cairo">
                End Date
              </label>
              <input 
                type="date"
                v-model="formData.endDate"
                class="w-full p-4 border-2 border-amber-300 rounded-xl focus:border-amber-600 focus:outline-none"
              />
            </div>
          </div>

          <div v-if="calculateDays() > 0" class="bg-amber-50 border-2 border-amber-400 rounded-xl p-4 text-center">
            <p class="text-amber-800 font-bold text-xl font-cairo">
              ⏱️ Trip Duration: {{ calculateDays() }} Days
            </p>
          </div>

          <div>
            <label class="block text-gray-700 font-semibold mb-2 font-cairo">
              Number of Travelers
            </label>
            <div class="flex items-center gap-4 bg-gray-50 p-4 rounded-xl">
              <button 
                @click="formData.travelers = Math.max(1, formData.travelers - 1)"
                class="w-12 h-12 bg-amber-600 text-white rounded-lg font-bold hover:bg-amber-700 transition-all"
              >
                -
              </button>
              <span class="text-3xl font-bold text-gray-800 flex-1 text-center">
                {{ formData.travelers }}
              </span>
              <button 
                @click="formData.travelers++"
                class="w-12 h-12 bg-amber-600 text-white rounded-lg font-bold hover:bg-amber-700 transition-all"
              >
                +
              </button>
            </div>
          </div>
        </div>

        <!-- Step 2: Preferences -->
        <div v-if="currentStep === 2" class="space-y-6">
          <h2 class="text-2xl font-bold text-gray-800 mb-6 pb-3 border-b-4 border-amber-600 inline-block font-cairo">
            ⚙️ Your Preferences
          </h2>

          <div>
            <label class="block text-gray-700 font-semibold mb-3 font-cairo">
              Your Interests (Select one or more)
            </label>
            <div class="grid grid-cols-2 md:grid-cols-4 gap-3">
              <button
                v-for="option in interestOptions"
                :key="option.id"
                @click="toggleInterest(option.id)"
                :class="[
                  'p-4 rounded-xl border-2 transition-all text-center',
                  formData.interests.includes(option.id)
                    ? 'bg-amber-600 text-white border-amber-600'
                    : 'bg-white border-amber-300 text-gray-700 hover:border-amber-500'
                ]"
              >
                <div class="text-3xl mb-2">{{ option.icon }}</div>
                <div class="text-sm font-semibold font-cairo">{{ option.label }}</div>
              </button>
            </div>
          </div>

          <div>
            <label class="block text-gray-700 font-semibold mb-3 font-cairo">
              Accommodation Type
            </label>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <button
                v-for="option in accommodationOptions"
                :key="option.value"
                @click="formData.accommodationType = option.value"
                :class="[
                  'p-6 rounded-xl border-2 transition-all',
                  formData.accommodationType === option.value
                    ? 'bg-amber-600 text-white border-amber-600'
                    : 'bg-white border-amber-300 text-gray-700 hover:border-amber-500'
                ]"
              >
                <div class="text-4xl mb-2">{{ option.icon }}</div>
                <div class="font-bold text-lg mb-1 font-cairo">{{ option.label }}</div>
                <div class="text-sm opacity-80 font-cairo">{{ option.desc }}</div>
              </button>
            </div>
          </div>

          <div>
            <label class="block text-gray-700 font-semibold mb-3 font-cairo">
              Transportation
            </label>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <button
                v-for="option in transportationOptions"
                :key="option.value"
                @click="formData.transportation = option.value"
                :class="[
                  'p-6 rounded-xl border-2 transition-all',
                  formData.transportation === option.value
                    ? 'bg-amber-600 text-white border-amber-600'
                    : 'bg-white border-amber-300 text-gray-700 hover:border-amber-500'
                ]"
              >
                <div class="text-4xl mb-2">{{ option.icon }}</div>
                <div class="font-bold text-lg mb-1 font-cairo">{{ option.label }}</div>
                <div class="text-sm opacity-80 font-cairo">{{ option.desc }}</div>
              </button>
            </div>
          </div>
        </div>

        <!-- Step 3: Budget & Review -->
        <div v-if="currentStep === 3" class="space-y-6">
          <h2 class="text-2xl font-bold text-gray-800 mb-6 pb-3 border-b-4 border-amber-600 inline-block font-cairo">
            💰 Budget & Review
          </h2>

          <div>
            <label class="block text-gray-700 font-semibold mb-3 font-cairo">
              Expected Budget
            </label>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <button
                v-for="option in budgetOptions"
                :key="option.value"
                @click="formData.budget = option.value"
                :class="[
                  'p-6 rounded-xl border-2 transition-all',
                  formData.budget === option.value
                    ? 'bg-amber-600 text-white border-amber-600'
                    : 'bg-white border-amber-300 text-gray-700 hover:border-amber-500'
                ]"
              >
                <div class="text-4xl mb-2">{{ option.icon }}</div>
                <div class="font-bold text-lg mb-1 font-cairo">{{ option.label }}</div>
                <div class="text-sm font-cairo">{{ option.range }}</div>
              </button>
            </div>
          </div>

          <div class="bg-gradient-to-br from-amber-50 to-orange-50 rounded-xl p-6 border-2 border-amber-300">
            <h3 class="font-bold text-xl text-gray-800 mb-4 font-cairo">
              📋 Your Trip Summary
            </h3>
            <div class="space-y-3">
              <div class="flex justify-between items-center p-3 bg-white rounded-lg">
                <span class="text-gray-600 font-cairo">Destination:</span>
                <span class="font-bold text-gray-800 font-cairo">{{ formData.destination }}</span>
              </div>
              <div class="flex justify-between items-center p-3 bg-white rounded-lg">
                <span class="text-gray-600 font-cairo">Duration:</span>
                <span class="font-bold text-gray-800 font-cairo">{{ calculateDays() }} Days</span>
              </div>
              <div class="flex justify-between items-center p-3 bg-white rounded-lg">
                <span class="text-gray-600 font-cairo">Travelers:</span>
                <span class="font-bold text-gray-800 font-cairo">{{ formData.travelers }} People</span>
              </div>
              <div class="flex justify-between items-center p-3 bg-white rounded-lg">
                <span class="text-gray-600 font-cairo">Accommodation:</span>
                <span class="font-bold text-gray-800 font-cairo">{{ getAccommodationLabel() }}</span>
              </div>
              <div class="flex justify-between items-center p-3 bg-white rounded-lg">
                <span class="text-gray-600 font-cairo">Interests:</span>
                <span class="font-bold text-gray-800 font-cairo">{{ formData.interests.length }} Selected</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Navigation Buttons -->
        <div class="flex justify-between mt-8 pt-6 border-t-2 border-gray-200">
          <button 
            v-if="currentStep > 1"
            @click="currentStep--"
            class="px-8 py-3 bg-gray-300 text-gray-700 rounded-xl font-semibold hover:bg-gray-400 transition-all flex items-center gap-2 font-cairo"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
            </svg>
            Previous
          </button>
          
          <div class="flex-1"></div>
          
          <button 
            v-if="currentStep < 3"
            @click="currentStep++"
            class="px-8 py-3 bg-amber-600 text-white rounded-xl font-semibold hover:bg-amber-700 transition-all flex items-center gap-2 font-cairo"
          >
            Next
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
            </svg>
          </button>

          <button 
            v-else
            @click="handleSubmit"
            class="px-8 py-3 bg-green-600 text-white rounded-xl font-semibold hover:bg-green-700 transition-all flex items-center gap-2 text-lg font-cairo"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
            </svg>
            Plan My Trip Now!
          </button>
        </div>
      </div>
    </div>

    <!-- Footer -->
    <footer class="bg-white border-t-4 border-amber-500 py-6 mt-12">
      <div class="max-w-7xl mx-auto px-4 text-center">
        <p class="text-gray-600 font-cairo">
          🏛️ Discover the beauty of Egypt with PYRAMIS - Your guide to an unforgettable journey
        </p>
      </div>
    </footer>
  </div>
</template>

<script>
export default {
  name: 'TripPlanningForm',
  data() {
    return {
      currentStep: 1,
      formData: {
        destination: 'Cairo',
        tripTitle: '',
        startDate: '',
        endDate: '',
        travelers: 2,
        budget: 'moderate',
        interests: [],
        accommodationType: 'luxury',
        transportation: 'car-rental'
      },
      egyptianCities: [
        'Cairo', 'Alexandria', 'Luxor', 'Aswan', 'Sharm El Sheikh', 
        'Hurghada', 'Dahab', 'Marsa Alam', 'Siwa Oasis'
      ],
      interestOptions: [
        { id: 'history', label: 'Historical Sites', icon: '🏛️' },
        { id: 'beach', label: 'Beach & Water', icon: '🏖️' },
        { id: 'food', label: 'Food & Cuisine', icon: '🍽️' },
        { id: 'shopping', label: 'Shopping & Bazaars', icon: '🛍️' },
        { id: 'adventure', label: 'Adventure Activities', icon: '🎿' },
        { id: 'culture', label: 'Culture & Arts', icon: '🎭' },
        { id: 'relaxation', label: 'Relaxation & Spa', icon: '🧘' },
        { id: 'nightlife', label: 'Nightlife', icon: '🌃' }
      ],
      accommodationOptions: [
        { value: 'budget', label: 'Budget', icon: '💰', desc: 'Hostels & Simple Hotels' },
        { value: 'moderate', label: 'Moderate', icon: '🏨', desc: '3-4 Star Hotels' },
        { value: 'luxury', label: 'Luxury', icon: '⭐', desc: '5 Star Hotels' }
      ],
      transportationOptions: [
        { value: 'car-rental', label: 'Car Rental', icon: '🚗', desc: 'Freedom to Explore' },
        { value: 'driver', label: 'Private Driver', icon: '👨‍✈️', desc: 'Comfort & Luxury' }
      ],
      budgetOptions: [
        { value: 'budget', label: 'Budget', range: '$500-1000', icon: '💵' },
        { value: 'moderate', label: 'Moderate', range: '$1000-2000', icon: '💸' },
        { value: 'luxury', label: 'Luxury', range: '$2000+', icon: '💎' }
      ]
    };
  },
  methods: {
    toggleInterest(interest) {
      const index = this.formData.interests.indexOf(interest);
      if (index > -1) {
        this.formData.interests.splice(index, 1);
      } else {
        this.formData.interests.push(interest);
      }
    },
    calculateDays() {
      if (this.formData.startDate && this.formData.endDate) {
        const start = new Date(this.formData.startDate);
        const end = new Date(this.formData.endDate);
        const days = Math.ceil((end - start) / (1000 * 60 * 60 * 24));
        return days > 0 ? days : 0;
      }
      return 0;
    },
    getAccommodationLabel() {
      const option = this.accommodationOptions.find(opt => opt.value === this.formData.accommodationType);
      return option ? option.label : '';
    },
    handleSubmit() {
      console.log('Trip Data:', this.formData);
      alert('🎉 Your trip is ready! You will be redirected to the trip details page...');
    }
  }
};
</script>

<style scoped>
.glass-morphism {
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);
}

.font-cairo {
  font-family: 'Cairo', sans-serif;
}
</style>