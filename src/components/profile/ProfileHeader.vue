<template>
  <div class="bg-base-100 border-b border-base-300">
    <div class="page-container py-8">
      <div class="flex items-start gap-6 flex-col md:flex-row">
        <div class="relative">
          <img
            :src="displayUser.avatar"
            :alt="displayUser.name"
            class="w-24 h-24 rounded-full object-cover border-4 border-white shadow-xl"
          />
          <div class="absolute bottom-0 left-0 w-6 h-6 bg-success rounded-full border-2 border-white"></div>
        </div>

        <div class="flex-1 w-full">
          <div class="flex items-center gap-3 mb-2 flex-wrap">
            <h1 class="text-3xl font-bold text-base-content">{{ displayUser.name }}</h1>
            <span class="badge badge-warning text-xs font-bold px-4 py-3">
              {{ displayUser.membershipType }}
            </span>
          </div>
          <p class="text-base-content/70 mb-4">{{ displayUser.email }}</p>
          <button @click="onEditProfile" class="btn btn-outline btn-sm gap-2">
            <i class="fas fa-user-edit"></i>
            Edit your profile
          </button>
        </div>
      </div>

      <div class="flex gap-4 mt-8 border-b border-base-300 overflow-x-auto">
        <button
          v-for="tab in tabs"
          :key="tab.id"
          @click="onTabChange(tab.id)"
          :class="[
            'pb-3 px-4 font-semibold transition-all flex items-center gap-2 whitespace-nowrap',
            activeTab === tab.id
              ? 'text-primary border-b-2 border-primary'
              : 'text-base-content/60 hover:text-base-content'
          ]"
        >
          <i :class="tab.icon"></i>
          {{ tab.name }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { getUserFromToken } from '@/Utils/auth';

const DEFAULT_AVATAR = 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&auto=format&fit=crop&q=80';

const props = defineProps({
  user: {
    type: Object,
    default: () => ({})
  },
  tabs: {
    type: Array,
    required: true
  },
  activeTab: {
    type: String,
    required: true
  }
});

const emit = defineEmits(['edit-profile', 'tab-change']);

const tokenUser = computed(() => getUserFromToken());

const displayUser = computed(() => {
  const fromProps = props.user || {};
  const fromToken = tokenUser.value || {};

  const nameFromProps = fromProps.name || fromProps.fullName;
  const nameFromToken = fromToken.fullName || [fromToken.firstName, fromToken.lastName].filter(Boolean).join(' ');

  return {
    name: nameFromProps || nameFromToken || 'Guest User',
    email: fromProps.email || fromToken.email || '',
    avatar: fromProps.avatar || fromProps.profileImage || fromToken.profileImage || DEFAULT_AVATAR,
    membershipType: fromProps.membershipType || fromToken.membershipType || 'Standard Member'
  };
});

const onEditProfile = () => emit('edit-profile');
const onTabChange = (tabId) => emit('tab-change', tabId);
</script>

<style scoped>
.page-container {
  width: 100%;
  padding-left: 120px;
  padding-right: 120px;
}

@media (max-width: 1280px) {
  .page-container {
    padding-left: 80px;
    padding-right: 80px;
  }
}

@media (max-width: 1024px) {
  .page-container {
    padding-left: 60px;
    padding-right: 60px;
  }
}

@media (max-width: 768px) {
  .page-container {
    padding-left: 24px;
    padding-right: 24px;
  }
}

@media (max-width: 640px) {
  .page-container {
    padding-left: 16px;
    padding-right: 16px;
  }
}

.btn {
  border-radius: 6px;
}

.badge {
  border-radius: 6px;
}
</style>