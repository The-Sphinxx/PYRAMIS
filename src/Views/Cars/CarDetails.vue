<!-- CarDetails.vue -->
<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import Carousel from '@/components/Common/Carousel.vue'
import ReviewsCarousel from '@/components/Common/ReviewsCarousel.vue'
import Rating from '@/components/Common/Rating.vue'
import BookingForm from '@/components/Common/BookingForm.vue'

const route = useRoute()
const car = ref(null)
const allCars = ref([])
const loading = ref(true)
const error = ref(null)

const selectedAddOns = ref({})

// Dummy reviews
const carReviews = ref([
  { id: 1, userName: 'Ahmed', userImage: '/images/users/user1.jpg', date: '2025-12-16', rating: 4.8, comment: 'Great car!' },
  { id: 2, userName: 'Sara', userImage: '/images/users/user2.jpg', date: '2025-12-15', rating: 4.5, comment: 'Smooth ride.' },
  { id: 3, userName: 'Omar', userImage: '/images/users/user3.jpg', date: '2025-12-14', rating: 5.0, comment: 'Highly recommended!' }
])

const overallReviews = ref({
  overallRating: 4.7,
  totalReviews: 120,
  ratingCriteria: {
    condition: 4.8,
    comfort: 4.6,
    performance: 4.7
  }
})

// Load car data
const loadCarData = async () => {
  try {
    loading.value = true
    const response = await fetch('/db.json')
    if (!response.ok) throw new Error('Failed to load data')
    const data = await response.json()

    allCars.value = data.cars || []

    const carId = parseInt(route.params.id) || 1
    const foundCar = allCars.value.find(c => c.id === carId)

    if (foundCar) car.value = foundCar
    else error.value = 'Car not found'
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

const allImages = computed(() => car.value?.images || ['/images-car/placeholder.jpg'])

const totalPrice = computed(() => {
  if (!car.value) return 0
  let total = car.value.price
  Object.values(selectedAddOns.value).forEach(price => total += price)
  return total
})

const toggleAddOn = (name, price) => {
  if (selectedAddOns.value[name]) delete selectedAddOns.value[name]
  else selectedAddOns.value[name] = price
}

const getRandomCars = (arr, count) => [...arr].sort(() => 0.5 - Math.random()).slice(0, count)

const suggestedCars = computed(() => {
  if (!allCars.value || !car.value) return []
  return getRandomCars(allCars.value.filter(c => c.id !== car.value.id), 5)
})

const handleBookingSubmit = ({ bookingData, costs }) => {
  console.log('Booking submitted:', bookingData, costs)
  alert(`Booking submitted! Total cost: $${costs.total}`)
}

onMounted(loadCarData)
</script>

<template>
<div class="font-cairo min-h-screen bg-base-100 page-container">
  <div v-if="loading" class="flex flex-col items-center justify-center min-h-screen gap-4">
    <span class="loading loading-spinner loading-lg text-primary"></span>
    <p class="text-base-content text-lg">Loading car details...</p>
  </div>

  <div v-else-if="error" class="flex items-center justify-center min-h-screen">
    <p class="text-error text-2xl font-bold">{{ error }}</p>
  </div>

  <div v-else-if="car" class="pb-12">
    <!-- Carousel -->
    <div class="my-6">
      <Carousel :images="allImages" :autoPlay="false" />
    </div>

    <!-- Content Grid: Details + BookingForm -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mt-12">
      <!-- Left Column -->
      <div class="lg:col-span-2 space-y-6">
        <div class="card bg-base-100 shadow-lg rounded-xl glass p-6 space-y-6">
          <h1 class="text-3xl lg:text-4xl font-bold text-primary mb-2">{{ car.name }}</h1>
          <p class="text-base-content/60 mb-4">{{ car.description }}</p>
          <div class="flex items-center gap-2 text-sm text-base-content/60 mb-4">
            <span>{{ car.seats }} Passengers</span> | <span>{{ car.transmission }}</span>
          </div>
          <div class="text-2xl font-bold text-accent mb-2">${{ car.price }}/day</div>
          <Rating :reviews="overallReviews" />
          <div class="mt-4">
            <ReviewsCarousel :userReviews="carReviews" :autoPlay="true" :autoPlayInterval="4000" />
          </div>
        </div>
      </div>

      <!-- Right Column: BookingForm -->
      <div class="lg:col-span-1 flex flex-col">
        <div class="sticky top-24">
          <BookingForm
            type="car"
            :basePrice="totalPrice"
            @submit="handleBookingSubmit"
          />
        </div>
      </div>
    </div>

    <!-- Suggested Cars (Full Width) -->
    <div class="mt-12 px-4 md:px-8 lg:px-12">
      <h2 class="text-2xl font-bold text-primary mb-6 text-center">You May also Like</h2>
      <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
        <div v-for="(suggestedCar, index) in suggestedCars" :key="index" class="cursor-pointer overflow-hidden rounded-xl shadow-lg transform transition-all hover:scale-105 hover:shadow-2xl bg-base-100 glass">
          <img :src="suggestedCar.images?.[0] || '/images-car/placeholder.jpg'" :alt="suggestedCar.name" class="w-full h-36 object-cover" />
          <div class="p-3">
            <p class="font-bold text-sm">{{ suggestedCar.name }}</p>
            <p class="text-xs text-base-content/60">{{ suggestedCar.description.slice(0, 30) }}...</p>
            <span class="text-lg font-bold text-accent">${{ suggestedCar.price }}</span>
          </div>
        </div>
      </div>
    </div>

  </div>
</div>
</template>

<script setup>
import { onMounted, computed, ref } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useCarsStore } from "@/stores/carsStore.js";
import LuxurySUVCardDynamic from "@/components/Cars/CarCard.vue";
//import { Star } from 'lucide-vue-next';

const store = useCarsStore();
const router = useRouter();
const route = useRoute();
const selectedImage = ref(0);
const youMayScroll = ref(null);

onMounted(async () => {
  await store.fetchCars();
});

const car = computed(() => store.cars.find(c => c.id === parseInt(route.params.id)));
const recommendedCars = computed(() => store.cars.filter(c => c.id !== parseInt(route.params.id)));

function handleViewCar(carId) {
  router.push({ name: 'CarDetails', params: { id: carId } });
}

// Scroll functions
function scrollRight() {
  youMayScroll.value.scrollBy({ left: 300, behavior: 'smooth' });
}
function scrollLeft() {
  youMayScroll.value.scrollBy({ left: -300, behavior: 'smooth' });
}
</script>
