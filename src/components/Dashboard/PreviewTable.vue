<template>
  <div v-if="items && items.length > 0" class="space-y-2">
    <div 
      v-for="item in items" 
      :key="item.id"
      class="flex items-center justify-between p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
    >
      <!-- Left side: Image and Info -->
      <div class="flex items-center gap-3 flex-1 min-w-0">
        <!-- Image -->
        <img 
          v-if="getImage(item)" 
          :src="getImage(item)" 
          :alt="getName(item)"
          class="w-12 h-12 rounded object-cover flex-shrink-0"
          @error="handleImageError"
        />
        <div 
          v-else
          class="w-12 h-12 rounded bg-gray-200 flex items-center justify-center flex-shrink-0"
        >
          <span class="text-gray-400 text-xs">No img</span>
        </div>

        <!-- Info -->
        <div class="flex-1 min-w-0">
          <p class="font-medium text-gray-900 truncate">{{ getName(item) }}</p>
          <p class="text-sm text-gray-600 truncate">{{ getLocation(item) }}</p>
        </div>
      </div>

      <!-- Right side: Price and Status -->
      <div class="text-right flex-shrink-0 ml-4">
        <p v-if="getPrice(item)" class="font-semibold text-gray-900">
          ${{ getPrice(item) }}
        </p>
        <span 
          v-if="getStatus(item)"
          :class="getStatusClass(getStatus(item))"
          class="text-xs px-2 py-1 rounded-full inline-block mt-1"
        >
          {{ getStatus(item) }}
        </span>
      </div>
    </div>
  </div>
  <div v-else class="text-center py-8 text-gray-500">
    No items to display
  </div>
</template>

<script setup>
const props = defineProps({
  items: {
    type: Array,
    required: true
  },
  type: {
    type: String,
    required: true
  }
});

const getName = (item) => {
  return item.name || item.title || item.firstName + ' ' + item.lastName || 'Unnamed';
};

const getImage = (item) => {
  if (Array.isArray(item.images) && item.images.length > 0) {
    return item.images[0];
  }
  if (typeof item.images === 'string') {
    return item.images;
  }
  if (item.image) {
    return item.image;
  }
  if (item.profileImage) {
    return item.profileImage;
  }
  return null;
};

const getLocation = (item) => {
  if (props.type === 'users' || props.type === 'admins') {
    return item.email || '';
  }
  if (props.type === 'bookings') {
    return `Booking Date: ${item.bookingDate || 'N/A'}`;
  }
  return item.city || item.location || item.destination || '';
};

const getPrice = (item) => {
  if (props.type === 'users' || props.type === 'admins') {
    return null;
  }
  return item.price || item.pricePerDay || item.basePrice || null;
};

const getStatus = (item) => {
  return item.status || item.availability || null;
};

const getStatusClass = (status) => {
  const statusLower = status?.toLowerCase() || '';
  
  const classes = {
    'active': 'bg-green-100 text-green-800',
    'available': 'bg-green-100 text-green-800',
    'confirmed': 'bg-green-100 text-green-800',
    'completed': 'bg-gray-100 text-gray-800',
    'verified': 'bg-green-100 text-green-800',
    'pending': 'bg-yellow-100 text-yellow-800',
    'upcoming': 'bg-blue-100 text-blue-800',
    'ongoing': 'bg-blue-100 text-blue-800',
    'rented': 'bg-orange-100 text-orange-800',
    'maintenance': 'bg-red-100 text-red-800',
    'disabled': 'bg-gray-100 text-gray-800',
    'suspended': 'bg-red-100 text-red-800',
    'cancelled': 'bg-red-100 text-red-800'
  };

  for (const [key, value] of Object.entries(classes)) {
    if (statusLower.includes(key)) {
      return value;
    }
  }

  return 'bg-gray-100 text-gray-800';
};

const handleImageError = (e) => {
  e.target.style.display = 'none';
};
</script>
