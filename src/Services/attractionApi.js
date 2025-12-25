// Services/attractionApi.js
import { api } from './api';

const attractionApi = {
  /**
   * Get all attractions with optional filtering via Specification Pattern
   * @param {Object} params - Query parameters for filtering
   * @param {number} params.pageIndex - Page number (0-based)
   * @param {number} params.pageSize - Items per page
   * @param {string} params.city - Filter by city
   * @param {string} params.category - Filter by category
   * @param {number} params.minPrice - Minimum price filter
   * @param {number} params.maxPrice - Maximum price filter
   * @param {number} params.minRating - Minimum rating filter
   * @param {string} params.searchQuery - Search text
   * @returns {Promise<Array|Object>} Attractions data or paginated result
   */
  async getAllAttractions(params = {}) {
    try {
      const response = await api.get('/Attractions', { params });
      return response.data;
    } catch (error) {
      console.error('Error fetching attractions:', error);
      throw error;
    }
  },

  /**
   * Get attraction by ID
   * @param {number} id - Attraction ID
   * @returns {Promise<Object>} Attraction details
   */
  async getAttractionById(id) {
    try {
      const response = await api.get(`/Attractions/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching attraction ${id}:`, error);
      throw error;
    }
  },

  /**
   * Get filtered attractions using Specification Pattern
   * @param {Object} filters - Filter object
   * @param {number} filters.pageIndex - Page number (0-based)
   * @param {number} filters.pageSize - Items per page
   * @param {string} filters.city - City name
   * @param {string} filters.category - Category type (can be array for multiple)
   * @param {number} filters.minPrice - Minimum price
   * @param {number} filters.maxPrice - Maximum price
   * @param {number} filters.minRating - Minimum rating
   * @param {string} filters.searchQuery - Search text
   * @returns {Promise<Array|Object>} Filtered attractions
   */
  async getFilteredAttractions(filters = {}) {
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

      const response = await api.get('/Attractions', { params });
      return response.data;
    } catch (error) {
      console.error('Error fetching filtered attractions:', error);
      throw error;
    }
  },

  /**
   * Get attractions by category
   * @param {string} category - Category name
   * @returns {Promise<Array>} Attractions in the category
   */
  async getAttractionsByCategory(category) {
    return this.getFilteredAttractions({ category });
  },

  /**
   * Get attractions by city
   * @param {string} city - City name
   * @returns {Promise<Array>} Attractions in the city
   */
  async getAttractionsByCity(city) {
    return this.getFilteredAttractions({ city });
  },

  /**
   * Search attractions
   * @param {string} query - Search query
   * @returns {Promise<Array>} Attractions matching search
   */
  async searchAttractions(query) {
    if (!query) return this.getAllAttractions();
    return this.getFilteredAttractions({ searchQuery: query });
  },

  /**
   * Get top rated attractions
   * @param {number} limit - Number of results
   * @returns {Promise<Array>} Top rated attractions
   */
  async getTopRatedAttractions(limit = 8) {
    try {
      const response = await api.get('/Attractions', {
        params: { pageSize: limit, minRating: 4.5 }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching top rated attractions:', error);
      throw error;
    }
  },

  /**
   * Book an attraction
   * @param {Object} bookingData - Booking information
   * @returns {Promise<Object>} Booking confirmation
   */
  async bookAttraction(attractionId, bookingData) {
    try {
      const response = await api.post('/Bookings', {
        attractionId,
        type: 'attraction',
        ...bookingData,
      });
      return response.data;
    } catch (error) {
      console.error('Error booking attraction:', error);
      throw error;
    }
  },

  /**
   * Get attraction booking details
   * @param {number} bookingId - Booking ID
   * @returns {Promise<Object>} Booking details
   */
  async getAttractionBooking(bookingId) {
    try {
      const response = await api.get(`/Bookings/${bookingId}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching booking details:', error);
      throw error;
    }
  },

  /**
   * Cancel an attraction booking
   * @param {number} bookingId - Booking ID
   * @returns {Promise<Object>} Updated booking
   */
  async cancelAttractionBooking(bookingId) {
    try {
      const response = await api.patch(`/Bookings/${bookingId}`, {
        status: 'cancelled',
        cancelledDate: new Date().toISOString()
      });
      return response.data;
    } catch (error) {
      console.error('Error cancelling booking:', error);
      throw error;
    }
  }
};

export default attractionApi;