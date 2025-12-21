import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { authApi, api } from '@/Services/api';

export const useAuthStore = defineStore('auth', () => {
  const user = ref(null);
  const token = ref(null);
  const refreshToken = ref(null);

  const isAuthenticated = computed(() => !!token.value);

  const setSession = (authResponse, rememberMe) => {
    const shapedUser = {
      id: authResponse.id,
      email: authResponse.email,
      firstName: authResponse.firstName,
      lastName: authResponse.lastName,
    };

    user.value = shapedUser;
    token.value = authResponse.token;
    refreshToken.value = authResponse.refreshToken;

    api.defaults.headers.common.Authorization = `Bearer ${authResponse.token}`;

    // Persist tokens for refresh handling; rememberMe still controls UX choices elsewhere if needed
    localStorage.setItem('user', JSON.stringify(shapedUser));
    localStorage.setItem('token', authResponse.token);
    localStorage.setItem('refreshToken', authResponse.refreshToken);
  };

  const clearSession = () => {
    user.value = null;
    token.value = null;
    refreshToken.value = null;
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    delete api.defaults.headers.common.Authorization;
  };

  const initAuth = () => {
    const savedUser = localStorage.getItem('user');
    const savedToken = localStorage.getItem('token');
    const savedRefresh = localStorage.getItem('refreshToken');

    if (savedUser && savedToken && savedRefresh) {
      user.value = JSON.parse(savedUser);
      token.value = savedToken;
      refreshToken.value = savedRefresh;
      api.defaults.headers.common.Authorization = `Bearer ${savedToken}`;
    }
  };

  const register = async (payload) => {
    try {
      const fullName = payload.fullName?.trim() || '';
      const [firstName, ...rest] = fullName.split(' ');
      const lastName = rest.join(' ') || firstName;

      const authResponse = await authApi.register({
        firstName: firstName || payload.firstName || '',
        lastName: lastName || payload.lastName || '',
        email: payload.email,
        password: payload.password,
        gender: payload.gender || 'Unspecified',
        nationality: payload.nationality || 'N/A',
        dateOfBirth: payload.dateOfBirth || new Date('1990-01-01').toISOString(),
        profileImage: payload.profileImage || null,
      });

      setSession(authResponse, true);
      return { success: true };
    } catch (error) {
      const message = error.response?.data?.message || 'Registration failed';
      return { success: false, error: message };
    }
  };

  const login = async (credentials) => {
    try {
      const authResponse = await authApi.login(credentials.email, credentials.password);
      setSession(authResponse, credentials.rememberMe);
      return { success: true };
    } catch (error) {
      const message = error.response?.data?.message || 'Invalid email or password';
      return { success: false, error: message };
    }
  };

  const loginWithGoogle = async (idToken) => {
    try {
      const authResponse = await authApi.googleLogin(idToken);
      setSession(authResponse, true);
      return { success: true };
    } catch (error) {
      const message = error.response?.data?.message || 'Google sign-in failed';
      return { success: false, error: message };
    }
  };

  const logout = () => {
    clearSession();
  };

  return {
    user,
    token,
    refreshToken,
    isAuthenticated,
    initAuth,
    register,
    login,
    loginWithGoogle,
    logout,
  };
});