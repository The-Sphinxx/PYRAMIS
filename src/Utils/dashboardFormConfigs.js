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
    },
    {
      key: 'images',
      label: 'Images',
      type: 'image-upload',
      maxImages: 4,
      required: true,
      accept: 'image/jpeg,image/png,image/webp',
      maxSize: 2 * 1024 * 1024 // 2MB per image
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
      key: 'overview',
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
      key: 'availability.totalRooms',
      label: 'Total Rooms',
      type: 'number',
      placeholder: 'Enter total rooms',
      min: 1,
      required: true
    },
    {
      key: 'availability.availableRooms',
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
    },
    {
      key: 'images',
      label: 'Images',
      type: 'image-upload',
      maxImages: 4,
      required: true,
      accept: 'image/jpeg,image/png,image/webp',
      maxSize: 2 * 1024 * 1024 // 2MB per image
    },
    {
      key: 'highlights',
      label: 'Highlights',
      type: 'tags',
      placeholder: 'Add highlight',
      required: false
    },
    {
      key: 'roomAmenities',
      label: 'Room Amenities',
      type: 'tags',
      placeholder: 'Add amenity',
      required: false
    },
    {
      key: 'nearbyLocations',
      label: 'Nearby Locations',
      type: 'tags',
      placeholder: 'Add location',
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
      required: false
    },
    {
      key: 'model',
      label: 'Model',
      type: 'text',
      placeholder: 'e.g., X5',
      required: false
    },
    {
      key: 'year',
      label: 'Year',
      type: 'number',
      placeholder: 'e.g., 2023',
      min: 1990,
      max: new Date().getFullYear() + 1,
      required: false
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
      key: 'totalFleet',
      label: 'Total Fleet',
      type: 'number',
      placeholder: 'Total number of this car',
      min: 1,
      required: true
    },
    {
      key: 'availableNow',
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
      key: 'nextAvailability',
      label: 'Next Availability',
      type: 'date',
      required: false
    },
    {
      key: 'description',
      label: 'Description',
      type: 'textarea',
      required: true
    },
    {
      key: 'overview',
      label: 'Overview',
      type: 'textarea',
      required: false
    },
    {
      key: 'features',
      label: 'Technical Features',
      type: 'tags',
      placeholder: 'Add feature (Press Enter)',
      required: false
    },
    {
      key: 'highlights',
      label: 'Highlights',
      type: 'tags',
      placeholder: 'Add highlight',
      required: false
    },
    {
      key: 'amenities',
      label: 'Amenities',
      type: 'tags',
      placeholder: 'Add amenity',
      required: false
    },
    {
      key: 'images',
      label: 'Images',
      type: 'image-upload',
      maxImages: 4,
      required: true,
      accept: 'image/jpeg,image/png,image/webp',
      maxSize: 2 * 1024 * 1024 // 2MB per image
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
      key: 'availableSpots',
      label: 'Available Spots',
      type: 'number',
      placeholder: 'Currently available spots',
      min: 0,
      required: false
    },

    {
      key: 'highlights',
      label: 'Highlights',
      type: 'tags',
      placeholder: 'Add highlight',
      required: false
    },
    {
      key: 'itinerary',
      label: 'Itinerary',
      type: 'itinerary-builder',
      required: false
    },
    {
      key: 'hotel.name',
      label: 'Hotel Name',
      type: 'text',
      required: false
    },
    {
      key: 'hotel.rating',
      label: 'Hotel Rating',
      type: 'number',
      step: 0.1,
      required: false
    },
    {
      key: 'amenities',
      label: 'Amenities',
      type: 'tags', // Assumes data is array of strings or handled
      required: false
    },
    {
      key: 'images',
      label: 'Images',
      type: 'image-upload',
      maxImages: 4,
      required: true,
      accept: 'image/jpeg,image/png,image/webp',
      maxSize: 2 * 1024 * 1024 // 2MB per image
    },
    {
      key: 'maxPeople',
      label: 'Max People',
      type: 'number',
      min: 1,
      required: true
    },
    {
      key: 'price',
      label: 'Price',
      type: 'number',
      min: 0,
      required: true
    },
    {
      key: 'availableDates',
      label: 'Available Dates',
      type: 'tags',
      placeholder: 'Add date range',
      required: false
    },
    {
      key: 'status',
      label: 'Status',
      type: 'select',
      options: [
        { label: 'Upcoming', value: 'Upcoming' },
        { label: 'Ongoing', value: 'Ongoing' },
        { label: 'Completed', value: 'Completed' }
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

// ===== BOOKINGS =====
export const bookingFormConfig = {
  title: 'Booking',
  fields: [
    {
      key: 'id',
      label: 'Booking ID',
      type: 'text',
      readonly: true
    },
    {
      key: 'type',
      label: 'Type',
      type: 'text',
      readonly: true
    },
    {
      key: 'userName',
      label: 'User Name',
      type: 'text',
      readonly: true
    },
    {
      key: 'bookingDate',
      label: 'Booking Date',
      type: 'date',
      readonly: true
    },
    {
      key: 'status',
      label: 'Status',
      type: 'status',
      required: true
    },
    {
      key: 'paymentStatus',
      label: 'Payment Status',
      type: 'status',
      required: true
    },
    {
      key: 'price',
      label: 'Total Amount',
      type: 'price',
      readonly: true
    }
  ]
};
