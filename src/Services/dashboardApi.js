// Services/dashboardApi.js
import axios from 'axios';

// Create a dedicated axios instance for json-server (db.json)
// This avoids conflicts with the main api.js which might point to an external Auth backend
export const jsonServer = axios.create({
  baseURL: 'http://localhost:3000',
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' }
});

// ==================== Attractions API ====================
export const attractionsAPI = {
  getAll: () => jsonServer.get('/attractions'),
  getOne: (id) => jsonServer.get(`/attractions/${id}`),
  create: (data) => jsonServer.post('/attractions', data),
  update: (id, data) => jsonServer.put(`/attractions/${id}`, data),
  patch: (id, data) => jsonServer.patch(`/attractions/${id}`, data),
  delete: (id) => jsonServer.delete(`/attractions/${id}`),
  
  // Specific updates
  updateStatus: (id, status) => jsonServer.patch(`/attractions/${id}`, { status }),
  updateAvailability: (id, availability) => jsonServer.patch(`/attractions/${id}`, { availability }),
  toggleFeatured: (id, isFeatured) => jsonServer.patch(`/attractions/${id}`, { isFeatured }),
};

// ==================== Hotels API ====================
export const hotelsAPI = {
  getAll: () => jsonServer.get('/hotels'),
  getOne: (id) => jsonServer.get(`/hotels/${id}`),
  create: (data) => jsonServer.post('/hotels', data),
  update: (id, data) => jsonServer.put(`/hotels/${id}`, data),
  patch: (id, data) => jsonServer.patch(`/hotels/${id}`, data),
  delete: (id) => jsonServer.delete(`/hotels/${id}`),
  
  // Specific updates
  updateStatus: (id, status) => jsonServer.patch(`/hotels/${id}`, { status }),
  toggleFeatured: (id, isFeatured) => jsonServer.patch(`/hotels/${id}`, { isFeatured }),
};

// ==================== Cars API ====================
export const carsAPI = {
  getAll: () => jsonServer.get('/cars'),
  getOne: (id) => jsonServer.get(`/cars/${id}`),
  create: (data) => jsonServer.post('/cars', data),
  update: (id, data) => jsonServer.put(`/cars/${id}`, data),
  patch: (id, data) => jsonServer.patch(`/cars/${id}`, data),
  delete: (id) => jsonServer.delete(`/cars/${id}`),
  
  // Specific updates
  updateStatus: (id, status) => jsonServer.patch(`/cars/${id}`, { status }),
  toggleFeatured: (id, isFeatured) => jsonServer.patch(`/cars/${id}`, { isFeatured }),
};

// ==================== Trips API ====================
export const tripsAPI = {
  getAll: () => jsonServer.get('/trips'),
  getOne: (id) => jsonServer.get(`/trips/${id}`),
  create: (data) => jsonServer.post('/trips', data),
  update: (id, data) => jsonServer.put(`/trips/${id}`, data),
  patch: (id, data) => jsonServer.patch(`/trips/${id}`, data),
  delete: (id) => jsonServer.delete(`/trips/${id}`),
  
  // Specific updates
  updateStatus: (id, status) => jsonServer.patch(`/trips/${id}`, { status }),
  toggleFeatured: (id, isFeatured) => jsonServer.patch(`/trips/${id}`, { isFeatured }),
};

// ==================== Bookings API ====================
export const bookingsAPI = {
  getAll: () => jsonServer.get('/bookings'),
  getOne: (id) => jsonServer.get(`/bookings/${id}`),
  create: (data) => jsonServer.post('/bookings', data),
  update: (id, data) => jsonServer.put(`/bookings/${id}`, data),
  patch: (id, data) => jsonServer.patch(`/bookings/${id}`, data),
  delete: (id) => jsonServer.delete(`/bookings/${id}`),
  
  // Specific updates
  updateStatus: (id, status) => jsonServer.patch(`/bookings/${id}`, { status }),
  updatePaymentStatus: (id, paymentStatus) => jsonServer.patch(`/bookings/${id}`, { paymentStatus }),
};

// ==================== Users API ====================
export const usersAPI = {
  getAll: () => jsonServer.get('/users'),
  getOne: (id) => jsonServer.get(`/users/${id}`),
  create: (data) => jsonServer.post('/users', data),
  update: (id, data) => jsonServer.put(`/users/${id}`, data),
  patch: (id, data) => jsonServer.patch(`/users/${id}`, data),
  delete: (id) => jsonServer.delete(`/users/${id}`),

  // Specific updates
  updateStatus: (id, status) => jsonServer.patch(`/users/${id}`, { status }),
  // Add other specific user updates here if needed (e.g., role, verification)
};

// ==================== Generic API Helper ====================
export const apiService = {
  get: (resource, params) => jsonServer.get(`/${resource}`, { params }),
  getOne: (resource, id) => jsonServer.get(`/${resource}/${id}`),
  create: (resource, data) => jsonServer.post(`/${resource}`, data),
  update: (resource, id, data) => jsonServer.put(`/${resource}/${id}`, data),
  patch: (resource, id, data) => jsonServer.patch(`/${resource}/${id}`, data),
  delete: (resource, id) => jsonServer.delete(`/${resource}/${id}`),
};