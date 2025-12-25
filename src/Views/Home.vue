<template>
  <div>
    <Navbar />

    <div class="relative w-full h-[100dvh] overflow-hidden">
      <div class="absolute inset-0 bg-cover bg-center" :style="{ backgroundImage: `url(${heroSection.backgroundImage || defaultHero.backgroundImage})` }">
        <div class="absolute inset-0 bg-gradient-to-b from-black/40 via-black/30 to-black/50"></div>
      </div>
      
      <div class="relative z-10 flex flex-col items-center justify-start pt-32 lg:justify-center lg:pt-20 h-full page-container text-center">
        <h1 class="font-cairo text-3xl sm:text-4xl md:text-6xl lg:text-7xl font-bold text-white mb-4 animate-fade-in" v-html="heroDisplayTitle"></h1>
        <p class="font-cairo text-xl md:text-2xl text-white/90 mb-12 animate-fade-in-delay">
          {{ heroSection.subtitle || defaultHero.subtitle }}
        </p>

        <div class="w-full max-w-6xl bg-base-100 rounded-2xl shadow-2xl p-4 md:p-6 animate-slide-up ">
          <!-- Tab Headers -->
          <div role="tablist" class="tabs bg-base-200 p-2 mb-4 md:mb-6 gap-2 flex-wrap md:flex-nowrap rounded-t-lg">
            <button
              role="tab"
              v-for="tab in tabs"
              :key="tab.id"
              @click="activeTab = tab.id"
              :class="[
                'tab font-cairo font-semibold text-sm md:text-base transition-all duration-300 flex-1 md:flex-initial rounded-lg flex items-center justify-center min-h-[48px]',
                activeTab === tab.id 
                  ? 'text-primary bg-base-100 shadow-sm' 
                  : 'text-base-content/60 hover:text-base-content hover:bg-base-300/50'
              ]"
            >
              <i :class="[tab.icon, 'mr-1 md:mr-2']"></i>
              <span class="hidden sm:inline">{{ tab.name }}</span>
              <span class="sm:hidden">{{ tab.name.split(' ')[0] }}</span>
            </button>
          </div>

          <!-- Tab Content -->
          <div>
            <!-- Attractions -->
            <div v-show="activeTab === 'attractions'">
              <Search 
                type="attractions"
                :show-ai-planner="true"
                :cities="egyptianCities"
                @search="handleAttractionSearch"
                @ai-planner="handleAiPlanner"
              />
            </div>

            <!-- Hotels -->
            <div v-show="activeTab === 'hotels'">
              <Search
                type="hotels"
                @search="handleHotelSearch"
              />
            </div>

            <!-- Trips -->
            <div v-show="activeTab === 'trips'">
              <Search
                type="trips"
                :show-ai-planner="true"
                @search="handleTripSearch"
                @ai-planner="handleAiPlanner"
              />
            </div>

            <!-- Car Rental -->
            <div v-show="activeTab === 'car-rental'">
              <Search
                type="cars"
                @search="handleCarSearch"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <section class="bg-base-200 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-4">Explore Egypt's Wonders</h2>
          <p class="font-cairo text-lg text-base-content/70 max-w-3xl mx-auto">Discover ancient treasures and timeless beauty across Egypt's most iconic attractions</p>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-10">
          <AttractionCard
            v-for="item in attractions.slice(0, 4)"
            :key="item.id"
            :id="item.id"
            :image="item.image"
            :title="item.name"
            :price="`${item.price}$`"
            :location="item.location"
            :rating="item.rating"
            :reviews="parseReviews(item.reviews)"
          />
        </div>
        <div class="text-center">
          <router-link to="/attractions/list" class="btn btn-primary font-cairo font-semibold w-64 h-12 min-h-0 text-lg shadow-md hover:shadow-xl hover:-translate-y-1 transition-all flex items-center justify-center mx-auto">View All</router-link>
        </div>
      </div>
    </section>

    <section class="bg-base-100 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-4">Luxury Hotels & Resorts</h2>
          <p class="font-cairo text-lg text-base-content/70 max-w-3xl mx-auto">Experience Egyptian hospitality at its finest in our handpicked selection of premium hotels</p>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-10">
          <HotelCard
            v-for="item in hotels.slice(0, 4)"
            :key="item.id"
            :hotel="{
              id: item.id,
              name: item.name,
              image: item.image,
              price: item.price,
              location: item.location,
              rating: item.rating,
              reviews: item.reviews,
              amenities: item.amenities || ['Wifi', 'Pool', 'Gym']
            }"
          />
        </div>
        <div class="text-center">
          <router-link to="/hotels/list" class="btn btn-primary font-cairo font-semibold w-64 h-12 min-h-0 text-lg shadow-md hover:shadow-xl hover:-translate-y-1 transition-all flex items-center justify-center mx-auto">View All</router-link>
        </div>
      </div>
    </section>

    <section class="bg-base-200 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-4">Featured Trips & Tours</h2>
          <p class="font-cairo text-lg text-base-content/70 max-w-3xl mx-auto">All-inclusive packages designed for unforgettable Egyptian adventures</p>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-10">
          <TripCard
            v-for="item in trips.slice(0, 4)"
            :key="item.id"
            :trip="{
              id: item.id,
              title: item.name,
              image: item.image,
              price: item.price,
              description: item.description,
              location: item.location,
              duration: item.duration,
              rating: item.rating,
              reviews: item.reviews,
              amenities: {
                transport: true,
                accommodation: true,
                meals: 'All'
              }
            }"
          />
        </div>
        <div class="text-center">
          <router-link to="/trips/list" class="btn btn-primary font-cairo font-semibold w-64 h-12 min-h-0 text-lg shadow-md hover:shadow-xl hover:-translate-y-1 transition-all flex items-center justify-center mx-auto">View All</router-link>
        </div>
      </div>
    </section>

    <section class="bg-base-100 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-4">Rent Your Perfect Ride</h2>
          <p class="font-cairo text-lg text-base-content/70 max-w-3xl mx-auto">Explore Egypt at your own pace with our diverse fleet of quality vehicles</p>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-10">
          <CarCard
            v-for="item in cars.slice(0, 4)"
            :key="item.id"
            :car="{
              id: item.id,
              name: item.name,
              images: [item.image],
              price: item.price,
              overview: item.description,
              reviews: {
                overallRating: 4.5,
                totalReviews: 120
              }
            }"
          />
        </div>
        <div class="text-center">
          <router-link to="/cars/list" class="btn btn-primary font-cairo font-semibold w-64 h-12 min-h-0 text-lg shadow-md hover:shadow-xl hover:-translate-y-1 transition-all flex items-center justify-center mx-auto">View All</router-link>
        </div>
      </div>
    </section>

    <section class="relative min-h-[600px] flex items-center justify-center overflow-hidden py-20">
  <div class="absolute inset-0 z-0">
    <img 
      src="/public/images/AI.png" 
      alt="Background" 
      class="w-full h-full object-cover"
    />
    <div class="absolute inset-0 bg-[#D8C4B6]/60 mix-blend-multiply"></div>
    <div class="absolute inset-0 bg-gradient-to-b from-transparent via-[#E8D5C4]/40 to-[#D8C4B6]/70"></div>
  </div>

  <div class="page-container relative z-10">
    <div class="flex flex-col items-center justify-center text-center">
      
      <div class="inline-flex items-center gap-2 px-6 py-2 mb-8 rounded-full border border-white/40 bg-white/20 backdrop-blur-xl shadow-sm">
        <i class="fas fa-sparkles text-[#C86A41]"></i>
        <span class="font-cairo text-sm font-medium text-[#5D4037]">Powered by AI</span>
      </div>

      <h2 class="font-cairo text-4xl md:text-6xl font-bold text-[#4A372F] mb-6 tracking-tight">
        Plan  <span class="bg-gradient-to-r from-[#C86A41] to-[#A5533A] bg-clip-text text-transparent">Your Trip with AI</span>
      </h2>

      <p class="font-cairo text-xl text-[#5D4037]/90 max-w-2xl mx-auto mb-12 leading-relaxed">
        Tell our smart assistant what you want, and it builds a full personalized itinerary.
      </p>

      <button 
        @click="handleAiPlanner" 
        class="group relative flex items-center gap-3 px-10 py-4 rounded-2xl border border-white/50 bg-white/20 backdrop-blur-md transition-all duration-300 hover:bg-white/30 hover:scale-105 active:scale-95 shadow-xl"
      >
        <i class="fas fa-magic text-[#C86A41] text-xl"></i>
        <span class="font-cairo text-xl font-bold text-[#4A372F]">
          Start <span class="bg-gradient-to-r from-[#C86A41] to-[#A5533A] bg-clip-text text-transparent">Ai Planning</span>
        </span>
      </button>
      
    </div>
  </div>
