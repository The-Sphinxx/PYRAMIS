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
        this.cars = response;
      } catch (err) {
        console.error("Error fetching cars:", err);
        this.error = err;
      } finally {
        this.loading = false;
      }
    }
  },

  getters: {
    getCarById: (state) => (id) => state.cars.find(c => c.id === id)
  }
  
});

