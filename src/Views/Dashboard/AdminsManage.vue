<template>
  <div>
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
      :title="editingAdmin ? 'Edit Admin' : 'Add New Admin'"
      :config="adminFormConfig"
      :initialData="editingAdmin"
      @close="closeModal"
      @submit="handleSubmit"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import DataTable from '@/components/Dashboard/DataTable.vue';
import FormModal from '@/components/Dashboard/FormModal.vue';
import { usersAPI } from '@/Services/dashboardApi';
import { useToast } from '@/composables/useToast';

const { toast } = useToast();
const router = useRouter();
const admins = ref([]);
const loading = ref(false);
const isModalOpen = ref(false);
const editingAdmin = ref(null);

const columns = [
  { label: 'Name', field: 'fullName', type: 'text', headerClass: 'w-1/4' },
  { label: 'Email', field: 'email', type: 'text', headerClass: 'w-1/4' },
  { label: 'Role', field: 'role', type: 'badge', variant: 'primary', headerClass: 'w-1/6' }, // Usually just 'admin
  { label: 'Status', field: 'status', type: 'status', headerClass: 'w-1/6' },
  { label: 'Actions', field: 'actions', type: 'actions', headerClass: 'w-1/6' }
];

const adminFormConfig = {
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
        { label: 'Admin', value: 'admin' },
        { label: 'Super Admin', value: 'superadmin' }
      ]
    }
  ]
};

const fetchAdmins = async () => {
  loading.value = true;
  try {
    const response = await usersAPI.getAll();
    const allUsers = response.data;
    // Filter for admins only
    admins.value = allUsers.filter(user => user.role === 'admin');
  } catch (error) {
    console.error('Error fetching admins:', error);
    toast.error('Failed to load admins');
  } finally {
    loading.value = false;
  }
};

const openAddModal = () => {
  editingAdmin.value = null;
  
  // Ensure password field is required/visible for creating
  const pwdField = adminFormConfig.fields.find(f => f.key === 'password');
  if(pwdField) pwdField.type = 'password';

  isModalOpen.value = true;
};

const openEditModal = (admin) => {
  editingAdmin.value = { ...admin };
  
  // Maybe hide/disable password for edit? Or keep it to allow reset.
  // For simplicity, let's allow editing everything.
  
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
  editingAdmin.value = null;
};

const handleSubmit = async ({ data: formData }) => {
  try {
    // Construct payload
    const payload = {
      ...formData,
      fullName: `${formData.firstName} ${formData.lastName}`,
      role: formData.role || 'admin', // Use selected role or default to 'admin'
      profileImage: editingAdmin.value?.profileImage || '/images/users/user_m_1.jpg',
      status: editingAdmin.value?.status || 'Active',
      updatedAt: new Date().toISOString()
    };

    if (editingAdmin.value) {
        // Update
        if (!formData.password) delete payload.password; // Don't overwrite blank pass
       await usersAPI.patch(editingAdmin.value.id, payload);
       toast.success('Admin updated successfully');
    } else {
        // Create
        payload.createdAt = new Date().toISOString();
        payload.isVerified = true; 
        await usersAPI.create(payload);
        toast.success('Admin created successfully');
    }
    closeModal();
    fetchAdmins();
  } catch (error) {
    console.error('Error saving admin:', error);
    toast.error('Failed to save admin');
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
  fetchAdmins();
});
</script>