</section>

    <section class="bg-base-200 py-16">
  <div class="page-container">
    <div class="text-center mb-16">
      <h2 class="font-cairo text-4xl md:text-5xl font-bold mb-4 tracking-tight">
        <span class="text-neutral dark:text-gray-300">What </span>
        <span class="bg-gradient-to-r from-[#C86A41] to-[#A5533A] bg-clip-text text-transparent">
          Travelers Say
        </span>
      </h2>
      <div class="relative flex justify-center items-center">
        <div class="w-48 h-[1px] bg-gradient-to-r from-transparent via-[#C86A41] to-transparent opacity-60"></div>
      </div>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
      <div v-for="item in testimonials" :key="item.id" 
           class="card bg-base-00 shadow-glass-shadow border border-base-300 p-8 hover:border-primary/30 transition-all duration-300">
        
        <i class="fas fa-quote-left text-3xl text-primary/20 mb-4"></i>
        
        <div class="flex gap-1 mb-4">
          <i v-for="i in 5" :key="i" class="fas fa-star text-[#D5B77A] text-xs"></i>
        </div>
        
        <p class="font-cairo text-base-content/80 mb-6 italic leading-relaxed">
          "{{item.text}}"
        </p>
        
        <div class="mt-auto pt-4 border-t border-base-300/50 flex items-center gap-4">
          <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center font-bold text-primary">
            {{ item.name.charAt(0) }}
          </div>
          <div>
            <h4 class="font-cairo font-bold text-base-content leading-none">{{item.name}}</h4>
            <p class="font-cairo text-xs text-primary mt-1">{{item.country}}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</section>

   

    <section class="bg-base-100 py-16">
  <div class="page-container">
    <div class="text-center mb-16">
      <h2 class="font-cairo text-4xl md:text-5xl font-bold mb-4 tracking-tight">
        <span class="text-neutral">Why  </span>
        <span class="bg-gradient-to-r  from-[#C86A41] to-[#A5533A] bg-clip-text text-transparent">
          Travelers Choose Us
        </span>
      </h2>
      <div class="relative flex justify-center items-center">
        <div class="w-56 h-[1.5px] bg-gradient-to-r from-transparent via-primary/60 to-transparent"></div>
      </div>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-12">
      <div v-for="item in whyChooseUs" :key="item.id" 
           class="flex flex-col items-center text-center group transition-all duration-300">
        <div class="mb-6 transform group-hover:scale-110 transition-transform duration-300">
          <i :class="[item.icon, 'text-6xl text-primary opacity-90']"></i>
        </div>
        
        <h3 class="font-cairo text-xl font-bold text-base-content mb-3 group-hover:text-primary transition-colors">
          {{item.title}}
        </h3>
        
        <p class="font-cairo text-base-content/70 leading-relaxed px-4">
          {{item.description}}
        </p>
      </div>
    </div>
  </div>
