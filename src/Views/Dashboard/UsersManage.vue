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
      :initial-filters="activeFilters"
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

    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="modal modal-open">
      <div class="modal-box">
        <h3 class="font-bold text-lg text-base-content">Confirm Delete</h3>
        <p class="py-4 text-base-content">
          Are you sure you want to delete <span class="font-semibold">"{{ userToDelete?.name }}"</span>? 
          This action cannot be undone.
        </p>
        <div class="modal-action">
          <button 
            class="btn btn-ghost" 
            @click="cancelDelete"
          >
            Cancel
          </button>
          <button 
            class="btn btn-error" 
            @click="confirmDelete"
          >
            Delete
          </button>
        </div>
      </div>
    </div>
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
import { useToast } from '@/composables/useToast';

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
const showDeleteModal = ref(false);
const userToDelete = ref(null);

// Toast notifications
const { toast } = useToast();

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
    users.value = response.data.data.map(user => {
      // Determine name
      let name = 'Unknown';
      if (user.fullName) name = user.fullName;
      else if (user.firstName && user.lastName) name = `${user.firstName} ${user.lastName}`;
      else if (user.firstName) name = user.firstName;

      // Determine status
      let status = 'Unverified';
      if (user.lockoutEnd && new Date(user.lockoutEnd) > new Date()) {
        status = 'Suspended';
      } else if (user.isVerified) {
        status = 'Verified';
      }

      // Safe date parsing
      let createdAt = user.createdAt ? new Date(user.createdAt) : new Date();

      return {
        id: user.id,
        name: name,
        firstName: user.firstName, // Store original fields
        lastName: user.lastName,   // Store original fields
        email: user.email || 'N/A',
        phone: user.phoneNumber || 'N/A',
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
  
  // Map backend fields to form fields
  selectedUser.value = {
    ...fullUser,
    // Map phoneNumber to phone for form compatibility
    phone: fullUser.phoneNumber || fullUser.phone || '',
    // Map profileImage to image for form compatibility  
    image: fullUser.profileImage || fullUser.image || '',
    // Ensure firstName and lastName are set
    firstName: fullUser.firstName || fullUser.name.split(' ')[0] || '',
    lastName: fullUser.lastName || fullUser.name.split(' ')[1] || ''
  };
  
  showFormModal.value = true;
};

const handleDelete = (row) => {
  userToDelete.value = row;
  showDeleteModal.value = true;
};

const confirmDelete = async () => {
  if (!userToDelete.value) return;

  // Close modal and show success immediately for better UX
  showDeleteModal.value = false;
  toast.success('User deleted successfully');
  
  // Remove from local state immediately
  users.value = users.value.filter(u => u.id !== userToDelete.value.id);
  
  const userId = userToDelete.value.id;
  userToDelete.value = null;

  // Perform API call in background
  try {
    await usersAPI.delete(userId);
  } catch (error) {
    console.error('Error deleting user:', error);
    toast.error('Failed to delete user from server');
    // Optionally refetch to restore the item if deletion failed
    await fetchUsers();
  }
};

const cancelDelete = () => {
  showDeleteModal.value = false;
  userToDelete.value = null;
};

const openFilterModal = () => {
  showFilterModal.value = true;
};

const applyFilters = (filters) => {
  activeFilters.value = filters;
};

const closeFormModal = () => {
  showFormModal.value = false;
  selectedUser.value = {};
};

const handleFormSubmit = async ({ mode, data }) => {
  loading.value = true;
  try {
    // Validate required fields
    if (!data.name?.trim() && !data.firstName?.trim()) {
      toast.error('User name is required');
      return;
    }
    if (!data.email?.trim()) {
      toast.error('Email is required');
      return;
    }

    // Transform form data to match backend DTO
    const transformedData = {
      firstName: data.firstName?.trim() || data.name?.trim() || '',
      lastName: data.lastName?.trim() || '',
      email: data.email.trim(),
      phone: data.phone?.trim() || '',
      nationality: data.nationality?.trim() || '',
      profileImage: data.image || data.profileImage || '',
      password: data.password || '' // Only for new users
    };

    if (mode === 'add') {
      const response = await usersAPI.create(transformedData);
      toast.success('User created successfully');
      
      // Fetch the newly created user and add it to local state
      try {
        const userResponse = await usersAPI.getOne(response.data);
        const newUser = {
          id: userResponse.data.id,
          name: userResponse.data.fullName || `${userResponse.data.firstName} ${userResponse.data.lastName}`,
          email: userResponse.data.email || 'N/A',
          phone: userResponse.data.phone || 'N/A',
          nationality: userResponse.data.nationality || 'N/A',
          image: userResponse.data.profileImage || userResponse.data.userImage || '/images/default-avatar.png',
          status: userResponse.data.status || (userResponse.data.isVerified ? 'Verified' : 'Unverified'),
          createdAt: userResponse.data.createdAt ? new Date(userResponse.data.createdAt) : new Date()
        };
        users.value.unshift(newUser);
      } catch (error) {
        console.error('Error fetching new user:', error);
        // Fallback to refreshing all data
        await fetchUsers();
      }
    } else {
      // Include the user ID in the update data
      const updateData = { ...transformedData, Id: Number(selectedUser.value.id) };
      await usersAPI.update(selectedUser.value.id, updateData);
      toast.success('User updated successfully');
      
      // Update local state
      const userIndex = users.value.findIndex(u => u.id === selectedUser.value.id);
      if (userIndex !== -1) {
        users.value[userIndex] = {
          ...users.value[userIndex],
          name: updateData.firstName + (updateData.lastName ? ' ' + updateData.lastName : ''),
          email: updateData.email,
          phone: updateData.phone,
          nationality: updateData.nationality,
          image: updateData.profileImage,
          phoneNumber: updateData.phone // Also update phoneNumber for consistency
        };
      }
    }
    closeFormModal();
  } catch (error) {
    console.error('Error submitting form:', error);
    console.error('Error response:', error.response?.data);
    const errorMessage = error.response?.data?.errors 
      ? Object.values(error.response.data.errors).flat().join(', ')
      : error.response?.data?.message || error.message;
    toast.error(`Failed to save user: ${errorMessage}`);
  } finally {
    loading.value = false;
  }
};



const handleView = (row) => {
  router.push({ name: 'DashboardDetails', params: { type: 'users', id: row.id } });
};

const handleStatusChange = async ({ row, newValue }) => {
  try {
    await usersAPI.updateStatus(row.id, newValue);
    // Update local state to reflect change immediately
    const userIndex = users.value.findIndex(u => u.id === row.id);
    if (userIndex !== -1) {
      users.value[userIndex].status = newValue;
    }
    toast.success('User status updated successfully');
  } catch (error) {
    console.error('Error updating status:', error);
    toast.error('Failed to update status');
    // Revert by refreshing data
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