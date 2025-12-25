import { defineStore } from "pinia";
import { getAllCars, getCarById } from "@/Services/carsApi"; 

export const useCarsStore = defineStore("carsStore", {
  state: () => ({
    cars: [],
    selectedCar: null,        
    loading: false,  
    error: null,
    pagination: {
      pageIndex: 0,
      pageSize: 12,
      totalCount: 0,
      totalPages: 0
    }
  }),

  actions: {
    async fetchCars(params = {}) {
      this.loading = true;
      this.error = null;
      this.cars = []; // Clear before fetch
      try {
        const response = await getAllCars(params);
        
        // Handle both paginated response and direct array
        // API returns camelCase: data, pageNumber, pageSize, totalRecords, totalPages
        const carsArray = Array.isArray(response) ? response : (response.data || response.Data || response.items || []);

        // Transform cars data - map PascalCase API response to camelCase
        this.cars = carsArray.map(car => this.transformCar(car));
        
        // Update pagination metadata - API uses camelCase
        this.pagination = {
          pageIndex: (response.pageNumber ? response.pageNumber - 1 : 0),
          pageSize: response.pageSize ?? params.pageSize ?? 12,
          totalCount: response.totalRecords ?? carsArray.length,
          totalPages: response.totalPages ?? 1
        };
        
        return this.pagination;
      } catch (err) {
        console.error("Error fetching cars:", err);
        this.error = err.message || "Failed to fetch cars";
        throw err;
      } finally {
        this.loading = false;
      }
    },

    async fetchCarById(id) {
      this.loading = true;
      this.error = null;
      try {
        // Try to find in existing state first
        let car = this.cars.find(c => c.id == id);
        
        if (!car) {
          // Fetch from API if not in state
          const response = await getCarById(id);
          car = this.transformCar(response);
        }
        
        this.selectedCar = car;
        return car;
      } catch (err) {
        console.error("Error fetching car by ID:", err);
        this.error = err.message || 'Failed to fetch car';
        throw err;
      } finally {
        this.loading = false;
      }
    },

    transformCar(car) {
      return {
        id: car.id,
        name: car.name,
        description: car.description,
        overview: car.overview,
        category: car.category,
        brand: car.brand,
        model: car.model,
        year: car.year,
        transmission: car.transmission,
        fuelType: car.fuelType,
        seats: car.seats || car.seating,
        seating: car.seating || car.seats,
        price: car.price || car.pricePerDay,
        pricePerDay: car.pricePerDay || car.price,
        images: car.images || [],
        image: car.images?.[0],
        imageUrl: car.images?.[0] || '/images/placeholder-car.jpg',
        features: car.features || [],
        amenities: car.amenities || [],
        rating: car.rating || 0,
        reviewSummary: car.reviewSummary || car.reviews || { 
          overallRating: car.rating || 0, 
          totalReviews: 0,
          ratingCriteria: {}
        },
        reviews: car.reviews || car.reviewSummary || { 
          overallRating: car.rating || 0, 
          totalReviews: 0 
        },
        userReviews: car.userReviews || [],
        totalReviews: car.reviews?.totalReviews || car.reviewSummary?.totalReviews || 0,
        available: car.available !== false,
        status: car.status,
        city: car.city,
        // Include original data for flexibility
        ...car
      };
    },

    clearSelectedCar() {
      this.selectedCar = null;
    }
  },

  getters: {
    getCarById: (state) => (id) => state.cars.find(c => c.id === id)
  }
  
});

