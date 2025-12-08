import { defineStore } from "pinia";
import { getAllCars } from "@/Services/carsApi";

export const useCarsStore = defineStore("carsStore", {
  state: () => ({
    cars: []
  }),
  actions: {
    async fetchCars() {
      this.cars = await getAllCars();
    }
  }
});
