<template>
  <div class="h-full flex flex-col bg-base-100 border-r border-base-300">
    <!-- Header -->
    <div class="p-4 border-b border-base-300">
      <div class="form-control mb-4">
        <label class="input input-bordered flex items-center gap-2">
          <input type="text" class="grow" placeholder="Search tickets..." v-model="searchQuery" />
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" fill="currentColor" class="w-4 h-4 opacity-70"><path fill-rule="evenodd" d="M9.965 11.026a5 5 0 1 1 1.06-1.06l2.755 2.754a.75.75 0 1 1-1.06 1.06l-2.755-2.754ZM10.5 7a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0Z" clip-rule="evenodd" /></svg>
        </label>
      </div>
      
      <div class="flex justify-between items-center">
        <h2 class="font-bold text-lg">{{ isUserView ? 'My Tickets' : 'All Tickets' }}</h2>
        <div class="dropdown dropdown-end">
          <div tabindex="0" role="button" class="btn btn-ghost btn-sm gap-2">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M12 3c2.755 0 5.455.232 8.083.678.533.09.917.556.917 1.096v1.044a2.25 2.25 0 0 1-.659 1.591l-5.432 5.432a2.25 2.25 0 0 0-.659 1.591v2.927a2.25 2.25 0 0 1-1.244 2.013L9.75 21v-6.568a2.25 2.25 0 0 0-.659-1.591L3.659 7.409A2.25 2.25 0 0 1 3 5.818V4.774c0-.54.384-1.006.917-1.096A48.32 48.32 0 0 1 12 3Z" />
            </svg>
            Filter
          </div>
          <div tabindex="0" class="dropdown-content z-[1] card card-compact w-64 p-2 shadow bg-base-100 text-primary-content border border-base-300 transform translate-y-1">
            <div class="card-body">
              <h3 class="font-bold text-base-content mb-2">Filters</h3>
              
              <div class="form-control w-full">
                <label class="label"><span class="label-text">Status</span></label>
                <select class="select select-bordered select-sm w-full text-base-content" v-model="filters.status">
                  <option value="All">All Statuses</option>
                  <option value="Open">Open</option>
                  <option value="Pending">Pending</option>
                  <option value="Resolved">Resolved</option>
                  <option value="Closed">Closed</option>
                </select>
              </div>

              <div class="form-control w-full mt-2">
                <label class="label"><span class="label-text">Priority</span></label>
                <select class="select select-bordered select-sm w-full text-base-content" v-model="filters.priority">
                  <option value="All">All Priorities</option>
                  <option value="High">High</option>
                  <option value="Medium">Medium</option>
                  <option value="Low">Low</option>
                </select>
              </div>

              <div class="form-control w-full mt-2">
                <label class="label"><span class="label-text">Date</span></label>
                <input type="date" class="input input-bordered input-sm w-full text-base-content" v-model="filters.date" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Ticket List -->
    <div class="flex-1 overflow-y-auto">
      <div v-if="loading" class="flex justify-center p-4">
        <span class="loading loading-spinner loading-md text-primary"></span>
      </div>
      
      <div v-else-if="filteredList.length === 0" class="p-8 text-center text-base-content/60">
        <p>No tickets found</p>
      </div>

      <div 
        v-else 
        v-for="ticket in filteredList" 
        :key="ticket.id"
        @click="selectTicket(ticket)"
        class="p-4 border-b border-base-200 cursor-pointer hover:bg-base-200 transition-colors"
        :class="{ 'bg-primary/10 border-l-4 border-l-primary': currentTicket?.id === ticket.id }"
      >
        <div class="flex justify-between items-start mb-1">
          <h3 class="font-semibold text-sm truncate pr-2 text-base-content">{{ ticket.subject }}</h3>
          <div v-if="!isUserView" class="badge badge-xs" :class="getPriorityClass(ticket.priority)">{{ ticket.priority }}</div>
        </div>
        
        <div class="flex justify-between items-center text-xs text-base-content/60 mb-2">
            <span>{{ ticket.userName }}</span>
            <span>{{ ticket.id }}</span>
        </div>

        <div class="flex justify-between items-center mt-2">
          <div v-if="!isUserView" class="badge" :class="getStatusClass(ticket.status)">{{ ticket.status }}</div>
          <span class="text-xs text-base-content/50">{{ formatDate(ticket.updatedAt) }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useSupportStore } from '@/stores/supportStore';
import { storeToRefs } from 'pinia';

const props = defineProps({
  isUserView: {
    type: Boolean,
    default: false
  },
  userId: {
    type: String,
    default: ''
  }
});

const store = useSupportStore();
const { currentTicket, loading } = storeToRefs(store);

const searchQuery = ref('');
const filters = ref({
  status: 'All',
  priority: 'All',
  date: ''
});

const filteredList = computed(() => {
  let list = store.filteredTickets(searchQuery.value, filters.value);
  if (props.isUserView && props.userId) {
    list = list.filter(t => t.userId === props.userId);
  }
  return list;
});

const selectTicket = (ticket) => {
  store.currentTicket = ticket;
};

// Helpers
const getStatusClass = (status) => {
  switch (status) {
    case 'Open': return 'badge-info';
    case 'Pending': return 'badge-warning';
    case 'Resolved': return 'badge-success';
    case 'Closed': return 'badge-neutral';
    default: return 'badge-ghost';
  }
};

const getPriorityClass = (priority) => {
  switch (priority) {
    case 'High': return 'badge-error badge-outline';
    case 'Medium': return 'badge-warning badge-outline';
    case 'Low': return 'badge-success badge-outline';
    default: return 'badge-ghost badge-outline';
  }
};

const formatDate = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  // Simple "time ago" logic if date-fns not available, or just use toLocaleDateString
  // Using a simple formatter for now to avoid dependency issues if date-fns isn't there
  const now = new Date();
  const diffInSeconds = Math.floor((now - date) / 1000);
  
  if (diffInSeconds < 60) return 'Just now';
  if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)}m ago`;
  if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)}h ago`;
  return date.toLocaleDateString();
};

onMounted(() => {
  store.fetchTickets();
});
</script>
