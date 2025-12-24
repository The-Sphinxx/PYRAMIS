import { api } from './api';

const hotelsApi = {
    /**
     * Get all hotels with optional filtering via Specification Pattern
     * @param {Object} params - Query parameters for filtering
     * @param {number} params.pageIndex - Page number (0-based)
     * @param {number} params.pageSize - Items per page
     * @param {string} params.city - Filter by city
     * @param {string} params.category - Filter by category
     * @param {number} params.minPrice - Minimum price filter
     * @param {number} params.maxPrice - Maximum price filter
     * @param {number} params.minRating - Minimum rating filter
     * @param {string} params.searchQuery - Search text
     * @returns {Promise<Array|Object>} Hotels data or paginated result
     */
    async getAllHotels(params = {}) {
        try {
            const response = await api.get('/Hotels', { params });
            return response.data;
        } catch (error) {
            console.error('Error fetching hotels:', error);
            throw error;
        }
    },

    // Alias for getHotels (used by stores)
    async getHotels(params = {}) {
        return this.getAllHotels(params);
    },

    /**
     * Get hotel by ID
     * @param {number} id - Hotel ID
     * @returns {Promise<Object>} Hotel details
     */
    async getHotelById(id) {
        try {
            const response = await api.get(`/Hotels/${id}`);
            return response.data;
        } catch (error) {
            console.error(`Error fetching hotel ${id}:`, error);
            throw error;
        }
    },

    /**
     * Get hotels with filtering parameters (Specification Pattern)
     * @param {Object} filters - Filter object
     * @param {number} filters.pageIndex - Page number (0-based)
     * @param {number} filters.pageSize - Items per page
     * @param {string} filters.city - City name
     * @param {string} filters.category - Category type
     * @param {number} filters.minPrice - Minimum price
     * @param {number} filters.maxPrice - Maximum price
     * @param {number} filters.minRating - Minimum rating
     * @param {string} filters.searchQuery - Search text
     * @returns {Promise<Array|Object>} Filtered hotels
     */
    async getFilteredHotels(filters = {}) {
        try {
            // Convert filters to API params format
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

            const response = await api.get('/Hotels', { params });
            return response.data;
        } catch (error) {
            console.error('Error fetching filtered hotels:', error);
            throw error;
        }
    },

    // Alias for filterHotels
    async filterHotels(filters = {}) {
        return this.getFilteredHotels(filters);
    },

    /**
     * Get hotels by city
     * @param {string} city - City name
     * @returns {Promise<Array>} Hotels in the city
     */
    async getHotelsByCity(city) {
        return this.getFilteredHotels({ city });
    },

    /**
     * Search hotels
     * @param {string} query - Search query
     * @returns {Promise<Array>} Hotels matching search
     */
    async searchHotels(query) {
        if (!query) return this.getAllHotels();
        return this.getFilteredHotels({ searchQuery: query });
    },

    /**
     * Get top rated hotels
     * @param {number} limit - Number of results
     * @returns {Promise<Array>} Top rated hotels
     */
    async getTopRatedHotels(limit = 8) {
        try {
            const response = await api.get('/Hotels', {
                params: { pageSize: limit, minRating: 4.5 }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching top rated hotels:', error);
            throw error;
        }
    },

    /**
     * Create a booking and Stripe PaymentIntent for hotels (Payment Element flow)
     * @param {Object} bookingData - { userId, entityId, bookingType, startDate, endDate, totalPrice }
     * @returns {Promise<Object>} { bookingId, clientSecret, paymentIntentId, publishableKey }
     */
    async createBookingIntent(bookingData) {
        try {
            const response = await api.post('/Bookings', bookingData);
            return response.data;
        } catch (error) {
            console.error('Error creating hotel booking intent:', error);
            throw error;
        }
    },

    // Backwards-compatible alias
    async bookHotel(bookingData) {
        return this.createBookingIntent(bookingData);
    }
};

export default hotelsApi;
