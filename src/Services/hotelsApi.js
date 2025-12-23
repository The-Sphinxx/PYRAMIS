import api from './api';

const hotelsApi = {
    // Get all hotels
    async getAllHotels() {
        try {
            const response = await api.get('/Hotels');
            return response.data;
        } catch (error) {
            console.error('Error fetching hotels:', error);
            throw new Error('Failed to fetch hotels');
        }
    },

    // Alias for getHotels (used by dev branch store)
    async getHotels() {
        return this.getAllHotels();
    },

    // Get hotel by ID
    async getHotelById(id) {
        try {
            const response = await api.get(`/Hotels/${id}`);
            return response.data;
        } catch (error) {
            console.error(`Error fetching hotel ${id}:`, error);
            throw new Error('Failed to fetch hotel details');
        }
    },

    // Get hotels by city
    async getHotelsByCity(city) {
        try {
            const response = await api.get('/Hotels', {
                params: { city }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching hotels by city:', error);
            throw new Error('Failed to fetch hotels by city');
        }
    },

    // Search hotels
    async searchHotels(query) {
        try {
            // First try API search if supported
            /* 
            const response = await axios.get(`${API_BASE_URL}/hotels`, {
                params: { q: query }
            });
            return response.data; 
            */
            
            // Fallback to client-side filtering to match robust dev logic
            const hotels = await this.getHotels();
            if (!query) return hotels;

            const lowerQuery = query.toLowerCase();
            return hotels.filter(hotel =>
                hotel.name.toLowerCase().includes(lowerQuery) ||
                hotel.city.toLowerCase().includes(lowerQuery) ||
                (hotel.highlights && hotel.highlights.some(h => h.toLowerCase().includes(lowerQuery)))
            );
        } catch (error) {
            console.error('Error searching hotels:', error);
            throw new Error('Failed to search hotels');
        }
    },

    // Get filtered hotels (Combines dev logic with API fetch)
    async getFilteredHotels(filters) {
        try {
            // Fetch all and filter client side for complex logic, or build params
             let results = await this.getHotels();

             if (filters) { // Apply client side filtering from dev branch
                // City filter
                if (filters.city && filters.city !== 'All' && filters.city !== 'All Cities') {
                    results = results.filter(h => h.city === filters.city);
                }

                // Category filter
                if (filters.category && filters.category !== 'all') {
                    results = results.filter(h => h.category === filters.category);
                }

                // Price filter
                if (filters.priceRange) {
                    results = results.filter(h =>
                        h.pricePerNight >= filters.priceRange.min &&
                        h.pricePerNight <= filters.priceRange.max
                    );
                }

                 // Rating filter
                if (filters.rating) {
                    results = results.filter(h => h.rating >= filters.rating);
                }
             }
            return results;
        } catch (error) {
            console.error('Error fetching filtered hotels:', error);
            throw new Error('Failed to fetch filtered hotels');
        }
    },

    // Alias for filterHotels
    async filterHotels(filters) {
        return this.getFilteredHotels(filters);
    },

    // Get top rated hotels
    async getTopRatedHotels(limit = 8) {
        try {
            const response = await api.get('/Hotels/top-rated', {
                params: { limit }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching top rated hotels:', error);
            throw new Error('Failed to fetch top rated hotels');
        }
    },

    // Book hotel (POST to bookings endpoint)
    async bookHotel(bookingData) {
        try {
            const response = await api.post('/Bookings', {
                type: 'hotel',
                ...bookingData,
            });
            return response.data;
        } catch (error) {
            console.error('Error booking hotel:', error);
            throw new Error('Failed to book hotel');
        }
    }
};

export default hotelsApi;
