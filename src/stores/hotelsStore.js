import { defineStore } from 'pinia';
import hotelsApi from '@/Services/hotelsApi';

export const useHotelsStore = defineStore('hotels', {
    state: () => ({
        hotels: [],
        selectedHotel: null,
        loading: false,
        error: null,
        pagination: {
            pageIndex: 0,
            pageSize: 12,
            totalCount: 0,
            totalPages: 0
        },
        filters: {
            searchQuery: '',
            city: 'All',
            category: 'all',
            priceRange: { min: 0, max: 10000 },
            rating: 0,
            minRating: 0, 
            amenities: []
        }
    }),

    getters: {
        getHotelById: (state) => (id) => state.hotels.find(h => h.id == id),

        uniqueCities: (state) => {
            const cities = state.hotels.map(hotel => hotel.city).filter(Boolean);
            return ['All', ...new Set(cities)].sort();
        },

        filteredHotels: (state) => {
            let result = [...state.hotels];

            // Filter by search query
            if (state.filters.searchQuery) {
                const query = state.filters.searchQuery.toLowerCase();
                result = result.filter(hotel =>
                    (hotel.name || '').toLowerCase().includes(query) ||
                    (hotel.city || '').toLowerCase().includes(query) ||
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

            // Filter by rating (using max of rating or minRating for compatibility)
            const targetRating = Math.max(state.filters.rating, state.filters.minRating);
            if (targetRating > 0) {
                result = result.filter(hotel => (hotel.rating || 0) >= targetRating);
            }

            // Filter by amenities
            if (state.filters.amenities.length > 0) {
                result = result.filter(h => 
                   h.amenities && state.filters.amenities.every(a => h.amenities.includes(a))
                );
            }

            return result;
        },

        hotelsByCategory: (state) => (category) => {
            if (category === 'all') return state.hotels;
            return state.hotels.filter(hotel => hotel.category === category);
        },

        hotelsByCity: (state) => (city) => {
            return state.hotels.filter(hotel => hotel.city === city);
        }
    },

    actions: {
        async fetchHotels(params = {}) {
            this.loading = true;
            this.error = null;
            this.hotels = []; // Clear before fetch

            try {
                const data = await hotelsApi.getHotels(params);

                // Transform hotels data to include required fields
                // Handle both paginated response and direct array
                // API returns camelCase: data, pageNumber, pageSize, totalRecords, totalPages
                const hotelsArray = Array.isArray(data) ? data : (data.data || data.Data || data.items || []);

                this.hotels = hotelsArray.map(hotel => ({
                    id: hotel.id,
                    name: hotel.name,
                    description: hotel.description,
                    city: hotel.city,
                    country: hotel.country,
                    category: hotel.category || this.inferCategory(hotel),
                    rating: hotel.rating || 0,
                    pricePerNight: hotel.pricePerNight,
                    images: hotel.images || [],
                    image: hotel.images?.[0],
                    imageUrl: hotel.images?.[0] || '/images/placeholder-hotel.jpg',
                    amenities: hotel.amenities || [],
                    highlights: hotel.highlights || [],
                    address: hotel.address,
                    phone: hotel.phone,
                    email: hotel.email,
                    reviews: hotel.reviews ? {
                        ...hotel.reviews,
                        overallRating: typeof hotel.reviews.overallRating === 'number' ? hotel.reviews.overallRating : (hotel.reviews.averageRating || hotel.rating || 0),
                        totalReviews: hotel.reviews.totalReviews || 0,
                        ratingCriteria: hotel.reviews.ratingCriteria || {}
                    } : { 
                        overallRating: hotel.rating || 0, 
                        totalReviews: 0, 
                        ratingCriteria: {}
                    },
                    totalReviews: hotel.reviews?.totalReviews || 0,
                    // Include original data for flexibility
                    ...hotel
                }));
                
                // Update pagination metadata - API uses camelCase
                this.pagination = {
                    pageIndex: (data.pageNumber ? data.pageNumber - 1 : 0),
                    pageSize: data.pageSize ?? params.pageSize ?? 12,
                    totalCount: data.totalRecords ?? hotelsArray.length,
                    totalPages: data.totalPages ?? 1
                };
                
                return this.pagination;
            } catch (error) {
                console.error('Error fetching hotels:', error);
                this.error = error.message || 'Failed to fetch hotels';
                this.hotels = [];
            } finally {
                this.loading = false;
            }
        },

        async fetchHotelById(id) {
            this.loading = true;
            this.error = null;
            try {
                // Try to find in existing state first
                if (!this.hotels.length) await this.fetchHotels();
                
                let hotel = this.hotels.find(h => h.id == id);
                if (!hotel) {
                    hotel = await hotelsApi.getHotelById(id);
                }
                this.selectedHotel = hotel;
                return hotel;
            } catch (err) {
                this.error = err.message || 'Failed to fetch hotel';
                throw err;
            } finally {
                this.loading = false;
            }
        },

        // Helper: Infer category from hotel data
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

        setFilter(filterType, value) {
            if (this.filters.hasOwnProperty(filterType)) {
                this.filters[filterType] = value;
            }
        },

        // Compatibility alias for setFilters from dev
        setFilters(filters) {
            this.filters = { ...this.filters, ...filters };
        },

        toggleAmenity(amenity) {
            const index = this.filters.amenities.indexOf(amenity);
            if (index > -1) this.filters.amenities.splice(index, 1);
            else this.filters.amenities.push(amenity);
        },

        resetFilters() {
            this.filters = {
                searchQuery: '',
                city: 'All',
                category: 'all',
                priceRange: { min: 0, max: 10000 },
                rating: 0,
                minRating: 0,
                amenities: []
            };
        },

        clearSelectedHotel() {
            this.selectedHotel = null;
        }
    }
});
