<template>
  <div class="min-h-screen bg-base-100 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-4xl mx-auto">
      <!-- Header -->
      <div class="text-center mb-12">
        <h1 class="text-4xl font-bold text-base-content mb-2 font-cairo">
          🤖 AI Trip Planner
        </h1>
        <p class="text-lg text-base-content/70">
          Let our AI create a personalized travel itinerary just for you
        </p>
      </div>

      <!-- Main Form Card with Glassmorphism -->
      <div class="glass backdrop-blur-glass bg-base-200/60 rounded-lg p-8 mb-8 border border-base-300/50">
        <form @submit.prevent="handleSubmit" class="space-y-6">
          
          <!-- Destination -->
          <div>
            <label for="destination" class="block text-sm font-semibold text-base-content mb-2">
              📍 Destination
            </label>
            <div class="relative">
              <input
                id="destination"
                v-model="searchQuery"
                @input="handleDestinationSearch"
                @focus="showDropdown = true"
                @blur="handleBlur"
                type="text"
                placeholder="Search for a destination..."
                class="w-full px-4 py-3 bg-base-300/50 border border-base-300 rounded-lg text-base-content placeholder-base-content/50 focus:ring-2 focus:ring-primary focus:border-transparent transition"
                :class="{ 'border-error': errors.destination }"
              />
              
              <!-- Autocomplete Dropdown -->
              <div
                v-if="showDropdown && filteredDestinations.length > 0"
                class="absolute z-10 w-full mt-1 bg-base-200 border border-base-300 rounded-lg shadow-2xl max-h-64 overflow-y-auto"
              >
                <div
                  v-for="dest in filteredDestinations"
                  :key="dest.name"
                  @mousedown="selectDestination(dest)"
                  class="px-4 py-3 hover:bg-primary/20 cursor-pointer transition flex items-center justify-between"
                >
                  <div>
                    <div class="font-medium text-base-content">{{ dest.displayName }}</div>
                    <div class="text-xs text-base-content/60">{{ dest.region }}</div>
                  </div>
                  <span v-if="dest.popular" class="text-xs bg-primary/30 text-primary px-2 py-1 rounded">
                    Popular
                  </span>
                </div>
              </div>
            </div>
            <p v-if="errors.destination" class="mt-1 text-sm text-error">{{ errors.destination }}</p>
          </div>

          <!-- Dates -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label for="startDate" class="block text-sm font-semibold text-base-content mb-2">
                📅 Start Date
              </label>
              <input
                id="startDate"
                v-model="formData.startDate"
                type="date"
                :min="minDate"
                class="w-full px-4 py-3 bg-base-300/50 border border-base-300 rounded-lg text-base-content focus:ring-2 focus:ring-primary focus:border-transparent"
                :class="{ 'border-error': errors.startDate }"
              />
              <p v-if="errors.startDate" class="mt-1 text-sm text-error">{{ errors.startDate }}</p>
            </div>

            <div>
              <label for="endDate" class="block text-sm font-semibold text-base-content mb-2">
                📅 End Date
              </label>
              <input
                id="endDate"
                v-model="formData.endDate"
                type="date"
                :min="formData.startDate || minDate"
                class="w-full px-4 py-3 bg-base-300/50 border border-base-300 rounded-lg text-base-content focus:ring-2 focus:ring-primary focus:border-transparent"
                :class="{ 'border-error': errors.endDate }"
              />
              <p v-if="errors.endDate" class="mt-1 text-sm text-error">{{ errors.endDate }}</p>
            </div>
          </div>

          <!-- Budget Level -->
          <div>
            <label class="block text-sm font-semibold text-base-content mb-2">
              💰 Budget Level
            </label>
            <div class="grid grid-cols-3 gap-3">
              <button
                v-for="budget in budgetLevels"
                :key="budget.value"
                type="button"
                @click="formData.budgetLevel = budget.value"
                class="px-4 py-3 border-2 rounded-lg font-medium transition-all"
                :class="formData.budgetLevel === budget.value
                  ? 'border-primary bg-primary/20 text-primary'
                  : 'border-base-300 hover:border-primary/50 text-base-content'"
              >
                <div class="text-2xl mb-1">{{ budget.icon }}</div>
                <div class="text-sm">{{ budget.label }}</div>
              </button>
            </div>
          </div>

          <!-- Travelers -->
          <div>
            <label for="travelers" class="block text-sm font-semibold text-base-content mb-2">
              👥 Number of Travelers
            </label>
            <input
              id="travelers"
              v-model.number="formData.travelers"
              type="number"
              min="1"
              max="20"
              class="w-full px-4 py-3 bg-base-300/50 border border-base-300 rounded-lg text-base-content focus:ring-2 focus:ring-primary focus:border-transparent"
              :class="{ 'border-error': errors.travelers }"
            />
            <p v-if="errors.travelers" class="mt-1 text-sm text-error">{{ errors.travelers }}</p>
          </div>

          <!-- Interests -->
          <div>
            <label class="block text-sm font-semibold text-base-content mb-2">
              ✨ Your Interests
            </label>
            <div class="flex flex-wrap gap-2">
              <button
                v-for="interest in availableInterests"
                :key="interest.id"
                type="button"
                @click="toggleInterest(interest.id)"
                class="px-4 py-2 rounded-full font-medium transition-all text-sm"
                :class="formData.interests.includes(interest.id)
                  ? 'bg-primary text-white'
                  : 'bg-base-300/50 text-base-content hover:bg-base-300'"
              >
                {{ interest.icon }} {{ interest.name }}
              </button>
            </div>
          </div>

          <!-- Submit -->
          <div class="pt-4">
            <button
              type="submit"
              :disabled="isLoading"
              class="w-full bg-primary hover:bg-primary-focus text-primary-content py-4 px-6 rounded-lg font-bold text-lg transition-all shadow-lg hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span v-if="!isLoading">🚀 Generate My Trip Plan</span>
              <span v-else class="flex items-center justify-center">
                <svg class="animate-spin h-5 w-5 mr-3" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" fill="none"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Generating...
              </span>
            </button>
          </div>
        </form>
      </div>

      <!-- Info Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div class="glass backdrop-blur-glass bg-base-200/40 rounded-lg p-6 text-center border border-base-300/30">
          <div class="text-3xl mb-2">🎯</div>
          <h3 class="font-semibold text-base-content font-cairo">Personalized</h3>
          <p class="text-sm text-base-content/60 mt-1">Tailored to your interests</p>
        </div>
        <div class="glass backdrop-blur-glass bg-base-200/40 rounded-lg p-6 text-center border border-base-300/30">
          <div class="text-3xl mb-2">⚡</div>
          <h3 class="font-semibold text-base-content font-cairo">Instant</h3>
          <p class="text-sm text-base-content/60 mt-1">Plan ready in seconds</p>
        </div>
        <div class="glass backdrop-blur-glass bg-base-200/40 rounded-lg p-6 text-center border border-base-300/30">
          <div class="text-3xl mb-2">📍</div>
          <h3 class="font-semibold text-base-content font-cairo">Real Data</h3>
          <p class="text-sm text-base-content/60 mt-1">Based on actual locations</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import aiPlannerService from '@/Services/aiPlannerService';
