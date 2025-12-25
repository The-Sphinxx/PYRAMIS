import { defineStore } from 'pinia';
import tripsApi from '../Services/tripsApi';

export const useTripsStore = defineStore('trips', {
    state: () => ({
        trips: [],
        selectedTrip: null,
        loading: false,
        error: null,
        pagination: {
            pageIndex: 0,
            pageSize: 12,
            totalCount: 0,
            totalPages: 0
        },
        filters: {
            categories: [],
            city: 'All',
            minRating: 0,
            searchQuery: ''
        }
    }),

    getters: {
        getTripById: (state) => (id) => state.trips.find(t => t.id == id),

        uniqueCities: (state) =>
            [...new Set(state.trips.map(t => t.city).filter(Boolean))].sort(),

        allCategories: (state) => {
            const set = new Set();
            state.trips.forEach(t => t.categories?.forEach(c => set.add(c)));
            return [...set].sort();
        },

        topRatedTrips: (state) =>
            [...state.trips]
                .sort((a, b) => (b.rating || 0) - (a.rating || 0))
                .slice(0, 8),

        filteredTrips: (state) => {
            let result = [...state.trips];

            if (state.filters.categories.length) {
                result = result.filter(t =>
                    t.categories && state.filters.categories.some(c => t.categories.includes(c))
                );
            }

            if (state.filters.city !== 'All') {
                result = result.filter(t => t.city === state.filters.city);
            }

            if (state.filters.minRating > 0) {
                result = result.filter(t => (t.rating || 0) >= state.filters.minRating);
            }

            if (state.filters.searchQuery) {
                const q = state.filters.searchQuery.toLowerCase();
                result = result.filter(t =>
                    (t.title || '').toLowerCase().includes(q) ||
                    (t.description || '').toLowerCase().includes(q) ||
                    (t.city || '').toLowerCase().includes(q)
                );
            }

            return result;
        }
    },

    actions: {
        async fetchTrips(params = {}) {
            this.loading = true;
            this.error = null;
            this.trips = []; // Clear before fetch
            try {
                const data = await tripsApi.getAllTrips(params);
                
                // Handle both paginated response and direct array
                // API returns camelCase: data, pageNumber, pageSize, totalRecords, totalPages
                const tripsArray = Array.isArray(data) ? data : (data.data || data.Data || data.items || []);

                // Transform trips data - map PascalCase API response to camelCase
                this.trips = tripsArray.map(trip => ({
                    id: trip.id,
                    title: trip.title,
                    description: trip.description,
                    city: trip.city,
                    country: trip.country,
                    category: trip.category,
                    categories: trip.categories || [],
                    pricePerDay: trip.pricePerDay,
                    totalPrice: trip.totalPrice,
                    images: trip.images || [],
                    image: trip.images?.[0],
                    imageUrl: trip.images?.[0] || '/images/placeholder-trip.jpg',
                    duration: trip.duration,
                    rating: trip.rating || 0,
                    reviews: trip.reviews || { totalReviews: 0 },
                    totalReviews: trip.reviews?.totalReviews || 0,
                    highlights: trip.highlights || [],
                    itinerary: trip.itinerary || [],
                    // Include original data for flexibility
                    ...trip
                }));
                
                // Update pagination metadata
                this.pagination = {
                    pageIndex: (data.pageNumber ? data.pageNumber - 1 : 0),
                    pageSize: data.pageSize ?? params.pageSize ?? 12,
                    totalCount: data.totalRecords ?? tripsArray.length,
                    totalPages: data.totalPages ?? 1
                };
                
                return this.pagination;
            } catch (err) {
                this.error = err.message || 'Failed to fetch trips';
                console.error('Error fetching trips:', err);
            } finally {
                this.loading = false;
            }
        },

        async fetchTripById(id) {
            this.loading = true;
            this.error = null;
            try {
                if (!this.trips.length) await this.fetchTrips();
                let trip = this.trips.find(t => t.id == id);
                if (!trip) {
                    trip = await tripsApi.getTripById(id);
                }
                this.selectedTrip = trip;
                return trip;
            } catch (err) {
                this.error = err.message || 'Failed to fetch trip';
                throw err;
            } finally {
                this.loading = false;
            }
        },

        setFilter(filterType, value) {
            this.filters[filterType] = value;
        },

        toggleCategory(category) {
            const index = this.filters.categories.indexOf(category);
            if (index > -1) this.filters.categories.splice(index, 1);
            else this.filters.categories.push(category);
        },

        resetFilters() {
            this.filters = {
                categories: [],
                city: 'All',
                minRating: 0,
                searchQuery: ''
            };
        },

        clearSelectedTrip() {
            this.selectedTrip = null;
        }
    }
});
