// Services/attractionApi.js
import axios from 'axios';

// Base URL - adjust according to your json-server configuration
const API_BASE_URL = 'http://localhost:3000';

const attractionApi = {
  // Get all attractions
  async getAllAttractions() {
    try {
      const response = await axios.get(`${API_BASE_URL}/attractions`);
      return response.data;
    } catch (error) {
      console.error('Error fetching attractions:', error);
      throw new Error('Failed to fetch attractions');
    }
  },

  // Get attraction by ID
  async getAttractionById(id) {
    try {
      const response = await axios.get(`${API_BASE_URL}/attractions/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching attraction ${id}:`, error);
      throw new Error('Failed to fetch attraction details');
    }
  },

  // Get attractions by category
  async getAttractionsByCategory(category) {
    try {
      const response = await axios.get(`${API_BASE_URL}/attractions`, {
        params: { category }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching attractions by category:', error);
      throw new Error('Failed to fetch attractions by category');
    }
  },

  // Get attractions by city
  async getAttractionsByCity(city) {
    try {
      const response = await axios.get(`${API_BASE_URL}/attractions`, {
        params: { city }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching attractions by city:', error);
      throw new Error('Failed to fetch attractions by city');
    }
  },

  // Search attractions
  async searchAttractions(query) {
    try {
      const response = await axios.get(`${API_BASE_URL}/attractions`, {
        params: { q: query }
      });
      return response.data;
    } catch (error) {
      console.error('Error searching attractions:', error);
      throw new Error('Failed to search attractions');
    }
  },

  // Get attractions with filters
  async getFilteredAttractions(filters) {
    try {
      const params = {};
      
      if (filters.category && filters.category !== 'All') {
        params.category = filters.category;
      }
      
      if (filters.city && filters.city !== 'All') {
        params.city = filters.city;
      }
      
      if (filters.minRating) {
        params.rating_gte = filters.minRating;
      }

      const response = await axios.get(`${API_BASE_URL}/attractions`, { params });
      return response.data;
    } catch (error) {
      console.error('Error fetching filtered attractions:', error);
      throw new Error('Failed to fetch filtered attractions');
    }
  },

  // Get top rated attractions
  async getTopRatedAttractions(limit = 8) {
    try {
      const response = await axios.get(`${API_BASE_URL}/attractions`, {
        params: {
          _sort: 'rating',
          _order: 'desc',
          _limit: limit
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching top rated attractions:', error);
      throw new Error('Failed to fetch top rated attractions');
    }
  },

  // Book attraction (POST to bookings endpoint)
  async bookAttraction(attractionId, bookingData) {
    try {
      const response = await axios.post(`${API_BASE_URL}/bookings`, {
        attractionId,
        type: 'attraction',
        ...bookingData,
        bookingDate: new Date().toISOString(),
        status: 'pending'
      });
      return response.data;
    } catch (error) {
      console.error('Error booking attraction:', error);
      throw new Error('Failed to book attraction');
    }
  },

  // Get attraction booking details
  async getAttractionBooking(bookingId) {
    try {
      const response = await axios.get(`${API_BASE_URL}/bookings/${bookingId}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching booking details:', error);
      throw new Error('Failed to fetch booking details');
    }
  },

  // Cancel attraction booking
  async cancelAttractionBooking(bookingId) {
    try {
      const response = await axios.patch(`${API_BASE_URL}/bookings/${bookingId}`, {
        status: 'cancelled',
        cancelledDate: new Date().toISOString()
      });
      return response.data;
    } catch (error) {
      console.error('Error cancelling booking:', error);
      throw new Error('Failed to cancel booking');
    }
  }
};

export default attractionApi;