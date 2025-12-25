<template>
  <div class=" mx-auto space-y-2">
    <!-- Header -->
    <div class="flex items-center justify-between border-b pb-2">
      <div class="flex items-center gap-4">
        <button @click="$router.back()" class="btn btn-circle btn-ghost">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
          </svg>
        </button>
        <div class="flex-1 min-w-0 pr-4">
          <h1 class="text-2xl md:text-3xl font-bold font-cairo capitalize break-words leading-tight">{{ config?.title }} Details</h1>
          <p class="text-base-content/60 text-sm">Viewing details for ID: {{ id }}</p>
        </div>
      </div>
      
      <div class="flex gap-2">
        <button @click="handleEdit" class="btn btn-primary gap-2">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
          </svg>
          Edit
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center py-12">
      <span class="loading loading-spinner loading-lg text-primary"></span>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="alert alert-error">
      <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
      <span>{{ error }}</span>
    </div>

    <!-- Content -->
    <div v-else class="space-y-8">
      
      <!-- Images Gallery (Top) -->
      <div class="card bg-base-100 shadow-sm border border-base-200 overflow-hidden">
        <div class="card-body p-0">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 h-96">
            <!-- Main Image -->
           <div class="h-full bg-base-300 relative group overflow-hidden">
              <img 
                v-if="mainImage" 
                :src="mainImage" 
                alt="Main Image" 
                class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                @error="e => e.target.src = 'https://placehold.co/600x400?text=No+Image'"
              />
              <div v-else class="flex items-center justify-center w-full h-full text-base-content/40">
                No Image Available
              </div>
           </div>

           <!-- Other Images Grid -->
           <div class="grid grid-cols-2 gap-4 h-full">
              <template v-if="otherImages.length > 0">
                 <div 
                   v-for="(img, idx) in otherImages.slice(0, 4)" 
                   :key="idx" 
                   class="relative bg-base-300 overflow-hidden group cursor-pointer"
                   @click="selectedImageIndex = idx"
                   :class="{'col-span-2 row-span-2': otherImages.length === 1}"
                 >
                   <img :src="img" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105" />
                   <div v-if="selectedImageIndex === idx" class="absolute inset-0 bg-primary/20 ring-4 ring-inset ring-primary"></div>
                 </div>
              </template>
              <div v-else class="col-span-2 row-span-2 flex items-center justify-center bg-base-200 text-base-content/40">
                No Additional Images
              </div>
           </div>
          </div>
        </div>
      </div>

      <!-- Information & Meta Grid -->
      <div class="grid grid-cols-1 gap-8">
        
        <!-- Main Information -->
        <div class="card bg-base-100 shadow-sm border border-base-200">
          <div class="card-body">
            <h2 class="text-xl font-bold mb-6 flex items-center gap-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-primary" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              Basic Information
            </h2>
            
            <div class="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-6">
              <div v-for="field in displayFields" :key="field.key" class="border-b border-base-200/50 pb-2 last:border-0 last:pb-0">
                <h3 class="text-xs font-bold uppercase tracking-wider text-base-content/50 mb-1">{{ field.label }}</h3>
                
                <!-- Text/Number/Date (Skip Images/Itinerary handled separately) -->
                <p v-if="['text', 'number', 'tel', 'email', 'date', 'textarea', 'select'].includes(field.type) && field.key !== 'itinerary' && field.key !== 'images'" class="text-base font-medium">
                  {{ formatValue(getNestedValue(data, field.key), field) }}
                </p>

                <!-- Status/Badge -->
                <div v-else-if="field.type === 'status'" class="badge badge-lg" :class="getStatusClass(getNestedValue(data, field.key))">
                  {{ getNestedValue(data, field.key) }}
                </div>
                
                <!-- Array of strings (Features, etc) -->
                <div v-else-if="Array.isArray(getNestedValue(data, field.key)) && typeof getNestedValue(data, field.key)?.[0] === 'string'" class="flex flex-wrap gap-2 mt-1">
                   <div v-for="(tag, idx) in getNestedValue(data, field.key)" :key="idx" class="badge badge-neutral">{{ tag }}</div>
                </div>

                 <!-- Features Object Array -->
                <div v-else-if="field.key === 'features' && Array.isArray(getNestedValue(data, field.key))" class="col-span-2 grid grid-cols-2 sm:grid-cols-3 gap-3 mt-2">
                   <div v-for="(feat, idx) in getNestedValue(data, field.key)" :key="idx" class="flex items-center gap-2 text-sm bg-base-200/50 p-2 rounded-lg">
                      <span class="text-success">✓</span> {{ feat.name || feat }}
                   </div>
                </div>

                <!-- Itinerary Grid (2 Columns) -->
                <div v-else-if="field.key === 'itinerary' && Array.isArray(getNestedValue(data, field.key))" class="col-span-1 md:col-span-2 mt-4">
                  <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div v-for="(day, idx) in getNestedValue(data, field.key)" :key="idx" class="card bg-base-200/30 border border-base-200 shadow-sm h-full">
                      <div class="card-body p-5">
                        <div class="flex items-center justify-between mb-2">
                          <span class="font-black text-xl font-mono text-primary/80">Day {{ day.day }}</span>
                          <span class="badge badge-neutral font-bold">{{ day.title }}</span>
                        </div>
                        <div class="divider my-0"></div>
                        <div class="space-y-3 mt-2">
                          <div v-for="(act, aIdx) in day.activities" :key="aIdx" class="flex flex-col gap-1 text-sm bg-base-100 p-3 rounded-lg border border-base-200/50">
                             <strong v-if="act.time" class="text-xs font-bold text-primary uppercase tracking-wide">{{ act.time }}</strong>
                             <span class="text-base-content/80 leading-relaxed">{{ act.desc || act.title }}</span>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- System Metadata -->
        <div class="card bg-base-100 shadow-sm border border-base-200">
           <div class="card-body">
             <h3 class="text-lg font-bold mb-4 flex items-center gap-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-secondary" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 3v2m6-2v2M9 19v2m6-2v2M5 9H3m2 6H3m18-6h-2m2 6h-2M7 19h10a2 2 0 002-2V7a2 2 0 00-2-2H7a2 2 0 00-2 2v10a2 2 0 002 2zM9 9h6v6H9V9z" />
              </svg>
              System Metadata
             </h3>
             <div class="grid grid-cols-1 sm:grid-cols-3 gap-4 text-sm">
               <div class="flex flex-col bg-base-200/30 p-3 rounded-lg">
                 <span class="text-base-content/50 text-xs uppercase font-bold">Created At</span>
                 <span class="font-mono mt-1">{{ formatDate(data.createdAt) }}</span>
               </div>
                <div class="flex flex-col bg-base-200/30 p-3 rounded-lg">
                 <span class="text-base-content/50 text-xs uppercase font-bold">Updated At</span>
                 <span class="font-mono mt-1">{{ formatDate(data.updatedAt) }}</span>
               </div>
               <div class="flex flex-col bg-base-200/30 p-3 rounded-lg">
                 <span class="text-base-content/50 text-xs uppercase font-bold">System ID</span>
                 <span class="font-mono mt-1 select-all">{{ data.id }}</span>
               </div>
             </div>
           </div>
        </div>

      </div>
    </div>

    <!-- Edit Modal -->
    <FormModal
      :is-open="showFormModal"
      :mode="'edit'"
      :config="config"
      :initial-data="selectedItem"
      :loading="modalloading"
      @close="showFormModal = false"
      @submit="handleUpdate"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import FormModal from '@/components/Dashboard/FormModal.vue';
