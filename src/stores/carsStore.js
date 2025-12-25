import { defineStore } from "pinia";
import { getAllCars } from "@/Services/carsApi"; 

export const useCarsStore = defineStore("carsStore", {
  state: () => ({
    cars: [],        
    loading: false,  
    error: null      
  }),

  actions: {
    async fetchCars() {
      this.loading = true;
      this.error = null;
      try {
        const response = await getAllCars();
        
        // Handle both paginated response and direct array
        const carsArray = Array.isArray(response) ? response : (response.items || response.data || []);

        // Transform cars data - map PascalCase API response to camelCase
        this.cars = carsArray.map(car => ({
          id: car.id,
          name: car.name,
          description: car.description,
          category: car.category,
          brand: car.brand,
          model: car.model,
          year: car.year,
          transmission: car.transmission,
          fuelType: car.fuelType,
          seating: car.seating,
          pricePerDay: car.pricePerDay,
          images: car.images || [],
          image: car.images?.[0],
          imageUrl: car.images?.[0] || '/images/placeholder-car.jpg',
          features: car.features || [],
          rating: car.rating || 0,
          reviews: car.reviews || { totalReviews: 0 },
          totalReviews: car.reviews?.totalReviews || 0,
          available: car.available !== false,
          // Include original data for flexibility
          ...car
        }));
      } catch (err) {
        console.error("Error fetching cars:", err);
        this.error = err.message || "Failed to fetch cars";
      } finally {
        this.loading = false;
      }
    }
  },

  getters: {
    getCarById: (state) => (id) => state.cars.find(c => c.id === id)
  }
  
});

