// Services/tripsApi.js
import axios from 'axios';

// Base URL - adjust according to your json-server configuration
const API_BASE_URL = 'http://localhost:3000';

const tripsApi = {
    // Get all trips
    async getAllTrips() {
        try {
            const response = await axios.get(`${API_BASE_URL}/trips`);
            return response.data;
        } catch (error) {
            console.error('Error fetching trips:', error);
            throw new Error('Failed to fetch trips');
        }
    },

    // Get trip by ID
    async getTripById(id) {
        try {
            const response = await axios.get(`${API_BASE_URL}/trips/${id}`);
            return response.data;
        } catch (error) {
            console.error(`Error fetching trip ${id}:`, error);
            throw new Error('Failed to fetch trip details');
        }
    },

    // Get trips by category
    async getTripsByCategory(category) {
        try {
            const response = await axios.get(`${API_BASE_URL}/trips`, {
                params: { category }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching trips by category:', error);
            throw new Error('Failed to fetch trips by category');
        }
    },

    // Get trips by city/destination
    async getTripsByCity(city) {
        try {
            const response = await axios.get(`${API_BASE_URL}/trips`, {
                params: { city }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching trips by city:', error);
            throw new Error('Failed to fetch trips by city');
        }
    },

    // Search trips
    async searchTrips(query) {
        try {
            const response = await axios.get(`${API_BASE_URL}/trips`, {
                params: { q: query }
            });
            return response.data;
        } catch (error) {
            console.error('Error searching trips:', error);
            throw new Error('Failed to search trips');
        }
    },

    // Get filtered trips
    async getFilteredTrips(filters) {
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

            if (filters.maxPrice) {
                params.price_lte = filters.maxPrice; // Assuming simple number, might need custom filtering if string
            }

            const response = await axios.get(`${API_BASE_URL}/trips`, { params });
            return response.data;
        } catch (error) {
            console.error('Error fetching filtered trips:', error);
            throw new Error('Failed to fetch filtered trips');
        }
    },

    // Get top rated trips
    async getTopRatedTrips(limit = 8) {
        try {
            const response = await axios.get(`${API_BASE_URL}/trips`, {
                params: {
                    _sort: 'rating',
                    _order: 'desc',
                    _limit: limit
                }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching top rated trips:', error);
            throw new Error('Failed to fetch top rated trips');
        }
    },

    // Book trip (POST to bookings endpoint)
    async bookTrip(tripId, bookingData) {
        try {
            const response = await axios.post(`${API_BASE_URL}/bookings`, {
                tripId, // or itemId generically
                type: 'trip',
                ...bookingData,
                bookingDate: new Date().toISOString(),
                status: 'pending'
            });
            return response.data;
        } catch (error) {
            console.error('Error booking trip:', error);
            throw new Error('Failed to book trip');
        }
    }
};

export default tripsApi;
