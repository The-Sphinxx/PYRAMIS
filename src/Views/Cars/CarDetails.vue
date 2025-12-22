<template>
  <div class="min-h-screen bg-base-200">
    <div v-if="loading" class="flex justify-center items-center min-h-screen">
      <span class="loading loading-spinner loading-lg text-primary"></span>
    </div>

    <div v-else-if="error" class="page-container py-20 text-center">
      <i class="fas fa-exclamation-triangle text-6xl text-error mb-4"></i>
      <h2 class="text-3xl font-bold mb-4">Car Not Found</h2>
      <p class="text-neutral mb-6">{{ error }}</p>
      <button @click="router.push('/cars')" class="btn btn-primary">
        Back to Cars
      </button>
    </div>

    <div v-else-if="car" class="bg-base-200 pb-16 !overflow-visible">
      <div class="page-container mb-8">
        <Carousel
          :images="images"
          :auto-play="false"
          :auto-play-interval="5000"
        />
      </div>

      <div class="page-container !overflow-visible">
        <div class="mb-8">
          <h1 class="text-3xl lg:text-4xl font-bold text-primary font-cairo mb-4">
            {{ car.name }}
          </h1>
          <p class="text-lg text-base-content/70 mb-4">
            {{ car.description }}
          </p>
          
          <div class="flex flex-wrap items-center gap-4 text-base-content/70">
            <div class="flex items-center gap-2">
              <i class="fas fa-users text-primary"></i>
              <span>{{ car.seats }} Passengers</span>
            </div>
            <div class="flex items-center gap-2">
              <i class="fas fa-cog text-primary"></i>
              <span>{{ car.transmission }}</span>
            </div>
            <div class="flex items-center gap-2">
              <i class="fas fa-star text-warning"></i>
              <span class="font-bold text-base-content">{{ overallReviews.overallRating }}</span>
              <span>({{ overallReviews.totalReviews }} reviews)</span>
            </div>
            <div class="flex items-center gap-2">
              <i class="fas fa-dollar-sign text-primary"></i>
              <span class="text-2xl font-bold text-accent">${{ car.price }}/day</span>
            </div>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div ref="contentColumn" class="lg:col-span-2 space-y-8">
            <div class="bg-base-100 rounded-lg shadow-lg p-8">
              <div class="flex items-center gap-3 mb-6">
                <div class="w-1 h-8 bg-primary rounded-full"></div>
                <h2 class="text-2xl font-bold text-base-content font-cairo">Overview</h2>
              </div>
              <p 
                class="text-base-content/80 leading-relaxed mb-4"
                :class="{ 'line-clamp-3': !showFullOverview }"
              >
                {{ car.overview || car.description }}
              </p>
              <button 
                @click="showFullOverview = !showFullOverview"
                class="text-primary font-semibold hover:underline"
              >
                {{ showFullOverview ? 'Read Less' : 'Read More' }}
              </button>

              <div class="flex items-center gap-3 my-6">
                <div class="w-1 h-8 bg-primary rounded-full"></div>
                <h2 class="text-2xl font-bold text-base-content font-cairo">Specifications</h2>
              </div>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div class="flex items-start gap-3">
                  <i class="fas fa-check text-primary mt-1"></i>
                  <span class="text-base-content/80">{{ car.seats }} Passengers</span>
                </div>
                <div class="flex items-start gap-3">
                  <i class="fas fa-check text-primary mt-1"></i>
                  <span class="text-base-content/80">{{ car.transmission }} Transmission</span>
                </div>
                <div v-if="car.fuelType" class="flex items-start gap-3">
                  <i class="fas fa-check text-primary mt-1"></i>
                  <span class="text-base-content/80">{{ car.fuelType }}</span>
                </div>
                <div v-if="car.year" class="flex items-start gap-3">
                  <i class="fas fa-check text-primary mt-1"></i>
                  <span class="text-base-content/80">Year: {{ car.year }}</span>
                </div>
              </div>

              <div v-if="car.features && car.features.length > 0" class="flex items-center gap-3 my-6">
                <div class="w-1 h-8 bg-primary rounded-full"></div>
                <h2 class="text-2xl font-bold text-base-content font-cairo">Features</h2>
              </div>
              <div v-if="car.features && car.features.length > 0" class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div 
                  v-for="(feature, index) in car.features" 
                  :key="index"
                  class="flex items-center gap-3 p-4 bg-base-200 rounded-lg"
                >
                  <div class="w-10 h-10 bg-primary/10 rounded-lg flex items-center justify-center flex-shrink-0">
                    <i class="fas fa-car text-primary"></i>
                  </div>
                  <span class="text-base-content/80">{{ feature }}</span>
                </div>
              </div>

              <Rating
                v-if="overallReviews"
                :reviews="overallReviews"
              />

              <ReviewsCarousel
                v-if="carReviews && carReviews.length > 0"
                ref="reviewsSection"
                :user-reviews="carReviews"
                :auto-play="true"
                :auto-play-interval="4000"
              />

          
            </div>
          </div>

          <!-- Right Column: Booking Form (Sticky) -->
          <div ref="bookingColumn" class="lg:col-span-1 h-full min-h-[450px] relative">
            <div ref="bookingWrapper" :style="{ minHeight: isSticky ? '1px' : 'auto' }"></div>
            
            <div 
              ref="stickyForm"
              :style="stickyStyle"
              :class="{ 'fixed top-24 z-50': isSticky }"
            >
              <BookingForm
                type="car"
                :base-price="totalPrice"
                :loading="bookingLoading"
                @submit="handleBooking"
              />
            </div>
          </div>
        </div>

        <div v-if="suggestedCars.length > 0" class="mt-16">
          <h2 class="text-3xl font-bold mb-8 font-cairo text-center text-primary">You may also like</h2>
          <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
            <div 
              v-for="suggestedCar in suggestedCars" 
              :key="suggestedCar.id" 
              @click="handleSimilarClick(suggestedCar)"
              class="cursor-pointer overflow-hidden rounded-xl shadow-lg transform transition-all hover:scale-105 hover:shadow-2xl bg-base-100 glass"
            >
              <img 
                :src="getSimilarImage(suggestedCar)" 
                :alt="suggestedCar.name" 
                class="w-full h-36 object-cover" 
              />
              <div class="p-3">
                <p class="font-bold text-sm">{{ suggestedCar.name }}</p>
                <p class="text-xs text-base-content/60">{{ suggestedCar.description.slice(0, 30) }}...</p>
                <span class="text-lg font-bold text-accent">${{ suggestedCar.price }}/day</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch, onUnmounted, nextTick } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import Carousel from '@/components/Common/Carousel.vue';
