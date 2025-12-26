<template>
  <div class="card bg-base-100 shadow-lg hover:shadow-xl transition-all overflow-hidden border border-base-300">
    <div class="flex flex-col md:flex-row">
      <!-- Image -->
      <div class="relative w-full md:w-48 h-48 md:h-auto flex-shrink-0">
        <img
          :src="booking.image || 'https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=400'"
          :alt="booking.title"
          class="w-full h-full object-cover"
        />
        <div class="absolute top-3 right-3 badge badge-lg bg-white/90 backdrop-blur-sm gap-2">
          <i :class="getTypeIcon(booking.type)" class="text-primary"></i>
          <span class="font-semibold capitalize">{{ formatType(booking.type) }}</span>
        </div>
      </div>

      <!-- Content -->
      <div class="card-body p-6 flex flex-col justify-between">
        <div>
          <h3 class="text-xl font-bold text-base-content mb-3">
            {{ booking.title }}
          </h3>
          <div class="flex items-center gap-2 text-base-content/70 mb-3">
            <i class="fas fa-map-marker-alt text-primary"></i>
            <span class="text-sm">{{ booking.location }}</span>
          </div>
          <div class="flex items-start gap-4 flex-wrap">
            <div class="flex items-center gap-2 text-base-content">
              <i class="fas fa-calendar text-primary"></i>
              <span class="text-sm font-medium">
                {{ booking.date }}
                <span v-if="booking.time" class="text-base-content/60"> • {{ booking.time }}</span>
              </span>
            </div>
            <span class="text-xs text-base-content/50 bg-base-200 px-3 py-1 rounded-md">
              Reference: {{ booking.reference }}
            </span>
          </div>
        </div>

        <div class="flex items-center justify-between mt-4 pt-4 border-t border-base-300">
          <span class="badge badge-success badge-lg gap-2">
            <i class="fas fa-check-circle"></i>
            {{ booking.status }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'BookingCard',
  props: {
    booking: {
      type: Object,
      required: true
    }
  },
  emits: ['view-details'],
  methods: {
    getTypeIcon(type) {
      const typeMap = {
        'trip': 'fas fa-map-marked-alt',
        'hotel': 'fas fa-hotel',
        'car': 'fas fa-car',
        'attraction': 'fas fa-landmark'
      };
      return typeMap[type?.toLowerCase()] || 'fas fa-map-marked-alt';
    },
    formatType(type) {
      return type?.charAt(0).toUpperCase() + type?.slice(1).toLowerCase() || 'Booking';
    }
  }
}
</script>

<style scoped>
.card {
  border-radius: 8px;
}

.badge {
  border-radius: 6px;
}
</style>