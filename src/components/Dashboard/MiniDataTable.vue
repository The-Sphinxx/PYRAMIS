<template>
  <div class="overflow-x-auto">
    <table class="table table-zebra w-full">
      <thead>
        <tr>
          <th class="bg-base-200">Item</th>
          <th class="bg-base-200">Location</th>
          <th class="bg-base-200">Price</th>
          <th class="bg-base-200">Status</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in items" :key="item.id" class="hover">
          <!-- Item with Image -->
          <td>
            <div class="flex items-center gap-3">
              <div class="avatar">
                <div class="mask mask-squircle w-12 h-12">
                  <img 
                    v-if="getImage(item)" 
                    :src="getImage(item)" 
                    :alt="getName(item)"
                    @error="handleImageError"
                  />
                  <div v-else class="w-12 h-12 bg-base-300 flex items-center justify-center">
                    <span class="text-xs text-base-content/40">No img</span>
                  </div>
                </div>
              </div>
              <div>
                <div class="font-bold text-sm">{{ getName(item) }}</div>
                <div v-if="type === 'users' || type === 'admins'" class="text-xs opacity-50">
                  {{ item.email }}
                </div>
              </div>
            </div>
          </td>

          <!-- Location/Info -->
          <td>
            <span class="text-sm">{{ getLocation(item) }}</span>
          </td>

          <!-- Price -->
          <td>
            <span v-if="getPrice(item)" class="font-semibold text-sm">
              ${{ getPrice(item) }}
            </span>
            <span v-else class="text-sm text-base-content/40">-</span>
          </td>

          <!-- Status -->
          <td>
            <div v-if="getStatus(item)" class="badge badge-sm" :class="getStatusBadgeClass(getStatus(item))">
              {{ getStatus(item) }}
            </div>
            <span v-else class="text-sm text-base-content/40">-</span>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Empty State -->
    <div v-if="!items || items.length === 0" class="text-center py-8 text-base-content/60">
      No items to display
    </div>
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
  if (props.type === 'users' || props.type === 'admins') {
    return `${item.firstName || ''} ${item.lastName || ''}`.trim() || 'Unnamed';
  }
  return item.name || item.title || 'Unnamed';
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
  if (props.type === 'bookings') {
    return item.itemName || 'N/A';
  }
  if (props.type === 'users' || props.type === 'admins') {
    return item.country || item.city || 'N/A';
  }
  return item.city || item.location || item.destination || 'N/A';
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

const getStatusBadgeClass = (status) => {
  const statusLower = status?.toLowerCase() || '';
  
  if (statusLower.includes('active') || statusLower.includes('available') || 
      statusLower.includes('confirmed') || statusLower.includes('verified')) {
    return 'badge-success';
  }
  if (statusLower.includes('pending')) {
    return 'badge-warning';
  }
  if (statusLower.includes('upcoming') || statusLower.includes('ongoing')) {
    return 'badge-info';
  }
  if (statusLower.includes('rented')) {
    return 'badge-accent';
  }
  if (statusLower.includes('maintenance') || statusLower.includes('suspended') || 
      statusLower.includes('cancelled') || statusLower.includes('disabled')) {
    return 'badge-error';
  }
  if (statusLower.includes('completed')) {
    return 'badge-ghost';
  }
  
  return 'badge-ghost';
};

const handleImageError = (e) => {
  e.target.style.display = 'none';
};
</script>