import Rating from '@/components/Common/Rating.vue';
import ReviewsCarousel from '@/components/Common/ReviewsCarousel.vue';
import BookingForm from '@/components/Common/BookingForm.vue';
import { useBookingStore } from '@/stores/bookingStore';

const route = useRoute();
const router = useRouter();
const bookingStore = useBookingStore();

// State
const loading = ref(true);
const error = ref(null);
const car = ref(null);
const allCars = ref([]);
const bookingLoading = ref(false);
const showFullOverview = ref(false);
const selectedAddOns = ref({});
const reviewsSection = ref(null);

// Reviews Data
const carReviews = ref([
  { id: 1, userName: 'Ahmed', userImage: '/images/users/user1.jpg', date: '2025-12-16', rating: 4.8, comment: 'Great car! Very comfortable for long trips.' },
  { id: 2, userName: 'Sara', userImage: '/images/users/user2.jpg', date: '2025-12-15', rating: 4.5, comment: 'Smooth ride and excellent service.' },
  { id: 3, userName: 'Omar', userImage: '/images/users/user3.jpg', date: '2025-12-14', rating: 5.0, comment: 'Highly recommended! Best rental experience.' }
]);

const overallReviews = ref({ overallRating: 4.7, totalReviews: 120, ratingCriteria: { condition: 4.8, comfort: 4.6, performance: 4.7 } });

// Images for carousel
const images = computed(() => car.value?.images?.length ? car.value.images : ['/images-car/placeholder.jpg']);

