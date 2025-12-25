import { defineStore } from 'pinia';
import apiService from '@/Services/apiService';

const mapConversation = (conversation) => ({
  id: conversation.id,
  subject: conversation.subject,
  status: conversation.status || 'Open',
  priority: conversation.priority || 'Medium',
  userName: conversation.clientName || 'Client',
  userEmail: conversation.clientEmail,
  userId: conversation.clientId,
  createdAt: conversation.createdAt || conversation.lastMessageAt || new Date().toISOString(),
  updatedAt: conversation.lastMessageAt || conversation.updatedAt || new Date().toISOString(),
  messages: conversation.messages || []
});

const mapMessage = (message) => ({
  id: message.id,
  conversationId: message.conversationId || message.conversationID || null,
  senderId: message.senderId,
  senderName: message.senderName,
  senderRole: message.senderRole,
  content: message.content,
  timestamp: message.sentAt || new Date().toISOString(),
});

export const useSupportStore = defineStore('support', {
  state: () => ({
    tickets: [],
    currentTicket: null,
    loading: false,
    error: null,
  }),

  getters: {
    getTicketById: (state) => (id) => state.tickets.find((t) => t.id === id),
    
    filteredTickets: (state) => (searchQuery, filters) => {
      let result = [...state.tickets];

      if (searchQuery) {
        const query = searchQuery.toLowerCase();
        result = result.filter(ticket => 
          String(ticket.id).toLowerCase().includes(query) ||
          ticket.subject.toLowerCase().includes(query) ||
          (ticket.userName && ticket.userName.toLowerCase().includes(query)) ||
          (ticket.userEmail && ticket.userEmail.toLowerCase().includes(query))
        );
      }

      if (filters.status && filters.status !== 'All') {
        result = result.filter(ticket => ticket.status === filters.status);
      }

      if (filters.priority && filters.priority !== 'All') {
        result = result.filter(ticket => ticket.priority === filters.priority);
      }

      if (filters.date) {
        const filterDateString = new Date(filters.date).toDateString();
        result = result.filter(ticket => {
          const ticketDate = new Date(ticket.createdAt).toDateString();
          return ticketDate === filterDateString;
        });
      }

      result.sort((a, b) => new Date(b.updatedAt) - new Date(a.updatedAt));
      return result;
    },

    getUserTickets: (state) => (userId) => {
      return state.tickets.filter(t => t.userId === userId).sort((a, b) => new Date(b.updatedAt) - new Date(a.updatedAt));
    }
  },

  actions: {
    async fetchTickets() {
      this.loading = true;
      try {
        const response = await apiService.get('/chat/conversations');
        this.tickets = (response.data || []).map(mapConversation);

        if (this.currentTicket) {
          this.currentTicket = this.tickets.find((t) => t.id === this.currentTicket.id) || null;
        }
      } catch (err) {
        this.error = err.message;
        console.error('Error fetching tickets:', err);
      } finally {
        this.loading = false;
      }
    },

    async fetchTicketById(id) {
      this.loading = true;
      try {
        const cached = this.tickets.find((t) => t.id === id);
        if (cached) {
          this.currentTicket = cached;
          return cached;
        }

        const response = await apiService.get('/chat/conversations');
        const conversations = (response.data || []).map(mapConversation);
        this.tickets = conversations;

        const found = conversations.find((t) => t.id === id) || null;
        this.currentTicket = found;
        return found;
      } catch (err) {
        this.error = err.message;
        console.error('Error fetching ticket:', err);
      } finally {
        this.loading = false;
      }
    },

    async createTicket(ticketData) {
      this.loading = true;
      try {
        // Use apiService which has auth interceptor configured
        const response = await apiService.post(
          '/chat/tickets',
          {
            subject: ticketData.subject,
            priority: ticketData.priority,
            initialMessage: ticketData.initialMessage
          }
        );

        const conversationData = response.data.data || response.data;
        const newConversation = mapConversation({
          ...conversationData,
          createdAt: conversationData.createdAt || new Date().toISOString(),
          updatedAt: conversationData.updatedAt || conversationData.lastMessageAt || new Date().toISOString(),
          clientName: conversationData.clientName || 'You'
        });

        this.tickets.unshift(newConversation);
        this.currentTicket = newConversation;
        return newConversation;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        console.error('Error creating ticket:', err);
        throw err;
      } finally {
        this.loading = false;
      }
    },

    async sendMessage(ticketId, message) {
      try {
        const ticket = this.tickets.find((t) => t.id === ticketId);
        if (!ticket) throw new Error('Ticket not found');

        const response = await apiService.post('/chat/messages', {
          conversationId: ticketId,
          content: message.content,
          subject: message.subject || ticket.subject || null,
        });

        const savedMessage = mapMessage(response.data || {});
        const updatedTicket = {
          ...ticket,
          messages: [...(ticket.messages || []), savedMessage],
          updatedAt: savedMessage.timestamp,
        };

        const index = this.tickets.findIndex((t) => t.id === ticketId);
        if (index !== -1) {
          this.tickets[index] = updatedTicket;
        }
        if (this.currentTicket && this.currentTicket.id === ticketId) {
          this.currentTicket = updatedTicket;
        }

        return savedMessage;
      } catch (err) {
        console.error('Error sending message:', err);
        throw err;
      }
    },

    async updateTicketStatus(ticketId, status) {
      try {
        const ticket = this.tickets.find((t) => t.id === ticketId);
        if (!ticket) throw new Error('Ticket not found');

        const updatedTicket = {
          ...ticket,
          status,
          updatedAt: new Date().toISOString()
        };

        const index = this.tickets.findIndex((t) => t.id === ticketId);
        if (index !== -1) {
          this.tickets[index] = updatedTicket;
        }
        if (this.currentTicket && this.currentTicket.id === ticketId) {
          this.currentTicket = updatedTicket;
        }
      } catch (err) {
        console.error('Error updating status:', err);
        throw err;
      }
    },

    async assignTicket(ticketId, adminId) {
      try {
        const ticket = this.tickets.find((t) => t.id === ticketId);
        if (!ticket) throw new Error('Ticket not found');

        const updatedTicket = { ...ticket, assignedTo: adminId, updatedAt: new Date().toISOString() };

        const index = this.tickets.findIndex((t) => t.id === ticketId);
        if (index !== -1) {
          this.tickets[index] = updatedTicket;
        }
        if (this.currentTicket && this.currentTicket.id === ticketId) {
          this.currentTicket = updatedTicket;
        }
      } catch (err) {
        console.error('Error assigning ticket:', err);
        throw err;
      }
    }
  }
});