import { useToast } from '@/composables/useToast.js';
import { 
  hotelsAPI, 
  carsAPI, 
  tripsAPI, 
  attractionsAPI, 
  usersAPI,
  bookingsAPI
} from '@/Services/dashboardApi';
import {
  hotelFormConfig,
  carFormConfig,
  tripFormConfig,
  attractionFormConfig,

  userFormConfig,
  bookingFormConfig
} from '@/Utils/dashboardFormConfigs';
import dayjs from 'dayjs';

const route = useRoute();
const router = useRouter();
const { toast } = useToast();

const id = route.params.id;
const resourceType = route.params.type; // hotels, cars, trips, etc.

const loading = ref(true);
const error = ref(null);
const data = ref({});
const selectedImageIndex = ref(0);

// Modal State
const showFormModal = ref(false);
const selectedItem = ref({});
const modalloading = ref(false);

// Config Mapping
const configMap = {
  hotels: hotelFormConfig,
  cars: carFormConfig,
  trips: tripFormConfig,
  attractions: attractionFormConfig,
  users: userFormConfig,
  bookings: bookingFormConfig
};

const apiMap = {
  hotels: hotelsAPI,
  cars: carsAPI,
  trips: tripsAPI,
  attractions: attractionsAPI,
  users: usersAPI,
  bookings: bookingsAPI
};

