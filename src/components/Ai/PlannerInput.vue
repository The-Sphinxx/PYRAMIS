<template>
  <div class="min-h-screen bg-base-100">
    <!-- Loading Skeleton Overlay -->
    <div v-if="isGeneratingPlan" class="fixed inset-0 z-50 bg-base-100/95 backdrop-blur-sm flex items-center justify-center">
      <div class="page-container py-8 w-full">
        <div class="glass-morphism rounded-xl p-8 mb-6 text-center shadow-xl border border-base-300 animate-pulse">
          <div class="h-10 bg-base-200 rounded w-3/4 mx-auto mb-4"></div>
          <div class="h-6 bg-base-200 rounded w-1/2 mx-auto"></div>
        </div>
        
        <div class="flex justify-center items-center mb-8 bg-base-100 rounded-xl p-6 shadow-lg border border-base-300 animate-pulse">
          <div class="flex items-center gap-4">
            <div v-for="step in [1, 2, 3]" :key="step" class="flex items-center">
              <div class="w-12 h-12 bg-base-200 rounded-full"></div>
              <div v-if="step < 3" class="w-16 h-1 mx-2 bg-base-200"></div>
            </div>
          </div>
        </div>

        <div class="glass-morphism rounded-xl p-8 shadow-xl border border-base-300 animate-pulse">
          <div class="space-y-6">
            <div class="h-8 bg-base-200 rounded w-1/3 mb-6"></div>
            <div class="h-12 bg-base-200 rounded w-full"></div>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="h-12 bg-base-200 rounded"></div>
              <div class="h-12 bg-base-200 rounded"></div>
            </div>
            <div class="h-16 bg-base-200 rounded w-full"></div>
          </div>
          
          <div class="mt-8 text-center">
            <div class="flex items-center justify-center gap-3 mb-4">
              <i class="fas fa-spinner fa-spin text-4xl text-primary"></i>
            </div>
            <p class="text-xl font-bold text-primary font-cairo mb-2">Planning Your Perfect Trip...</p>
            <p class="text-base-content opacity-70 font-cairo">Our AI is crafting a personalized itinerary just for you</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="page-container py-8">
      <!-- Hero Section -->
      <div class="glass-morphism rounded-xl p-8 mb-6 text-center shadow-xl border border-base-300">
        <h1 class="text-4xl font-bold text-primary mb-3 font-cairo">
          Plan Your Trip to Egypt
        </h1>
        <p class="text-base-content opacity-80 text-lg font-cairo">
          Start your adventure in the land of the Pharaohs - Design your perfect journey
        </p>
      </div>

      <!-- Progress Steps - Centered -->
      <div class="flex justify-center items-center mb-8 bg-base-100 rounded-xl p-6 shadow-lg border border-base-300">
        <div class="flex items-center gap-4">
          <div v-for="step in [1, 2, 3]" :key="step" class="flex items-center">
            <div :class="[
              'flex items-center justify-center w-12 h-12 rounded-full font-bold transition-all',
              currentStep >= step ? 'bg-primary text-primary-content' : 'bg-base-300 text-base-content opacity-50'
            ]">
              {{ step }}
            </div>
            <div v-if="step < 3" :class="[
              'w-16 h-1 mx-2',
              currentStep > step ? 'bg-primary' : 'bg-base-300'
            ]"></div>
          </div>
        </div>
      </div>

      <!-- Form Steps -->
      <div class="glass-morphism rounded-xl p-8 shadow-xl border border-base-300">
        
        <!-- Step 1: Basic Info -->
        <div v-if="currentStep === 1" class="space-y-6">
          <h2 class="text-2xl font-bold text-base-content mb-6 pb-3 border-b-4 border-primary font-cairo flex items-center gap-2">
            <i class="fas fa-map-marker-alt text-primary"></i>
            Basic Information
          </h2>

          <div>
            <label class="block text-base-content font-semibold mb-2 font-cairo">
              Choose Your Destination
            </label>
            <select 
              v-model="formData.headingTo"
              :disabled="isLoadingDestinations"
              class="select select-bordered w-full font-cairo"
            >
              <option v-if="isLoadingDestinations" value="">Loading destinations...</option>
              <option 
                v-for="city in egyptianCities" 
                :key="city.name || city" 
                :value="city.name || city"
              >
                {{ city.displayName || city }}
              </option>
            </select>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <DatePicker
              v-model="formData.startDate"
              label="Start Date"
              placeholder="Select start date"
              :min-date="new Date()"
            />
            <DatePicker
              v-model="formData.endDate"
              label="End Date"
              placeholder="Select end date"
              :min-date="formData.startDate || new Date()"
            />
          </div>

          <div v-if="calculateDays() > 0" class="bg-primary bg-opacity-10 border-2 border-primary rounded-xl p-4 text-center">
            <p class="text-primary font-bold text-xl font-cairo flex items-center justify-center gap-2">
              <i class="fas fa-clock"></i>
              Trip Duration: {{ calculateDays() }} Days
            </p>
          </div>

          <div>
            <label class="block text-base-content font-semibold mb-2 font-cairo">
              Number of Travelers
            </label>
            <div class="flex items-center gap-4 bg-base-200 p-4 rounded-xl">
              <button 
                @click="formData.travelers = Math.max(1, formData.travelers - 1)"
                class="btn btn-primary btn-circle"
              >
                <i class="fas fa-minus"></i>
              </button>
              <span class="text-3xl font-bold text-base-content flex-1 text-center">
                {{ formData.travelers }}
              </span>
              <button 
                @click="formData.travelers++"
                class="btn btn-primary btn-circle"
              >
                <i class="fas fa-plus"></i>
              </button>
            </div>
          </div>
        </div>

        <!-- Step 2: Preferences -->
        <div v-if="currentStep === 2" class="space-y-6">
          <h2 class="text-2xl font-bold text-base-content mb-6 pb-3 border-b-4 border-primary font-cairo flex items-center gap-2">
            <i class="fas fa-sliders-h text-primary"></i>
            Your Preferences
          </h2>

          <div>
            <label class="block text-base-content font-semibold mb-3 font-cairo">
              Who are you traveling with?
            </label>
            <div class="grid grid-cols-2 md:grid-cols-4 gap-3">
              <button
                v-for="option in withWhomOptions"
                :key="option.value"
                @click="formData.withWhom = option.value"
                :class="[
                  'p-4 rounded-xl border-2 transition-all text-center',
                  formData.withWhom === option.value
                    ? 'bg-primary text-primary-content border-primary'
                    : 'bg-base-100 border-base-300 text-base-content hover:border-primary'
                ]"
              >
                <div class="text-3xl mb-2">
                  <i :class="option.icon"></i>
                </div>
                <div class="text-sm font-semibold font-cairo">{{ option.label }}</div>
              </button>
            </div>
          </div>

          <div>
            <label class="block text-base-content font-semibold mb-3 font-cairo">
              Your Travel Style (Select one or more)
            </label>
            <div class="grid grid-cols-2 md:grid-cols-5 gap-3">
              <button
                v-for="option in travelStyleOptions"
                :key="option.id"
                @click="toggleTravelStyle(option.id)"
                :class="[
                  'p-4 rounded-xl border-2 transition-all text-center',
                  formData.travelStyle.includes(option.id)
                    ? 'bg-primary text-primary-content border-primary'
                    : 'bg-base-100 border-base-300 text-base-content hover:border-primary'
                ]"
              >
                <div class="text-3xl mb-2">
                  <i :class="option.icon"></i>
                </div>
                <div class="text-sm font-semibold font-cairo">{{ option.label }}</div>
              </button>
            </div>
          </div>

          <div>
            <label class="block text-base-content font-semibold mb-3 font-cairo">
              Travel Pace
            </label>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <button
                v-for="option in travelPaceOptions"
                :key="option.value"
                @click="formData.travelPace = option.value"
                :class="[
                  'p-6 rounded-xl border-2 transition-all',
                  formData.travelPace === option.value
                    ? 'bg-primary text-primary-content border-primary'
                    : 'bg-base-100 border-base-300 text-base-content hover:border-primary'
                ]"
              >
                <div class="text-4xl mb-2">
                  <i :class="option.icon"></i>
                </div>
                <div class="font-bold text-lg mb-1 font-cairo">{{ option.label }}</div>
                <div class="text-sm opacity-80 font-cairo">{{ option.desc }}</div>
              </button>
            </div>
          </div>

        </div>

        <!-- Step 3: Budget & Review -->
        <div v-if="currentStep === 3" class="space-y-6">
          <h2 class="text-2xl font-bold text-base-content mb-6 pb-3 border-b-4 border-primary font-cairo flex items-center gap-2">
            <i class="fas fa-clipboard-check text-primary"></i>
            Final Details & Review
          </h2>

          <div>
            <label class="block text-base-content font-semibold mb-3 font-cairo">
              Accommodation Type
            </label>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <button
                v-for="option in accommodationOptions"
                :key="option.value"
                @click="formData.accommodation = option.value"
                :class="[
                  'p-6 rounded-xl border-2 transition-all',
                  formData.accommodation === option.value
                    ? 'bg-primary text-primary-content border-primary'
                    : 'bg-base-100 border-base-300 text-base-content hover:border-primary'
                ]"
              >
                <div class="text-4xl mb-2">
                  <i :class="option.icon"></i>
                </div>
                <div class="font-bold text-lg mb-1 font-cairo">{{ option.label }}</div>
                <div class="text-sm opacity-80 font-cairo">{{ option.desc }}</div>
              </button>
            </div>
          </div>

          <div>
            <label class="block text-base-content font-semibold mb-3 font-cairo">
              Daily Rhythm Preference
            </label>
            <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
              <button
                v-for="option in daysRhythmOptions"
                :key="option.value"
                @click="formData.daysRhythm = option.value"
                :class="[
                  'p-6 rounded-xl border-2 transition-all',
                  formData.daysRhythm === option.value
                    ? 'bg-primary text-primary-content border-primary'
                    : 'bg-base-100 border-base-300 text-base-content hover:border-primary'
                ]"
              >
                <div class="text-4xl mb-2">
                  <i :class="option.icon"></i>
                </div>
                <div class="font-bold text-lg mb-1 font-cairo">{{ option.label }}</div>
                <div class="text-sm opacity-80 font-cairo">{{ option.desc }}</div>
              </button>
            </div>
          </div>

          <div>
            <label class="block text-base-content font-semibold mb-3 font-cairo">
              Special Requests or Additional Needs
            </label>
            <textarea
              v-model="formData.otherNeeds"
              rows="4"
              placeholder="Tell us about any specific interests, accessibility needs, dietary restrictions, or anything else we should know..."
              class="textarea textarea-bordered w-full font-cairo"
            ></textarea>
          </div>

          <div class="bg-base-200 rounded-xl p-6 border-2 border-base-300">
            <h3 class="font-bold text-xl text-base-content mb-4 font-cairo flex items-center gap-2">
              <i class="fas fa-clipboard-list text-primary"></i>
              Your Trip Summary
            </h3>
            <div class="space-y-3">
              <div class="flex justify-between items-center p-3 bg-base-100 rounded-lg">
                <span class="text-base-content opacity-70 font-cairo">Destination:</span>
                <span class="font-bold text-base-content font-cairo">{{ formData.headingTo }}</span>
              </div>
              <div class="flex justify-between items-center p-3 bg-base-100 rounded-lg">
                <span class="text-base-content opacity-70 font-cairo">Duration:</span>
                <span class="font-bold text-base-content font-cairo">{{ calculateDays() }} Days</span>
              </div>
              <div class="flex justify-between items-center p-3 bg-base-100 rounded-lg">
                <span class="text-base-content opacity-70 font-cairo">Travelers:</span>
                <span class="font-bold text-base-content font-cairo">{{ formData.travelers }} People</span>
              </div>
              <div class="flex justify-between items-center p-3 bg-base-100 rounded-lg">
                <span class="text-base-content opacity-70 font-cairo">Traveling With:</span>
                <span class="font-bold text-base-content font-cairo">{{ getWithWhomLabel() }}</span>
              </div>
              <div class="flex justify-between items-center p-3 bg-base-100 rounded-lg">
                <span class="text-base-content opacity-70 font-cairo">Accommodation:</span>
                <span class="font-bold text-base-content font-cairo">{{ getAccommodationLabel() }}</span>
              </div>
              <div class="flex justify-between items-center p-3 bg-base-100 rounded-lg">
                <span class="text-base-content opacity-70 font-cairo">Travel Styles:</span>
                <span class="font-bold text-base-content font-cairo">{{ formData.travelStyle.length }} Selected</span>
              </div>
              <div class="flex justify-between items-center p-3 bg-base-100 rounded-lg">
                <span class="text-base-content opacity-70 font-cairo">Pace:</span>
                <span class="font-bold text-base-content font-cairo">{{ formData.travelPace }}</span>
              </div>
              <div class="flex justify-between items-center p-3 bg-base-100 rounded-lg">
                <span class="text-base-content opacity-70 font-cairo">Daily Rhythm:</span>
                <span class="font-bold text-base-content font-cairo">{{ formData.daysRhythm }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Navigation Buttons -->
        <div class="flex justify-between mt-8 pt-6 border-t-2 border-base-300">
          <button 
            v-if="currentStep > 1"
            @click="currentStep--"
            class="btn btn-ghost gap-2 font-cairo"
          >
            <i class="fas fa-arrow-left"></i>
            Previous
          </button>
          
          <div class="flex-1"></div>
          
          <button 
            v-if="currentStep < 3"
            @click="nextStep"
            :disabled="!canProceedToNextStep"
            class="btn btn-primary gap-2 font-cairo"
            :class="{ 'btn-disabled': !canProceedToNextStep }"
          >
            Next
            <i class="fas fa-arrow-right"></i>
          </button>

          <button 
            v-else
            @click="handleSubmit"
            :disabled="isGeneratingPlan || !canSubmit"
            :class="[
              'btn gap-2 text-lg font-cairo',
              isGeneratingPlan || !canSubmit
                ? 'btn-disabled' 
                : 'btn-primary'
            ]"
          >
            <i v-if="!isGeneratingPlan" class="fas fa-check"></i>
            <i v-else class="fas fa-spinner fa-spin"></i>
            {{ isGeneratingPlan ? 'Planning Your Perfect Trip...' : 'Plan My Trip Now!' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import aiPlannerService from '@/Services/aiPlannerService';
import DatePicker from '@/components/Common/DatePicker.vue';

export default {
  name: 'TripPlanningForm',
  components: {
    DatePicker
  },
  data() {
    return {
      currentStep: 1,
      isGeneratingPlan: false,
      formData: {
        headingTo: '',
        startDate: '',
        endDate: '',
        travelers: 2,
        withWhom: 'Couple',
        travelStyle: [],
        travelPace: 'Moderate',
        accommodation: 'Luxury',
        daysRhythm: 'EarlyStart',
        otherNeeds: ''
      },
      egyptianCities: [],
      isLoadingDestinations: false,
      travelStyleOptions: [
        { id: 'Historical', label: 'Historical Sites', icon: 'fas fa-landmark' },
        { id: 'Beach', label: 'Beach & Water', icon: 'fas fa-umbrella-beach' },
        { id: 'Culinary', label: 'Food & Cuisine', icon: 'fas fa-utensils' },
        { id: 'Shopping', label: 'Shopping & Bazaars', icon: 'fas fa-shopping-bag' },
        { id: 'Adventure', label: 'Adventure Activities', icon: 'fas fa-hiking' },
        { id: 'Cultural', label: 'Culture & Arts', icon: 'fas fa-theater-masks' },
        { id: 'Relaxation', label: 'Relaxation & Spa', icon: 'fas fa-spa' },
        { id: 'Nightlife', label: 'Nightlife', icon: 'fas fa-moon' },
        { id: 'Nature', label: 'Nature & Wildlife', icon: 'fas fa-tree' },
        { id: 'Cityscape', label: 'Urban Exploration', icon: 'fas fa-city' }
      ],
      withWhomOptions: [
        { value: 'Solo', label: 'Solo Travel', icon: 'fas fa-user', desc: 'Just me' },
        { value: 'Couple', label: 'Couple', icon: 'fas fa-heart', desc: 'Romantic trip' },
        { value: 'Family', label: 'Family', icon: 'fas fa-users', desc: 'With family' },
        { value: 'Friends', label: 'Friends', icon: 'fas fa-user-friends', desc: 'Group of friends' }
      ],
      accommodationOptions: [
        { value: 'Budget', label: 'Budget', icon: 'fas fa-wallet', desc: 'Hostels & Simple Hotels' },
        { value: 'Moderate', label: 'Moderate', icon: 'fas fa-hotel', desc: '3-4 Star Hotels' },
        { value: 'Luxury', label: 'Luxury', icon: 'fas fa-star', desc: '5 Star Hotels' }
      ],
      travelPaceOptions: [
        { value: 'Slow', label: 'Slow & Relaxed', icon: 'fas fa-turtle', desc: 'Take it easy' },
        { value: 'Moderate', label: 'Moderate', icon: 'fas fa-walking', desc: 'Balanced pace' },
        { value: 'Fast', label: 'Fast & Packed', icon: 'fas fa-running', desc: 'See everything' }
      ],
      daysRhythmOptions: [
        { value: 'EarlyStart', label: 'Early Bird', icon: 'fas fa-sun', desc: 'Start early, end early' },
        { value: 'Standard', label: 'Standard', icon: 'fas fa-clock', desc: 'Normal schedule' },
        { value: 'LateStart', label: 'Night Owl', icon: 'fas fa-moon', desc: 'Late start, late end' },
        { value: 'Flexible', label: 'Flexible', icon: 'fas fa-sync-alt', desc: 'No fixed schedule' }
      ]
    };
  },
  computed: {
    canProceedToNextStep() {
      if (this.currentStep === 1) {
        return this.formData.headingTo && this.formData.startDate && this.formData.endDate && this.calculateDays() > 0;
      } else if (this.currentStep === 2) {
        return this.formData.withWhom && this.formData.travelStyle.length > 0 && this.formData.travelPace;
      }
      return true;
    },
    canSubmit() {
      return this.formData.accommodation && this.formData.daysRhythm;
    }
  },
  mounted() {
    this.loadDestinations();
  },
  methods: {
    async loadDestinations() {
      this.isLoadingDestinations = true;
      try {
        const destinations = await aiPlannerService.getDestinations();
        this.egyptianCities = destinations.length > 0 ? destinations : [
          'Cairo', 'Alexandria', 'Luxor', 'Aswan', 'Sharm El Sheikh', 
          'Hurghada', 'Dahab', 'Marsa Alam', 'Siwa Oasis'
        ];
        if (this.egyptianCities.length > 0 && !this.formData.headingTo) {
          // If cities are objects, use the 'name' property; otherwise use the string directly
          this.formData.headingTo = this.egyptianCities[0].name || this.egyptianCities[0];
        }
      } catch (error) {
        console.error('Error loading destinations:', error);
        this.egyptianCities = [
          'Cairo', 'Alexandria', 'Luxor', 'Aswan', 'Sharm El Sheikh', 
          'Hurghada', 'Dahab', 'Marsa Alam', 'Siwa Oasis'
        ];
        this.formData.headingTo = this.egyptianCities[0];
      } finally {
        this.isLoadingDestinations = false;
      }
    },
    toggleTravelStyle(style) {
      const index = this.formData.travelStyle.indexOf(style);
      if (index > -1) {
        this.formData.travelStyle.splice(index, 1);
      } else {
        this.formData.travelStyle.push(style);
      }
    },
    nextStep() {
      if (this.canProceedToNextStep) {
        this.currentStep++;
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
      const option = this.accommodationOptions.find(opt => opt.value === this.formData.accommodation);
      return option ? option.label : '';
    },
    getWithWhomLabel() {
      const option = this.withWhomOptions.find(opt => opt.value === this.formData.withWhom);
      return option ? option.label : '';
    },
    async handleSubmit() {
      // Format data to match TripSearchCriteria DTO
      const tripCriteria = {
        headingTo: this.formData.headingTo,
        startDate: new Date(this.formData.startDate).toISOString(),
        endDate: new Date(this.formData.endDate).toISOString(),
        travelers: this.formData.travelers,
        withWhom: this.formData.withWhom,
        travelStyle: this.formData.travelStyle,
        travelPace: this.formData.travelPace,
        accommodation: this.formData.accommodation,
        daysRhythm: this.formData.daysRhythm,
        otherNeeds: this.formData.otherNeeds
      };
      
      console.log('Trip Criteria JSON:', JSON.stringify(tripCriteria, null, 2));
      
      this.isGeneratingPlan = true;
      
      try {
        // Call the AI planner service and wait for response
        const result = await aiPlannerService.generateTripPlan(tripCriteria);
        console.log('AI Trip Plan Result:', result);
        
        // Store in sessionStorage for retrieval in result page
        sessionStorage.setItem('tripPlanData', JSON.stringify(result));
        
        // Navigate to result page only after data is ready
        this.$router.push({ name: 'AiPlannerResult' });
      } catch (error) {
        console.error('Error generating trip plan:', error);
        alert('❌ Failed to generate trip plan. Please try again.');
      } finally {
        this.isGeneratingPlan = false;
      }
    }
  }
};
</script>

<style scoped>
.glass-morphism {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(12px) saturate(180%);
  -webkit-backdrop-filter: blur(12px) saturate(180%);
}

.font-cairo {
  font-family: 'Cairo', sans-serif;
}
</style>