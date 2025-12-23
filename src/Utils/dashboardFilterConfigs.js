// src/Utils/dashboardFilterConfigs.js
// Dashboard-specific filter configurations for management pages

// ===== USERS =====
export const userFilterConfig = {
  showPriceFilter: false,
  priceRange: { min: 0, max: 0 },
  categoryOptions: [],
  customFilters: [
    {
      key: 'nationality',
      title: 'Nationality',
      type: 'single',
      options: [
        { label: 'Egyptian', value: 'Egyptian' },
        { label: 'American', value: 'American' },
        { label: 'British', value: 'British' },
        { label: 'French', value: 'French' },
        { label: 'German', value: 'German' },
        { label: 'Italian', value: 'Italian' },
        { label: 'Russian', value: 'Russian' },
        { label: 'Saudi', value: 'Saudi' },
        { label: 'Emirati', value: 'Emirati' },
        { label: 'Other', value: 'Other' }
      ]
    },
    {
      key: 'status',
      title: 'Status',
      type: 'single',
      options: [
        { label: 'Verified', value: 'Verified' },
        { label: 'Unverified', value: 'Unverified' },
        { label: 'Suspended', value: 'Suspended' }
      ]
    },
    {
      key: 'createdFrom',
      title: 'Created From',
      inputType: 'date'
    },
    {
      key: 'createdTo',
      title: 'Created To',
      inputType: 'date'
    }
  ]
};

// ===== BOOKINGS =====
export const bookingFilterConfig = {
  showPriceFilter: false,
  priceRange: { min: 0, max: 10000 },
  priceLabel: 'Total Amount',
  categoryOptions: [],
  customFilters: [
    {
      key: 'bookingType',
      title: 'Booking Type',
      type: 'single',
      options: [
        { label: 'Attraction', value: 'attraction' },
        { label: 'Hotel', value: 'hotel' },
        { label: 'Car', value: 'car' },
        { label: 'Trip', value: 'trip' }
      ]
    },
    {
      key: 'status',
      title: 'Booking Status',
      type: 'single',
      options: [
        { label: 'Confirmed', value: 'confirmed' },
        { label: 'Pending', value: 'pending' },
        { label: 'Cancelled', value: 'cancelled' },
        { label: 'Completed', value: 'completed' }
      ]
    },
    {
      key: 'paymentStatus',
      title: 'Payment Status',
      type: 'single',
      options: [
        { label: 'Paid', value: 'paid' },
        { label: 'Pending', value: 'pending' },
        { label: 'Failed', value: 'failed' },
        { label: 'Refunded', value: 'refunded' }
      ]
    },
    {
      key: 'dateFrom',
      title: 'Booking Date From',
      inputType: 'date'
    },
    {
      key: 'dateTo',
      title: 'Booking Date To',
      inputType: 'date'
    },
    {
      key: 'bookingId',
      title: 'Booking ID',
      inputType: 'text',
      placeholder: 'Enter booking ID'
    }
  ]
};

// ===== DASHBOARD ATTRACTIONS (Extended from existing) =====
export const dashboardAttractionFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 0, max: 1000 },
  priceLabel: 'Filter By Price',
  categoryOptions: [
    { label: 'Museums', value: 'MUSEUMS' },
    { label: 'Historical Sites', value: 'HISTORICAL' },
    { label: 'Culture', value: 'CULTURE' },
    { label: 'Nature & Wildlife', value: 'NATURE' },
    { label: 'Religious Sites', value: 'RELIGIOUS' },
    { label: 'Top Picks', value: 'TOP PICKS' }
  ],
  categoryLabel: 'Category',
  customFilters: [
    {
      key: 'city',
      title: 'City',
      inputType: 'text',
      placeholder: 'Enter city name'
    },
    {
      key: 'status',
      title: 'Status',
      type: 'single',
      options: [
        { label: 'Active', value: 'Active' },
        { label: 'Pending', value: 'Pending' },
        { label: 'Disabled', value: 'Disabled' },
        { label: 'Suspended', value: 'Suspended' }
      ]
    },
    {
      key: 'availability',
      title: 'Availability',
      type: 'single',
      options: [
        { label: 'Available', value: 'Available' },
        { label: 'Sold Out', value: 'Sold Out' }
      ]
    },
    {
      key: 'featured',
      title: 'Featured',
      type: 'single',
      options: [
        { label: 'Featured Only', value: 'true' },
        { label: 'Non-Featured', value: 'false' }
      ]
    },
    {
      key: 'rating',
      title: 'Minimum Rating',
      type: 'single',
      options: [
        { label: '4.5+ Stars', value: '4.5' },
        { label: '4.0+ Stars', value: '4.0' },
        { label: '3.5+ Stars', value: '3.5' },
        { label: '3.0+ Stars', value: '3.0' }
      ]
    }
  ]
};

