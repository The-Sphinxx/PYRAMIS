<template>
  <div v-if="isValidRole">
    <!-- Stats Cards (Optional, can just show total admins) -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
      <div class="stats shadow bg-base-100">
        <div class="stat">
          <div class="stat-figure text-secondary">
             <svg xmlns="http://www.w3.org/2000/svg" class="w-8 h-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
            </svg>
          </div>
          <div class="stat-title">Total Admins</div>
          <div class="stat-value">{{ admins.length }}</div>
        </div>
      </div>
    </div>

    <!-- Data Table -->
    <DataTable
      :columns="columns"
      :data="admins"
      :loading="loading"
      :show-add-button="true"
      :show-filter="false" 
      add-button-text="Add New Admin"
      @add="openAddModal"
      @edit="openEditModal"
      @delete="handleDelete"
      @view="handleView"
    />

    <!-- Add/Edit Modal -->
    <FormModal
      :isOpen="isModalOpen"
      :mode="editingAdmin ? 'edit' : 'add'"
      :title="editingAdmin ? 'Edit Admin' : 'Add New Admin'"
      :config="adminFormConfig"
      :initialData="editingAdmin"
      :errors="formErrors"
      :loading="isSubmitting"
      @close="closeModal"
      @submit="handleSubmit"
    />
  </div>
  <div v-else class="flex flex-col items-center justify-center h-96 space-y-4">
    <div class="alert alert-error max-w-lg">
      <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
      <span>Unauthorized Access. This page is restricted to Administrators only.</span>
    </div>
    <button @click="router.push('/dashboard/overview')" class="btn btn-primary">Go to Overview</button>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import DataTable from '@/components/Dashboard/DataTable.vue';
import FormModal from '@/components/Dashboard/FormModal.vue';
import { usersAPI } from '@/Services/dashboardApi';
import { useToast } from '@/composables/useToast';
import { useAuthStore } from '@/stores/authStore';
import { computed } from 'vue';

const authStore = useAuthStore();
const isValidRole = computed(() => {
  const role = authStore.user?.role?.toLowerCase();
  return role === 'superadmin';
});

const { toast } = useToast();
const router = useRouter();
const admins = ref([]);
const loading = ref(false);
const isModalOpen = ref(false);
const editingAdmin = ref(null);
const isSubmitting = ref(false);
const formErrors = ref({});

const columns = [
  { label: 'Name', field: 'fullName', type: 'text', headerClass: 'w-1/4' },
  { label: 'Email', field: 'email', type: 'text', headerClass: 'w-1/4' },
  { label: 'Role', field: 'role', type: 'badge', variant: 'primary', headerClass: 'w-1/6' }, // Usually just 'admin
  { label: 'Status', field: 'status', type: 'status', headerClass: 'w-1/6' },
  { label: 'Actions', field: 'actions', type: 'actions', headerClass: 'w-1/6' }
];

const adminFormConfig = ref({
  title: 'Admin',
  fields: [
    { key: 'firstName', label: 'First Name', type: 'text', required: true, placeholder: 'Enter first name' },
    { key: 'lastName', label: 'Last Name', type: 'text', required: true, placeholder: 'Enter last name' },
    { key: 'email', label: 'Email', type: 'email', required: true, placeholder: 'Enter email' },
    { key: 'password', label: 'Password', type: 'password', required: true, placeholder: 'Enter password' },
    { key: 'phone', label: 'Phone', type: 'tel', placeholder: 'Enter phone number' },
    { 
      key: 'role', 
      label: 'Role', 
      type: 'select', 
      required: true,
      options: [
        { label: 'Admin', value: 'Admin' },
        { label: 'SuperAdmin', value: 'SuperAdmin' }
      ]
    }
  ]
});

const fetchAdmins = async () => {
  loading.value = true;
  try {
    const response = await usersAPI.getAll();
    const allUsers = response.data.data;
    // Filter for admins only
    admins.value = allUsers.filter(user => 
      user.role?.toLowerCase() === 'admin'
    );
  } catch (error) {
    console.error('Error fetching admins:', error);
    toast.error('Failed to load admins');
  } finally {
    loading.value = false;
  }
};

const openAddModal = () => {
  editingAdmin.value = null;
  formErrors.value = {};
  
  // Set password required for new admin
  const pwdField = adminFormConfig.value.fields.find(f => f.key === 'password');
  if (pwdField) pwdField.required = true;

  isModalOpen.value = true;
};

const openEditModal = (admin) => {
  // Normalize data for form (casing fix)
  editingAdmin.value = {
    ...admin,
    firstName: admin.firstName || admin.FirstName,
    lastName: admin.lastName || admin.LastName,
    email: admin.email || admin.Email,
    phone: admin.phone || admin.Phone || admin.phoneNumber,
    role: admin.role || admin.Role
  };
  
  formErrors.value = {};
  
  // Password not required for editing
  const pwdField = adminFormConfig.value.fields.find(f => f.key === 'password');
  if (pwdField) pwdField.required = false;

  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
  editingAdmin.value = null;
  formErrors.value = {};
};

const handleSubmit = async ({ data: formData }) => {
  isSubmitting.value = true;
  formErrors.value = {};

  try {
    // Construct payload
    const payload = {
      ...formData,
      fullName: `${formData.firstName} ${formData.lastName}`,
      role: formData.role || 'Admin',
      profileImage: editingAdmin.value?.profileImage || '/images/users/user_m_1.jpg',
      status: editingAdmin.value?.status || 'Active'
    };

    if (editingAdmin.value) {
        // Update - handle ID casing (Id vs id)
        const updateId = editingAdmin.value.id || editingAdmin.value.Id;
        
        // Remove password if empty in edit mode
        if (!formData.password) {
          delete payload.password;
        }
        
        await usersAPI.patch(updateId, payload);
        toast.success('Admin updated successfully');
    } else {
        // Create
        payload.isVerified = true; 
        await usersAPI.create(payload);
        toast.success('Admin created successfully');
    }
    closeModal();
    fetchAdmins();
  } catch (error) {
    console.error('Error saving admin:', error);
    
    // Handle validation errors from API
    if (error.response?.status === 400 && error.response.data.errors) {
      const apiErrors = error.response.data.errors;
      const mappedErrors = {};
      
      // Map API property names to form field keys
      Object.keys(apiErrors).forEach(key => {
        const formKey = key.charAt(0).toLowerCase() + key.slice(1);
        mappedErrors[formKey] = Array.isArray(apiErrors[key]) ? apiErrors[key][0] : apiErrors[key];
      });
      
      formErrors.value = mappedErrors;
      toast.error('Please fix the validation errors');
    } else {
      const message = error.response?.data?.message || 'Failed to save admin';
      toast.error(message);
    }
  } finally {
    isSubmitting.value = false;
  }
};

const handleDelete = async (row) => {
    if(!confirm('Are you sure you want to remove this admin?')) return;
    try {
        await usersAPI.delete(row.id);
        toast.success('Admin removed successfully');
        fetchAdmins();
    } catch (error) {
        console.error('Error deleting admin', error);
        toast.error('Failed to delete admin');
    }
}

const handleView = (row) => {
  router.push({ name: 'DashboardDetails', params: { type: 'users', id: row.id } });
};

onMounted(() => {
  if (!isValidRole.value) {
    toast.error('Access restricted to Administrators');
    return;
  }
  fetchAdmins();
});
</script>
