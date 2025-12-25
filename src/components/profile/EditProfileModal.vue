<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm">
    <div class="modal-box w-full max-w-lg bg-base-100 shadow-xl rounded-xl p-6 relative">
      <!-- Close Button -->
      <button @click="$emit('close')" class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">✕</button>
      
      <div class="flex gap-4 mb-6 border-b border-base-300">
        <button 
          @click="activeMode = 'details'"
          :class="[
            'pb-2 font-semibold transition-all',
            activeMode === 'details' ? 'text-primary border-b-2 border-primary' : 'text-base-content/60'
          ]"
        >
          Edit Details
        </button>
        <button 
          @click="activeMode = 'password'"
          :class="[
            'pb-2 font-semibold transition-all',
            activeMode === 'password' ? 'text-primary border-b-2 border-primary' : 'text-base-content/60'
          ]"
        >
          Change Password
        </button>
      </div>
      
      <!-- Details Form -->
      <form v-if="activeMode === 'details'" @submit.prevent="handleSubmit" class="space-y-4">
        <!-- Name Fields -->
        <div class="grid grid-cols-2 gap-4">
          <div class="form-control">
            <label class="label">
              <span class="label-text">First Name</span>
            </label>
            <input 
              v-model="formData.firstName" 
              type="text" 
              class="input input-bordered w-full" 
              required
            />
          </div>
          <div class="form-control">
            <label class="label">
              <span class="label-text">Last Name</span>
            </label>
            <input 
              v-model="formData.lastName" 
              type="text" 
              class="input input-bordered w-full" 
              required
            />
          </div>
        </div>

        <!-- Contact Info -->
        <div class="form-control">
          <label class="label">
            <span class="label-text">Phone Number</span>
          </label>
          <input 
            v-model="formData.phone" 
            type="tel" 
            class="input input-bordered w-full" 
            placeholder="+1 234 567 890"
          />
        </div>

        <div class="form-control">
          <label class="label">
            <span class="label-text">Nationality</span>
          </label>
          <input 
            v-model="formData.nationality" 
            type="text" 
            class="input input-bordered w-full" 
          />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div class="form-control">
             <DatePicker
                v-model="formData.dateOfBirth"
                label="Date of Birth"
                placeholder="Select date of birth"
                format="YYYY-MM-DD"
                displayFormat="DD/MM/YYYY"
              />
          </div>
          <div class="form-control">
            <label class="label">
              <span class="label-text">Gender</span>
            </label>
            <select v-model="formData.gender" class="select select-bordered w-full">
              <option value="">Select Gender</option>
              <option value="male">Male</option>
              <option value="female">Female</option>
            </select>
          </div>
        </div>

        <!-- Profile Image Upload -->
        <div class="form-control">
          <label class="label">
            <span class="label-text">Profile Photo</span>
          </label>
          <input 
            ref="fileInput"
            type="file" 
            accept="image/*"
            @change="handleImageSelect"
            class="file-input file-input-bordered w-full" 
          />
          <div v-if="imagePreview" class="mt-3">
            <img :src="imagePreview" alt="Preview" class="w-24 h-24 rounded-lg object-cover" />
          </div>
          <p v-if="uploadError" class="text-error text-sm mt-2">{{ uploadError }}</p>
        </div>

        <div class="modal-action mt-6">
          <button type="button" @click="$emit('close')" class="btn btn-ghost" :disabled="loading">Cancel</button>
          <button type="submit" class="btn btn-primary" :disabled="loading">
            <span v-if="loading" class="loading loading-spinner"></span>
            <span v-else>Save Changes</span>
          </button>
        </div>
      </form>

      <!-- Password Form -->
      <form v-else @submit.prevent="handlePasswordSubmit" class="space-y-4">
        <div class="form-control">
           <label class="label">
             <span class="label-text">Current Password</span>
           </label>
           <input 
             v-model="passwordData.currentPassword" 
             type="password" 
             class="input input-bordered w-full" 
             required
           />
        </div>

         <div class="form-control">
            <label class="label">
              <span class="label-text">New Password</span>
            </label>
            <input 
              v-model="passwordData.newPassword" 
              type="password" 
              class="input input-bordered w-full" 
              required
              minlength="8"
            />
            <label class="label">
              <span class="label-text-alt text-base-content/60">Min 8 characters, with uppercase, number, and special character.</span>
            </label>
         </div>
 
         <div class="form-control">
            <label class="label">
              <span class="label-text">Confirm New Password</span>
            </label>
            <input 
              v-model="passwordData.confirmPassword" 
              type="password" 
              class="input input-bordered w-full" 
              required
              minlength="8"
            />
           <label class="label" v-if="passwordError">
             <span class="label-text-alt text-error">{{ passwordError }}</span>
           </label>
        </div>

        <div class="modal-action mt-6">
          <button type="button" @click="$emit('close')" class="btn btn-ghost" :disabled="loading">Cancel</button>
          <button type="submit" class="btn btn-primary" :disabled="loading">
            <span v-if="loading" class="loading loading-spinner"></span>
            <span v-else>Update Password</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, watch } from 'vue';
