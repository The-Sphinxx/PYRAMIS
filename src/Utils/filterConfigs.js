// src/utils/filterConfigs.js

// ===== ATTRACTIONS: Price + Category + Duration + Rating =====
export const attractionFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 50, max: 500 },
  categoryOptions: [
    { label: 'Museums', value: 'museums' },
    { label: 'Historical Sites', value: 'historical' },
    { label: 'Adventure Parks', value: 'adventure' },
    { label: 'Cultural Tours', value: 'cultural' },
    { label: 'Nature & Wildlife', value: 'nature' },
    { label: 'Religious Sites', value: 'religious' },
    { label: 'Entertainment', value: 'entertainment' }
  ],

  customFilters: [
    {
      key: 'duration',
      title: 'Duration',
      options: [
        { label: 'Less than 2 hours', value: 'short' },
        { label: '2-4 hours', value: 'medium' },
        { label: 'Half day (4-6 hours)', value: 'half-day' },
        { label: 'Full day (6+ hours)', value: 'full-day' }
      ]
    },
    {
      key: 'rating',
      title: 'Customer Rating',
      options: [
        { label: '5 Stars', value: '5' },
        { label: '4+ Stars', value: '4' },
        { label: '3+ Stars', value: '3' },
        { label: '2+ Stars', value: '2' }
      ]
    }
  ]
};

// ===== TRIPS: Price + Category + Duration + Rating =====
export const tripFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 500, max: 5000 },
  categoryOptions: [
    { label: 'Beach Trips', value: 'beach' },
    { label: 'Desert Safari', value: 'desert' },
    { label: 'Nile Cruise', value: 'cruise' },
    { label: 'City Tours', value: 'city' },
    { label: 'Diving & Snorkeling', value: 'diving' },
    { label: 'Historical Tours', value: 'historical' },
    { label: 'Adventure Tours', value: 'adventure' },
    { label: 'Wellness & Spa', value: 'wellness' }
  ],
  customFilters: [
    {
      key: 'duration',
      title: 'Trip Duration',
      options: [
        { label: '1 Day Trip', value: '1' },
        { label: '2-3 Days', value: '2-3' },
        { label: '4-7 Days', value: '4-7' },
        { label: 'More than 1 Week', value: '7+' }
      ]
    },
    {
      key: 'rating',
      title: 'Customer Rating',
      options: [
        { label: '5 Stars', value: '5' },
        { label: '4+ Stars', value: '4' },
        { label: '3+ Stars', value: '3' },
        { label: '2+ Stars', value: '2' }
      ]
    }
  ]
};

// ===== HOTELS: Price + Rating + Amenities =====
export const hotelFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 200, max: 3000 },
  categoryOptions: [], // مش محتاجين categories للهوتلز
  customFilters: [
    {
      key: 'rating',
      title: 'Star Rating',
      options: [
        { label: '5 Stars Luxury', value: '5' },
        { label: '4 Stars', value: '4' },
        { label: '3 Stars', value: '3' },
        { label: '2 Stars', value: '2' },
        { label: '1 Star', value: '1' }
      ]
    },
    {
      key: 'amenities',
      title: 'Hotel Amenities',
      options: [
        { label: 'Swimming Pool', value: 'pool' },
        { label: 'Free WiFi', value: 'wifi' },
        { label: 'Free Parking', value: 'parking' },
        { label: 'Fitness Center', value: 'gym' },
        { label: 'Restaurant', value: 'restaurant' },
        { label: 'Beach Access', value: 'beach' },
        { label: 'Spa & Wellness', value: 'spa' },
        { label: 'Room Service', value: 'room-service' },
        { label: 'Airport Shuttle', value: 'shuttle' },
        { label: 'Pet Friendly', value: 'pets' }
      ]
    }
  ]
};

// ===== CARS: Price + Transmission + With Driver =====
export const carFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 100, max: 2000 },
  categoryOptions: [], // مش محتاجين categories للعربيات
  customFilters: [
    {
      key: 'transmission',
      title: 'Transmission Type',
      options: [
        { label: 'Automatic', value: 'automatic' },
        { label: 'Manual', value: 'manual' }
      ]
    },
    {
      key: 'withDriver',
      title: 'Driver Options',
      options: [
        { label: 'With Driver', value: 'yes' },
        { label: 'Self Drive', value: 'no' }
      ]
    }
  ]
};