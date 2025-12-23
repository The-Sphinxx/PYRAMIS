import { ref, reactive } from 'vue';
import apiService from '@/Services/apiService';
import { HubConnectionBuilder } from '@microsoft/signalr';

const API_BASE = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5137';

class ChatService {
  constructor() {
    this.connection = null;
    this.conversations = reactive({
      list: [],
      currentId: null
    });
    this.messages = reactive({
      current: [],
      loading: false
    });
    this.typing = reactive({
      userId: null,
      userName: null,
      isTyping: false
    });
  }

  /**
   * Initialize SignalR connection and authenticate
   */
  async connect() {
    if (this.connection) return;

    const token = localStorage.getItem('token');
    if (!token) {
      console.warn('ChatService: No auth token found');
      return;
    }

    try {
      this.connection = new HubConnectionBuilder()
        .withUrl(`${API_BASE}/hubs/chat`, {
          accessTokenFactory: () => token
        })
        .withAutomaticReconnect([0, 1000, 3000, 5000, 10000])
        .build();

      // Setup event handlers
      this.connection.on('ReceiveMessage', (message) => {
        console.log('New message received:', message);
        this.messages.current.push({
          id: message.id || Date.now(),
          conversationId: message.conversationId,
          senderId: message.senderId,
          senderName: message.senderName,
          senderRole: message.senderRole,
          content: message.content,
          timestamp: message.sentAt || new Date().toISOString(),
          isSupport: message.senderRole === 'Admin'
        });
      });

      this.connection.on('NewTicketCreated', (ticketData) => {
        console.log('New ticket created by client:', ticketData);
        // Emit event that can be listened to by components
        // This will trigger a refresh of conversations for admins
        window.dispatchEvent(new CustomEvent('newTicketCreated', { detail: ticketData }));
      });

      this.connection.on('UserTyping', (data) => {
        this.typing.userId = data.userId;
        this.typing.userName = data.userName;
        this.typing.isTyping = data.isTyping;
      });

      this.connection.on('Error', (error) => {
        console.error('Chat Hub Error:', error);
      });

      await this.connection.start();
      console.log('ChatService connected');
    } catch (error) {
      console.error('Failed to connect to chat hub:', error);
    }
  }

  /**
   * Disconnect from SignalR hub
   */
  async disconnect() {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
    }
  }

  /**
   * Fetch all conversations for the current user (via REST API)
   */
  async fetchConversations() {
    try {
      const response = await apiService.get('/chat/conversations');
      this.conversations.list = response.data.map((conv) => ({
        id: conv.id,
        subject: conv.subject,
        userName: conv.clientName,
        userEmail: conv.clientEmail,
        status: conv.status,
        priority: conv.priority,
        createdAt: conv.createdAt,
        updatedAt: conv.updatedAt,
        lastMessageAt: conv.lastMessageAt,
        messages: conv.messages || []
      }));
    } catch (error) {
      console.error('Failed to fetch conversations:', error);
    }
  }

  /**
   * Fetch chat history for a specific conversation
   */
  async fetchHistory(conversationId) {
    this.messages.loading = true;
    try {
      const response = await apiService.get(`/chat/conversations/${conversationId}/messages`);
      this.messages.current = response.data.map((msg) => ({
        id: msg.id,
        conversationId: msg.conversationId,
        senderId: msg.senderId,
        senderName: msg.senderName,
        content: msg.content,
        timestamp: msg.sentAt,
        isSupport: msg.senderRole === 'Admin'
      }));
      this.conversations.currentId = conversationId;
    } catch (error) {
      console.error('Failed to fetch chat history:', error);
      this.messages.current = [];
    } finally {
      this.messages.loading = false;
    }
  }

  /**
   * Send a message via REST API
   */
  async sendMessage(conversationId, content, subject = null) {
    if (!content.trim()) return;

    try {
      const command = {
        conversationId: conversationId || null,
        subject: subject || null,
        content
      };

      const response = await apiService.post('/chat/messages', command);
      return response.data;
    } catch (error) {
      console.error('Failed to send message:', error);
      throw error;
    }
  }

  /**
   * Send message via SignalR (real-time)
   */
  async sendMessageRealtime(conversationId, content) {
    if (!this.connection || this.connection.state !== 'Connected') {
      console.warn('Chat not connected, falling back to REST API');
      return this.sendMessage(conversationId, content);
    }

    try {
      await this.connection.invoke('SendMessage', conversationId, content);
    } catch (error) {
      console.error('Failed to send message via SignalR:', error);
      // Fallback to REST API
      return this.sendMessage(conversationId, content);
    }
  }

  /**
   * Join a conversation group (for targeted messaging)
   */
  async joinConversation(conversationId) {
    if (!this.connection || this.connection.state !== 'Connected') return;

    try {
      await this.connection.invoke('JoinConversation', conversationId);
      await this.fetchHistory(conversationId);
    } catch (error) {
      console.error('Failed to join conversation:', error);
    }
  }

  /**
   * Leave a conversation group
   */
  async leaveConversation(conversationId) {
    if (!this.connection || this.connection.state !== 'Connected') return;

    try {
      await this.connection.invoke('LeaveConversation', conversationId);
    } catch (error) {
      console.error('Failed to leave conversation:', error);
    }
  }

  /**
   * Notify typing status to other users
   */
  async notifyTyping(conversationId, isTyping) {
    if (!this.connection || this.connection.state !== 'Connected') return;

    try {
      await this.connection.invoke('NotifyTyping', conversationId, isTyping);
    } catch (error) {
      console.error('Failed to notify typing:', error);
    }
  }

  /**
   * Select a conversation and fetch its history
   */
  async selectConversation(conversationId) {
    await this.joinConversation(conversationId);
    this.conversations.currentId = conversationId;
  }

  /**
   * Get current conversation object
   */
  getCurrentConversation() {
    if (!this.conversations.currentId) return null;
    return this.conversations.list.find((c) => c.id === this.conversations.currentId);
  }

  /**
   * Clear current messages
   */
  clearMessages() {
    this.messages.current = [];
  }

  /**
   * Notify admins of a new ticket via SignalR
   * Called after ticket creation on the client side
   */
  async notifyAdminsOfNewTicket(ticketData) {
    if (!this.connection || this.connection.state !== 'Connected') {
      console.warn('Chat not connected, skipping admin notification');
      return;
    }

    try {
      await this.connection.invoke('NotifyNewTicket', ticketData);
      console.log('Admin notification sent for new ticket');
    } catch (error) {
      console.error('Failed to notify admins of new ticket:', error);
      // Non-critical error, don't throw
    }
  }
}

export const chatService = new ChatService();

/**
 * Composable for using chat service in Vue components
 */
export function useChat() {
  const isConnected = ref(false);

  const connect = async () => {
    await chatService.connect();
    isConnected.value = true;
  };

  const disconnect = async () => {
    await chatService.disconnect();
    isConnected.value = false;
  };

  return {
    isConnected,
    connect,
    disconnect,
    chatService,
    conversations: chatService.conversations,
    messages: chatService.messages,
    typing: chatService.typing
  };
}
