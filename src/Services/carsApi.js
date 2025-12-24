import { api } from './api';

/**
 * Get all cars with optional filtering via Specification Pattern
 * @param {Object} params - Query parameters for filtering
 * @param {number} params.pageIndex - Page number (0-based)
 * @param {number} params.pageSize - Items per page
 * @param {string} params.city - Filter by city
 * @param {string} params.category - Filter by category
 * @param {number} params.minPrice - Minimum price filter
 * @param {number} params.maxPrice - Maximum price filter
 * @param {number} params.minRating - Minimum rating filter
 * @param {string} params.searchQuery - Search text
 * @returns {Promise<Array|Object>} Cars data or paginated result
 */
export const getAllCars = async (params = {}) => {
  try {
    const response = await api.get('/Cars', { params });
    return response.data;
  } catch (error) {
    console.error("Error fetching cars:", error);
    throw error;
  }
};

/**
 * Get car by ID
 * @param {number} id - Car ID
 * @returns {Promise<Object>} Car details
 */
export const getCarById = async (id) => {
  try {
    const response = await api.get(`/Cars/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching car ${id}:`, error);
    throw error;
  }
};

/**
 * Get filtered cars using Specification Pattern
 * @param {Object} filters - Filter object
 * @param {number} filters.pageIndex - Page number (0-based)
 * @param {number} filters.pageSize - Items per page
 * @param {string} filters.city - City name
 * @param {string} filters.category - Category type
 * @param {number} filters.minPrice - Minimum price
 * @param {number} filters.maxPrice - Maximum price
 * @param {number} filters.minRating - Minimum rating
 * @param {string} filters.searchQuery - Search text
 * @returns {Promise<Array|Object>} Filtered cars
 */
export const getFilteredCars = async (filters = {}) => {
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

    const response = await api.get('/Cars', { params });
    return response.data;
  } catch (error) {
    console.error("Error fetching filtered cars:", error);
    throw error;
  }
};

/**
 * Get cars by category
 * @param {string} category - Category name
 * @returns {Promise<Array>} Cars in the category
 */
export const getCarsByCategory = async (category) => {
  return getFilteredCars({ category });
};

/**
 * Get cars by city
 * @param {string} city - City name
 * @returns {Promise<Array>} Cars in the city
 */
export const getCarsByCity = async (city) => {
  return getFilteredCars({ city });
};

/**
 * Search cars
 * @param {string} query - Search query
 * @returns {Promise<Array>} Cars matching search
 */
export const searchCars = async (query) => {
  if (!query) return getAllCars();
  return getFilteredCars({ searchQuery: query });
};

/**
 * Book a car
 * @param {Object} bookingData - Booking information
 * @returns {Promise<Object>} Booking confirmation
 */
export const createCarBookingIntent = async (bookingData) => {
  try {
    const response = await api.post('/Bookings', bookingData);
    return response.data;
  } catch (error) {
    console.error('Error creating car booking intent:', error);
    throw error;
  }
};

// Backwards-compatible alias
export const bookCar = async (bookingData) => {
  return createCarBookingIntent(bookingData);
};
