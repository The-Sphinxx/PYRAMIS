// src/Utils/dashboardFilterConfigs.js
// Dashboard-specific filter configurations for management pages

// ===== USERS =====
export const userFilterConfig = {
  showPriceFilter: false,
  priceRange: { min: 0, max: 0 },
  categoryOptions: [],
  customFilters: [
    {
      key: 'email',
      title: 'Email',
      inputType: 'text',
      placeholder: 'Enter email address'
    },
    {
      key: 'phone',
      title: 'Phone',
      inputType: 'text',
      placeholder: 'Enter phone number'
    },
    {
      key: 'nationality',
      title: 'Nationality',
      inputType: 'text',
      placeholder: 'Enter nationality'
    },
    {
      key: 'status',
      title: 'Status',
      type: 'single',
      options: [
        { label: 'Verified', value: 'Verified' },
        { label: 'Unverified', value: 'Unverified' }
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
  showPriceFilter: true,
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
      key: 'userName',
      title: 'User Name',
      inputType: 'text',
      placeholder: 'Enter user name'
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
  priceRange: { min: 0, max: 5000 },
  priceLabel: 'Price / Night',
  categoryOptions: [],
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
        { label: 'Active', value: 'active' },
        { label: 'Maintenance', value: 'Maintenance' },
        { label: 'Draft', value: 'Draft' }
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
      title: 'Hotel Rating',
      type: 'single',
      options: [
        { label: '5 Stars', value: '5' },
        { label: '4 Stars', value: '4' },
        { label: '3 Stars', value: '3' }
      ]
    },
    {
      key: 'availability',
      title: 'Room Availability',
      type: 'single',
      options: [
        { label: 'Rooms Available', value: 'available' },
        { label: 'Fully Booked', value: 'booked' }
      ]
    }
  ]
};

// ===== DASHBOARD CARS (Extended from existing) =====
export const dashboardCarFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 0, max: 3000 },
  priceLabel: 'Price / Day',
  categoryOptions: [],
  customFilters: [
    {
      key: 'city',
      title: 'City',
      inputType: 'text',
      placeholder: 'Enter city name'
    },
    {
      key: 'carType',
      title: 'Car Type',
      type: 'single',
      options: [
        { label: 'Sedan', value: 'sedan' },
        { label: 'SUV', value: 'suv' },
        { label: 'Luxury', value: 'luxury' },
        { label: 'Van', value: 'van' },
        { label: 'Economy', value: 'economy' }
      ]
    },
    {
      key: 'transmission',
      title: 'Transmission',
      type: 'single',
      options: [
        { label: 'Automatic', value: 'automatic' },
        { label: 'Manual', value: 'manual' }
      ]
    },
    {
      key: 'status',
      title: 'Availability',
      type: 'single',
      options: [
        { label: 'Available', value: 'available' },
        { label: 'Maintenance', value: 'maintenance' },
        { label: 'Rented', value: 'rented' }
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
        { label: 'Active', value: 'active' },
        { label: 'Inactive', value: 'inactive' },
        { label: 'Full', value: 'full' }
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
