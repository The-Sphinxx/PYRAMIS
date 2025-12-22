<template>
  <!-- Modal Overlay & Content -->
  <Teleport to="body">
    <Transition name="modal">
      <div 
        v-if="isOpen" 
        class="modal-overlay fixed inset-0 z-50 flex items-center justify-center p-4"
        @click.self="handleCancel"
      >
        <!-- Modal Container -->
        <div class="modal-container bg-base-100 rounded-lg shadow-2xl max-w-3xl w-full max-h-[90vh] overflow-hidden">
          <!-- Modal Header -->
          <div class="modal-header px-6 py-5 flex items-center justify-between border-b border-base-300">
            <h2 class="text-2xl font-bold font-cairo text-base-content">
              {{ mode === 'add' ? 'Add New' : 'Edit' }} {{ config.title }}
            </h2>
            <button 
              @click="handleCancel" 
              class="btn btn-ghost btn-sm btn-square"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <!-- Modal Body (Scrollable Form) -->
          <form @submit.prevent="handleSubmit" class="modal-body overflow-y-auto px-6 py-6" style="max-height: calc(90vh - 180px);">
            <div class="space-y-4">
              <div 
                v-for="field in visibleFields" 
                :key="field.key"
                class="form-control"
              >
                <label class="label">
                  <span class="label-text font-cairo font-semibold text-base-content">
                    {{ field.label }}
                    <span v-if="field.required" class="text-error">*</span>
                  </span>
                </label>

                <!-- Text, Email, Tel, Password, Number -->
                <input
                  v-if="['text', 'email', 'tel', 'password', 'number'].includes(field.type)"
                  :type="field.type"
                  v-model="formData[field.key]"
                  :placeholder="field.placeholder"
                  :required="field.required"
                  :min="field.min"
                  :max="field.max"
                  :step="field.step"
                  class="input input-bordered w-full font-cairo text-base-content"
                />

                <!-- Textarea -->
                <textarea
                  v-else-if="field.type === 'textarea'"
                  v-model="formData[field.key]"
                  :placeholder="field.placeholder"
                  :required="field.required"
                  :rows="field.rows || 3"
                  class="textarea textarea-bordered w-full font-cairo text-base-content"
                ></textarea>

                <!-- Date -->
                <input
                  v-else-if="field.type === 'date'"
                  type="date"
                  v-model="formData[field.key]"
                  :required="field.required"
                  class="input input-bordered w-full font-cairo text-base-content"
                />

                <!-- Select (Single) -->
                <select
                  v-else-if="field.type === 'select'"
                  v-model="formData[field.key]"
                  :required="field.required"
                  class="select select-bordered w-full font-cairo text-base-content"
                >
                  <option value="" disabled>Select {{ field.label.toLowerCase() }}</option>
                  <option 
                    v-for="option in field.options" 
                    :key="option.value"
                    :value="option.value"
                  >
                    {{ option.label }}
                  </option>
                </select>

                <!-- Multi-Select (Checkboxes) -->
                <div v-else-if="field.type === 'multi-select'" class="space-y-2 p-3 border border-base-300 rounded-lg">
                  <label
                    v-for="option in field.options"
                    :key="option.value"
                    class="flex items-center gap-3 p-2 hover:bg-base-200 rounded-md cursor-pointer transition-colors"
                  >
                    <input
                      type="checkbox"
                      :value="option.value"
                      v-model="formData[field.key]"
                      class="checkbox checkbox-primary checkbox-sm"
                    />
                    <span class="label-text font-cairo text-base-content">{{ option.label }}</span>
                  </label>
                </div>

                <!-- Toggle -->
                <div v-else-if="field.type === 'toggle'" class="flex items-center gap-3">
                  <input
                    type="checkbox"
                    v-model="formData[field.key]"
                    class="toggle toggle-primary"
                  />
                  <span class="label-text font-cairo text-base-content">
                    {{ formData[field.key] ? 'Yes' : 'No' }}
                  </span>
                </div>

                <!-- Rating (Visual) -->
                <div v-else-if="field.type === 'rating'" class="flex items-center gap-2">
                  <input
                    type="number"
                    v-model.number="formData[field.key]"
                    :min="field.min || 0"
                    :max="field.max || 5"
                    :step="field.step || 0.1"
                    :required="field.required"
                    class="input input-bordered w-24 font-cairo text-base-content"
                  />
                  <div class="flex items-center gap-1">
                    <svg 
                      v-for="star in 5" 
                      :key="star"
                      xmlns="http://www.w3.org/2000/svg" 
                      class="h-5 w-5"
                      :class="star <= Math.floor(formData[field.key] || 0) ? 'text-warning fill-current' : 'text-base-300'"
                      viewBox="0 0 20 20" 
                      fill="currentColor"
                    >
                      <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                    </svg>
                  </div>
                </div>

                <!-- Image Upload (Simplified) -->
                <div v-else-if="field.type === 'image'" class="space-y-2">
                  <input
                    type="file"
                    @change="handleImageUpload($event, field.key)"
                    accept="image/*"
                    :multiple="field.multiple"
                    class="file-input file-input-bordered w-full"
                  />
                  <div v-if="formData[field.key]" class="text-sm text-base-content/60">
                    {{ Array.isArray(formData[field.key]) ? formData[field.key].length + ' file(s) selected' : 'File selected' }}
                  </div>
                </div>
              </div>
            </div>
          </form>

          <!-- Modal Footer -->
          <div class="modal-footer px-6 py-4 border-t border-base-300 flex gap-3">
            <button 
              type="button"
              @click="handleCancel" 
              class="btn btn-outline flex-1 font-cairo font-bold"
              :disabled="loading"
            >
              Cancel
            </button>
            <button 
              type="button"
              @click="handleSubmit" 
              class="btn btn-primary flex-1 font-cairo font-bold"
              :disabled="loading"
            >
              <span v-if="loading" class="loading loading-spinner loading-sm"></span>
              <span v-else>{{ mode === 'add' ? 'Add' : 'Update' }} {{ config.title }}</span>
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { ref, computed, watch } from 'vue';