const config = computed(() => configMap[resourceType]);
const displayFields = computed(() => {
  if (!config.value) return [];
  // Filter out password fields and hidden fields
  return config.value.fields.filter(f => f.type !== 'password');
});

const mainImage = computed(() => {
  const imgs = data.value.images || data.value.profileImage;
  if (Array.isArray(imgs) && imgs.length > 0) return imgs[selectedImageIndex.value] || imgs[0];
  if (typeof imgs === 'string') return imgs;
  return null;
});

const otherImages = computed(() => {
  const imgs = data.value.images;
  if (Array.isArray(imgs)) return imgs;
  return [];
});

const formatDate = (date) => {
  if (!date) return 'N/A';
  return dayjs(date).format('MMM D, YYYY HH:mm');
};

const getNestedValue = (obj, path) => {
  if (!obj || !path) return undefined;
  return path.split('.').reduce((acc, part) => acc && acc[part], obj);
};

const formatValue = (val, field) => {
  if (val === null || val === undefined) return '-';
  if (field.type === 'date') return dayjs(val).format('MMM D, YYYY');
  if (field.type === 'price') return `$${val}`;
  if (field.type === 'select' && field.options) {
      const opt = field.options.find(o => o.value === val);
      return opt ? opt.label : val;
  }
  return val;
};

const getStatusClass = (status) => {
  if (!status) return 'badge-ghost';
  const s = status.toLowerCase();
  if (s === 'active' || s === 'verified' || s === 'available') return 'badge-success text-white';
  if (s === 'inactive' || s === 'booked') return 'badge-error text-white';
  if (s === 'pending' || s === 'maintenance') return 'badge-warning';
  return 'badge-ghost';
};

const fetchData = async () => {
  loading.value = true;
  error.value = null;
  
  const api = apiMap[resourceType];
  if (!api) {
    error.value = 'Invalid resource type';
    loading.value = false;
    return;
  }

  try {
    const response = await api.getOne(id);
    const rawData = response.data;
    
    // Standardize price fields across all types
    const priceKeyMap = {
      hotels: ['rawPricePerNight', 'pricePerNight', 'rawBasePrice', 'basePrice'],
      cars: ['rawPrice', 'price'],
      trips: ['rawPrice', 'price'],
      attractions: ['rawPrice', 'price']
    };

    const keys = priceKeyMap[resourceType] || [];
    let normalizedPrice = 0;
    for (const key of keys) {
      const val = rawData[key];
      if (val !== undefined && val !== null) {
        if (typeof val === 'number') {
          normalizedPrice = val;
          break;
        } else if (typeof val === 'string') {
          const stripped = Number(val.replace(/[^0-9.]/g, ''));
          if (!isNaN(stripped)) {
            normalizedPrice = stripped;
            break;
          }
        }
      }
    }

    data.value = {
      ...rawData,
      price: normalizedPrice || rawData.price, // Final fallback
      images: Array.isArray(rawData.images) ? rawData.images : (rawData.images ? [rawData.images] : (rawData.profileImage ? [rawData.profileImage] : []))
    };
  } catch (err) {
    console.error('Fetch error:', err);
    error.value = 'Failed to load details';
  } finally {
    loading.value = false;
  }
};

const handleEdit = () => {
  selectedItem.value = JSON.parse(JSON.stringify(data.value)); // Deep copy
  
  // Optimization: Load only up to 4 images
  if (selectedItem.value.images && Array.isArray(selectedItem.value.images)) {
    selectedItem.value.images = selectedItem.value.images.slice(0, 4);
  }

  // Data Normalization: Convert amenities object to array for Trips
  if (resourceType === 'trips' && selectedItem.value.amenities && !Array.isArray(selectedItem.value.amenities) && typeof selectedItem.value.amenities === 'object') {
     selectedItem.value.amenities = Object.keys(selectedItem.value.amenities).filter(k => selectedItem.value.amenities[k]);
  }
  
  // Data Normalization: Map user data fields for form
  if (resourceType === 'users') {
    // Map phoneNumber to phone for form compatibility
    if (selectedItem.value.phoneNumber) {
      selectedItem.value.phone = selectedItem.value.phoneNumber;
    }
    // Map profileImage to image for form compatibility
    if (selectedItem.value.profileImage) {
      selectedItem.value.image = selectedItem.value.profileImage;
    }
    // Ensure name field exists (combine firstName + lastName if needed)
    if (!selectedItem.value.name && (selectedItem.value.firstName || selectedItem.value.lastName)) {
      selectedItem.value.name = `${selectedItem.value.firstName || ''} ${selectedItem.value.lastName || ''}`.trim();
    }
  }
  
  showFormModal.value = true;
};

