// src/stores/filterStore.js
import { defineStore } from 'pinia';

export const useFilterStore = defineStore('filter', {
  state: () => ({
    // Attractions: Price + Category + Duration + Rating
    attractionFilters: {
      maxPrice: 500,
      categories: [],
      duration: [],
      rating: []
    },
    
    // Trips: Price + Category + Duration + Rating
    tripFilters: {
      maxPrice: 5000,
      categories: [],
      duration: [],
      rating: []
    },
    
    // Hotels: Price + Rating + Amenities
    hotelFilters: {
      maxPrice: 3000,
      rating: [],
      amenities: []
    },
    
    // Cars: Price + Transmission + With Driver
    carFilters: {
      maxPrice: 2000,
      transmission: [],
      withDriver: []
    }
  }),

  getters: {
    getFiltersForSection: (state) => (section) => {
      const sectionKey = `${section}Filters`;
      return state[sectionKey] || {};
    },

    hasActiveFilters: (state) => (section) => {
      const sectionKey = `${section}Filters`;
      const filters = state[sectionKey];
      
      if (!filters) return false;

      for (const key in filters) {
        if (Array.isArray(filters[key]) && filters[key].length > 0) {
          return true;
        }
      }
      
      return false;
    }
  },

  actions: {
    updateFilters(section, filters) {
      const sectionKey = `${section}Filters`;
      if (this[sectionKey]) {
        this[sectionKey] = { ...this[sectionKey], ...filters };
      }
    },

    resetFilters(section) {
      const sectionKey = `${section}Filters`;
      
      const defaults = {
        attraction: {
          maxPrice: 500,
          categories: [],
          duration: [],
          rating: []
        },
        trip: {
          maxPrice: 5000,
          categories: [],
          duration: [],
          rating: []
        },
        hotel: {
          maxPrice: 3000,
          rating: [],
          amenities: []
        },
        car: {
          maxPrice: 2000,
          transmission: [],
          withDriver: []
        }
      };

      if (this[sectionKey] && defaults[section]) {
        this[sectionKey] = { ...defaults[section] };
      }
    },

    // ===== Apply Filters Based on Section =====
    applyFilters(data, section) {
      const filters = this.getFiltersForSection(section);
      let filtered = [...data];

      // ===== COMMON: Price Filter (All Sections) =====
      if (filters.maxPrice) {
        filtered = filtered.filter(item => item.price <= filters.maxPrice);
      }

      // ===== ATTRACTIONS Filters =====
      if (section === 'attraction') {
        // Category filter
        if (filters.categories && filters.categories.length > 0) {
          filtered = filtered.filter(item => 
            filters.categories.includes(item.category)
          );
        }

        // Duration filter
        if (filters.duration && filters.duration.length > 0) {
          filtered = filtered.filter(item => 
            filters.duration.includes(item.duration)
          );
        }

        // Rating filter
        if (filters.rating && filters.rating.length > 0) {
          filtered = filtered.filter(item => {
            const itemRating = Math.floor(item.rating);
            return filters.rating.some(r => {
              const minRating = parseInt(r);
              return itemRating >= minRating;
            });
          });
        }
      }

      // ===== TRIPS Filters =====
      if (section === 'trip') {
        // Category filter
        if (filters.categories && filters.categories.length > 0) {
          filtered = filtered.filter(item => 
            filters.categories.includes(item.category)
          );
        }

        // Duration filter
        if (filters.duration && filters.duration.length > 0) {
          filtered = filtered.filter(item => 
            filters.duration.includes(item.duration)
          );
        }

        // Rating filter
        if (filters.rating && filters.rating.length > 0) {
          filtered = filtered.filter(item => {
            const itemRating = Math.floor(item.rating);
            return filters.rating.some(r => {
              const minRating = parseInt(r);
              return itemRating >= minRating;
            });
          });
        }
      }

      // ===== HOTELS Filters =====
      if (section === 'hotel') {
        // Rating filter (Star rating)
        if (filters.rating && filters.rating.length > 0) {
          filtered = filtered.filter(item => 
            filters.rating.includes(item.stars?.toString())
          );
        }

        // Amenities filter
        if (filters.amenities && filters.amenities.length > 0) {
          filtered = filtered.filter(item => {
            // Check if hotel has ALL selected amenities
            return filters.amenities.every(amenity => 
              item.amenities?.includes(amenity)
            );
          });
        }
      }

      // ===== CARS Filters =====
      if (section === 'car') {
        // Transmission filter
        if (filters.transmission && filters.transmission.length > 0) {
          filtered = filtered.filter(item => 
            filters.transmission.includes(item.transmission)
          );
        }

        // With Driver filter
        if (filters.withDriver && filters.withDriver.length > 0) {
          filtered = filtered.filter(item => 
            filters.withDriver.includes(item.withDriver)
          );
        }
      }

      return filtered;
    }
  }
});