<template>
  <div v-if="currentTicket" class="h-full flex flex-col bg-base-100">
    <!-- Chat Header -->
    <div class="p-6 border-b border-base-300 flex justify-between items-start bg-base-100">
      <div>
        <h2 class="text-xl font-bold mb-1">{{ currentTicket.subject }}</h2>
        <div class="flex items-center gap-2 text-sm">
          <span class="font-mono bg-base-200 px-1 rounded text-xs">{{ currentTicket.id }}</span>
          <div v-if="!isUserView" class="badge" :class="getStatusClass(currentTicket.status)">{{ currentTicket.status }}</div>
          <div v-if="!isUserView" class="badge badge-outline" :class="getPriorityClass(currentTicket.priority)">{{ currentTicket.priority }} Priority</div>
        </div>
      </div>
      
      <div class="flex items-center gap-2">
        <div v-if="!isUserView" class="dropdown dropdown-end">
            <div tabindex="0" role="button" class="btn btn-sm btn-outline">
                Assign
            </div>
            <ul tabindex="0" class="dropdown-content z-[1] menu p-2 shadow bg-base-100 rounded-box w-52">
                <li><a @click="assignToMe">Assign to Me</a></li>
                <!-- Future: List other admins -->
            </ul>
        </div>
        <div v-if="!isUserView" class="dropdown dropdown-end">
            <div tabindex="0" role="button" class="btn btn-sm btn-ghost btn-circle">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-6 h-6">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M12 6.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 12.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 18.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5Z" />
                </svg>
            </div>
            <ul tabindex="0" class="dropdown-content z-[1] menu p-2 shadow bg-base-100 rounded-box w-52">
                <li><a @click="updateStatus('Resolved')">Mark as Resolved</a></li>
                <li><a @click="updateStatus('Pending')">Mark as Pending</a></li>
                <li><a @click="updateStatus('Closed')">Close Ticket</a></li>
            </ul>
        </div>
      </div>
    </div>

    <!-- User Info Header (Compact) -->
    <div class="px-6 py-3 bg-base-200/50 flex items-center justify-between border-b border-base-300">
      <div class="flex items-center gap-4">
        <div class="avatar placeholder">
          <div class="bg-neutral text-neutral-content rounded-full w-10">
            <span>{{ getInitials(currentTicket.userName) }}</span>
          </div>
        </div>
        <div>
          <div class="font-bold">{{ currentTicket.userName }}</div>
          <div class="text-xs opacity-70">{{ currentTicket.userEmail }}</div>
        </div>
      </div>
      <div class="text-xs opacity-60">
        Created {{ formatDate(currentTicket.createdAt) }}
      </div>
    </div>

    <!-- Messages Area -->
    <div class="flex-1 overflow-y-auto p-6 space-y-4 bg-base-200/30" ref="messagesContainer">
      <div v-for="msg in currentTicket.messages" :key="msg.id" class="chat" :class="isMessageFromMe(msg) ? 'chat-end' : 'chat-start'">
        <!-- Avatar logic: if message is NOT from me, show avatar -->
        <div class="chat-image avatar placeholder" v-if="!isMessageFromMe(msg)">
          <div class="w-10 rounded-full bg-neutral text-neutral-content" :class="{'bg-primary': !msg.isSupport && !isUserView, 'bg-neutral': msg.isSupport && isUserView}">
             <span>{{ getInitials(msg.senderName) }}</span>
          </div>
        </div>

        <div class="chat-header text-xs opacity-50 mb-1">
          {{ msg.senderName }}
          <time class="ml-1">{{ formatTime(msg.timestamp) }}</time>
        </div>
        <div class="chat-bubble" :class="isMessageFromMe(msg) ? 'chat-bubble-primary' : 'bg-base-200 text-base-content border border-base-300'">
          {{ msg.content }}
        </div>
      </div>
    </div>

    <!-- Reply Input -->
    <div class="p-4 bg-base-100 border-t border-base-300">
      <div class="flex gap-2">
        <textarea 
            class="textarea textarea-bordered w-full resize-none" 
            placeholder="Type your reply..." 
            rows="3"
            v-model="newMessage"
            @keydown.enter.prevent="sendMessage"
        ></textarea>
        <button class="btn btn-primary h-auto" @click="sendMessage" :disabled="!newMessage.trim() || sending">
            <span v-if="sending" class="loading loading-spinner"></span>
            <svg v-else xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="w-6 h-6">
                <path d="M3.478 2.404a.75.75 0 0 0-.926.941l2.432 7.905H13.5a.75.75 0 0 1 0 1.5H4.984l-2.432 7.905a.75.75 0 0 0 .926.94 60.519 60.519 0 0 0 18.445-8.986.75.75 0 0 0 0-1.218A60.517 60.517 0 0 0 3.478 2.404Z" />
            </svg>
        </button>
      </div>
    </div>
  </div>

  <div v-else class="h-full flex items-center justify-center bg-base-200/30 text-base-content/50">
      <div class="text-center">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-16 h-16 mx-auto mb-4 opacity-50">
              <path stroke-linecap="round" stroke-linejoin="round" d="M2.25 12.76c0 1.6 1.123 2.994 2.707 3.227 1.068.157 2.148.279 3.238.364.466.037.893.281 1.153.671L12 21l2.652-3.978c.26-.39.687-.634 1.153-.67 1.09-.086 2.17-.208 3.238-.365 1.584-.233 2.707-1.626 2.707-3.228V6.741c0-1.602-1.123-2.995-2.707-3.228A48.394 48.394 0 0 0 12 3c-2.392 0-4.744.015-7.03.046-1.584.233-2.707 1.626-2.707 3.228v6.741Z" />
          </svg>
          <p class="text-lg font-semibold">Select a ticket to view details</p>
      </div>
  </div>
