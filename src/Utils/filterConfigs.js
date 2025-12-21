// src/utils/filterConfigs.js

// ===== ATTRACTIONS =====
export const attractionFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 50, max: 1000 },
  priceLabel: 'Filter By Price',
  categoryOptions: [
    { label: 'Museums', value: 'museums' },
    { label: 'Historical Sites', value: 'historical' },
    { label: 'Adventure Parks', value: 'adventure' },
    { label: 'Cultural Tours', value: 'cultural' },
    { label: 'Nature & Wildlife', value: 'nature' },
    { label: 'Religious Sites', value: 'religious' },
    { label: 'Entertainment', value: 'entertainment' }
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
      key: 'ticketType',
      title: 'Ticket Type',
      type: 'single',
      options: [
        { label: 'Single Entry', value: 'single' },
        { label: 'Group Entry', value: 'group' },
        { label: 'VIP Entry', value: 'vip' }
      ]
    },
    {
      key: 'checkIn',
      title: 'Check-In',
      inputType: 'date'
    },
    {
      key: 'checkOut',
      title: 'Check-Out',
      inputType: 'date'
    },
    {
      key: 'duration',
      title: 'Duration',
      type: 'single',
      options: [
        { label: 'Less than 2 hours', value: 'short' },
        { label: '2-4 hours', value: 'medium' },
        { label: 'Half day', value: 'half-day' },
        { label: 'Full day', value: 'full-day' }
      ]
    },
    {
      key: 'availability',
      title: 'Availability',
      type: 'single',
      options: [
        { label: 'Available Now', value: 'now' },
        { label: 'Book in Advance', value: 'advance' }
      ]
    }
  ]
};

// ===== TRIPS =====
export const tripFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 500, max: 5000 },
  priceLabel: 'Filter By Price',
  categoryOptions: [],
  customFilters: [
    {
      key: 'checkIn',
      title: 'Check-In',
      inputType: 'date'
    },
    {
      key: 'checkOut',
      title: 'Check-Out',
      inputType: 'date'
    },
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
      key: 'groupSize',
      title: 'Group Size',
      type: 'single',
      options: [
        { label: 'Private', value: 'private' },
        { label: 'Small (2-8)', value: 'small' },
        { label: 'Medium (9-15)', value: 'medium' },
        { label: 'Large (16+)', value: 'large' }
      ]
    }
  ]
};

// ===== HOTELS =====
export const hotelFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 200, max: 3000 },
  priceLabel: 'Price / Night',
  categoryOptions: [],
  customFilters: [
    {
      key: 'location',
      title: 'Location',
      inputType: 'text',
      placeholder: 'Enter location'
    },
    {
      key: 'checkIn',
      title: 'Check-In',
      inputType: 'date'
    },
    {
      key: 'checkOut',
      title: 'Check-Out',
      inputType: 'date'
    },
    {
      key: 'guests',
      title: 'Guests',
      inputType: 'number',
      placeholder: 'Number of guests'
    },
    {
      key: 'rating',
      title: 'Hotel Rating',
      type: 'single',
      options: [
        { label: '5 Stars', value: '5' },
        { label: '4 Stars', value: '4' },
        { label: '3 Stars', value: '3' },
        { label: '2 Stars', value: '2' },
        { label: '1 Star', value: '1' }
      ]
    },
    {
      key: 'amenities',
      title: 'Amenities',
      options: [
        { label: 'Swimming Pool', value: 'pool' },
        { label: 'Free WiFi', value: 'wifi' },
        { label: 'Free Parking', value: 'parking' },
        { label: 'Gym', value: 'gym' },
        { label: 'Restaurant', value: 'restaurant' },
        { label: 'Beach Access', value: 'beach' },
        { label: 'Spa', value: 'spa' },
        { label: 'Room Service', value: 'room-service' }
      ]
    }
  ]
};

// ===== CARS =====
export const carFilterConfig = {
  showPriceFilter: true,
  priceRange: { min: 100, max: 2000 },
  priceLabel: 'Price / Day',
  categoryOptions: [],
  customFilters: [
    {
      key: 'pickupLocation',
      title: 'Pickup Location',
      inputType: 'text',
      placeholder: 'Enter pickup location'
    },
    {
      key: 'pickupDate',
      title: 'Pickup Date',
      inputType: 'date'
    },
    {
      key: 'returnDate',
      title: 'Return Date',
      inputType: 'date'
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
      key: 'seats',
      title: 'Seats',
      type: 'single',
      options: [
        { label: '2 Seats', value: '2' },
        { label: '4 Seats', value: '4' },
        { label: '5 Seats', value: '5' },
        { label: '7+ Seats', value: '7+' }
      ]
    }
  ]
};