import { useToast } from '@/composables/useToast';

const router = useRouter();
const { showToast } = useToast();

const formData = ref({
  destination: '',
  startDate: '',
  endDate: '',
  budgetLevel: 'mid-range',
  travelers: 2,
  interests: []
});

const searchQuery = ref('');
const showDropdown = ref(false);
const isLoading = ref(false);
const errors = ref({});
const destinations = ref([]);
const availableInterests = ref([]);

const budgetLevels = [
  { value: 'budget', label: 'Budget', icon: '💵' },
  { value: 'mid-range', label: 'Mid-Range', icon: '💰' },
  { value: 'luxury', label: 'Luxury', icon: '💎' }
];

const minDate = computed(() => {
  const today = new Date();
  return today.toISOString().split('T')[0];
});

const filteredDestinations = computed(() => {
  if (!searchQuery.value) return destinations.value;
  const query = searchQuery.value.toLowerCase();
  return destinations.value.filter(dest =>
    dest.name.toLowerCase().includes(query) ||
    dest.displayName.toLowerCase().includes(query) ||
    dest.region.toLowerCase().includes(query)
  );
});

const handleDestinationSearch = () => {
  showDropdown.value = true;
};

const selectDestination = (dest) => {
  formData.value.destination = dest.name;
  searchQuery.value = dest.displayName;
  showDropdown.value = false;
  delete errors.value.destination;
};

const handleBlur = () => {
  setTimeout(() => {
    showDropdown.value = false;
  }, 200);
};

const toggleInterest = (interestId) => {
  const index = formData.value.interests.indexOf(interestId);
  if (index > -1) {
    formData.value.interests.splice(index, 1);
  } else {
    formData.value.interests.push(interestId);
  }
};

const validateForm = () => {
  errors.value = {};
  let isValid = true;

  if (!formData.value.destination) {
    errors.value.destination = 'Please select a destination';
    isValid = false;
  }

  if (!formData.value.startDate) {
    errors.value.startDate = 'Start date is required';
    isValid = false;
  }

  if (!formData.value.endDate) {
    errors.value.endDate = 'End date is required';
    isValid = false;
  }

  if (formData.value.startDate && formData.value.endDate) {
    if (new Date(formData.value.endDate) <= new Date(formData.value.startDate)) {
      errors.value.endDate = 'End date must be after start date';
      isValid = false;
    }
  }

  if (formData.value.travelers < 1) {
    errors.value.travelers = 'At least one traveler is required';
    isValid = false;
  }

  return isValid;
};

const handleSubmit = async () => {
  if (!validateForm()) {
    showToast('Please fix the errors in the form', 'error');
    return;
  }

  isLoading.value = true;

  try {
    const response = await aiPlannerService.generateTripPlan(formData.value);
    
    if (response.data) {
      localStorage.setItem('tripPlanData', JSON.stringify(response.data));
      router.push({ name: 'PlannerResult' });
    } else {
      showToast(response.message || 'Failed to generate trip plan', 'error');
    }
  } catch (error) {
    console.error('Error:', error);
    showToast('An error occurred while generating your trip plan', 'error');
  } finally {
    isLoading.value = false;
  }
};

onMounted(async () => {
  try {
    const [destData, interestsData] = await Promise.all([
      aiPlannerService.getDestinations(),
      aiPlannerService.getInterests()
    ]);
    destinations.value = destData;
    availableInterests.value = interestsData;
  } catch (error) {
    console.error('Error loading data:', error);
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
