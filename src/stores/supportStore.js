
import { defineStore } from 'pinia';
import axios from 'axios';

const API_URL = 'http://localhost:3000/tickets';

export const useSupportStore = defineStore('support', {
  state: () => ({
    tickets: [],
    currentTicket: null,
    loading: false,
    error: null,
  }),

  getters: {
    getTicketById: (state) => (id) => state.tickets.find((t) => t.id === id),
    
    // Filter tickets based on search query and filters
    filteredTickets: (state) => (searchQuery, filters) => {
      let result = [...state.tickets];

      // 1. Text Search (ID, Subject, UserName, UserEmail)
      if (searchQuery) {
        const query = searchQuery.toLowerCase();
        result = result.filter(ticket => 
          ticket.id.toLowerCase().includes(query) ||
          ticket.subject.toLowerCase().includes(query) ||
          (ticket.userName && ticket.userName.toLowerCase().includes(query)) ||
          (ticket.userEmail && ticket.userEmail.toLowerCase().includes(query))
        );
      }

      // 2. Status Filter
      if (filters.status && filters.status !== 'All') {
        result = result.filter(ticket => ticket.status === filters.status);
      }

      // 3. Priority Filter
      if (filters.priority && filters.priority !== 'All') {
        result = result.filter(ticket => ticket.priority === filters.priority);
      }

      // 4. Date Filter (CreatedAt)
      if (filters.date) {
        // Assuming filters.date is "YYYY-MM-DD" or similar
        // Or if it's a date range. For simple implementation, let's match the date part.
        const filterDateString = new Date(filters.date).toDateString();
        result = result.filter(ticket => {
          const ticketDate = new Date(ticket.createdAt).toDateString();
          return ticketDate === filterDateString;
        });
      }

      // Sort by newest Date
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
        const response = await axios.get(API_URL);
        this.tickets = response.data;
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
        const response = await axios.get(`${API_URL}/${id}`);
        this.currentTicket = response.data;
        return response.data;
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
        const newTicket = {
          ...ticketData,
          id: `TK-${Date.now().toString().slice(-4)}`, // Simple ID generation
          status: 'Open',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
          messages: ticketData.messages || []
        };
        const response = await axios.post(API_URL, newTicket);
        this.tickets.push(response.data);
        return response.data;
      } catch (err) {
        this.error = err.message;
        throw err;
      } finally {
        this.loading = false;
      }
    },

    async sendMessage(ticketId, message) {
      try {
        // First get the ticket to ensure we have latest messages
        const ticket = this.tickets.find(t => t.id === ticketId);
        if (!ticket) throw new Error('Ticket not found');

        const newMessage = {
          id: Date.now(),
          ...message,
          timestamp: new Date().toISOString()
        };

        const updatedMessages = [...ticket.messages, newMessage];
        const updatedTicket = { 
          ...ticket, 
          messages: updatedMessages,
          updatedAt: new Date().toISOString()
        };

        const response = await axios.put(`${API_URL}/${ticketId}`, updatedTicket);
        
        // Update local state
        const index = this.tickets.findIndex(t => t.id === ticketId);
        if (index !== -1) {
          this.tickets[index] = response.data;
        }
        if (this.currentTicket && this.currentTicket.id === ticketId) {
          this.currentTicket = response.data;
        }

        return response.data;
      } catch (err) {
        console.error('Error sending message:', err);
        throw err;
      }
    },

    async updateTicketStatus(ticketId, status) {
      try {
        const ticket = this.tickets.find(t => t.id === ticketId);
        if (!ticket) throw new Error('Ticket not found');

        const updatedTicket = { 
          ...ticket, 
          status,
          updatedAt: new Date().toISOString()
        };

        const response = await axios.put(`${API_URL}/${ticketId}`, updatedTicket);
        
        const index = this.tickets.findIndex(t => t.id === ticketId);
        if (index !== -1) {
          this.tickets[index] = response.data;
        }
        if (this.currentTicket && this.currentTicket.id === ticketId) {
          this.currentTicket = response.data;
        }
      } catch (err) {
        console.error('Error updating status:', err);
        throw err;
      }
    },

    async assignTicket(ticketId, adminId) {
      try {
        const ticket = this.tickets.find(t => t.id === ticketId);
        if (!ticket) throw new Error('Ticket not found');

        const updatedTicket = { ...ticket, assignedTo: adminId };
        const response = await axios.put(`${API_URL}/${ticketId}`, updatedTicket);

        const index = this.tickets.findIndex(t => t.id === ticketId);
        if (index !== -1) {
          this.tickets[index] = response.data;
        }
        if (this.currentTicket && this.currentTicket.id === ticketId) {
          this.currentTicket = response.data;
        }
      } catch (err) {
        console.error('Error assigning ticket:', err);
        throw err;
      }
    }
  }
});
