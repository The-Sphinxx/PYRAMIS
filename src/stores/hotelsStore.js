import { defineStore } from 'pinia';
import hotelsApi from '@/Services/hotelsApi';

export const useHotelStore = defineStore('hotels', {
    state: () => ({
        hotels: [],
        loading: false,
        error: null,
        filters: {
            searchQuery: '',
            city: '',
            category: 'all',
            priceRange: { min: 0, max: 10000 },
            rating: 0
        }
    }),

    getters: {
        // Get unique cities from all hotels
        uniqueCities: (state) => {
            const cities = state.hotels.map(hotel => hotel.city);
            return [...new Set(cities)].sort();
        },

        // Get filtered hotels based on current filters logic is now handled in store getters for reactivity
        // mimicking the previous logic to ensure UI stays consistent
        filteredHotels: (state) => {
            let result = [...state.hotels];

            // Filter by search query
            if (state.filters.searchQuery) {
                const query = state.filters.searchQuery.toLowerCase();
                result = result.filter(hotel =>
                    hotel.name.toLowerCase().includes(query) ||
                    hotel.city.toLowerCase().includes(query) ||
                    (hotel.highlights && hotel.highlights.some(h => h.toLowerCase().includes(query)))
                );
            }

            // Filter by city
            if (state.filters.city && state.filters.city !== 'All' && state.filters.city !== 'All Cities') {
                result = result.filter(hotel => hotel.city === state.filters.city);
            }

            // Filter by category
            if (state.filters.category && state.filters.category !== 'all') {
                result = result.filter(hotel => {
                    if (!hotel.category) return false;
                    return hotel.category === state.filters.category;
                });
            }

            // Filter by price range
            if (state.filters.priceRange) {
                result = result.filter(hotel =>
                    hotel.pricePerNight >= state.filters.priceRange.min &&
                    hotel.pricePerNight <= state.filters.priceRange.max
                );
            }

            // Filter by minimum rating
            if (state.filters.rating > 0) {
                result = result.filter(hotel => hotel.rating >= state.filters.rating);
            }

            return result;
        },

        // Get hotels by category
        hotelsByCategory: (state) => (category) => {
            if (category === 'all') return state.hotels;
            return state.hotels.filter(hotel => hotel.category === category);
        },

        // Get hotels by city
        hotelsByCity: (state) => (city) => {
            return state.hotels.filter(hotel => hotel.city === city);
        }
    },

    actions: {
        // Fetch all hotels using API service
        async fetchHotels() {
            this.loading = true;
            this.error = null;

            try {
                const data = await hotelsApi.getHotels();

                // Transform hotels data to include required fields
                this.hotels = data.map(hotel => ({
                    ...hotel,
                    imageUrl: hotel.images && hotel.images.length > 0 ? hotel.images[0] : '/images/placeholder-hotel.jpg',
                    totalReviews: hotel.reviews?.totalReviews || 0,
                    // Assign category based on hotel characteristics if not present
                    category: hotel.category || this.inferCategory(hotel)
                }));
            } catch (error) {
                console.error('Error fetching hotels:', error);
                this.error = error.message;
                this.hotels = [];
            } finally {
                this.loading = false;
            }
        },

        // Infer category from hotel data
        inferCategory(hotel) {
            if (hotel.rating >= 4.8) return 'LUXURY';
            if (hotel.rating >= 4.5) return 'FIVE_STAR';
            if (hotel.pricePerNight < 100) return 'BUDGET';
            if (hotel.amenities?.some(a => a.toLowerCase().includes('spa') || a.toLowerCase().includes('pool'))) {
                return 'RESORT';
            }
            if (hotel.amenities?.some(a => a.toLowerCase().includes('beach'))) {
                return 'BEACH';
            }
            return 'LUXURY';
        },

        // Set individual filter
        setFilter(filterName, value) {
            if (this.filters.hasOwnProperty(filterName)) {
                this.filters[filterName] = value;
            }
        },

        // Set multiple filters at once
        setFilters(filters) {
            this.filters = { ...this.filters, ...filters };
        },

        // Reset all filters to default
        resetFilters() {
            this.filters = {
                searchQuery: '',
                city: '',
                category: 'all',
                priceRange: { min: 0, max: 10000 },
                rating: 0
            };
        },

        // Get hotel by ID
        getHotelById(id) {
            return this.hotels.find(hotel => hotel.id === id || hotel.id === String(id));
        }
    }
});
