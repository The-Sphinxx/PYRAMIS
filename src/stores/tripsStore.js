import { defineStore } from 'pinia';
import tripsApi from '../Services/tripsApi';

export const useTripsStore = defineStore('trips', {
    state: () => ({
        trips: [],
        selectedTrip: null,
        loading: false,
        error: null,
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
        async fetchTrips() {
            this.loading = true;
            this.error = null;
            try {
                this.trips = await tripsApi.getAllTrips();
            } catch (err) {
                this.error = err.message || 'Failed to fetch trips';
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
