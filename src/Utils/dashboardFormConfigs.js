// src/Utils/dashboardFormConfigs.js
// Form field configurations for dashboard management pages

// ===== USERS =====
export const userFormConfig = {
  title: 'User',
  fields: [
    {
      key: 'firstName',
      label: 'First Name',
      type: 'text',
      placeholder: 'Enter first name',
      required: true
    },
    {
      key: 'lastName',
      label: 'Last Name',
      type: 'text',
      placeholder: 'Enter last name',
      required: true
    },
    {
      key: 'email',
      label: 'Email',
      type: 'email',
      placeholder: 'Enter email address',
      required: true
    },
    {
      key: 'phone',
      label: 'Phone',
      type: 'tel',
      placeholder: '+20 XXX XXX XXXX',
      required: true
    },
    {
      key: 'dateOfBirth',
      label: 'Date of Birth',
      type: 'date',
      required: false
    },
    {
      key: 'nationality',
      label: 'Nationality',
      type: 'text',
      placeholder: 'Enter nationality',
      required: false
    },
    {
      key: 'gender',
      label: 'Gender',
      type: 'select',
      options: [
        { label: 'Male', value: 'male' },
        { label: 'Female', value: 'female' }
      ],
      required: false
    },
    {
      key: 'password',
      label: 'Password',
      type: 'password',
      placeholder: 'Enter password (leave empty to keep current)',
      required: false, // Required only on add, not on edit
      addOnly: true
    }
  ]
};

// ===== ATTRACTIONS =====
export const attractionFormConfig = {
  title: 'Attraction',
  fields: [
    {
      key: 'name',
      label: 'Attraction Name',
      type: 'text',
      placeholder: 'Enter attraction name',
      required: true
    },
    {
      key: 'city',
      label: 'City',
      type: 'text',
      placeholder: 'Enter city',
      required: true
    },
    {
      key: 'description',
      label: 'Description',
      type: 'textarea',
      placeholder: 'Enter brief description',
      required: true,
      rows: 3
    },
    {
      key: 'overview',
      label: 'Overview',
      type: 'textarea',
      placeholder: 'Enter detailed overview',
      required: true,
      rows: 5
    },
    {
      key: 'price',
      label: 'Price',
      type: 'text',
      placeholder: 'e.g., 12$ or Free',
      required: true
    },
    {
      key: 'capacity',
      label: 'Capacity',
      type: 'text',
      placeholder: 'e.g., 18 / 42 Ticket',
      required: true
    },
    {
      key: 'rating',
      label: 'Rating',
      type: 'number',
      placeholder: '0.0 - 5.0',
      min: 0,
      max: 5,
      step: 0.1,
      required: true
    },
    {
      key: 'latitude',
      label: 'Latitude',
      type: 'number',
      placeholder: 'Enter latitude',
      step: 0.000001,
      required: false
    },
    {
      key: 'longitude',
      label: 'Longitude',
      type: 'number',
      placeholder: 'Enter longitude',
      step: 0.000001,
      required: false
    },
    {
      key: 'categories',
      label: 'Categories',
      type: 'multi-select',
      options: [
        { label: 'Top Picks', value: 'TOP PICKS' },
        { label: 'Historical', value: 'HISTORICAL' },
        { label: 'Culture', value: 'CULTURE' },
        { label: 'Museums', value: 'MUSEUMS' },
        { label: 'Nature', value: 'NATURE' },
        { label: 'Religious (Ancient)', value: 'RELIGIOUS (Ancient)' },
        { label: 'Ancient Sites', value: 'ANCIENT SITES' },
        { label: 'Shopping', value: 'SHOPPING' },
        { label: 'Family', value: 'FAMILY' },
        { label: 'Beaches', value: 'BEACHES' }
      ],
      required: true
    },
    {
      key: 'status',
      label: 'Status',
      type: 'select',
      options: [
        { label: 'Active', value: 'Active' },
        { label: 'Pending', value: 'Pending' },
        { label: 'Disabled', value: 'Disabled' },
        { label: 'Suspended', value: 'Suspended' }
      ],
      required: true
    },
    {
      key: 'availability',
      label: 'Availability',
      type: 'select',
      options: [
        { label: 'Available', value: 'Available' },
        { label: 'Sold Out', value: 'Sold Out' }
      ],
      required: true
    },
    {
      key: 'isFeatured',
      label: 'Featured',
      type: 'toggle',
      required: false
    }
  ]
};

