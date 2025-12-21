// Services/hotelsApi.js
import axios from 'axios';

// Base URL - adjust according to your json-server configuration
const API_BASE_URL = 'http://localhost:3000';

const hotelsApi = {
    // Get all hotels
    async getAllHotels() {
        try {
            const response = await axios.get(`${API_BASE_URL}/hotels`);
            return response.data;
        } catch (error) {
            console.error('Error fetching hotels:', error);
            throw new Error('Failed to fetch hotels');
        }
    },

    // Get hotel by ID
    async getHotelById(id) {
        try {
            const response = await axios.get(`${API_BASE_URL}/hotels/${id}`);
            return response.data;
        } catch (error) {
            console.error(`Error fetching hotel ${id}:`, error);
            throw new Error('Failed to fetch hotel details');
        }
    },

    // Get hotels by city
    async getHotelsByCity(city) {
        try {
            const response = await axios.get(`${API_BASE_URL}/hotels`, {
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
            const response = await axios.get(`${API_BASE_URL}/hotels`, {
                params: { q: query }
            });
            return response.data;
        } catch (error) {
            console.error('Error searching hotels:', error);
            throw new Error('Failed to search hotels');
        }
    },

    // Get filtered hotels
    async getFilteredHotels(filters) {
        try {
            const params = {};

            if (filters.city && filters.city !== 'All') {
                params.city = filters.city;
            }

            if (filters.minRating) {
                params.rating_gte = filters.minRating;
            }

            if (filters.maxPrice) {
                params.price_lte = filters.maxPrice;
            }
            
            // Amenities filtering usually happens client-side with json-server 
            // unless using q=amenity or custom middleware

            const response = await axios.get(`${API_BASE_URL}/hotels`, { params });
            return response.data;
        } catch (error) {
            console.error('Error fetching filtered hotels:', error);
            throw new Error('Failed to fetch filtered hotels');
        }
    },

    // Get top rated hotels
    async getTopRatedHotels(limit = 8) {
        try {
            const response = await axios.get(`${API_BASE_URL}/hotels`, {
                params: {
                    _sort: 'rating',
                    _order: 'desc',
                    _limit: limit
                }
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
            const response = await axios.post(`${API_BASE_URL}/bookings`, {
                type: 'hotel',
                ...bookingData,
                bookingDate: new Date().toISOString(),
                status: 'pending'
            });
            return response.data;
        } catch (error) {
            console.error('Error booking hotel:', error);
            throw new Error('Failed to book hotel');
        }
    }
};

export default hotelsApi;