</section>

    <Footer />
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { getHomePageData } from '@/Services/homeService';
import { getBackgrounds } from '@/Services/systemService';
import Navbar from '@/components/Common/Navbar.vue';
import Footer from '@/components/Common/Footer.vue';
import Search from '@/components/Common/Search.vue';
import AttractionCard from '@/components/Attractions/AttractionCard.vue';
import HotelCard from '@/components/Hotels/HotelCard.vue';
import TripCard from '@/components/Trips/TripCard.vue';
import CarCard from '@/components/Cars/CarCard.vue';

const router = useRouter();
const activeTab = ref('attractions');
const isHomeLoading = ref(false);

const tabs = [
  { id: 'attractions', name: 'Attractions', icon: 'fas fa-landmark' },
  { id: 'hotels', name: 'Hotels', icon: 'fas fa-hotel' },
  { id: 'trips', name: 'Trips', icon: 'fas fa-bus' },
  { id: 'car-rental', name: 'Car Rental', icon: 'fas fa-car' }
];

// Egyptian cities
const egyptianCities = ref([
  'All Cities',
  'Cairo',
  'Giza',
  'Alexandria',
  'Luxor',
  'Aswan',
  'Sharm El Sheikh',
  'Dahab',
  'Hurghada',
  'Faiyum',
  'Ain Sokhna',
  'El Gouna',
  'Marsa Alam',
  'Port Said',
  'Suez',
  'Ismailia'
]);

