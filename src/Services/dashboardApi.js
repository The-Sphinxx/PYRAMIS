// Services/dashboardApi.js
import { api } from './api.js';

// ==================== Attractions API ====================
export const attractionsAPI = {
  getAll: (params = {}) => api.get('/attractions', { params: { pageSize: 10, ...params } }),
  getOne: (id) => api.get(`/attractions/${id}`),
  create: (data) => api.post('/attractions', data),
  update: (id, data) => api.put(`/attractions/${id}`, data),
  patch: (id, data) => api.patch(`/attractions/${id}`, data),
  delete: (id) => api.delete(`/attractions/${id}`),

  // Specific updates
  updateStatus: (id, status) => api.patch(`/attractions/${id}`, { status }),
  updateAvailability: (id, availability) => api.patch(`/attractions/${id}`, { availability }),
  toggleFeatured: (id, isFeatured) => api.patch(`/attractions/${id}`, { isFeatured }),
};

// ==================== Hotels API ====================
export const hotelsAPI = {
  getAll: (params = {}) => api.get('/hotels', { params: { pageSize: 10000, ...params } }),
  getOne: (id) => api.get(`/hotels/${id}`),
  create: (data) => api.post('/hotels', data),
  update: (id, data) => api.put(`/hotels/${id}`, data),
  patch: (id, data) => api.patch(`/hotels/${id}`, data),
  delete: (id) => api.delete(`/hotels/${id}`),

  // Specific updates
  updateStatus: (id, status) => api.patch(`/hotels/${id}`, { status }),
  toggleFeatured: (id, isFeatured) => api.patch(`/hotels/${id}`, { featured: isFeatured }),
};

// ==================== Cars API ====================
export const carsAPI = {
  getAll: () => api.get('/cars', { params: { pageSize: 10000 } }),
  getOne: (id) => api.get(`/cars/${id}`),
  create: (data) => api.post('/cars', data),
  update: (id, data) => api.put(`/cars/${id}`, data),
  patch: (id, data) => api.patch(`/cars/${id}`, data),
  delete: (id) => api.delete(`/cars/${id}`),

  // Specific updates
  updateStatus: (id, status) => api.patch(`/cars/${id}`, { status }),
  toggleFeatured: (id, isFeatured) => api.patch(`/cars/${id}`, { isFeatured }),
};

// ==================== Trips API ====================
export const tripsAPI = {
  getAll: (params = {}) => api.get('/trips', { params: { pageSize: 10, ...params } }),
  getOne: (id) => api.get(`/trips/${id}`),
  create: (data) => api.post('/trips', data),
  update: (id, data) => api.put(`/trips/${id}`, data),
  patch: (id, data) => api.patch(`/trips/${id}`, data),
  delete: (id) => api.delete(`/trips/${id}`),

  // Specific updates
  updateStatus: (id, status) => api.patch(`/trips/${id}`, { status }),
  toggleFeatured: (id, isFeatured) => api.patch(`/trips/${id}`, { isFeatured }),
};

// ==================== Bookings API ====================
export const bookingsAPI = {
  getAll: () => api.get('/bookings', { params: { pageSize: 10000 } }),
  getOne: (id) => api.get(`/bookings/${id}`),
  create: (data) => api.post('/bookings', data),
  update: (id, data) => api.put(`/bookings/${id}`, data),
  patch: (id, data) => api.patch(`/bookings/${id}`, data),
  delete: (id) => api.delete(`/bookings/${id}`),

  // Specific updates
  updateStatus: (id, status) => api.patch(`/bookings/${id}`, { status }),
  updatePaymentStatus: (id, paymentStatus) => api.patch(`/bookings/${id}`, { paymentStatus }),
};

// ==================== Users API ====================
export const usersAPI = {
  getAll: () => api.get('/admin/users', { params: { pageSize: 10000 } }),
  getOne: (id) => api.get(`/admin/users/${id}`),
  create: (data) => api.post('/admin/users', data),
  update: (id, data) => api.put(`/admin/users/${id}`, data),
  patch: (id, data) => api.patch(`/admin/users/${id}`, data),
  delete: (id) => api.delete(`/admin/users/${id}`),

  // Specific updates
  updateStatus: (id, status) => api.patch(`/admin/users/${id}`, { status }),
  // Add other specific user updates here if needed (e.g., role, verification)
};

// ==================== Generic API Helper ====================
export const apiService = {
  get: (resource, params) => api.get(`/${resource}`, { params }),
  getOne: (resource, id) => api.get(`/${resource}/${id}`),
  create: (resource, data) => api.post(`/${resource}`, data),
  update: (resource, id, data) => api.put(`/${resource}/${id}`, data),
  patch: (resource, id, data) => api.patch(`/${resource}/${id}`, data),
  delete: (resource, id) => api.delete(`/${resource}/${id}`),
};