// ===== HOTELS =====
export const hotelFormConfig = {
  title: 'Hotel',
  fields: [
    {
      key: 'name',
      label: 'Hotel Name',
      type: 'text',
      placeholder: 'Enter hotel name',
      required: true
    },
    {
      key: 'city',
      label: 'City',
      type: 'text',
      placeholder: 'Enter city',
      required: true
    },
    {
      key: 'address',
      label: 'Address',
      type: 'textarea',
      placeholder: 'Enter full address',
      required: true,
      rows: 2
    },
    {
      key: 'description',
      label: 'Description',
      type: 'textarea',
      placeholder: 'Enter description',
      required: true,
      rows: 4
    },
    {
      key: 'pricePerNight',
      label: 'Price Per Night',
      type: 'number',
      placeholder: 'Enter price',
      min: 0,
      required: true
    },
    {
      key: 'rating',
      label: 'Rating',
      type: 'number',
      placeholder: '0.0 - 5.0',
      min: 0,
      max: 5,
      step: 0.1,
      required: true
    },
    {
      key: 'stars',
      label: 'Star Rating',
      type: 'select',
      options: [
        { label: '5 Stars', value: 5 },
        { label: '4 Stars', value: 4 },
        { label: '3 Stars', value: 3 },
        { label: '2 Stars', value: 2 },
        { label: '1 Star', value: 1 }
      ],
      required: true
    },
    {
      key: 'totalRooms',
      label: 'Total Rooms',
      type: 'number',
      placeholder: 'Enter total rooms',
      min: 1,
      required: true
    },
    {
      key: 'availableRooms',
      label: 'Available Rooms',
      type: 'number',
      placeholder: 'Enter available rooms',
      min: 0,
      required: true
    },
    {
      key: 'amenities',
      label: 'Amenities',
      type: 'multi-select',
      options: [
        { label: 'Swimming Pool', value: 'pool' },
        { label: 'Free WiFi', value: 'wifi' },
        { label: 'Free Parking', value: 'parking' },
        { label: 'Gym', value: 'gym' },
        { label: 'Restaurant', value: 'restaurant' },
        { label: 'Beach Access', value: 'beach' },
        { label: 'Spa', value: 'spa' },
        { label: 'Room Service', value: 'room-service' }
      ],
      required: false
    },
    {
      key: 'status',
      label: 'Status',
      type: 'select',
      options: [
        { label: 'Active', value: 'active' },
        { label: 'Maintenance', value: 'Maintenance' },
        { label: 'Draft', value: 'Draft' }
      ],
      required: true
    },
    {
      key: 'featured',
      label: 'Featured',
      type: 'toggle',
      required: false
    }
  ]
};