const defaultHero = {
  title: 'Discover Egypt — Your Journey Starts Here',
  subtitle: 'Explore ancient wonders, luxury stays, and unforgettable experiences',
  backgroundImage: ''
};

const defaultMetadata = {
  siteName: 'PYRAMIS',
  primaryCtaText: 'View All',
  secondaryCtaText: 'Start AI Planning',
  supportEmail: 'Pyramis@Pyramis.Com',
  supportPhone: '+20 (123) 456-7890',
  country: 'Egypt'
};

const defaultAttractions = [
  { id: 1, name: 'Pyramids of Giza', price: 200, location: 'Cairo', rating: 4.8, reviews: '5,205', featured: false, image: 'https://images.unsplash.com/photo-1572252009286-268acec5ca0a?w=800' },
  { id: 2, name: 'Abu Simbel Temples', price: 200, location: 'Aswan', rating: 4.8, reviews: '5,205', featured: false, image: 'https://images.unsplash.com/photo-1553913861-c0fddf2619ee?w=800' },
  { id: 3, name: 'The Great Pyramid', price: 200, location: 'Cairo', rating: 4.8, reviews: '5,205', featured: true, image: 'https://images.unsplash.com/photo-1568322445389-f64ac2515020?w=800' },
  { id: 4, name: 'Valley of the Kings', price: 200, location: 'Luxor', rating: 4.8, reviews: '5,205', featured: true, image: 'https://images.unsplash.com/photo-1539768942893-daf53e448371?w=800' }
];

