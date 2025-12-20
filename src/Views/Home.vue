<template>
  <div>
    <!-- Navbar -->
    <nav class="bg-base-100 shadow-md sticky top-0 z-50">
      <div class="page-container">
        <div class="flex items-center justify-between h-20">
          <!-- Logo -->
          <div class="flex items-center">
            <a href="/" class="flex items-center">
              <span class="text-3xl font-bold text-primary font-cairo">PYRAMIS</span>
            </a>
          </div>

          <!-- Desktop Menu -->
          <div class="hidden md:flex items-center space-x-8">
            <a href="/" class="font-cairo font-semibold text-primary hover:text-primary/80 transition-colors">Home</a>
            <a href="/attractions" class="font-cairo text-base-content hover:text-primary transition-colors">Attractions</a>
            <a href="/hotels" class="font-cairo text-base-content hover:text-primary transition-colors">Hotels</a>
            <a href="/trips" class="font-cairo text-base-content hover:text-primary transition-colors">Trips</a>
            <a href="/cars" class="font-cairo text-base-content hover:text-primary transition-colors">Car Rental</a>
            <a href="/ai-planner" class="font-cairo text-base-content hover:text-primary transition-colors">Ai Trip Planner</a>
          </div>

          <!-- Auth Buttons -->
          <div class="hidden md:flex items-center space-x-4">
            <button class="btn btn-ghost font-cairo">
              <i class="fas fa-user mr-2"></i>
              Login
            </button>
            <button class="btn btn-primary font-cairo">Sign Up</button>
            <button class="btn btn-ghost btn-circle">
              <i class="fas fa-adjust text-xl"></i>
            </button>
          </div>

          <!-- Mobile Menu Button -->
          <button class="md:hidden btn btn-ghost btn-circle" @click="mobileMenuOpen = !mobileMenuOpen">
            <i class="fas fa-bars text-xl"></i>
          </button>
        </div>

        <!-- Mobile Menu -->
        <div v-if="mobileMenuOpen" class="md:hidden py-4 border-t border-base-300">
          <div class="flex flex-col space-y-4">
            <a href="/" class="font-cairo font-semibold text-primary">Home</a>
            <a href="/attractions" class="font-cairo text-base-content">Attractions</a>
            <a href="/hotels" class="font-cairo text-base-content">Hotels</a>
            <a href="/trips" class="font-cairo text-base-content">Trips</a>
            <a href="/cars" class="font-cairo text-base-content">Car Rental</a>
            <a href="/ai-planner" class="font-cairo text-base-content">Ai Trip Planner</a>
            <div class="flex gap-2 pt-4">
              <button class="btn btn-ghost btn-sm font-cairo flex-1">Login</button>
              <button class="btn btn-primary btn-sm font-cairo flex-1">Sign Up</button>
            </div>
          </div>
        </div>
      </div>
    </nav>

    <!-- Hero Section -->
    <div class="relative w-full h-screen overflow-hidden">
      <div class="absolute inset-0">
        <img 
          src="https://images.unsplash.com/photo-1568322445389-f64ac2515020?q=80&w=2070" 
          alt="Egyptian Pyramid" 
          class="w-full h-full object-cover"
        />
        <div class="absolute inset-0 bg-gradient-to-b from-black/40 via-black/30 to-black/50"></div>
      </div>

      <div class="relative z-10 flex flex-col items-center justify-center h-full page-container text-center">
        <h1 class="font-cairo text-5xl md:text-7xl font-bold text-white mb-4 animate-fade-in">
          Discover Egypt —
          <br />
          Your Journey Starts Here
        </h1>
        <p class="font-cairo text-xl md:text-2xl text-white/90 mb-12 animate-fade-in-delay">
          Explore ancient wonders, luxury stays, and unforgettable experiences
        </p>

        <div class="w-full max-w-5xl bg-base-100 rounded-lg shadow-2xl p-6 animate-slide-up">
          <div class="flex justify-center gap-4 mb-6 flex-wrap">
            <button
              v-for="tab in tabs"
              :key="tab.id"
              @click="activeTab = tab.id"
              :class="[
                'btn font-cairo font-semibold transition-all duration-300',
                activeTab === tab.id ? 'btn-primary' : 'btn-ghost hover:bg-base-200'
              ]"
            >
              <i :class="[tab.icon, 'mr-2']"></i>
              {{ tab.name }}
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
                </select>
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Search</label>
                <input type="text" placeholder="Pyramids, Museums..." class="input input-bordered w-full" />
              </div>
              <button class="btn btn-primary self-end font-cairo font-semibold">
                <i class="fas fa-search mr-2"></i>Search
              </button>
            </div>

            <div v-if="activeTab === 'hotels'" class="flex flex-col md:flex-row gap-4">
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Destination</label>
                <input type="text" placeholder="City or hotel name" class="input input-bordered w-full" />
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Check-in</label>
                <input type="date" class="input input-bordered w-full" />
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Check-out</label>
                <input type="date" class="input input-bordered w-full" />
              </div>
              <button class="btn btn-primary self-end font-cairo font-semibold">
                <i class="fas fa-search mr-2"></i>Search
              </button>
            </div>

            <div v-if="activeTab === 'trips'" class="flex flex-col md:flex-row gap-4">
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Pick-up Location</label>
                <input type="text" placeholder="City or station" class="input input-bordered w-full" />
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Pick-up Date</label>
                <input type="date" class="input input-bordered w-full" />
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Drop-off Date</label>
                <input type="date" class="input input-bordered w-full" />
              </div>
              <button class="btn btn-primary self-end font-cairo font-semibold">
                <i class="fas fa-search mr-2"></i>Search
              </button>
            </div>

            <div v-if="activeTab === 'car-rental'" class="flex flex-col md:flex-row gap-4">
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Pick-up Location</label>
                <input type="text" placeholder="City or station" class="input input-bordered w-full" />
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Pick-up Date</label>
                <input type="date" class="input input-bordered w-full" />
              </div>
              <div class="flex-1">
                <label class="block text-sm text-base-content/70 mb-2 font-cairo">Time</label>
                <input type="time" value="10:00" class="input input-bordered w-full" />
              </div>
              <button class="btn btn-primary self-end font-cairo font-semibold">
                <i class="fas fa-search mr-2"></i>Search
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Attractions Section -->
    <div class="w-full bg-base-200 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-4">
            Explore Egypt's Wonders
          </h2>
          <p class="font-cairo text-lg text-base-content/70 max-w-3xl mx-auto">
            Discover ancient treasures and timeless beauty across Egypt's most iconic attractions
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
          <div 
            v-for="attraction in attractions" 
            :key="attraction.id"
            class="group card bg-base-100 shadow-lg hover:shadow-xl transition-all duration-300 transform hover:-translate-y-2 overflow-hidden"
          >
            <figure class="relative h-64 overflow-hidden">
              <img 
                :src="attraction.image" 
                :alt="attraction.name"
                class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
              />
              <div 
                v-if="attraction.featured"
                class="absolute top-4 right-4 bg-base-100/90 backdrop-blur-sm rounded-full p-2"
              >
                <i class="fas fa-sparkles text-primary"></i>
              </div>
            </figure>

            <div class="card-body p-5">
              <div class="flex justify-between items-start mb-3">
                <h3 class="font-cairo text-xl font-bold text-base-content">
                  {{ attraction.name }}
                </h3>
                <span class="font-cairo text-xl font-bold text-primary">
                  {{ attraction.price }}$
                </span>
              </div>

              <div class="flex items-center text-base-content/70 mb-3">
                <i class="fas fa-map-marker-alt text-primary mr-2"></i>
                <span class="font-cairo text-sm">{{ attraction.location }}</span>
              </div>

              <div class="flex items-center">
                <i class="fas fa-star text-warning mr-1"></i>
                <span class="font-cairo font-semibold text-base-content mr-1">
                  {{ attraction.rating }}
                </span>
                <span class="font-cairo text-sm text-base-content/60">
                  ({{ attraction.reviews }} reviews)
                </span>
              </div>
            </div>
          </div>
        </div>

        <div class="text-center">
          <button class="btn btn-primary btn-lg font-cairo font-semibold px-12" @click="handleViewAllAttractions">
            View All
          </button>
        </div>
      </div>
    </div>

    <!-- Hotels Section -->
    <div class="w-full bg-base-100 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-4">
            Luxury Hotels & Resorts
          </h2>
          <p class="font-cairo text-lg text-base-content/70 max-w-3xl mx-auto">
            Experience Egyptian hospitality at its finest in our handpicked selection of premium hotels
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
          <div 
            v-for="hotel in hotels" 
            :key="hotel.id"
            class="group card bg-base-100 shadow-lg hover:shadow-xl transition-all duration-300 transform hover:-translate-y-2 overflow-hidden border border-base-300"
          >
            <figure class="relative h-64 overflow-hidden">
              <img 
                :src="hotel.image" 
                :alt="hotel.name"
                class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
              />
            </figure>

            <div class="card-body p-5">
              <div class="flex justify-between items-start mb-3">
                <h3 class="font-cairo text-xl font-bold text-base-content">
                  {{ hotel.name }}
                </h3>
                <span class="font-cairo text-xl font-bold text-primary">
                  ${{ hotel.price }}
                </span>
              </div>

              <div class="flex items-center text-base-content/70 mb-3">
                <i class="fas fa-map-marker-alt text-primary mr-2"></i>
                <span class="font-cairo text-sm">{{ hotel.location }}</span>
              </div>

              <div class="flex items-center mb-3">
                <i class="fas fa-star text-warning mr-1"></i>
                <span class="font-cairo font-semibold text-base-content mr-1">
                  {{ hotel.rating }}
                </span>
                <span class="font-cairo text-sm text-base-content/60">
                  ({{ hotel.reviews }})
                </span>
              </div>

              <div class="flex flex-wrap gap-3 text-sm">
                <div class="flex items-center text-accent">
                  <i class="fas fa-wifi mr-1"></i>
                  <span class="font-cairo">Wifi</span>
                </div>
                <div class="flex items-center text-accent">
                  <i class="fas fa-swimming-pool mr-1"></i>
                  <span class="font-cairo">Pool</span>
                </div>
                <div class="flex items-center text-accent">
                  <i class="fas fa-dumbbell mr-1"></i>
                  <span class="font-cairo">Gym</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="text-center">
          <button class="btn btn-primary btn-lg font-cairo font-semibold px-12" @click="handleViewAllHotels">
            View All
          </button>
        </div>
      </div>
    </div>

    <!-- Trips & Tours Section -->
    <div class="w-full bg-base-200 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-4">
            Featured Trips & Tours
          </h2>
          <p class="font-cairo text-lg text-base-content/70 max-w-3xl mx-auto">
            All-inclusive packages designed for unforgettable Egyptian adventures
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
          <div 
            v-for="trip in trips" 
            :key="trip.id"
            class="group card bg-base-100 shadow-lg hover:shadow-xl transition-all duration-300 transform hover:-translate-y-2 overflow-hidden"
          >
            <figure class="relative h-64 overflow-hidden">
              <img 
                :src="trip.image" 
                :alt="trip.name"
                class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
              />
            </figure>

            <div class="card-body p-5">
              <div class="flex justify-between items-start mb-3">
                <h3 class="font-cairo text-xl font-bold text-base-content">
                  {{ trip.name }}
                </h3>
                <span class="font-cairo text-xl font-bold text-primary">
                  ${{ trip.price }}
                </span>
              </div>

              <p class="font-cairo text-sm text-base-content/70 mb-3">
                {{ trip.description }}
              </p>

              <div class="flex items-center text-base-content/70 mb-2">
                <i class="fas fa-map-marker-alt text-primary mr-2"></i>
                <span class="font-cairo text-sm">{{ trip.location }}</span>
              </div>

              <div class="flex items-center text-base-content/70 mb-3">
                <i class="fas fa-clock text-primary mr-2"></i>
                <span class="font-cairo text-sm">{{ trip.duration }}</span>
              </div>

              <div class="flex items-center">
                <i class="fas fa-star text-warning mr-1"></i>
                <span class="font-cairo font-semibold text-base-content mr-1">
                  {{ trip.rating }}
                </span>
                <span class="font-cairo text-sm text-base-content/60">
                  ({{ trip.reviews }})
                </span>
              </div>
            </div>
          </div>
        </div>

        <div class="text-center">
          <button class="btn btn-primary btn-lg font-cairo font-semibold px-12" @click="handleViewAllTrips">
            View All
          </button>
        </div>
      </div>
    </div>

    <!-- Car Rental Section -->
    <div class="w-full bg-base-100 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-4">
            Rent Your Perfect Ride
          </h2>
          <p class="font-cairo text-lg text-base-content/70 max-w-3xl mx-auto">
            Explore Egypt at your own pace with our diverse fleet of quality vehicles
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
          <div 
            v-for="car in cars" 
            :key="car.id"
            class="group card bg-base-100 shadow-lg hover:shadow-xl transition-all duration-300 transform hover:-translate-y-2 overflow-hidden border border-base-300"
          >
            <figure class="relative h-64 overflow-hidden bg-neutral">
              <img 
                :src="car.image" 
                :alt="car.name"
                class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
              />
            </figure>

            <div class="card-body p-5">
              <div class="flex justify-between items-start mb-3">
                <h3 class="font-cairo text-xl font-bold text-base-content">
                  {{ car.name }}
                </h3>
                <span class="font-cairo text-xl font-bold text-primary">
                  ${{ car.price }}
                </span>
              </div>

              <p class="font-cairo text-sm text-base-content/70 mb-3">
                {{ car.description }}
              </p>

              <div class="flex flex-wrap gap-3 text-sm">
                <div class="flex items-center text-accent">
                  <i class="fas fa-cog mr-1"></i>
                  <span class="font-cairo">{{ car.transmission }}</span>
                </div>
                <div class="flex items-center text-accent">
                  <i class="fas fa-users mr-1"></i>
                  <span class="font-cairo">{{ car.seats }}</span>
                </div>
                <div class="flex items-center text-accent">
                  <i class="fas fa-compass mr-1"></i>
                  <span class="font-cairo">{{ car.gps }}</span>
                </div>
                <div class="flex items-center text-accent">
                  <i class="fas fa-snowflake mr-1"></i>
                  <span class="font-cairo">{{ car.ac }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="text-center">
          <button class="btn btn-primary btn-lg font-cairo font-semibold px-12" @click="handleViewAllCars">
            View All
          </button>
        </div>
      </div>
    </div>

    <!-- AI Trip Planner Section -->
    <div class="w-full bg-gradient-to-br from-accent/20 via-base-200 to-accent/30 py-20">
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

          <button class="btn btn-lg bg-base-100 hover:bg-base-200 border-accent/30 text-base-content font-cairo font-semibold px-8 shadow-lg" @click="handleStartPlanning">
            <i class="fas fa-magic mr-2 text-primary"></i>
            Start <span class="text-primary">AI Planning</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Testimonials Section -->
    <div class="w-full bg-base-100 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-2">
            What <span class="text-primary">Travelers Say</span>
          </h2>
          <div class="w-24 h-1 bg-primary mx-auto"></div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div 
            v-for="testimonial in testimonials" 
            :key="testimonial.id"
            class="card bg-base-100 shadow-lg border border-base-300 p-6"
          >
            <i class="fas fa-quote-left text-4xl text-primary mb-4"></i>
            
            <div class="flex gap-1 mb-4">
              <i v-for="i in 5" :key="i" class="fas fa-star text-primary"></i>
            </div>

            <p class="font-cairo text-base-content/80 mb-6">
              {{ testimonial.text }}
            </p>

            <div class="mt-auto">
              <h4 class="font-cairo font-bold text-base-content">{{ testimonial.name }}</h4>
              <p class="font-cairo text-sm text-primary">{{ testimonial.country }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Why Choose Us Section -->
    <div class="w-full bg-base-200 py-16">
      <div class="page-container">
        <div class="text-center mb-12">
          <h2 class="font-cairo text-4xl md:text-5xl font-bold text-base-content mb-2">
            Why Travelers <span class="text-primary">Choose Us</span>
          </h2>
          <div class="w-24 h-1 bg-primary mx-auto"></div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
          <div 
            v-for="feature in whyChooseUs" 
            :key="feature.id"
            class="flex flex-col items-center text-center"
          >
            <i :class="[feature.icon, 'text-6xl text-primary mb-4']"></i>
            <h3 class="font-cairo text-xl font-bold text-base-content mb-2">
              {{ feature.title }}
            </h3>
            <p class="font-cairo text-base-content/70">
              {{ feature.description }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Newsletter Section -->
    <div class="w-full bg-base-100 py-16">
      <div class="page-container">
        <div class="text-center max-w-3xl mx-auto">
          <h2 class="font-cairo text-3xl md:text-4xl font-bold text-base-content mb-4">
            Get Exclusive Travel Deals
          </h2>
          <p class="font-cairo text-base-content/70 mb-8">
            Subscribe to receive curated travel ideas, exclusive Egypt-only experiences, special offers, and early access to new destinations and attractions.
          </p>

          <div class="flex flex-col md:flex-row gap-4 max-w-2xl mx-auto">
            <div class="relative flex-1">
              <i class="fas fa-envelope absolute left-4 top-1/2 transform -translate-y-1/2 text-base-content/50"></i>
              <input 
                type="email" 
                placeholder="Enter your email to explore Egypt better"
                class="input input-bordered w-full pl-12"
              />
            </div>
            <button class="btn btn-primary font-cairo font-semibold px-8">
              Subscribe
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Footer -->
    <footer class="bg-base-200 py-12">
      <div class="page-container">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-8 mb-8">
          <!-- Brand -->
          <div class="lg:col-span-1">
            <h3 class="text-2xl font-bold text-primary font-cairo mb-4">PYRAMIS</h3>
            <p class="font-cairo text-base-content/70 mb-4">
              Book your next adventure in Egypt with us!
            </p>
            <div class="space-y-2">
              <div class="flex items-center text-base-content/70">
                <i class="fas fa-map-marker-alt mr-3 text-primary"></i>
                <span class="font-cairo text-sm">456 Nile Avenue, Cairo, Egypt 12345</span>
              </div>
              <div class="flex items-center text-base-content/70">
                <i class="fas fa-phone mr-3 text-primary"></i>
                <span class="font-cairo text-sm">+20 (123) 456-7890</span>
              </div>
              <div class="flex items-center text-base-content/70">
                <i class="fas fa-envelope mr-3 text-primary"></i>
                <span class="font-cairo text-sm">Pyramis@Pyramis.Com</span>
              </div>
            </div>
          </div>

          <!-- Explore -->
          <div>
            <h4 class="font-cairo font-bold text-base-content mb-4">Explore</h4>
            <ul class="space-y-2">
              <li><a href="/hotels" class="font-cairo text-base-content/70 hover:text-primary">Hotels</a></li>
              <li><a href="/trips" class="font-cairo text-base-content/70 hover:text-primary">Trips</a></li>
              <li><a href="/attractions" class="font-cairo text-base-content/70 hover:text-primary">Attractions</a></li>
              <li><a href="/cars" class="font-cairo text-base-content/70 hover:text-primary">Cars</a></li>
            </ul>
          </div>

          <!-- Company -->
          <div>
            <h4 class="font-cairo font-bold text-base-content mb-4">Company</h4>
            <ul class="space-y-2">
              <li><a href="/about" class="font-cairo text-base-content/70 hover:text-primary">About Us</a></li>
              <li><a href="/how-it-works" class="font-cairo text-base-content/70 hover:text-primary">How it Works</a></li>
              <li><a href="/contact" class="font-cairo text-base-content/70 hover:text-primary">Contact</a></li>
            </ul>
          </div>

          <!-- Help & Trust -->
          <div>
            <h4 class="font-cairo font-bold text-base-content mb-4">Help & Trust</h4>
            <ul class="space-y-2">
              <li><a href="/faq" class="font-cairo text-base-content/70 hover:text-primary">FAQ</a></li>
              <li><a href="/support" class="font-cairo text-base-content/70 hover:text-primary">Support</a></li>
              <li><a href="/cancellation" class="font-cairo text-base-content/70 hover:text-primary">Cancellation Policy</a></li>
              <li><a href="/terms" class="font-cairo text-base-content/70 hover:text-primary">Terms & Conditions</a></li>
            </ul>
          </div>

          <!-- Social -->
          <div>
            <h4 class="font-cairo font-bold text-base-content mb-4">Social</h4>
            <ul class="space-y-2">
              <li><a href="/facebook" class="font-cairo text-base-content/70 hover:text-primary">Facebook</a></li>
              <li><a href="/twitter" class="font-cairo text-base-content/70 hover:text-primary">Twitter</a></li>
              <li><a href="/instagram" class="font-cairo text-base-content/70 hover:text-primary">Instagram</a></li>
              <li><a href="/linkedin" class="font-cairo text-base-content/70 hover:text-primary">LinkedIn</a></li>
            </ul>
          </div>
        </div>

        <div class="text-center mt-8">
          <p class="font-cairo text-base-content/70 mb-4">
            &copy; 2025 PYRAMIS. All rights reserved.
          </p>
          <div class="flex justify-center gap-4">
            <a href="/facebook" class="text-base-content/70 hover:text-primary"><i class="fab fa-facebook-f"></i></a>
            <a href="/twitter" class="text-base-content/70 hover:text-primary"><i class="fab fa-twitter"></i></a>
            <a href="/instagram" class="text-base-content/70 hover:text-primary"><i class="fab fa-instagram"></i></a>
            <a href="/linkedin" class="text-base-content/70 hover:text-primary"><i class="fab fa-linkedin-in"></i></a>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const activeTab = ref('attractions');
const mobileMenuOpen = ref(false);

const tabs = [
  { id: 'attractions', name: 'Attractions', icon: 'fas fa-landmark' },
  { id: 'hotels', name: 'Hotels', icon: 'fas fa-hotel' },
  { id: 'trips', name: 'Trips', icon: 'fas fa-bus' },
  { id: 'car-rental', name: 'Car Rental', icon: 'fas fa-car' }
];

const attractions = ref([
  {
    id: 1,
    name: 'Pyramids of Giza',
    price: 200,
    location: 'Cairo',
    rating: 4.8,
    reviews: '5,205 reviews',
    image: 'https://images.unsplash.com/photo-1572252009286-268acec5ca0a?q=80&w=800',
    featured: false
  },
  {
    id: 2,
    name: 'Abu Simbel Temples',
    price: 200,
    location: 'Aswan',
    rating: 4.8,
    reviews: '5,205 reviews',
    image: 'https://images.unsplash.com/photo-1553913861-c0fddf2619ee?q=80&w=800',
    featured: false
  },
  {
    id: 3,
    name: 'The Great Pyramid',
    price: 200,
    location: 'Cairo',
    rating: 4.8,
    reviews: '5,205 reviews',
    image: 'https://images.unsplash.com/photo-1568322445389-f64ac2515020?q=80&w=800',
    featured: true
  },
  {
    id: 4,
    name: 'Valley of the Kings',
    price: 200,
    location: 'Luxor',
    rating: 4.8,
    reviews: '5,205 reviews',
    image: 'https://images.unsplash.com/photo-1539768942893-daf53e448371?q=80&w=800',
    featured: true
  }
]);

const cars = ref([
  {
    id: 1,
    name: 'Family SUV',
    price: 200,
    description: 'Great for families exploring Cairo, Giza, and Alexandria.',
    transmission: 'Auto',
    seats: '7 Seats',
    gps: 'GPS',
    ac: 'A/C',
    image: 'https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?q=80&w=800'
  },
  {
    id: 2,
    name: 'Family SUV',
    price: 200,
    description: 'Great for families exploring Cairo, Giza, and Alexandria.',
    transmission: 'Auto',
    seats: '7 Seats',
    gps: 'GPS',
    ac: 'A/C',
    image: 'https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?q=80&w=800'
  },
  {
    id: 3,
    name: 'Family SUV',
    price: 200,
    description: 'Great for families exploring Cairo, Giza, and Alexandria.',
    transmission: 'Auto',
    seats: '7 Seats',
    gps: 'GPS',
    ac: 'A/C',
    image: 'https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?q=80&w=800'
  },
  {
    id: 4,
    name: 'Family SUV',
    price: 200,
    description: 'Great for families exploring Cairo, Giza, and Alexandria.',
    transmission: 'Auto',
    seats: '7 Seats',
    gps: 'GPS',
    ac: 'A/C',
    image: 'https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?q=80&w=800'
  }
]);

const hotels = ref([
  {
    id: 1,
    name: 'Luxor Palace',
    price: 200,
    location: 'Cairo',
    rating: 4.8,
    reviews: '5,205 reviews',
    image: 'https://images.unsplash.com/photo-1566073771259-6a8506099945?q=80&w=800'
  },
  {
    id: 2,
    name: 'Luxor Palace',
    price: 200,
    location: 'Cairo',
    rating: 4.8,
    reviews: '5,205 reviews',
    image: 'https://images.unsplash.com/photo-1566073771259-6a8506099945?q=80&w=800'
  },
  {
    id: 3,
    name: 'Luxor Palace',
    price: 200,
    location: 'Cairo',
    rating: 4.8,
    reviews: '5,205 reviews',
    image: 'https://images.unsplash.com/photo-1566073771259-6a8506099945?q=80&w=800'
  },
  {
    id: 4,
    name: 'Luxor Palace',
    price: 200,
    location: 'Cairo',
    rating: 4.8,
    reviews: '5,205 reviews',
    image: 'https://images.unsplash.com/photo-1566073771259-6a8506099945?q=80&w=800'
  }
]);

const trips = ref([
  {
    id: 1,
    name: 'Pyramids',
    price: 200,
    description: 'A full guided tour covering the Pyramids, Sphinx, and Valley Temple.',
    location: 'Cairo',
    duration: '4 Days / 3 Nights',
    rating: 4.8,
    reviews: '5,205 reviews',
    image: 'https://images.unsplash.com/photo-1568322445389-f64ac2515020?q=80&w=800'
  },
  {
    id: 2,
    name: 'Pyramids',
    price: 200,
    description: 'A full guided tour covering the Pyramids, Sphinx, and Valley Temple.',
    location: 'Cairo',
    duration: '4 Days / 3 Nights',
    rating: 4.8,
    reviews: '5,205 reviews',
    image: 'https://images.unsplash.com/photo-1568322445389-f64ac2515020?q=80&w=800'
  },
  {
    id: 3,
    name: 'Pyramids',
    price: 200,
    description: 'A full guided tour covering the Pyramids, Sphinx, and Valley Temple.',
    location: 'Cairo',
    duration: '4 Days / 3 Nights',
    rating: 4.8,
    reviews: '5,205 reviews',
    image: 'https://images.unsplash.com/photo-1568322445389-f64ac2515020?q=80&w=800'
  },
  {
    id: 4,
    name: 'Pyramids',
    price: 200,
    description: 'A full guided tour covering the Pyramids, Sphinx, and Valley Temple.',
    location: 'Cairo',
    duration: '4 Days / 3 Nights',
    rating: 4.8,
    reviews: '5,205 reviews',
    image: 'https://images.unsplash.com/photo-1568322445389-f64ac2515020?q=80&w=800'
  }
]);

const testimonials = ref([
  {
    id: 1,
    text: "This was the trip of a lifetime! The guides were knowledgeable and the organization was flawless.",
    name: "Sarah Johnson",
    country: "USA"
  },
  {
    id: 2,
    text: "Beautiful hotels and comfortable transport. I felt safe and welcomed everywhere I went.",
    name: "James Wilson",
    country: "UK"
  },
  {
    id: 3,
    text: "The Nile cruise was magical. Watching the sunset over the temples is something I'll never forget.",
    name: "Maria Garcia",
    country: "Spain"
  }
]);

const whyChooseUs = ref([
  {
    id: 1,
    icon: "fas fa-medal",
    title: "Best Price Guarantee",
    description: "We match any competitor's price for identical packages."
  },
  {
    id: 2,
    icon: "fas fa-user-shield",
    title: "Trusted & Secure",
    description: "Your safety and security are our top priority throughout the journey."
  },
  {
    id: 3,
    icon: "fas fa-headset",
    title: "24/7 Support",
    description: "Our dedicated support team is available around the clock to assist you."
  },
  {
    id: 4,
    icon: "fas fa-star",
    title: "Top-Rated Guides",
    description: "Explore with certified Egyptologists who bring history to life."
  }
]);

const handleViewAllAttractions = () => {
  console.log('View all attractions clicked');
};

const handleViewAllCars = () => {
  console.log('View all cars clicked');
};

const handleStartPlanning = () => {
  console.log('Start AI planning clicked');
};

const handleViewAllHotels = () => {
  console.log('View all hotels clicked');
};

const handleViewAllTrips = () => {
  console.log('View all trips clicked');
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700;900&display=swap');
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css');

.font-cairo {
  font-family: 'Cairo', sans-serif;
}

@keyframes fade-in {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes slide-up {
  from {
    opacity: 0;
    transform: translateY(50px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fade-in 1s ease-out;
}

.animate-fade-in-delay {
  animation: fade-in 1s ease-out 0.3s both;
}

.animate-slide-up {
  animation: slide-up 1s ease-out 0.6s both;
}
</style>