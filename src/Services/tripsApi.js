// Services/tripsApi.js
import { api } from './api';

const tripsApi = {
    /**
     * Get all trips with optional filtering via Specification Pattern
     * @param {Object} params - Query parameters for filtering
     * @param {number} params.pageIndex - Page number (0-based)
     * @param {number} params.pageSize - Items per page
     * @param {string} params.city - Filter by city
     * @param {string} params.category - Filter by category
     * @param {number} params.minPrice - Minimum price filter
     * @param {number} params.maxPrice - Maximum price filter
     * @param {number} params.minRating - Minimum rating filter
     * @param {string} params.searchQuery - Search text
     * @returns {Promise<Array|Object>} Trips data or paginated result
     */
    async getAllTrips(params = {}) {
        try {
            const response = await api.get('/Trips', { params });
            return response.data;
        } catch (error) {
            console.error('Error fetching trips:', error);
            throw error;
        }
    },

    /**
     * Get trip by ID
     * @param {number} id - Trip ID
     * @returns {Promise<Object>} Trip details
     */
    async getTripById(id) {
        try {
            const response = await api.get(`/Trips/${id}`);
            return response.data;
        } catch (error) {
            console.error(`Error fetching trip ${id}:`, error);
            throw error;
        }
    },

    /**
     * Get filtered trips using Specification Pattern
     * @param {Object} filters - Filter object
     * @param {number} filters.pageIndex - Page number (0-based)
     * @param {number} filters.pageSize - Items per page
     * @param {string} filters.city - City name
     * @param {string} filters.category - Category type
     * @param {number} filters.minPrice - Minimum price
     * @param {number} filters.maxPrice - Maximum price
     * @param {number} filters.minRating - Minimum rating
     * @param {string} filters.searchQuery - Search text
     * @returns {Promise<Array|Object>} Filtered trips
     */
    async getFilteredTrips(filters = {}) {
        try {
            const params = {
                ...(filters.pageIndex !== undefined && { pageIndex: filters.pageIndex }),
                ...(filters.pageSize !== undefined && { pageSize: filters.pageSize }),
                ...(filters.city && filters.city !== 'All' && filters.city !== 'All Cities' && { city: filters.city }),
                ...(filters.category && filters.category !== 'all' && { category: filters.category }),
                ...(filters.minPrice !== undefined && { minPrice: filters.minPrice }),
                ...(filters.maxPrice !== undefined && { maxPrice: filters.maxPrice }),
                ...(filters.minRating !== undefined && { minRating: filters.minRating }),
                ...(filters.searchQuery && { searchQuery: filters.searchQuery })
            };

            const response = await api.get('/Trips', { params });
            return response.data;
        } catch (error) {
            console.error('Error fetching filtered trips:', error);
            throw error;
        }
    },

    /**
     * Get trips by category
     * @param {string} category - Category name
     * @returns {Promise<Array>} Trips in the category
     */
    async getTripsByCategory(category) {
        return this.getFilteredTrips({ category });
    },

    /**
     * Get trips by city/destination
     * @param {string} city - City name
     * @returns {Promise<Array>} Trips in the city
     */
    async getTripsByCity(city) {
        return this.getFilteredTrips({ city });
    },

    /**
     * Search trips
     * @param {string} query - Search query
     * @returns {Promise<Array>} Trips matching search
     */
    async searchTrips(query) {
        if (!query) return this.getAllTrips();
        return this.getFilteredTrips({ searchQuery: query });
    },

    /**
     * Get top rated trips
     * @param {number} limit - Number of results
     * @returns {Promise<Array>} Top rated trips
     */
    async getTopRatedTrips(limit = 8) {
        try {
            const response = await api.get('/Trips', {
                params: { pageSize: limit, minRating: 4.5 }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching top rated trips:', error);
            throw error;
        }
    },

    /**
     * Create booking + PaymentIntent for trips (Payment Element flow)
     * @param {Object} bookingData - { userId, entityId, bookingType, startDate, endDate, totalPrice }
     * @returns {Promise<Object>} { bookingId, clientSecret, paymentIntentId, publishableKey }
     */
    async createBookingIntent(bookingData) {
        try {
            const response = await api.post('/Bookings', bookingData);
            return response.data;
        } catch (error) {
            console.error('Error creating trip booking intent:', error);
            throw error;
        }
    },

    // Backwards-compatible alias
    async bookTrip(tripId, bookingData) {
        return this.createBookingIntent({
            tripId,
            ...bookingData
        });
    }
};

export default tripsApi;
