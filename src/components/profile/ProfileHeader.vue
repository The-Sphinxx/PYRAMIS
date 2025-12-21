<template>
  <div class="bg-base-100 border-b border-base-300">
    <div class="page-container py-8">
      <div class="flex items-start gap-6 flex-col md:flex-row">
        <!-- Profile Image -->
        <div class="relative">
          <img
            :src="user.avatar"
            :alt="user.name"
            class="w-24 h-24 rounded-full object-cover border-4 border-white shadow-xl"
          />
          <div class="absolute bottom-0 left-0 w-6 h-6 bg-success rounded-full border-2 border-white"></div>
        </div>

        <!-- Profile Info -->
        <div class="flex-1 w-full">
          <div class="flex items-center gap-3 mb-2 flex-wrap">
            <h1 class="text-3xl font-bold text-base-content">{{ user.name }}</h1>
            <span class="badge badge-warning text-xs font-bold px-4 py-3">
              {{ user.membershipType }}
            </span>
          </div>
          <p class="text-base-content/70 mb-4">{{ user.email }}</p>
          <button 
            @click="$emit('edit-profile')"
            class="btn btn-outline btn-sm gap-2"
          >
            <i class="fas fa-user-edit"></i>
            تعديل الملف الشخصي
          </button>
        </div>
      </div>

      <!-- Tabs -->
      <div class="flex gap-4 mt-8 border-b border-base-300 overflow-x-auto">
        <button
          v-for="tab in tabs"
          :key="tab.id"
          @click="$emit('tab-change', tab.id)"
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

<script>
export default {
  name: 'ProfileHeader',
  props: {
    user: {
      type: Object,
      required: true
    },
    tabs: {
      type: Array,
      required: true
    },
    activeTab: {
      type: String,
      required: true
    }
  },
  emits: ['edit-profile', 'tab-change']
}
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