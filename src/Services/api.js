import axios from 'axios';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5137';
const authPrefix = '/api/Auth';

export const api = axios.create({
  baseURL: apiBaseUrl,
  timeout: 15000,
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => Promise.reject(error)
);

api.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config || {};
    const isUnauthorized = error.response?.status === 401;

    if (isUnauthorized && !originalRequest._retry) {
      originalRequest._retry = true;
      const refreshToken = localStorage.getItem('refreshToken');
      const token = localStorage.getItem('token');
      if (!refreshToken || !token) {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        window.location.href = '/auth/login';
        return Promise.reject(error);
      }

      try {
        const { data } = await axios.post(
          `${apiBaseUrl}${authPrefix}/refresh-token`,
          { token, refreshToken },
          { headers: { 'Content-Type': 'application/json' } }
        );

        localStorage.setItem('token', data.token);
        localStorage.setItem('refreshToken', data.refreshToken);
        api.defaults.headers.common.Authorization = `Bearer ${data.token}`;
        originalRequest.headers.Authorization = `Bearer ${data.token}`;
        return api(originalRequest);
      } catch (refreshError) {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        window.location.href = '/auth/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export const authApi = {
  login: async (email, password) => {
    const { data } = await api.post(`${authPrefix}/login`, { email, password });
    return data;
  },

  register: async payload => {
    const { data } = await api.post(`${authPrefix}/register`, payload);
    return data;
  },

  googleLogin: async idToken => {
    const { data } = await api.post(`${authPrefix}/google-login`, { idToken });
    return data;
  },

  forgotPassword: async email => {
    const { data } = await api.post(`${authPrefix}/forgot-password`, { email });
    return data;
  },

  resetPassword: async (email, token, newPassword) => {
    const { data } = await api.post(`${authPrefix}/reset-password`, {
      email,
      token,
      newPassword,
    });
    return data;
  },

  verifyEmail: async (email, code) => {
    const { data } = await api.post(`${authPrefix}/verify-email`, { email, code });
    return data;
  },

  verifyEmailByLink: async (email, token) => {
    const { data } = await api.get(`${authPrefix}/verify-email`, { params: { email, token } });
    return data;
  },

  resendOtp: async email => {
    const { data } = await api.post(`${authPrefix}/resend-otp`, { email });
    return data;
  },
};

export default api;