const props = defineProps({
  isOpen: {
    type: Boolean,
    required: true
  },
  mode: {
    type: String,
    default: 'add', // 'add' or 'edit'
    validator: (value) => ['add', 'edit'].includes(value)
  },
  config: {
    type: Object,
    required: true
    // Expected structure: { title: String, fields: Array }
  },
  initialData: {
    type: Object,
    default: () => ({})
  }
});

const emit = defineEmits(['close', 'submit']);

const formData = ref({});
const loading = ref(false);

// Filter fields based on mode (some fields might be add-only)
const visibleFields = computed(() => {
  return props.config.fields.filter(field => {
    if (props.mode === 'edit' && field.addOnly) {
      return false;
    }
    return true;
  });
});

// Initialize form data
const initializeForm = () => {
  const data = {};
  props.config.fields.forEach(field => {
    if (props.mode === 'edit' && props.initialData[field.key] !== undefined) {
      data[field.key] = props.initialData[field.key];
    } else {
      // Initialize with default values
      if (field.type === 'multi-select') {
        data[field.key] = [];
      } else if (field.type === 'toggle') {
        data[field.key] = false;
      } else if (field.type === 'number' || field.type === 'rating') {
        data[field.key] = 0;
      } else {
        data[field.key] = '';
      }
    }
  });
  formData.value = data;
};

// Handle image upload
const handleImageUpload = (event, key) => {
  const files = event.target.files;
  if (files && files.length > 0) {
    if (props.config.fields.find(f => f.key === key)?.multiple) {
      formData.value[key] = Array.from(files);
    } else {
      formData.value[key] = files[0];
    }
  }
};

// Handle form submission
const handleSubmit = async () => {
  loading.value = true;
  try {
    // Create a clean copy of form data
    const dataToSubmit = { ...formData.value };
    
    // Emit the submit event with form data
    emit('submit', {
      mode: props.mode,
      data: dataToSubmit
    });
  } catch (error) {
    console.error('Form submission error:', error);
  } finally {
    loading.value = false;
  }
};

// Handle cancel
const handleCancel = () => {
  if (!loading.value) {
    emit('close');
  }
};

// Watch for modal open/close to initialize form
watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    initializeForm();
    document.body.style.overflow = 'hidden';
  } else {
    document.body.style.overflow = '';
  }
});

// Initialize on mount if already open
if (props.isOpen) {
  initializeForm();
}
</script>

<style scoped>
.modal-overlay {
  background-color: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  animation: fadeIn 0.2s ease-out;
}

.modal-container {
  animation: slideUp 0.3s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px) scale(0.98);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

/* Modal transitions */
.modal-enter-active,
.modal-leave-active {
  transition: all 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .modal-container,
.modal-leave-to .modal-container {
  transform: translateY(20px) scale(0.98);
}

/* Scrollbar styling */
.modal-body::-webkit-scrollbar {
  width: 6px;
}

.modal-body::-webkit-scrollbar-track {
  background: transparent;
}

.modal-body::-webkit-scrollbar-thumb {
  background: var(--bc, #2F2F2F);
  opacity: 0.2;
  border-radius: 3px;
}

.modal-body::-webkit-scrollbar-thumb:hover {
  opacity: 0.3;
}
</style>