const handleUpdate = async ({ mode, data: formData }) => {
  modalloading.value = true;
  
  // Merge with initial data to ensure all fields are present
  const finalData = { ...selectedItem.value, ...formData };
  
  // Validate required fields based on resource type
  if (resourceType === 'hotels') {
    if (!finalData.name?.trim()) {
      toast.error('Hotel name is required');
      modalloading.value = false;
      return;
    }
    if (!finalData.city?.trim()) {
      toast.error('City is required');
      modalloading.value = false;
      return;
    }
    if (!finalData.pricePerNight && !finalData.price) {
      toast.error('Price is required');
      modalloading.value = false;
      return;
    }
    if (!finalData.overview?.trim() && !finalData.description?.trim()) {
      toast.error('Description is required');
      modalloading.value = false;
      return;
    }
  } else if (resourceType === 'users') {
    if (!finalData.name?.trim() && !finalData.firstName?.trim()) {
      toast.error('User name is required');
      modalloading.value = false;
      return;
    }
    if (!finalData.email?.trim()) {
      toast.error('Email is required');
      modalloading.value = false;
      return;
    }
  } else if (resourceType === 'trips') {
    if (!finalData.title?.trim()) {
      toast.error('Trip title is required');
      modalloading.value = false;
      return;
    }
    if (!finalData.destination?.trim() && !finalData.city?.trim()) {
      toast.error('Destination is required');
      modalloading.value = false;
      return;
    }
    if (!finalData.price || finalData.price <= 0) {
      toast.error('Valid price is required');
      modalloading.value = false;
      return;
    }
  } else if (resourceType === 'cars') {
    if (!finalData.name?.trim() && !finalData.brand?.trim()) {
      toast.error('Brand/Name is required');
      modalloading.value = false;
      return;
    }
    if (!finalData.price || finalData.price <= 0) {
      toast.error('Valid price is required');
      modalloading.value = false;
      return;
    }
    if (!finalData.city?.trim() && !finalData.location?.trim()) {
      toast.error('Location is required');
      modalloading.value = false;
      return;
    }
  } else if (resourceType === 'attractions') {
    if (!finalData.name?.trim()) {
      toast.error('Attraction name is required');
      modalloading.value = false;
      return;
    }
    if (!finalData.price || finalData.price <= 0) {
      toast.error('Valid price is required');
      modalloading.value = false;
      return;
    }
    if (!finalData.location?.trim() && !finalData.city?.trim()) {
      toast.error('Location is required');
      modalloading.value = false;
      return;
    }
  }
  
  // Transform form data to match backend expectations
  let transformedData = { ...finalData };
  if (resourceType === 'hotels') {
    transformedData = {
      ...finalData,
      name: finalData.name?.trim(),
      city: finalData.city?.trim(),
      basePrice: Number(finalData.price || finalData.pricePerNight),
      pricePerNight: Number(finalData.pricePerNight || finalData.price),
      description: (finalData.description || finalData.overview)?.trim(),
      images: Array.isArray(finalData.images) ? finalData.images.filter(img => typeof img === 'string') : [],
      facilities: Array.isArray(finalData.amenities) ? finalData.amenities.filter(f => typeof f === 'string') : [],
      Id: Number(id)
    };
  } else if (resourceType === 'users') {
    transformedData = {
      firstName: finalData.firstName?.trim() || finalData.name?.trim() || '',
      lastName: finalData.lastName?.trim() || '',
      email: finalData.email?.trim(),
      phone: (finalData.phone || finalData.phoneNumber)?.trim() || '',
      nationality: finalData.nationality?.trim() || '',
      profileImage: finalData.image || finalData.profileImage || '',
      Id: id // Use the global id (string)
    };
  } else if (resourceType === 'trips') {
    const rawAmenities = finalData.amenities || [];
    let transportValue = false;
    let accommodationValue = false;
    let mealsValue = 0;

    if (Array.isArray(rawAmenities)) {
      transportValue = rawAmenities.some(a => typeof a === 'string' && a.toLowerCase().includes('transport'));
      accommodationValue = rawAmenities.some(a => typeof a === 'string' && a.toLowerCase().includes('hotel'));
      mealsValue = rawAmenities.some(a => typeof a === 'string' && a.toLowerCase().includes('meal')) ? 1 : 0;
    } else if (rawAmenities && typeof rawAmenities === 'object') {
      transportValue = !!(rawAmenities.transport || rawAmenities.Transport);
      accommodationValue = !!(rawAmenities.accommodation || rawAmenities.Accommodation);
      mealsValue = Number(rawAmenities.meals || rawAmenities.Meals || 0);
    }

    transformedData = {
      Id: Number(id),
      Title: finalData.title?.trim() || "",
      Description: finalData.description?.trim() || finalData.title?.trim() || "",
      City: (finalData.city || finalData.destination)?.trim() || "",
      Destination: (finalData.destination || finalData.city)?.trim() || "",
      TripType: finalData.tripType || "Beach Getaway",
      Price: Number(finalData.price || 0),
      Duration: String(finalData.duration || "1 Day / 1 Night"),
      MaxPeople: Number(finalData.maxPeople || 10),
      AvailableSpots: Number(finalData.availableSpots || finalData.maxPeople || 10),
      MainImage: (finalData.images && finalData.images.length > 0) ? finalData.images[0] : "placeholder.jpg",
      Images: Array.isArray(finalData.images) ? finalData.images.filter(img => typeof img === 'string') : [],
      AvailableDates: Array.isArray(finalData.availableDates) ? finalData.availableDates : [],
      Highlights: Array.isArray(finalData.highlights) ? finalData.highlights : [],
      Status: finalData.status || "Ongoing",
      IsFeatured: !!(finalData.isFeatured || finalData.featured),
      Featured: !!(finalData.featured || finalData.isFeatured),

      Amenities: {
        Transport: transportValue,
        Accommodation: accommodationValue,
        Meals: mealsValue
      },

      HotelInfo: finalData['hotel.name'] || finalData.hotelInfo?.name ? {
        Name: finalData['hotel.name'] || finalData.hotelInfo?.name,
        Rating: Number(finalData['hotel.rating'] || finalData.hotelInfo?.rating || 4.5),
        Image: "hotel-placeholder.jpg",
        Features: []
      } : null,

      Itinerary: (finalData.itinerary || []).map(day => ({
        Day: Number(day.day || 1),
        Title: day.title || "Day Title",
        Description: day.description || day.title || "Day Description",
        Activities: (day.activities || []).map(act => ({
          Time: act.time || "09:00 AM",
          Title: act.title || "Activity Title",
          Description: act.description || act.desc || "Activity Description"
        }))
      }))
    };
  } else if (resourceType === 'attractions') {
    transformedData = {
      ...finalData,
      name: finalData.name?.trim(),
      city: (finalData.city || finalData.location)?.trim(),
      location: (finalData.location || finalData.city)?.trim(),
      price: Number(finalData.price),
      capacity: finalData.capacity?.toString(),
      images: Array.isArray(finalData.images) ? finalData.images.filter(img => typeof img === 'string') : [],
      Id: Number(id)
    };
  } else if (resourceType === 'cars') {
    transformedData = {
      ...finalData,
      name: finalData.name?.trim() || `${finalData.brand} ${finalData.model}`,
      brand: finalData.brand?.trim(),
      model: finalData.model?.trim(),
      city: (finalData.city || finalData.location)?.trim(),
      price: Number(finalData.price),
      seats: Number(finalData.seats || 4),
      totalFleet: Number(finalData.totalFleet || 1),
      availableNow: Number(finalData.availableNow || 0),
      images: Array.isArray(finalData.images) ? finalData.images.filter(img => typeof img === 'string') : [],
      Id: Number(id)
    };
  }
  
  try {
    const api = apiMap[resourceType];
    const updateId = resourceType === 'users' ? transformedData.Id : Number(transformedData.Id); 
    await api.update(updateId, transformedData);
    
    toast.success(`${config.value?.title || 'Item'} updated successfully`);
    showFormModal.value = false;
    await fetchData(); // Re-fetch to sync UI with server state
  } catch (err) {
    console.error('Update failed:', err);
    const errorMessage = err.response?.data?.errors 
      ? Object.values(err.response.data.errors).flat().join(', ')
      : err.response?.data?.message || err.message;
    toast.error(`Failed to update ${config?.title?.toLowerCase() || 'item'}: ${errorMessage}`);
  } finally {
    modalloading.value = false;
  }
};

onMounted(() => {
  fetchData();
});
</script>