// ===== DASHBOARD HOTELS (Extended from existing) =====
export const dashboardHotelFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 0, max: 1000 },
  priceLabel: 'Price / Night',
  categoryOptions: [],
  customFilters: [
    {
      key: 'city',
      title: 'City',
      type: 'text',
      placeholder: 'Search by city...'
    },
    {
      key: 'statusSelected',
      title: 'Status',
      type: 'single',
      options: [
        { label: 'Active', value: 'Active' },
        { label: 'Pending', value: 'Pending' },
        { label: 'Suspended', value: 'Suspended' }
      ]
    },
    {
      key: 'ratingSelected',
      title: 'Rating',
      type: 'single',
      options: [
        { label: '4 Stars & Up', value: '4' },
        { label: '3 Stars & Up', value: '3' },
        { label: '2 Stars & Up', value: '2' }
      ]
    },
    {
      key: 'featuredSelected',
      title: 'Featured',
      type: 'single',
      options: [
        { label: 'Featured Only', value: 'true' },
        { label: 'All', value: 'false' }
      ]
    }
  ]
};

// ===== DASHBOARD CARS (Extended from existing) =====
export const dashboardCarFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 0, max: 500 },
  customFilters: [
    {
      key: 'city',
      title: 'City',
      type: 'single',
      options: [
        { label: 'Cairo', value: 'Cairo' },
        { label: 'Giza', value: 'Giza' },
        { label: 'Alexandria', value: 'Alexandria' },
        { label: 'Luxor', value: 'Luxor' },
        { label: 'Aswan', value: 'Aswan' },
        { label: 'Sharm El Sheikh', value: 'Sharm El Sheikh' },
        { label: 'Hurghada', value: 'Hurghada' },
        { label: 'Dahab', value: 'Dahab' },
        { label: 'Siwa', value: 'Siwa' },
        { label: 'Faiyum', value: 'Faiyum' },
        { label: 'Ain Sokhna', value: 'Ain Sokhna' },
        { label: 'Marsa Alam', value: 'Marsa Alam' },
        { label: 'Nuweiba', value: 'Nuweiba' },
        { label: 'Port Said', value: 'Port Said' }
      ]
    },
    {
      key: 'carTypeSelected',
      title: 'Car Type',
      type: 'single',
      options: [
        { label: 'SUV', value: 'SUV' },
        { label: 'Sedan', value: 'Sedan' },
        { label: 'Luxury', value: 'Luxury' },
        { label: 'Van', value: 'Van' }
      ]
    },
    {
      key: 'statusSelected',
      title: 'Status',
      type: 'single',
      options: [
        { label: 'Available', value: 'Available' },
        { label: 'Rented', value: 'Rented' },
        { label: 'Maintenance', value: 'Maintenance' }
      ]
    },
    {
      key: 'featured',
      title: 'Featured',
      type: 'single',
      options: [
        { label: 'Featured Only', value: 'true' },
        { label: 'Non-Featured', value: 'false' }
      ]
    }
  ]
};

// ===== DASHBOARD TRIPS (Extended from existing) =====
export const dashboardTripFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 0, max: 10000 },
  priceLabel: 'Filter By Price',
  categoryOptions: [],
  customFilters: [
    {
      key: 'destination',
      title: 'Destination',
      inputType: 'text',
      placeholder: 'Enter destination'
    },
    {
      key: 'tripType',
      title: 'Trip Type',
      type: 'single',
      options: [
        { label: 'Beach Trip', value: 'beach' },
        { label: 'Desert Safari', value: 'desert' },
        { label: 'Nile Cruise', value: 'cruise' },
        { label: 'City Tour', value: 'city' },
        { label: 'Diving', value: 'diving' },
        { label: 'Historical', value: 'historical' }
      ]
    },
    {
      key: 'status',
      title: 'Status',
      type: 'single',
      options: [
        { label: 'Upcoming', value: 'Upcoming' },
        { label: 'Ongoing', value: 'Ongoing' },
        { label: 'Completed', value: 'Completed' }
      ]
    },
    {
      key: 'featured',
      title: 'Featured',
      type: 'single',
      options: [
        { label: 'Featured Only', value: 'true' },
        { label: 'Non-Featured', value: 'false' }
      ]
    },
    {
      key: 'duration',
      title: 'Duration (Days)',
      type: 'single',
      options: [
        { label: '1-3 Days', value: '1-3' },
        { label: '4-7 Days', value: '4-7' },
        { label: '8-14 Days', value: '8-14' },
        { label: '15+ Days', value: '15+' }
      ]
    }
  ]
};
