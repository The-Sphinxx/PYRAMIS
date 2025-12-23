<template>
  <div class=" mx-auto">
 
    <!-- Top Navigation Tabs -->
    <div class="tabs tabs-boxed mb-6 bg-base-100 p-1 border border-base-200">
      <a 
        class="tab tab-lg flex-1 transition-all duration-200" 
        :class="{ 'tab-active': activeTab === 'general', 'font-bold': activeTab === 'general' }" 
        @click="activeTab = 'general'"
      >
        General Profile
      </a>
      <a 
        class="tab tab-lg flex-1 transition-all duration-200" 
        :class="{ 'tab-active': activeTab === 'security', 'font-bold': activeTab === 'security' }" 
        @click="activeTab = 'security'"
      >
        Security
      </a>
    </div>

    <!-- Main Content Area -->
    <div class="space-y-6">
      
      <!-- General Tab -->
      <div v-if="activeTab === 'general'" class="card bg-base-100 shadow-xl border border-base-200">
         <div class="card-header px-6 py-4 border-b border-base-200 flex justify-between items-center">
           <h3 class="font-bold text-lg">General Information</h3>
           <button class="btn btn-primary btn-sm" @click="handleUpdateProfile" :disabled="loadingProfile">
              <span v-if="loadingProfile" class="loading loading-spinner loading-xs"></span>
              Save Changes
           </button>
         </div>
         <div class="card-body">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="form-control w-full">
                <label class="label"><span class="label-text font-medium">First Name</span></label>
                <input v-model="profileForm.firstName" type="text" class="input input-bordered w-full" />
              </div>
              <div class="form-control w-full">
                <label class="label"><span class="label-text font-medium">Last Name</span></label>
                <input v-model="profileForm.lastName" type="text" class="input input-bordered w-full" />
              </div>
              <div class="form-control w-full">
                <label class="label"><span class="label-text font-medium">Email Address</span></label>
                <input v-model="profileForm.email" type="email" class="input input-bordered w-full bg-base-200" readonly title="Email cannot be changed directly" />
                <label class="label"><span class="label-text-alt text-warning">Contact super admin to change email</span></label>
              </div>
              <div class="form-control w-full">
                <label class="label"><span class="label-text font-medium">Phone Number</span></label>
                <input v-model="profileForm.phone" type="tel" class="input input-bordered w-full" />
              </div>
            </div>
         </div>
      </div>

      <!-- Security Tab -->
      <div v-if="activeTab === 'security'" class="card bg-base-100 shadow-xl border border-base-200">
        <div class="card-header px-6 py-4 border-b border-base-200">
           <h3 class="font-bold text-lg text-error">Security Settings</h3>
        </div>
        <div class="card-body">
           <div class="alert alert-warning shadow-sm mb-6">
             <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>
             <span>Ensure your password is at least 8 characters long and includes special characters.</span>
           </div>

           <form @submit.prevent="handleChangePassword" class="max-w-md">
              <div class="form-control w-full mb-4">
                <label class="label"><span class="label-text font-medium">Current Password</span></label>
                <input v-model="passwordForm.current" type="password" class="input input-bordered w-full" required />
              </div>
               <div class="divider"></div>
              <div class="form-control w-full mb-4">
                <label class="label"><span class="label-text font-medium">New Password</span></label>
                <input v-model="passwordForm.new" type="password" class="input input-bordered w-full" required minlength="8" />
              </div>
              <div class="form-control w-full mb-6">
                <label class="label"><span class="label-text font-medium">Confirm New Password</span></label>
                <input v-model="passwordForm.confirm" type="password" class="input input-bordered w-full" required minlength="8" />
              </div>
              
              <button type="submit" class="btn btn-error text-white" :disabled="loadingPassword">
                 <span v-if="loadingPassword" class="loading loading-spinner loading-sm"></span>
                 Update Password
              </button>
           </form>
        </div>
      </div>
    
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useAuthStore } from '@/stores/authStore';
import { useToast } from '@/composables/useToast';
import dayjs from 'dayjs';

const authStore = useAuthStore();
const { toast } = useToast();

const activeTab = ref('general');
const loadingProfile = ref(false);
const loadingPassword = ref(false);

const profileForm = ref({
  firstName: '',
  lastName: '',
  email: '',
  phone: ''
});

const passwordForm = ref({
  current: '',
  new: '',
  confirm: ''
});

const formatDate = (date) => {
  return date ? dayjs(date).format('MMM D, YYYY') : 'N/A';
}

// Initialize form with current user data
onMounted(() => {
  if (authStore.user) {
    profileForm.value.firstName = authStore.user.firstName || '';
    profileForm.value.lastName = authStore.user.lastName || '';
    profileForm.value.email = authStore.user.email || '';
    profileForm.value.phone = authStore.user.phone || '';
  }
});

const handleUpdateProfile = async () => {
  loadingProfile.value = true;
  try {
    const updatedData = {
      firstName: profileForm.value.firstName,
      lastName: profileForm.value.lastName,
      fullName: `${profileForm.value.firstName} ${profileForm.value.lastName}`,
      email: profileForm.value.email, // Kept for API compatibility, though readonly in UI mainly
      phone: profileForm.value.phone
    };
    
    const result = await authStore.updateProfile(updatedData);
    
    if (result.success) {
      toast.success('Profile updated successfully');
    } else {
      toast.error(result.error || 'Failed to update profile');
    }
  } catch (error) {
    console.error('Update profile error:', error);
    toast.error('An error occurred');
  } finally {
    loadingProfile.value = false;
  }
};

const handleChangePassword = async () => {
  if (passwordForm.value.new !== passwordForm.value.confirm) {
    toast.error('New passwords do not match');
    return;
  }

  loadingPassword.value = true;
  try {
    const result = await authStore.changePassword(passwordForm.value.current, passwordForm.value.new);
    
    if (result.success) {
      toast.success('Password updated successfully');
      // Reset form
      passwordForm.value = { current: '', new: '', confirm: '' };
    } else {
      toast.error(result.error || 'Failed to change password');
    }
  } catch (error) {
    console.error('Change password error:', error);
    toast.error('An error occurred');
  } finally {
    loadingPassword.value = false;
  }
};
</script>
