import axios from 'axios';

// Create axios instance with default config
const apiClient = axios.create({
    baseURL: '/', // Using relative path since valid db.json is in public folder or served via proxy
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    }
});

export default {
    // Get all hotels
    async getHotels() {
        try {
            const response = await apiClient.get('db.json');
            return response.data.hotels;
        } catch (error) {
            console.error('API Error: Failed to fetch hotels', error);
            throw error;
        }
    },

    // Get hotel by ID
    async getHotelById(id) {
        try {
            const hotels = await this.getHotels();
            const hotel = hotels.find(h => h.id === id || h.id === String(id));
            if (!hotel) throw new Error(`Hotel with ID ${id} not found`);
            return hotel;
        } catch (error) {
            console.error(`API Error: Failed to fetch hotel ${id}`, error);
            throw error;
        }
    },

    // Search hotels
    async searchHotels(query) {
        try {
            const hotels = await this.getHotels();
            if (!query) return hotels;

            const lowerQuery = query.toLowerCase();
            return hotels.filter(hotel =>
                hotel.name.toLowerCase().includes(lowerQuery) ||
                hotel.city.toLowerCase().includes(lowerQuery) ||
                (hotel.highlights && hotel.highlights.some(h => h.toLowerCase().includes(lowerQuery)))
            );
        } catch (error) {
            console.error('API Error: Failed to search hotels', error);
            throw error;
        }
    },

    // Filter hotels
    async filterHotels(filters) {
        try {
            let results = await this.getHotels();

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

            return results;
        } catch (error) {
            console.error('API Error: Failed to filter hotels', error);
            throw error;
        }
    }
};