</template>

<script setup>
import { ref, watch, nextTick, onMounted } from 'vue';
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
  },
  userName: {
    type: String,
    default: 'User'
  }
});

const store = useSupportStore();
const { currentTicket } = storeToRefs(store);
const newMessage = ref('');
const sending = ref(false);
const messagesContainer = ref(null);

const isMessageFromMe = (msg) => {
  if (props.isUserView) {
    return !msg.isSupport; // Me (User) = !isSupport
  } else {
    return msg.isSupport; // Me (Admin) = isSupport
  }
};

const scrollToBottom = () => {
    if (messagesContainer.value) {
        messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
    }
};

watch(() => currentTicket.value?.messages?.length, () => {
    nextTick(scrollToBottom);
});

watch(() => currentTicket.value, () => {
    nextTick(scrollToBottom);
});

const sendMessage = async () => {
    if (!newMessage.value.trim() || !currentTicket.value) return;
    
    sending.value = true;
    try {
        const messageData = {
            content: newMessage.value,
        };

        if (props.isUserView) {
            messageData.senderName = props.userName;
            messageData.senderId = props.userId;
            messageData.isSupport = false;
        } else {
            messageData.senderName = 'Support Team'; // Should be dynamic admin name
            messageData.senderId = 'admin_1';
            messageData.isSupport = true;
        }

        await store.sendMessage(currentTicket.value.id, messageData);
        newMessage.value = '';
    } catch (err) {
        console.error('Failed to send message', err);
    } finally {
        sending.value = false;
    }
};

const updateStatus = async (status) => {
    if (!currentTicket.value) return;
    await store.updateTicketStatus(currentTicket.value.id, status);
};

const assignToMe = async () => {
    if (!currentTicket.value) return;
    // Assuming 'admin_1' is the current user ID for now
    await store.assignTicket(currentTicket.value.id, 'admin_1');
};

const getInitials = (name) => {
    if (!name) return '?';
    return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase();
};

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
    case 'High': return 'badge-error';
    case 'Medium': return 'badge-warning';
    case 'Low': return 'badge-success';
    default: return 'badge-ghost';
  }
};

const formatDate = (dateString) => {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('en-US', {
        year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit'
    });
};

const formatTime = (dateString) => {
    if (!dateString) return '';
    return new Date(dateString).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
};
</script>
