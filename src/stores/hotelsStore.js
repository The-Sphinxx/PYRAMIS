import { defineStore } from 'pinia';
import hotelsApi from '../Services/hotelsApi';

export const useHotelsStore = defineStore('hotels', {
    state: () => ({
        hotels: [],
        selectedHotel: null,
        loading: false,
        error: null,
        filters: {
            city: 'All',
            minRating: 0,
            searchQuery: '',
            amenities: []
        }
    }),

    getters: {
        getHotelById: (state) => (id) => state.hotels.find(h => h.id == id),

        uniqueCities: (state) =>
            [...new Set(state.hotels.map(h => h.city).filter(Boolean))].sort(),

        filteredHotels: (state) => {
            let result = [...state.hotels];

            if (state.filters.city !== 'All') {
                result = result.filter(h => h.city === state.filters.city);
            }

            if (state.filters.minRating > 0) {
                result = result.filter(h => (h.rating || 0) >= state.filters.minRating);
            }

            if (state.filters.searchQuery) {
                const q = state.filters.searchQuery.toLowerCase();
                result = result.filter(h =>
                    (h.name || '').toLowerCase().includes(q) ||
                    (h.description || '').toLowerCase().includes(q) ||
                    (h.city || '').toLowerCase().includes(q)
                );
            }

            if (state.filters.amenities.length > 0) {
                result = result.filter(h => 
                   h.amenities && state.filters.amenities.every(a => h.amenities.includes(a))
                );
            }

            return result;
        }
    },

    actions: {
        async fetchHotels() {
            this.loading = true;
            this.error = null;
            try {
                this.hotels = await hotelsApi.getAllHotels();
            } catch (err) {
                this.error = err.message || 'Failed to fetch hotels';
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

        setFilter(filterType, value) {
            this.filters[filterType] = value;
        },

        toggleAmenity(amenity) {
            const index = this.filters.amenities.indexOf(amenity);
            if (index > -1) this.filters.amenities.splice(index, 1);
            else this.filters.amenities.push(amenity);
        },

        resetFilters() {
            this.filters = {
                city: 'All',
                minRating: 0,
                searchQuery: '',
                amenities: []
            };
        },

        clearSelectedHotel() {
            this.selectedHotel = null;
        }
    }
});
