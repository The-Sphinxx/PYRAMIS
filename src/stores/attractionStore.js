// stores/attractionStore.js
import { defineStore } from 'pinia';
import attractionApi from '../Services/attractionApi';

export const useAttractionStore = defineStore('attraction', {
  state: () => ({
    attractions: [],
    selectedAttraction: null,
    filteredAttractions: [],
    loading: false,
    error: null,
    categories: ['TOP PICKS', 'HISTORICAL', 'MUSEUMS', 'NATURE', 'FAMILY', 'RELIGIOUS', 'CULTURE'],
    cities: [],
    filters: {
      categories: [],
      city: 'All',
      minRating: 0,
      searchQuery: ''
    }
  }),

  getters: {
    // Get attractions by category (updated for array)
    getAttractionsByCategory: (state) => (category) => {
      if (category === 'All' || !category) return state.attractions;
      return state.attractions.filter(attr => 
        attr.categories && attr.categories.includes(category)
      );
    },

    // Get attractions by city
    getAttractionsByCity: (state) => (city) => {
      if (city === 'All') return state.attractions;
      return state.attractions.filter(attr => attr.city === city);
    },

    // Get top rated attractions
    topRatedAttractions: (state) => {
      return [...state.attractions]
        .sort((a, b) => b.rating - a.rating)
        .slice(0, 8);
    },

    // Get attraction by ID
    getAttractionById: (state) => (id) => {
      return state.attractions.find(attr => attr.id == id);
    },

    // Get unique cities
    uniqueCities: (state) => {
      const cities = [...new Set(state.attractions.map(attr => attr.city))];
      return cities.sort();
    },

    // Get all unique categories from attractions
    allCategories: (state) => {
      const categoriesSet = new Set();
      state.attractions.forEach(attr => {
        if (attr.categories) {
          attr.categories.forEach(cat => categoriesSet.add(cat));
        }
      });
      return Array.from(categoriesSet).sort();
    },

    // Apply all filters
    getFilteredAttractions: (state) => {
      let result = [...state.attractions];

      // Filter by categories (supports multiple)
      if (state.filters.categories.length > 0) {
        result = result.filter(attr => 
          attr.categories && 
          state.filters.categories.some(cat => attr.categories.includes(cat))
        );
      }

      // Filter by city
      if (state.filters.city !== 'All') {
        result = result.filter(attr => attr.city === state.filters.city);
      }

      // Filter by rating
      if (state.filters.minRating > 0) {
        result = result.filter(attr => attr.rating >= state.filters.minRating);
      }

      // Filter by search query
      if (state.filters.searchQuery) {
        const query = state.filters.searchQuery.toLowerCase();
        result = result.filter(attr =>
          attr.name.toLowerCase().includes(query) ||
          attr.description.toLowerCase().includes(query) ||
          attr.city.toLowerCase().includes(query)
        );
      }

      return result;
    },

    // Get attractions count by category
    getCategoryCount: (state) => (category) => {
      return state.attractions.filter(attr => 
        attr.categories && attr.categories.includes(category)
      ).length;
    }
  },

  actions: {
    // Fetch all attractions
    async fetchAttractions() {
      this.loading = true;
      this.error = null;
      try {
        const data = await attractionApi.getAllAttractions();
        this.attractions = data;
        this.filteredAttractions = data;
        this.cities = this.uniqueCities;
      } catch (error) {
        this.error = error.message || 'Failed to fetch attractions';
        console.error('Error fetching attractions:', error);
      } finally {
        this.loading = false;
      }
    },

    // Fetch single attraction by ID
async fetchAttractionById(id) {
  this.loading = true;
  this.error = null;

  try {
    // 1) تأكد أن قائمة الـ attractions محمّلة (مهمّ لحساب الـ Similar)
    if (this.attractions.length === 0) {
      await this.fetchAttractions();
    }

    // 2) ابحث محلياً
    let attraction = this.attractions.find(
      (attr) => attr.id == id
    );

    if (attraction) {
      this.selectedAttraction = attraction;
    } else {
      // 3) Fetch من الـ API لو مش موجود محلياً
      const data = await attractionApi.getAttractionById(id);
      this.selectedAttraction = data;
      attraction = data;
    }

    return attraction;
  } catch (error) {
    this.error = error.message || "Failed to fetch attraction";
    console.error("Error fetching attraction:", error);
    throw error;
  } finally {
    this.loading = false;
  }
},


    // Set single filter
    setFilter(filterType, value) {
      this.filters[filterType] = value;
      this.applyFilters();
    },

    // Add category to filters
    addCategoryFilter(category) {
      if (!this.filters.categories.includes(category)) {
        this.filters.categories.push(category);
        this.applyFilters();
      }
    },

    // Remove category from filters
    removeCategoryFilter(category) {
      const index = this.filters.categories.indexOf(category);
      if (index > -1) {
        this.filters.categories.splice(index, 1);
        this.applyFilters();
      }
    },

    // Set categories filter (replace all)
    setCategoriesFilter(categories) {
      this.filters.categories = Array.isArray(categories) ? categories : [categories];
      this.applyFilters();
    },

    // Apply filters
    applyFilters() {
      this.filteredAttractions = this.getFilteredAttractions;
    },

    // Reset filters
    resetFilters() {
      this.filters = {
        categories: [],
        city: 'All',
        minRating: 0,
        searchQuery: ''
      };
      this.filteredAttractions = this.attractions;
    },

    // Search attractions
    searchAttractions(query) {
      this.filters.searchQuery = query;
      this.applyFilters();
    },

    // Clear selected attraction
    clearSelectedAttraction() {
      this.selectedAttraction = null;
    },

    // Book attraction
    async bookAttraction(attractionId, bookingData) {
      this.loading = true;
      this.error = null;
      try {
        const result = await attractionApi.bookAttraction(attractionId, bookingData);
        return result;
      } catch (error) {
        this.error = error.message || 'Failed to book attraction';
        console.error('Error booking attraction:', error);
        throw error;
      } finally {
        this.loading = false;
      }
    }
  }
});