const defaultHotels = [
  { id: 1, name: 'Luxor Palace', price: 200, location: 'Cairo', rating: 4.8, reviews: '5,205 reviews', image: 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800' },
  { id: 2, name: 'Luxor Palace', price: 200, location: 'Cairo', rating: 4.8, reviews: '5,205 reviews', image: 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800' },
  { id: 3, name: 'Luxor Palace', price: 200, location: 'Cairo', rating: 4.8, reviews: '5,205 reviews', image: 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800' },
  { id: 4, name: 'Luxor Palace', price: 200, location: 'Cairo', rating: 4.8, reviews: '5,205 reviews', image: 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800' }
];

const defaultTrips = [
  { id: 1, name: 'Pyramids of Giza', price: 200, description: 'A full guided tour covering the Pyramids, Sphinx, and Valley Temple.', location: 'Cairo', duration: '4 Days / 3 Nights', rating: 4.8, reviews: '5,205 reviews', image: 'https://images.unsplash.com/photo-1568322445389-f64ac2515020?w=800' },
  { id: 2, name: 'Luxor Temple Tour', price: 250, description: 'Explore the magnificent temples of Luxor and Karnak.', location: 'Luxor', duration: '3 Days / 2 Nights', rating: 4.7, reviews: '3,420 reviews', image: 'https://images.unsplash.com/photo-1553913861-c0fddf2619ee?w=800' },
  { id: 3, name: 'Red Sea Diving', price: 180, description: 'Dive into crystal clear waters and explore vibrant coral reefs.', location: 'Hurghada', duration: '2 Days / 1 Night', rating: 4.9, reviews: '2,150 reviews', image: 'https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=800' },
  { id: 4, name: 'Desert Safari Adventure', price: 300, description: 'Experience the beauty of the Western Desert with Bedouin guides.', location: 'Siwa', duration: '5 Days / 4 Nights', rating: 4.6, reviews: '1,890 reviews', image: 'https://images.unsplash.com/photo-1509316785289-025f5b846b35?w=800' }
];

const defaultCars = [
  { id: 1, name: 'Luxury Sedan', price: 150, description: 'Comfortable and elegant travel through the streets of Cairo.', image: 'https://images.unsplash.com/photo-1563720360172-67b8f3dce741?w=800' },
  { id: 2, name: 'Family SUV', price: 200, description: 'Great for families exploring Cairo, Giza, and Alexandria.', image: 'https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?w=800' },
  { id: 3, name: 'Convertible Sports', price: 350, description: 'Experience the thrill of driving along the Red Sea coast.', image: 'https://images.unsplash.com/photo-1544636331-e26879cd4d9b?w=800' },
  { id: 4, name: 'Economy Compact', price: 100, description: 'Affordable and fuel-efficient for city exploration.', image: 'https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?w=800' }
];

const defaultTestimonials = [
  { id: 1, name: 'Sarah Mitchell', country: 'United States', text: 'An unforgettable journey through ancient wonders. The pyramids exceeded every expectation, and our guide\'s knowledge brought history to life in the most magical way.' },
  { id: 2, name: 'James Thompson', country: 'United Kingdom', text: 'Luxurious accommodations, impeccable service, and breathtaking sites at every turn. Egypt\'s beauty and rich culture left us absolutely mesmerized.' },
  { id: 3, name: 'Marie Laurent', country: 'France', text: 'From the Nile cruise to the temples of Luxor, every moment was pure perfection. This trip transformed the way we see the world and ancient civilizations.' }
];

const defaultWhyChooseUs = [
  { id: 1, icon: 'fas fa-map', title: 'Complete Trips', description: 'Plan, book, and manage in one flow' },
  { id: 2, icon: 'fas fa-route', title: 'Built for Egypt', description: 'Local travel logic, not global templates.' },
  { id: 3, icon: 'fas fa-plane-departure', title: 'AI Trip Planning', description: 'Personalized itineraries, real data.' },
  { id: 4, icon: 'fas fa-shield-alt', title: 'Clear & Flexible', description: 'No hidden fees, full control.' }
];

const heroSection = ref({ ...defaultHero });
const metadata = ref({ ...defaultMetadata });
const attractions = ref([]);
const hotels = ref([]);
const trips = ref([]);
const cars = ref([]);
const testimonials = ref([]);
const whyChooseUs = ref([]);

const heroDisplayTitle = computed(() => {
  const value = heroSection.value.title || defaultHero.title;
  const parts = value.split('—');

  if (parts.length > 1) {
    const first = parts.shift()?.trim() ?? '';
    const rest = parts.join('—').trim();
    return `${first} —<br/>${rest}`;
  }

  return value;
});

const applyHomeData = (payload) => {
  heroSection.value = { ...defaultHero, ...(payload?.heroSection ?? {}) };
  metadata.value = { ...defaultMetadata, ...(payload?.metadata ?? {}) };

  attractions.value = payload?.popularAttractions?.length ? payload.popularAttractions : defaultAttractions;
  hotels.value = payload?.featuredHotels?.length ? payload.featuredHotels : defaultHotels;
  trips.value = payload?.featuredTrips?.length ? payload.featuredTrips : defaultTrips;
  cars.value = payload?.featuredCars?.length ? payload.featuredCars : defaultCars;
  testimonials.value = payload?.testimonials?.length ? payload.testimonials : defaultTestimonials;
  whyChooseUs.value = payload?.whyChooseUs?.length ? payload.whyChooseUs : defaultWhyChooseUs;
};

// Search handlers
const handleAttractionSearch = (searchData) => {
  console.log('Attraction Search:', searchData);
  router.push({
    name: 'AttractionsList',
    query: {
      city: searchData.city,
      query: searchData.query
    }
  });
};

const handleHotelSearch = (searchData) => {
  console.log('Hotel Search:', searchData);
  router.push({
    name: 'HotelsList',
    query: {
      destination: searchData.destination,
      checkIn: searchData.checkIn,
      checkOut: searchData.checkOut,
      guests: searchData.guests,
      rooms: searchData.rooms
    }
  });
};

const handleTripSearch = (searchData) => {
  console.log('Trip Search:', searchData);
  router.push({
    name: 'TripsList',
    query: {
      from: searchData.pickupLocation,
      pickupDate: searchData.pickupDate,
      dropoffDate: searchData.dropoffDate
    }
  });
};

const handleCarSearch = (searchData) => {
  console.log('Car Search:', searchData);
  router.push({
    name: 'CarsList',
    query: {
      location: searchData.pickupLocation,
      pickupDate: searchData.pickupDate,
      pickupTime: searchData.pickupTime,
      dropoffDate: searchData.dropoffDate,
      dropoffTime: searchData.dropoffTime
    }
  });
};

const handleAiPlanner = () => {
  console.log('Opening AI Trip Planner...');
  router.push({ name: 'AiPlanner' });
};

onMounted(async () => {
  isHomeLoading.value = true;
  try {
    const [data, backgrounds] = await Promise.all([
      getHomePageData(),
      getBackgrounds('HomeHero')
    ]);

    console.log('Backgrounds response:', backgrounds);
    applyHomeData(data);
    if (backgrounds && backgrounds.length > 0 && backgrounds[0].url) {
      console.log('Setting background to:', backgrounds[0].url);
      heroSection.value.backgroundImage = backgrounds[0].url;
    } else {
      console.warn('No backgrounds found or URL is empty');
    }
  } catch (error) {
    console.error('Failed to load home page data', error);
    applyHomeData({});
  } finally {
    isHomeLoading.value = false;
  }
});

const parseReviews = (reviewsStr) => {
  if (typeof reviewsStr === 'number') return reviewsStr;
  if (!reviewsStr) return 0;
  const cleaned = reviewsStr.replace(/,/g, '').replace(/reviews?/i, '').trim();
  return parseInt(cleaned) || 0;
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700;900&display=swap');
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css');

.font-cairo { font-family: 'Cairo', sans-serif; }

/* Tab styling */
.tabs-boxed {
  border-radius: 1rem;
}

.tab {
  border-radius: 0.75rem;
  padding: 0.75rem 1.5rem;
}

@keyframes fade-in {
  from { opacity: 0; transform: translateY(-20px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes slide-up {
  from { opacity: 0; transform: translateY(50px); }
  to { opacity: 1; transform: translateY(0); }
}

.animate-fade-in { animation: fade-in 1s ease-out; }
.animate-fade-in-delay { animation: fade-in 1s ease-out 0.3s both; }
.animate-slide-up { animation: slide-up 1s ease-out 0.6s both; }

/* Smooth transitions */
* {
  transition-property: color, background-color, border-color, transform, box-shadow;
  transition-duration: 300ms;
  transition-timing-function: ease-in-out;
}
</style>