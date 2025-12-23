<template>
  <div>
    <Navbar />

    <div class="relative w-full h-screen overflow-hidden">
      <div class="absolute inset-0 bg-cover bg-center" :style="{ backgroundImage: `url(${heroSection.backgroundImage || defaultHero.backgroundImage})` }">
        <div class="absolute inset-0 bg-gradient-to-b from-black/40 via-black/30 to-black/50"></div>
      </div>
      
      <div class="relative z-10 flex flex-col items-center justify-center h-full page-container text-center">
        <h1 class="font-cairo text-5xl md:text-7xl font-bold text-white mb-4 animate-fade-in" v-html="heroDisplayTitle"></h1>
        <p class="font-cairo text-xl md:text-2xl text-white/90 mb-12 animate-fade-in-delay">
          {{ heroSection.subtitle || defaultHero.subtitle }}
        </p>

        <div class="w-full max-w-5xl bg-base-100 rounded-lg shadow-2xl p-6 animate-slide-up">
          <div class="flex justify-center gap-4 mb-6 flex-wrap">
            <button v-for="tab in tabs" :key="tab.id" @click="activeTab = tab.id" 
              :class="['btn font-cairo font-semibold transition-all', activeTab === tab.id ? 'btn-primary' : 'btn-ghost hover:bg-base-200']">
              <i :class="[tab.icon, 'mr-2']"></i>{{ tab.name }}
            </button>
          </div>

          <div class="bg-base-100 rounded-lg p-4">
            <div v-if="activeTab === 'attractions'" class="flex flex-col md:flex-row gap-4">
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">City</label>
                <select class="select select-bordered w-full">
                  <option>All Cities</option>
                  <option>Cairo</option>
                  <option>Luxor</option>
                  <option>Aswan</option>
                  <option>Alexandria</option>
                </select>
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Search</label>
                <input type="text" placeholder="Pyramids, Museums..." class="input input-bordered w-full"/>
              </div>
              <button class="btn btn-primary self-end font-cairo font-semibold">
                <i class="fas fa-search mr-2"></i>Search
              </button>
            </div>

            <div v-if="activeTab === 'hotels'" class="flex flex-col md:flex-row gap-4">
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Destination</label>
                <input type="text" placeholder="City or hotel name" class="input input-bordered w-full"/>
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Check-in</label>
                <input type="date" class="input input-bordered w-full"/>
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Check-out</label>
                <input type="date" class="input input-bordered w-full"/>
              </div>
              <button class="btn btn-primary self-end font-cairo font-semibold">
                <i class="fas fa-search mr-2"></i>Search
              </button>
            </div>

            <div v-if="activeTab === 'trips'" class="flex flex-col md:flex-row gap-4">
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Pick-up Location</label>
                <input type="text" placeholder="City or station" class="input input-bordered w-full"/>
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Pick-up Date</label>
                <input type="date" class="input input-bordered w-full"/>
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Drop-off Date</label>
                <input type="date" class="input input-bordered w-full"/>
              </div>
              <button class="btn btn-primary self-end font-cairo font-semibold">
                <i class="fas fa-search mr-2"></i>Search
              </button>
            </div>

            <div v-if="activeTab === 'car-rental'" class="flex flex-col md:flex-row gap-4">
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Pick-up Location</label>
                <input type="text" placeholder="City or station" class="input input-bordered w-full"/>
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Pick-up Date</label>
                <input type="date" class="input input-bordered w-full"/>
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Time</label>
                <input type="time" value="10:00" class="input input-bordered w-full"/>
              </div>
              <button class="btn btn-primary self-end font-cairo font-semibold">
                <i class="fas fa-search mr-2"></i>Search
              </button>
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
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
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
          <button class="btn btn-primary font-cairo font-semibold w-64 h-12 min-h-0 text-lg shadow-md hover:shadow-xl hover:-translate-y-1 transition-all">View All</button>
        </div>
      </div>
    </section>

    <section class="bg-base-100 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-4">Luxury Hotels & Resorts</h2>
          <p class="font-cairo text-lg text-base-content/70 max-w-3xl mx-auto">Experience Egyptian hospitality at its finest in our handpicked selection of premium hotels</p>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
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
          <button class="btn btn-primary font-cairo font-semibold w-64 h-12 min-h-0 text-lg shadow-md hover:shadow-xl hover:-translate-y-1 transition-all">View All</button>
        </div>
      </div>
    </section>

    <section class="bg-base-200 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-4">Featured Trips & Tours</h2>
          <p class="font-cairo text-lg text-base-content/70 max-w-3xl mx-auto">All-inclusive packages designed for unforgettable Egyptian adventures</p>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
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
          <button class="btn btn-primary font-cairo font-semibold w-64 h-12 min-h-0 text-lg shadow-md hover:shadow-xl hover:-translate-y-1 transition-all">View All</button>
        </div>
      </div>
    </section>

    <section class="bg-base-100 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-4">Rent Your Perfect Ride</h2>
          <p class="font-cairo text-lg text-base-content/70 max-w-3xl mx-auto">Explore Egypt at your own pace with our diverse fleet of quality vehicles</p>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
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
          <button class="btn btn-primary font-cairo font-semibold w-64 h-12 min-h-0 text-lg shadow-md hover:shadow-xl hover:-translate-y-1 transition-all">View All</button>
        </div>
      </div>
    </section>

    <section class="bg-gradient-to-br from-accent/20 via-base-200 to-accent/30 py-20">
      <div class="page-container">
        <div class="flex flex-col items-center justify-center text-center">
          <div class="badge badge-lg bg-base-100 border-accent/30 mb-6 px-4 py-3">
            <i class="fas fa-sparkles text-primary mr-2"></i>
            <span class="font-cairo text-base-content">Powered by AI</span>
          </div>
          <h2 class="font-cairo text-4xl md:text-6xl font-bold text-base-content mb-6">
            Plan Your <span class="text-primary">Trip with AI</span>
          </h2>
          <p class="font-cairo text-xl text-base-content/80 max-w-3xl mx-auto mb-10">
            Tell our smart assistant what you want, and it builds a full personalized itinerary.
          </p>
          <button class="btn btn-lg bg-base-100 hover:bg-base-200 border-accent/30 text-base-content font-cairo font-semibold px-8 shadow-lg">
            <i class="fas fa-magic mr-2 text-primary"></i>
            Start <span class="text-primary">AI Planning</span>
          </button>
        </div>
      </div>
    </section>

    <section class="bg-base-100 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-2">
            What <span class="text-primary">Travelers Say</span>
          </h2>
          <div class="w-24 h-1 bg-primary mx-auto"></div>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div v-for="item in testimonials" :key="item.id" class="card bg-base-100 shadow-lg border border-base-300 p-6">
            <i class="fas fa-quote-left text-4xl text-primary mb-4"></i>
            <div class="flex gap-1 mb-4">
              <i v-for="i in 5" :key="i" class="fas fa-star text-primary"></i>
            </div>
            <p class="font-cairo text-base-content/80 mb-6">{{item.text}}</p>
            <div class="mt-auto">
              <h4 class="font-cairo font-bold text-base-content">{{item.name}}</h4>
              <p class="font-cairo text-sm text-primary">{{item.country}}</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <section class="bg-base-200 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-2">
            Why Travelers <span class="text-primary">Choose Us</span>
          </h2>
          <div class="w-24 h-1 bg-primary mx-auto"></div>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
          <div v-for="item in whyChooseUs" :key="item.id" class="flex flex-col items-center text-center">
            <i :class="[item.icon, 'text-6xl text-primary mb-4']"></i>
            <h3 class="font-cairo text-xl font-bold text-base-content mb-2">{{item.title}}</h3>
            <p class="font-cairo text-base-content/70">{{item.description}}</p>
          </div>
        </div>
      </div>
    </section>

    <section class="bg-base-100 py-16">
      <div class="page-container">
        <div class="text-center max-w-3xl mx-auto">
          <h2 class="font-cairo text-3xl md:text-4xl font-bold text-base-content mb-4">Get Exclusive Travel Deals</h2>
          <p class="font-cairo text-base-content/70 mb-8">Subscribe to receive curated travel ideas, exclusive Egypt-only experiences, special offers, and early access to new destinations and attractions.</p>
          <div class="flex flex-col md:flex-row gap-4 max-w-2xl mx-auto">
            <div class="relative flex-1">
              <i class="fas fa-envelope absolute left-4 top-1/2 transform -translate-y-1/2 text-base-content/50"></i>
              <input type="email" placeholder="Enter your email to explore Egypt better" class="input input-bordered w-full pl-12"/>
            </div>
            <button class="btn btn-primary font-cairo font-semibold px-8">Subscribe</button>
          </div>
        </div>
      </div>
    </section>

    <Footer />
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue';
import { getHomePageData } from '@/Services/homeService';
import Navbar from '@/components/Common/Navbar.vue';
import Footer from '@/components/Common/Footer.vue';
import AttractionCard from '@/components/Attractions/AttractionCard.vue';
import HotelCard from '@/components/Hotels/HotelCard.vue';
import TripCard from '@/components/Trips/TripCard.vue';
import CarCard from '@/components/Cars/CarCard.vue';

