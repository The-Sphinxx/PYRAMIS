import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { authApi, api } from '@/Services/api';
import { decodeJwt, buildUserFromToken } from '@/Utils/auth';

export const useAuthStore = defineStore('auth', () => {
  const user = ref(null);
  const token = ref(null);
  const refreshToken = ref(null);

  const isAuthenticated = computed(() => !!token.value);

  const setSession = async (authResponse, rememberMe) => {
    const shapedUser = authResponse?.role
      ? {
        id: authResponse.id,
        email: authResponse.email,
        firstName: authResponse.firstName,
        lastName: authResponse.lastName,
        role: authResponse.role || '',
      }
      : buildUserFromToken(authResponse.token);

    user.value = shapedUser;
    token.value = authResponse.token;
    refreshToken.value = authResponse.refreshToken;

    api.defaults.headers.common.Authorization = `Bearer ${authResponse.token}`;

    // Persist tokens for refresh handling; rememberMe still controls UX choices elsewhere if needed
    localStorage.setItem('token', authResponse.token);
    localStorage.setItem('refreshToken', authResponse.refreshToken);

    // Load wishlist after successful login
    try {
      const { useWishlistStore } = await import('./wishlistStore');
      const wishlistStore = useWishlistStore();
      await wishlistStore.loadWishlist();
    } catch (error) {
      console.error('Failed to load wishlist:', error);
    }
  };

  const clearSession = async () => {
    user.value = null;
    token.value = null;
    refreshToken.value = null;
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    delete api.defaults.headers.common.Authorization;

    // Clear wishlist on logout
    try {
      const { useWishlistStore } = await import('./wishlistStore');
      const wishlistStore = useWishlistStore();
      wishlistStore.clearWishlist();
    } catch (error) {
      console.error('Failed to clear wishlist:', error);
    }
  };

  const initAuth = async () => {
    const savedToken = localStorage.getItem('token');
    const savedRefresh = localStorage.getItem('refreshToken');

    if (savedToken && savedRefresh) {
      const decoded = decodeJwt(savedToken);
      const nowSeconds = Math.floor(Date.now() / 1000);
      const isExpired = decoded?.exp && decoded.exp < nowSeconds;

      if (isExpired) {
        await clearSession();
        return;
      }

      user.value = buildUserFromToken(savedToken);
      token.value = savedToken;
      refreshToken.value = savedRefresh;
      api.defaults.headers.common.Authorization = `Bearer ${savedToken}`;

      // Load wishlist after restoring session
      try {
        const { useWishlistStore } = await import('./wishlistStore');
        const wishlistStore = useWishlistStore();
        await wishlistStore.loadWishlist();
      } catch (error) {
        console.error('Failed to load wishlist:', error);
      }
    }
  };

  const register = async (payload) => {
    try {
      const name = (payload.fullName || '').trim();
      const parts = name.split(/\s+/);
      const firstName = parts[0] || '';
      const lastName = parts.length > 1 ? parts.slice(1).join(' ') : '';

      const registerPayload = {
        FirstName: firstName,
        LastName: lastName,
        Email: payload.email,
        Password: payload.password,
        Nationality: payload.nationality || '',
        Gender: payload.gender || '',
        DateOfBirth: payload.dateOfBirth || '2000-01-01T00:00:00.000Z',
        ProfileImage: payload.profileImage || undefined,
      };

      await authApi.register(registerPayload);
      return { success: true };
    } catch (error) {
      const message = error.response?.data?.message || 'Registration failed';
      return { success: false, error: message };
    }
  };

  const login = async (credentials) => {
    try {
      const authResponse = await authApi.login(credentials.email, credentials.password);
      await setSession(authResponse, credentials.rememberMe);
      return { success: true };
    } catch (error) {
      const message = error.response?.data?.message || 'Invalid email or password';
      return { success: false, error: message };
    }
  };

  const loginWithGoogle = async (idToken) => {
    try {
      const authResponse = await authApi.googleLogin(idToken);
      await setSession(authResponse, true);
      return { success: true };
    } catch (error) {
      const message = error.response?.data?.message || 'Google sign-in failed';
      return { success: false, error: message };
    }
  };

  // Update Profile
  const updateProfile = async (updatedData) => {
    try {
      if (!user.value || !user.value.id) {
        throw new Error('No user logged in');
      }

      // Use the backend API to update profile
      const payload = {
        firstName: updatedData.firstName || '',
        lastName: updatedData.lastName || '',
        phone: updatedData.phone || '',
        nationality: updatedData.nationality || '',
        gender: updatedData.gender || '',
        profileImage: updatedData.profileImage || ''
      };

      // Only include dateOfBirth if it's provided
      if (updatedData.dateOfBirth) {
        payload.dateOfBirth = updatedData.dateOfBirth;
      }

      console.log('Sending profile update:', payload);

      await api.put('/Profile', payload);

      // Refresh user data from profile endpoint to get updated info
      const profileResponse = await api.get('/Profile');

      // Update local user state with new data
      user.value = buildUserFromToken(token.value);

      return { success: true, data: profileResponse.data };
    } catch (error) {
      console.error('Update profile error:', error);
      const message = error.response?.data?.message || error.message || 'Failed to update profile';
      return { success: false, error: message };
    }
  };

  // Change Password
  const changePassword = async (currentPassword, newPassword) => {
    try {
      if (!user.value || !user.value.email) {
        throw new Error('No user logged in');
      }

      // Use the backend /api/Auth/change-password endpoint
      await api.post('/Auth/change-password', {
        email: user.value.email,
        currentPassword: currentPassword,
        newPassword: newPassword
      });

      return { success: true };
    } catch (error) {
      console.error('Change password error:', error);
      const message = error.response?.data?.message || error.message || 'Failed to change password';
      return { success: false, error: message };
    }
  };

  // Logout
  const logout = async () => {
    await clearSession();
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
    updateProfile,
    changePassword,
    logout
  };
});