import axios from 'axios';

const base = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:5137').replace(/\/+$/, '');
const apiBaseUrl = `${base}/api`;

const apiService = axios.create({
  baseURL: apiBaseUrl,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json',
  },
});

apiService.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

apiService.interceptors.response.use(
  (response) => response,
  (error) => Promise.reject(error)
);

export default apiService;