const activeTab = ref('attractions');
const isHomeLoading = ref(false);

const tabs = [
  { id: 'attractions', name: 'Attractions', icon: 'fas fa-landmark' },
  { id: 'hotels', name: 'Hotels', icon: 'fas fa-hotel' },
  { id: 'trips', name: 'Trips', icon: 'fas fa-bus' },
  { id: 'car-rental', name: 'Car Rental', icon: 'fas fa-car' }
];

const defaultHero = {
  title: 'Discover Egypt — Your Journey Starts Here',
  subtitle: 'Explore ancient wonders, luxury stays, and unforgettable experiences',
  backgroundImage: ''
};
  import { getBackgrounds } from '@/Services/systemService';

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
  { id: 1, name: 'Pyramids', price: 200, description: 'A full guided tour covering the Pyramids, Sphinx, and Valley Temple.', location: 'Cairo', duration: '4 Days / 3 Nights', rating: 4.8, reviews: '5,205 reviews', image: 'https://images.unsplash.com/photo-1568322445389-f64ac2515020?w=800' },
  { id: 2, name: 'Pyramids', price: 200, description: 'A full guided tour covering the Pyramids, Sphinx, and Valley Temple.', location: 'Cairo', duration: '4 Days / 3 Nights', rating: 4.8, reviews: '5,205 reviews', image: 'https://images.unsplash.com/photo-1568322445389-f64ac2515020?w=800' },
  { id: 3, name: 'Pyramids', price: 200, description: 'A full guided tour covering the Pyramids, Sphinx, and Valley Temple.', location: 'Cairo', duration: '4 Days / 3 Nights', rating: 4.8, reviews: '5,205 reviews', image: 'https://images.unsplash.com/photo-1568322445389-f64ac2515020?w=800' },
  { id: 4, name: 'Pyramids', price: 200, description: 'A full guided tour covering the Pyramids, Sphinx, and Valley Temple.', location: 'Cairo', duration: '4 Days / 3 Nights', rating: 4.8, reviews: '5,205 reviews', image: 'https://images.unsplash.com/photo-1568322445389-f64ac2515020?w=800' }
];

const defaultCars = [
  { id: 1, name: 'Family SUV', price: 200, description: 'Great for families exploring Cairo, Giza, and Alexandria.', image: 'https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?w=800' },
  { id: 2, name: 'Family SUV', price: 200, description: 'Great for families exploring Cairo, Giza, and Alexandria.', image: 'https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?w=800' },
  { id: 3, name: 'Family SUV', price: 200, description: 'Great for families exploring Cairo, Giza, and Alexandria.', image: 'https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?w=800' },
  { id: 4, name: 'Family SUV', price: 200, description: 'Great for families exploring Cairo, Giza, and Alexandria.', image: 'https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?w=800' }
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
</style>