// ===== CARS =====
export const carFormConfig = {
  title: 'Vehicle',
  fields: [
    {
      key: 'name',
      label: 'Vehicle Name',
      type: 'text',
      placeholder: 'e.g., BMW X5',
      required: true
    },
    {
      key: 'brand',
      label: 'Brand',
      type: 'text',
      placeholder: 'e.g., BMW',
      required: true
    },
    {
      key: 'model',
      label: 'Model',
      type: 'text',
      placeholder: 'e.g., X5',
      required: true
    },
    {
      key: 'year',
      label: 'Year',
      type: 'number',
      placeholder: 'e.g., 2023',
      min: 1990,
      max: new Date().getFullYear() + 1,
      required: true
    },
    {
      key: 'city',
      label: 'City',
      type: 'text',
      placeholder: 'Enter city',
      required: true
    },
    {
      key: 'price',
      label: 'Price Per Day',
      type: 'number',
      placeholder: 'Enter price',
      min: 0,
      required: true
    },
    {
      key: 'type',
      label: 'Car Type',
      type: 'select',
      options: [
        { label: 'Sedan', value: 'sedan' },
        { label: 'SUV', value: 'suv' },
        { label: 'Luxury', value: 'luxury' },
        { label: 'Van', value: 'van' },
        { label: 'Economy', value: 'economy' }
      ],
      required: true
    },
    {
      key: 'transmission',
      label: 'Transmission',
      type: 'select',
      options: [
        { label: 'Automatic', value: 'automatic' },
        { label: 'Manual', value: 'manual' }
      ],
      required: true
    },
    {
      key: 'seats',
      label: 'Number of Seats',
      type: 'number',
      placeholder: 'Enter number of seats',
      min: 2,
      max: 15,
      required: true
    },
    {
      key: 'total_fleet',
      label: 'Total Fleet',
      type: 'number',
      placeholder: 'Total number of this car',
      min: 1,
      required: true
    },
    {
      key: 'available_now',
      label: 'Available Now',
      type: 'number',
      placeholder: 'Currently available',
      min: 0,
      required: true
    },
    {
      key: 'fuelType',
      label: 'Fuel Type',
      type: 'select',
      options: [
        { label: 'Petrol', value: 'petrol' },
        { label: 'Diesel', value: 'diesel' },
        { label: 'Electric', value: 'electric' },
        { label: 'Hybrid', value: 'hybrid' }
      ],
      required: false
    },
    {
      key: 'status',
      label: 'Status',
      type: 'select',
      options: [
        { label: 'Available', value: 'available' },
        { label: 'Maintenance', value: 'maintenance' },
        { label: 'Rented', value: 'rented' }
      ],
      required: true
    },
    {
      key: 'featured',
      label: 'Featured',
      type: 'toggle',
      required: false
    }
  ]
};

// ===== TRIPS =====
export const tripFormConfig = {
  title: 'Trip',
  fields: [
    {
      key: 'title',
      label: 'Trip Title',
      type: 'text',
      placeholder: 'Enter trip title',
      required: true
    },
    {
      key: 'destination',
      label: 'Destination',
      type: 'text',
      placeholder: 'Enter destination',
      required: true
    },
    {
      key: 'description',
      label: 'Description',
      type: 'textarea',
      placeholder: 'Enter trip description',
      required: true,
      rows: 4
    },
    {
      key: 'price',
      label: 'Price',
      type: 'number',
      placeholder: 'Enter price',
      min: 0,
      required: true
    },
    {
      key: 'duration',
      label: 'Duration (Days)',
      type: 'number',
      placeholder: 'Number of days',
      min: 1,
      required: true
    },
    {
      key: 'tripType',
      label: 'Trip Type',
      type: 'select',
      options: [
        { label: 'Beach Trip', value: 'beach' },
        { label: 'Desert Safari', value: 'desert' },
        { label: 'Nile Cruise', value: 'cruise' },
        { label: 'City Tour', value: 'city' },
        { label: 'Diving', value: 'diving' },
        { label: 'Historical', value: 'historical' }
      ],
      required: true
    },
    {
      key: 'maxGroupSize',
      label: 'Max Group Size',
      type: 'number',
      placeholder: 'Maximum participants',
      min: 1,
      required: true
    },
    {
      key: 'availableSpots',
      label: 'Available Spots',
      type: 'number',
      placeholder: 'Currently available spots',
      min: 0,
      required: true
    },
    {
      key: 'startDate',
      label: 'Start Date',
      type: 'date',
      required: false
    },
    {
      key: 'endDate',
      label: 'End Date',
      type: 'date',
      required: false
    },
    {
      key: 'includes',
      label: 'Includes',
      type: 'multi-select',
      options: [
        { label: 'Accommodation', value: 'accommodation' },
        { label: 'Meals', value: 'meals' },
        { label: 'Transport', value: 'transport' },
        { label: 'Guide', value: 'guide' },
        { label: 'Activities', value: 'activities' },
        { label: 'Equipment', value: 'equipment' }
      ],
      required: false
    },
    {
      key: 'status',
      label: 'Status',
      type: 'select',
      options: [
        { label: 'Active', value: 'active' },
        { label: 'Inactive', value: 'inactive' },
        { label: 'Full', value: 'full' }
      ],
      required: true
    },
    {
      key: 'featured',
      label: 'Featured',
      type: 'toggle',
      required: false
    }
  ]
};
