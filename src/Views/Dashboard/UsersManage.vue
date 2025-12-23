<template>
  <div class="space-y-2">
    <!-- Stats Cards -->
    <StatsCard :stats="stats" />

    <!-- Data Table -->
    <DataTable
      title="Users"
      :columns="columns"
      :data="filteredUsers"
      :show-filter="true"
      :show-actions="{ edit: true, delete: true, view: true }"
      empty-message="No users found"
      :per-page="10"
      resource="users"
      :loading="loading"
      @add="handleAdd"
      @edit="handleEdit"
      @delete="handleDelete"
      @view="handleView"
      @filter="openFilterModal"
      @status-change="handleStatusChange"
    />

    <!-- Filter Modal -->
    <FilterModal 
      :is-open="showFilterModal"
      v-bind="filterConfig"
      @filter-change="applyFilters"
      @close="showFilterModal = false"
    />

    <!-- Form Modal -->
    <FormModal
      :is-open="showFormModal"
      :mode="formMode"
      :config="formConfig"
      :initial-data="selectedUser"
      @close="closeFormModal"
      @submit="handleFormSubmit"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import DataTable from '@/components/Dashboard/DataTable.vue';
import StatsCard from '@/components/Dashboard/StatsCard.vue';
import FilterModal from '@/components/Dashboard/FilterModal.vue';
import FormModal from '@/components/Dashboard/FormModal.vue';
import { usersAPI } from '@/Services/dashboardApi';
import { userFilterConfig } from '@/Utils/dashboardFilterConfigs';
import { userFormConfig } from '@/Utils/dashboardFormConfigs';

// State
const users = ref([]);
const loading = ref(false);
const showFilterModal = ref(false);
const router = useRouter();
const route = useRoute();
const showFormModal = ref(false);
const formMode = ref('add');
const selectedUser = ref({});
const activeFilters = ref({});

// Configs
const filterConfig = userFilterConfig;
const formConfig = userFormConfig;

// Table columns configuration
const columns = [
  {
    label: 'User',
    field: 'image',
    type: 'image',
    showNameWithImage: true,
    headerClass: 'w-1/4'
  },
  {
    label: 'Email',
    field: 'email',
    type: 'text',
    headerClass: 'w-1/4'
  },
  {
    label: 'Phone',
    field: 'phone',
    type: 'text',
    headerClass: 'w-1/6'
  },
  {
    label: 'Nationality',
    field: 'nationality',
    type: 'text',
    headerClass: 'w-1/6'
  },
  {
    label: 'Status',
    field: 'status',
    type: 'status-dropdown',
    options: ['Verified', 'Unverified', 'Suspended'],
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
  const total = users.value.length;
  const verified = users.value.filter(u => u.status === 'Verified').length;
  const newUsers = users.value.filter(u => {
    const date = new Date(u.createdAt);
    const now = new Date();
    // Consider "new" if created in the last 30 days
    const diffTime = Math.abs(now - date);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays <= 30;
  }).length;
  
  // Fake "Active" users based on some logic or random for demo if not in DB
  // For now, let's assume all verified are active
  const active = verified;

  return [
    {
      label: 'Total Users',
      value: total,
      icon: 'total', 
      trend: 2
    },
    {
      label: 'Verified Users',
      value: verified,
      icon: 'active', // Using 'active' icon for verified
      trend: 5
    },
    {
      label: 'New Users (30d)',
      value: newUsers,
      icon: 'featured', // Using 'featured' icon as proxy for 'new'
      trend: 10
    },
    {
      label: 'Active Users',
      value: active,
      icon: 'available',
      trend: 1
    }
  ];
});

