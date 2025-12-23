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
        <div class="flex items-center gap-2">
          <!-- Create Ticket Button (Client Only) - Sidebar admin view -->
          <button 
            v-if="isClient && !isUserView"
            @click="handleCreateTicket"
            class="btn btn-ghost btn-sm btn-circle text-primary hover:bg-primary/10"
            title="Create new ticket"
            :disabled="isCreatingTicket"
          >
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
            </svg>
          </button>

         

          <!-- Filter Button (Admins only) -->
          <div v-if="isAdmin" class="dropdown dropdown-end">
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

           <!-- Create Ticket Button (Client user view when tickets already exist) -->
          <button
            v-if="isClient && isUserView && hasTickets"
            @click="handleCreateTicket"
            class="btn btn-primary btn-sm gap-2"
            :disabled="isCreatingTicket"
          >
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-4 h-4">
              <path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
            </svg>
          </button>
        </div>
      </div>
    </div>

    <!-- Ticket List -->
    <div class="flex-1 overflow-y-auto">
      <div v-if="loading" class="flex justify-center p-4">
        <span class="loading loading-spinner loading-md text-primary"></span>
      </div>
      
      <!-- Empty State -->
      <div v-else-if="filteredList.length === 0" class="h-full flex flex-col items-center justify-center p-8 text-center">
        <!-- Icon -->
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-16 h-16 text-base-content/30 mb-4">
          <path stroke-linecap="round" stroke-linejoin="round" d="M7.5 8.25h9m-9 3H12m-9.75 1.51c0 1.6 1.123 2.994 2.707 3.227 1.129.166 2.27.293 3.423.379.35.026.67.21.865.501L12 21l2.755-4.007a1.72 1.72 0 0 1 .865-.501 48.394 48.394 0 0 0 3.423-.379c1.584-.233 2.707-1.626 2.707-3.228V6.741c0-1.602-1.123-2.995-2.707-3.228A48.394 48.394 0 0 0 12 3c-2.392 0-4.744.175-7.043.513C3.373 3.746 2.25 5.14 2.25 6.741v10.759Z" />
        </svg>
        
        <!-- Message -->
        <p class="text-base-content/60 mb-6">
          {{ isClient ? 'No tickets found' : 'No active support requests from clients' }}
        </p>
        
        <!-- Create Ticket Button (Client Only) - Empty State -->
        <button 
          v-if="isClient"
          @click="openCreateTicketModal"
          class="btn btn-primary btn-sm gap-2"
          :disabled="isCreatingTicket"
        >
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-4 h-4">
            <path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
          </svg>
          Create Ticket
        </button>
      </div>

      <!-- Ticket List -->
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

    <!-- Create Ticket Modal (Client Only) -->
    <div 
      v-if="isClient && showCreateModal"
      class="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
      @click.self="closeCreateTicketModal"
    >
      <div class="modal-box w-full max-w-md shadow-xl">
        <h2 class="font-bold text-lg mb-4">Create New Ticket</h2>
        
        <div class="space-y-4">
          <!-- Subject Input -->
          <div class="form-control">
            <label class="label"><span class="label-text">Subject</span></label>
            <input 
              v-model="newTicket.subject"
              type="text" 
              placeholder="Brief description of your issue"
              class="input input-bordered"
              @keyup.enter="submitCreateTicket"
            />
          </div>

          <!-- Priority Select (hidden for client view; admin assigns priority) -->
          <div v-if="!isUserView" class="form-control">
            <label class="label"><span class="label-text">Priority</span></label>
            <select v-model="newTicket.priority" class="select select-bordered">
              <option value="Low">Low</option>
              <option value="Medium" selected>Medium</option>
              <option value="High">High</option>
            </select>
          </div>

          <!-- Initial Message -->
          <div class="form-control">
            <label class="label"><span class="label-text">Message</span></label>
            <textarea 
              v-model="newTicket.message"
              placeholder="Describe your issue in detail..."
              class="textarea textarea-bordered h-24"
            ></textarea>
          </div>

          <!-- Error Message -->
          <div v-if="createError" class="alert alert-error">
            <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l-2-2m0 0l-2-2m2 2l2-2m-2 2l-2 2" /></svg>
            <span>{{ createError }}</span>
          </div>
        </div>

        <!-- Modal Actions -->
        <div class="modal-action mt-6">
          <button @click="closeCreateTicketModal" class="btn btn-ghost">Cancel</button>
          <button 
            @click="submitCreateTicket"
            class="btn btn-primary"
            :disabled="isCreatingTicket || !newTicket.subject || !newTicket.message"
          >
            <span v-if="isCreatingTicket" class="loading loading-spinner loading-sm"></span>
            {{ isCreatingTicket ? 'Creating...' : 'Create Ticket' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import { useSupportStore } from '@/stores/supportStore';
import { useAuthStore } from '@/stores/authStore';
import { useChat } from '@/Services/chatService';
import { useToast } from '@/composables/useToast';
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
const authStore = useAuthStore();
const { currentTicket, loading } = storeToRefs(store);
const { chatService } = useChat();
const { toast } = useToast();

// Auth state
const isClient = computed(() => authStore.user?.role?.toLowerCase() === 'client');
const isAdmin = computed(() => authStore.user?.role?.toLowerCase() === 'admin');

// UI state
const searchQuery = ref('');
const filters = ref({
  status: 'All',
  priority: 'All',
  date: ''
});
const hasTickets = computed(() => (store.tickets?.length || 0) > 0);

// Create Ticket modal state
const showCreateModal = ref(false);
const isCreatingTicket = ref(false);
const createError = ref('');
const newTicket = ref({
  subject: '',
  priority: 'Medium',
  message: ''
});

const filteredList = computed(() => {
  let list = store.filteredTickets(searchQuery.value, filters.value);
  if (props.isUserView && props.userId) {
    list = list.filter(t => t.userId === props.userId);
  }
  return list;
});

const selectTicket = async (ticket) => {
  store.currentTicket = ticket;
  // Load real messages from backend via chatService
  if (ticket.id) {
    await chatService.fetchHistory(ticket.id);
  }
};

const openCreateTicketModal = () => {
  showCreateModal.value = true;
  createError.value = '';
  newTicket.value = {
    subject: '',
    priority: 'Medium',
    message: ''
  };
};

const closeCreateTicketModal = () => {
  showCreateModal.value = false;
  createError.value = '';
  newTicket.value = {
    subject: '',
    priority: 'Medium',
    message: ''
  };
};

const submitCreateTicket = async () => {
  if (!newTicket.value.subject.trim() || !newTicket.value.message.trim()) {
    createError.value = 'Please fill in all required fields';
    return;
  }

  isCreatingTicket.value = true;
  createError.value = '';

  try {
    const createdConversation = await store.createTicket({
      subject: newTicket.value.subject,
      priority: newTicket.value.priority,
      initialMessage: newTicket.value.message
    });

    // Select the newly created ticket
    if (createdConversation) {
      store.currentTicket = createdConversation;
      await chatService.fetchHistory(createdConversation.id);
      
      // Notify admins of the new ticket via SignalR (non-blocking)
      chatService.notifyAdminsOfNewTicket({
        id: createdConversation.id,
        subject: createdConversation.subject,
        clientName: createdConversation.userName,
        clientEmail: createdConversation.userEmail,
        priority: createdConversation.priority,
        status: createdConversation.status,
        createdAt: createdConversation.createdAt
      });

      toast.success('Ticket created successfully! Your admin will respond shortly.', 3000);
    }

    closeCreateTicketModal();
  } catch (error) {
    console.error('Failed to create ticket:', error);
    createError.value = error.response?.data?.message || 'Failed to create ticket. Please try again.';
    toast.error(createError.value, 4000);
  } finally {
    isCreatingTicket.value = false;
  }
};

const handleCreateTicket = () => {
  if (!isClient.value) {
    toast.warning('Only clients can create tickets');
    return;
  }
  openCreateTicketModal();
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
  const now = new Date();
  const diffInSeconds = Math.floor((now - date) / 1000);
  
  if (diffInSeconds < 60) return 'Just now';
  if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)}m ago`;
  if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)}h ago`;
  return date.toLocaleDateString();
};

onMounted(() => {
  store.fetchTickets();
  chatService.fetchConversations();

  // Listen for new tickets created by other clients (for Admins)
  window.addEventListener('newTicketCreated', async (event) => {
    console.log('New ticket received via SignalR:', event.detail);
    // Refresh tickets for admins
    await store.fetchTickets();
  });
});

onBeforeUnmount(() => {
  // Clean up event listener
  window.removeEventListener('newTicketCreated', null);
});
</script>