// Total price with add-ons
const totalPrice = computed(() => {
  if (!car.value) return 0;
  let total = car.value.price;
  Object.values(selectedAddOns.value).forEach(price => total += price);
  return total;
});

// Suggested cars
const suggestedCars = computed(() => {
  if (!allCars.value || !car.value) return [];
  const filtered = allCars.value.filter(c => c.id !== car.value.id);
  return getRandomCars(filtered, 5);
});

// Methods
const fetchCar = async () => {
  loading.value = true;
  error.value = null;
  try {
    const response = await fetch('/db.json'); 
    if (!response.ok) throw new Error('Failed to load data');
    const data = await response.json();
    allCars.value = data.cars || [];
    const carId = route.params.id;
    const foundCar = allCars.value.find(c => c.id == carId); 
    if (foundCar) car.value = foundCar;
    else error.value = 'Car not found';
  } catch (err) {
    error.value = err.message || 'Failed to load car';
    console.error('Error loading car:', err);
  } finally {
    loading.value = false;
  }
};

const handleBooking = (eventData) => {
  if (!car.value) return;

  const { bookingData } = eventData;

  // Initialize booking in store
  bookingStore.initializeBooking('car', car.value.id, car.value);
  
  // Direct assignment to match TripDetails pattern
  const bookingDataVal = eventData.bookingData || eventData;
  const clonedData = JSON.parse(JSON.stringify(bookingDataVal));
  
  bookingStore.bookingInProgress.bookingData = clonedData;
  bookingStore.persistState();
  
  router.push({ 
    name: 'CarReview', 
    params: { id: car.value.id } 
  });
};

const toggleAddOn = (name, price) => {
  if (selectedAddOns.value[name]) delete selectedAddOns.value[name];
  else selectedAddOns.value[name] = price;
};

const getRandomCars = (arr, count) => [...arr].sort(() => 0.5 - Math.random()).slice(0, count);
const getSimilarImage = (car) => car.images?.length ? car.images[0] : '/images-car/placeholder.jpg';
const handleSimilarClick = (suggestedCar) => router.push({ name: 'CarDetails', params: { id: suggestedCar.id } });

// Scroll to reviews
const scrollToReviews = async () => {
  await nextTick();
  reviewsSection.value?.$el.scrollIntoView({ behavior: 'smooth', block: 'start' });
};

// Sticky Logic
const bookingWrapper = ref(null);
const stickyForm = ref(null);
const isSticky = ref(false);
const stickyStyle = ref({});
const bookingColumn = ref(null);
const contentColumn = ref(null);

const handleScroll = () => {
  if (!bookingWrapper.value || !bookingColumn.value || !contentColumn.value) return;
  const rect = bookingWrapper.value.getBoundingClientRect();
  const contentRect = contentColumn.value.getBoundingClientRect();
  const formHeight = stickyForm.value.offsetHeight;
  const offsetTop = 100;
  if (contentRect.bottom <= offsetTop + formHeight) {
    isSticky.value = false;
    stickyStyle.value = { position: 'absolute', bottom: '0', left: '0', width: '100%', zIndex: 40 };
  } else if (rect.top <= offsetTop) {
    isSticky.value = true;
    stickyStyle.value = { position: 'fixed', top: `${offsetTop}px`, width: `${bookingWrapper.value.getBoundingClientRect().width}px`, zIndex: 50 };
  } else {
    isSticky.value = false;
    stickyStyle.value = {};
  }
};

const handleResize = () => {
  if (isSticky.value && bookingWrapper.value) {
    const rect = bookingWrapper.value.getBoundingClientRect();
    stickyStyle.value.width = `${rect.width}px`;
  }
};

onMounted(() => {
  fetchCar();
  window.addEventListener('scroll', handleScroll);
  window.addEventListener('resize', handleResize);
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
  window.removeEventListener('resize', handleResize);
});

watch(() => route.params.id, () => {
  if (route.name === 'CarDetails') {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    fetchCar();
  }
});
</script>

<style scoped>
.font-cairo { font-family: 'Cairo', sans-serif; }
.line-clamp-3 { display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; line-clamp: 3; }
@media (min-width: 1024px) { .lg\:sticky { position: -webkit-sticky !important; position: sticky !important; } }
</style>