import { useAuthStore } from '@/stores/authStore';
import uploadApi from '@/Services/uploadApi';
import DatePicker from '@/components/Common/DatePicker.vue';

const props = defineProps({
  isOpen: Boolean,
  user: {
    type: Object,
    required: true
  }
});

const emit = defineEmits(['close', 'save']);
const loading = ref(false);
const activeMode = ref('details');
const authStore = useAuthStore();
const passwordError = ref('');
const uploadError = ref('');
const fileInput = ref(null);
const selectedFile = ref(null);
const imagePreview = ref('');

const formData = reactive({
  firstName: '',
  lastName: '',
  phone: '',
  nationality: '',
  dateOfBirth: '',
  gender: '',
  profileImage: ''
});

const passwordData = reactive({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
});

// Sync props to form data when modal opens
watch(() => props.isOpen, (newVal) => {
  if (newVal && props.user) {
    // Reset mode and forms
    activeMode.value = 'details';
    passwordError.value = '';
    uploadError.value = '';
    selectedFile.value = null;
    imagePreview.value = '';
    passwordData.currentPassword = '';
    passwordData.newPassword = '';
    passwordData.confirmPassword = '';
    
    const safeName = (props.user.name || props.user.fullName || '').trim();
    const nameParts = safeName ? safeName.split(/\s+/) : [];

    formData.firstName = props.user.firstName || nameParts[0] || '';
    formData.lastName = props.user.lastName || nameParts.slice(1).join(' ') || '';
    formData.phone = props.user.phone || '';
    formData.nationality = props.user.nationality || '';
    
    // Format dateOfBirth for HTML date input (YYYY-MM-DD)
    if (props.user.dateOfBirth) {
      const date = new Date(props.user.dateOfBirth);
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const day = String(date.getDate()).padStart(2, '0');
      formData.dateOfBirth = `${year}-${month}-${day}`;
    } else {
      formData.dateOfBirth = '';
    }
    
    formData.gender = props.user.gender || '';
    formData.profileImage = props.user.avatar || props.user.profileImage || '';
    imagePreview.value = formData.profileImage;
  }
});

const handleImageSelect = (event) => {
  const file = event.target.files?.[0];
  if (!file) return;

  selectedFile.value = file;
  uploadError.value = '';
  
  const reader = new FileReader();
  reader.onload = (e) => {
    imagePreview.value = e.target?.result || '';
  };
  reader.readAsDataURL(file);
};

const uploadImage = async () => {
  if (!selectedFile.value) return null;

  try {
    const result = await uploadApi.uploadPhoto(selectedFile.value);
    if (!result || !result.url) {
      throw new Error('No URL in upload response');
    }
    return result.url;
  } catch (error) {
    uploadError.value = error.response?.data?.error || error.message || 'Failed to upload image';
    throw error;
  }
};

const handleSubmit = async () => {
  loading.value = true;
  uploadError.value = '';
  
  try {
    let profileImageUrl = formData.profileImage;

    // If a new file is selected, upload it first
    if (selectedFile.value) {
      profileImageUrl = await uploadImage();
    }

    // Build the update payload with only non-empty fields
    const updatedData = {
      firstName: formData.firstName || '',
      lastName: formData.lastName || '',
      fullName: `${formData.firstName} ${formData.lastName}`.trim(),
      phone: formData.phone || '',
      nationality: formData.nationality || '',
      gender: formData.gender || '',
      profileImage: profileImageUrl || ''
    };

    // Only add dateOfBirth if it's set
    if (formData.dateOfBirth) {
      const date = new Date(formData.dateOfBirth);
      updatedData.dateOfBirth = date.toISOString();
    }
    
    emit('save', updatedData);
  } catch (error) {
    console.error('Save error:', error);
    // uploadError is already set by uploadImage if needed
  } finally {
    loading.value = false;
  }
};

const handlePasswordSubmit = async () => {
  passwordError.value = '';
  
  if (passwordData.newPassword !== passwordData.confirmPassword) {
    passwordError.value = 'Passwords do not match';
    return;
  }

  loading.value = true;
  try {
    const result = await authStore.changePassword(
      passwordData.currentPassword,
      passwordData.newPassword
    );

    if (result.success) {
      alert('Password updated successfully!');
      emit('close');
    } else {
       passwordError.value = result.error;
    }
  } catch (err) {
    passwordError.value = err.message || 'An error occurred';
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.modal-box {
  max-height: 90vh;
  overflow-y: auto;
}
</style>