// Filtered users based on active filters
const filteredUsers = computed(() => {
  let result = users.value;

  // Global Search from Sidebar
  if (route.query.q) {
    const search = route.query.q.toLowerCase();
    result = result.filter(u => 
      u.name?.toLowerCase().includes(search) || 
      u.email?.toLowerCase().includes(search) ||
      u.nationality?.toLowerCase().includes(search) ||
      u.status?.toLowerCase().includes(search) || // Text match for status
      u.phone?.toLowerCase().includes(search)
    );
  }

  // Apply nationality filter
  if (activeFilters.value.nationality) {
      if (Array.isArray(activeFilters.value.nationality) && activeFilters.value.nationality.length > 0) {
           const searchNat = activeFilters.value.nationality[0].toLowerCase();
           result = result.filter(u => u.nationality?.toLowerCase().includes(searchNat));
      } else if (typeof activeFilters.value.nationality === 'string' && activeFilters.value.nationality) {
           const searchNat = activeFilters.value.nationality.toLowerCase();
           result = result.filter(u => u.nationality?.toLowerCase().includes(searchNat));
      }
  }

  // Apply status filter
  if (activeFilters.value.statusSelected) {
    result = result.filter(u => u.status === activeFilters.value.statusSelected);
  } else if (activeFilters.value.status && activeFilters.value.status.length > 0) {
      // Handle array or string status
      const status = Array.isArray(activeFilters.value.status) ? activeFilters.value.status[0] : activeFilters.value.status;
      if (status) {
         result = result.filter(u => u.status === status);
      }
  }

  // Apply date range filters
  if (activeFilters.value.createdFrom) {
    const fromDate = new Date(activeFilters.value.createdFrom);
    result = result.filter(u => new Date(u.createdAt) >= fromDate);
  }
  if (activeFilters.value.createdTo) {
    const toDate = new Date(activeFilters.value.createdTo);
    result = result.filter(u => new Date(u.createdAt) <= toDate);
  }

  return result;
});

// Fetch users from API
const fetchUsers = async () => {
  loading.value = true;
  try {
    const response = await usersAPI.getAll();
    
    // Transform data to match table requirements
    users.value = response.data.map(user => {
      // Determine name
      let name = 'Unknown';
      if (user.fullName) name = user.fullName;
      else if (user.firstName && user.lastName) name = `${user.firstName} ${user.lastName}`;
      else if (user.firstName) name = user.firstName;

      // Determine status
      let status = user.status || (user.isVerified ? 'Verified' : 'Unverified');

      // Safe date parsing
      let createdAt = user.createdAt ? new Date(user.createdAt) : new Date();

      return {
        id: user.id,
        name: name,
        email: user.email || 'N/A',
        phone: user.phone || 'N/A',
        nationality: user.nationality || 'N/A',
        image: user.profileImage || user.userImage || '/images/default-avatar.png', // Fallback image
        status: status,
        createdAt: createdAt
      };
    });
  } catch (error) {
    console.error('Error fetching users:', error);
  } finally {
    loading.value = false;
  }
};

// Event handlers
const handleAdd = () => {
  formMode.value = 'add';
  selectedUser.value = {};
  showFormModal.value = true;
};

const handleEdit = (row) => {
  formMode.value = 'edit';
  // Find full user data from API data
  const fullUser = users.value.find(u => u.id === row.id);
  selectedUser.value = { ...fullUser };
  showFormModal.value = true;
};

const handleDelete = async (row) => {
  if (confirm(`Are you sure you want to delete "${row.name}"?`)) {
    try {
      await usersAPI.delete(row.id);
      await fetchUsers(); // Refresh data
      // console.log('Deleted user:', row); 
    } catch (error) {
      console.error('Error deleting user:', error);
      alert('Failed to delete user');
    }
  }
};

const openFilterModal = () => {
  showFilterModal.value = true;
};

const applyFilters = (filters) => {
  activeFilters.value = filters;
  showFilterModal.value = false;
};

const closeFormModal = () => {
  showFormModal.value = false;
  selectedUser.value = {};
};

const handleFormSubmit = async ({ mode, data }) => {
  loading.value = true;
  try {
    if (mode === 'add') {
      // Add new user
      await usersAPI.create(data);
    } else {
      // Update existing user
      await usersAPI.update(selectedUser.value.id, data);
    }
    await fetchUsers(); // Refresh data
    closeFormModal();
  } catch (error) {
    console.error('Error submitting form:', error);
    alert('Failed to save user');
  } finally {
    loading.value = false;
  }
};

const deleteUser = async (user) => {
// ... existing delete logic ...
};

const handleView = (row) => {
  router.push({ name: 'DashboardDetails', params: { type: 'users', id: row.id } });
};

const handleStatusChange = async ({ row, newValue }) => {
    try {
      await usersAPI.updateStatus(row.id, newValue);
      // Update local state to reflect change immediately (optimistic UI is handled by DataTable but good to sync)
      const userIndex = users.value.findIndex(u => u.id === row.id);
      if (userIndex !== -1) {
        users.value[userIndex].status = newValue;
      }
    } catch (error) {
      console.error('Error updating status:', error);
      alert('Failed to update status');
      // Revert if needed, but DataTable might need a refresh or prop update to revert
      await fetchUsers();
    }
};

// Lifecycle
onMounted(() => {
  fetchUsers();
});
</script>

<style scoped>
/* Additional custom styles if needed */
</style>