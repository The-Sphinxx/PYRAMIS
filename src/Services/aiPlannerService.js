import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5137/api';

const aiPlannerService = {
  /**
   * Generate a personalized AI trip plan
   */
  async generateTripPlan(criteria) {
    try {
      const response = await axios.post(`${API_BASE_URL}/api/AiPlanner/generate`, criteria);
      return response.data;
    } catch (error) {
      console.error('Error generating trip plan:', error);
      throw error;
    }
  },

  /**
   * Get available destinations
   */
  async getDestinations() {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/AiPlanner/destinations`);
      return response.data;
    } catch (error) {
      console.error('Error fetching destinations:', error);
      return [];
    }
  },

  /**
   * Get available interest categories
   */
  async getInterests() {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/AiPlanner/interests`);
      return response.data;
    } catch (error) {
      console.error('Error fetching interests:', error);
      return [];
    }
  }
};

export default aiPlannerService;
