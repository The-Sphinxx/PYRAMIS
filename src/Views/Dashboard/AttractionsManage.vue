<template>
  <div class="space-y-6 p-6">
    <!-- Stats Cards -->
    <StatsCard :stats="stats" />

    <!-- Data Table -->
    <DataTable
      title="Attractions"
      :columns="columns"
      :data="attractions"
      :show-filter="true"
      add-button-text="Add New Attraction"
      :show-actions="{ edit: true, delete: true, view: true }"
      empty-message="No attractions available"
      :per-page="8"
      resource="attractions"
      :loading="loading"
      @add="handleAdd"
      @edit="handleEdit"
      @delete="handleDelete"
      @view="handleView"
      @toggle="handleToggle"
      @status-click="handleStatusClick"
      @status-change="handleStatusChange"
      @filter="handleFilter"
      v-model:data="attractions"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import DataTable from '@/components/Dashboard/DataTable.vue';
import StatsCard from '@/components/Dashboard/StatsCard.vue';
import { attractionsAPI } from '@/Services/dashboardApi';

// State
const attractions = ref([]);
const loading = ref(false);

// Table columns configuration
const columns = [
  {
    label: 'Attraction',
    field: 'images',
    type: 'image',
    showNameWithImage: true,
    headerClass: 'w-1/4'
  },
  {
    label: 'Price',
    field: 'price',
    type: 'text',
    headerClass: 'w-1/6'
  },
  {
    label: 'Capacity',
    field: 'capacity',
    type: 'text',
    headerClass: 'w-1/6'
  },
  {
    label: 'Featured',
    field: 'isFeatured',
    type: 'toggle',
    headerClass: 'w-1/8'
  },
  {
    label: 'Availability',
    field: 'availability',
    type: 'status',
    headerClass: 'w-1/8',
    clickable: true
  },
  {
    label: 'Status',
    field: 'status',
    type: 'status-dropdown',
    options: ['Active', 'Pending', 'Disabled', 'Suspended'],
    headerClass: 'w-1/8'
  },
  {
    label: 'Action',
    type: 'actions',
    headerClass: 'w-1/8'
  }
];

// Computed stats
const stats = computed(() => {
  const total = attractions.value.length;
  const active = attractions.value.filter(a => a.status === 'Active').length;
  const featured = attractions.value.filter(a => a.isFeatured).length;
  const available = attractions.value.filter(a => a.availability === 'Available').length;

  return [
    {
      label: 'Total Attractions',
      value: total,
      icon: 'total',
      trend: 5
    },
    {
      label: 'Active',
      value: active,
      icon: 'active',
      trend: 3
    },
    {
      label: 'Featured',
      value: featured,
      icon: 'featured',
      trend: -2
    },
    {
      label: 'Available',
      value: available,
      icon: 'available',
      trend: 8
    }
  ];
});

// Fetch attractions from API
const fetchAttractions = async () => {
  loading.value = true;
  try {
    const response = await attractionsAPI.getAll();
    
    // Transform data to match table requirements
    attractions.value = response.data.map(attraction => ({
      id: attraction.id,
      name: attraction.name,
      location: `${attraction.city}, Egypt`,
      year: attraction.year || '',
      images: attraction.images?.[0] || '/placeholder-attraction.jpg',
      price: attraction.price || 'Free',
      capacity: attraction.capacity || '18 / 42 Ticket',
      isFeatured: attraction.isFeatured || false,
      availability: attraction.availability || (Math.random() > 0.5 ? 'Available' : 'Sold Out'),
      status: attraction.status || (Math.random() > 0.7 ? 'Active' : Math.random() > 0.5 ? 'Pending' : 'Disabled'),
      rating: attraction.rating,
      reviewsCount: attraction.reviews?.totalReviews || 0
    }));
  } catch (error) {
    console.error('Error fetching attractions:', error);
  } finally {
    loading.value = false;
  }
};

// Event handlers
const handleAdd = () => {
  console.log('Add new attraction');
  // TODO: Open modal or navigate to add page
};

const handleEdit = (row) => {
  console.log('Edit attraction:', row);
  // TODO: Open edit modal or navigate to edit page
};

const handleDelete = async (row) => {
  if (confirm(`Are you sure you want to delete "${row.name}"?`)) {
    try {
      await attractionsAPI.delete(row.id);
      await fetchAttractions(); // Refresh data
      console.log('Deleted attraction:', row);
    } catch (error) {
      console.error('Error deleting attraction:', error);
      alert('Failed to delete attraction');
    }
  }
};

const handleView = (row) => {
  console.log('View attraction:', row);
  // TODO: Navigate to attraction details page
};

const handleToggle = async ({ row, field, newValue }) => {
  try {
    // Update the featured status
    await attractionsAPI.toggleFeatured(row.id, newValue);
    console.log(`Toggled ${field} for ${row.name} to ${newValue}`);
  } catch (error) {
    console.error('Error toggling featured:', error);
    // Revert the change
    await fetchAttractions();
  }
};

const handleStatusClick = async ({ row, field, value }) => {
  if (field === 'availability') {
    const newAvailability = value === 'Available' ? 'Sold Out' : 'Available';
    
    // Optimistic update
    const rowIndex = attractions.value.findIndex(a => a.id === row.id);
    if (rowIndex !== -1) {
      attractions.value[rowIndex].availability = newAvailability;
    }

    try {
      await attractionsAPI.updateAvailability(row.id, newAvailability);
      console.log(`Updated availability for ${row.name} to ${newAvailability}`);
    } catch (error) {
      console.error('Error updating availability:', error);
      // Revert on error
      if (rowIndex !== -1) {
        attractions.value[rowIndex].availability = value;
      }
      alert('Failed to update availability');
    }
  }
};

const handleStatusChange = async ({ row, field, newValue }) => {
  try {
    // Update the status
    if (field === 'status') {
      await attractionsAPI.updateStatus(row.id, newValue);
    } else if (field === 'availability') {
      await attractionsAPI.updateAvailability(row.id, newValue);
    }
    console.log(`Changed ${field} for ${row.name} to ${newValue}`);
  } catch (error) {
    console.error('Error updating status:', error);
    // Revert the change
    await fetchAttractions();
  }
};

const handleFilter = () => {
  console.log('Open filter modal');
  // TODO: Implement filter functionality
};

// Lifecycle
onMounted(() => {
  fetchAttractions();
});
</script>

<style scoped>
/* Additional custom styles if needed */
</style>
