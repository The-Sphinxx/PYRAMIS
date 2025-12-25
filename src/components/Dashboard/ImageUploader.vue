<template>
  <div class="image-uploader space-y-4">
    <!-- Upload Zone -->
    <div 
      class="upload-zone border-2 border-dashed border-base-300 rounded-lg p-8 text-center transition-colors"
      :class="{ 'border-primary bg-primary/5': isDragging, 'hover:border-primary/50': !isDragging }"
      @drop.prevent="handleDrop"
      @dragover.prevent="isDragging = true"
      @dragleave.prevent="isDragging = false"
      @click="triggerFileInput"
    >
      <input 
        ref="fileInput"
        type="file" 
        multiple 
        :accept="accept"
        @change="handleFileSelect"
        class="hidden"
      />
      
      <div class="flex flex-col items-center gap-3">
        <svg class="w-12 h-12 text-base-content/40" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
        </svg>
        <div>
          <p class="text-base-content font-semibold">
            Drag & drop images here or click to browse
          </p>
         
        </div>
      </div>
    </div>

    <!-- Error Message -->
    <div v-if="errorMessage" class="alert alert-error">
      <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
      </svg>
      <span>{{ errorMessage }}</span>
    </div>

    <!-- Image Previews Grid -->
    <div v-if="images.length > 0" class="grid grid-cols-2 md:grid-cols-4 gap-4">
      <div 
        v-for="(image, index) in images" 
        :key="index"
        class="relative group aspect-square rounded-lg overflow-hidden border-2 border-base-300 hover:border-primary transition-colors"
      >
        <img 
          :src="getImageSrc(image)" 
          :alt="`Image ${index + 1}`"
          class="w-full h-full object-cover"
          @error="handleImageError($event, index)"
        />
        
        <!-- Image Overlay -->
        <div class="absolute inset-0 bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center gap-2">
          <!-- Remove Button -->
          <button
            type="button"
            @click="removeImage(index)"
            class="btn btn-sm btn-circle btn-error"
            title="Remove image"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Image Number Badge -->
        <div class="absolute top-2 left-2 badge badge-sm badge-primary">
          {{ index + 1 }}
        </div>
      </div>
    </div>

    <!-- Image Count Info -->
    <div v-if="images.length > 0" class="text-sm text-base-content/60 text-center">
      {{ images.length }} / {{ maxImages }} images uploaded
      <span v-if="required && images.length === 0" class="text-error ml-2">
        (At least 1 image required)
      </span>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';

const props = defineProps({
  modelValue: {
    type: Array,
    default: () => []
  },
  maxImages: {
    type: Number,
    default: 4
  },
  accept: {
    type: String,
    default: 'image/jpeg,image/png,image/webp'
  },
  maxSize: {
    type: Number,
    default: 2 * 1024 * 1024 // 2MB
  },
  required: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['update:modelValue']);

const fileInput = ref(null);
const images = ref([...props.modelValue]);
const isDragging = ref(false);
const errorMessage = ref('');

// Watch for external changes to modelValue
watch(() => props.modelValue, (newVal) => {
  images.value = [...newVal];
}, { deep: true });

// Watch for internal changes and emit
watch(images, (newVal) => {
  emit('update:modelValue', newVal);
}, { deep: true });

const triggerFileInput = () => {
  fileInput.value?.click();
};

const handleFileSelect = (event) => {
  const files = Array.from(event.target.files);
  processFiles(files);
  // Reset input so same file can be selected again
  event.target.value = '';
};

const handleDrop = (event) => {
  isDragging.value = false;
  const files = Array.from(event.dataTransfer.files);
  processFiles(files);
};

const processFiles = async (files) => {
  errorMessage.value = '';

  // Check if adding these files would exceed max images
  if (images.value.length + files.length > props.maxImages) {
    errorMessage.value = `You can only upload up to ${props.maxImages} images. Currently have ${images.value.length}.`;
    return;
  }

  // Filter valid image files
  const imageFiles = files.filter(file => {
    if (!file.type.startsWith('image/')) {
      errorMessage.value = `${file.name} is not an image file.`;
      return false;
    }
    if (file.size > props.maxSize) {
      errorMessage.value = `${file.name} is too large. Max size is ${formatFileSize(props.maxSize)}.`;
      return false;
    }
    return true;
  });

  // Convert images to base64
  for (const file of imageFiles) {
    try {
      const base64 = await fileToBase64(file);
      images.value.push(base64);
    } catch (error) {
      console.error('Error converting file to base64:', error);
      errorMessage.value = `Failed to process ${file.name}`;
    }
  }
};

const fileToBase64 = (file) => {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(reader.result);
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
};

const removeImage = (index) => {
  images.value.splice(index, 1);
  errorMessage.value = '';
};

const getImageSrc = (image) => {
  // If it's already a base64 data URI, return as-is
  if (image && image.startsWith('data:')) {
    return image;
  }
  // If it's a URL path, return as-is (browser will resolve it)
  return image;
};

const handleImageError = (event, index) => {
  console.warn(`Failed to load image at index ${index}:`, images.value[index]);
  // Set a placeholder SVG for broken images
  event.target.src = 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="200" height="200"%3E%3Crect fill="%23ddd" width="200" height="200"/%3E%3Ctext fill="%23999" font-size="14" x="50%25" y="50%25" text-anchor="middle" dy=".3em"%3EImage not found%3C/text%3E%3C/svg%3E';
};

const formatFileSize = (bytes) => {
  if (bytes === 0) return '0 Bytes';
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
};
</script>

<style scoped>
.upload-zone {
  cursor: pointer;
}
</style>
