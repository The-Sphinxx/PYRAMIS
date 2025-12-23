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

  // Update Profile
  const updateProfile = async (updatedData) => {
    try {
      if (!user.value || !user.value.id) {
        throw new Error('No user logged in');
      }

      const response = await fetch(`http://localhost:3000/users/${user.value.id}`, {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(updatedData),
      });

      if (response.ok) {
        const updatedUser = await response.json();
        
        // Remove password before storing
        const userWithoutPassword = { ...updatedUser };
        delete userWithoutPassword.password;

        user.value = userWithoutPassword;
        
        // Update localStorage
        localStorage.setItem('user', JSON.stringify(userWithoutPassword));
        
        return { success: true };
      } else {
        return { success: false, error: 'Failed to update profile' };
      }
    } catch (error) {
      console.error('Update profile error:', error);
      return { success: false, error: error.message };
    }
  };

  // Change Password
  const changePassword = async (currentPassword, newPassword) => {
    try {
      if (!user.value || !user.value.id) {
        throw new Error('No user logged in');
      }

      // 1. Verify old password
      // Since we don't store the password in the state, we check against the server
      const verifyResponse = await fetch(
        `http://localhost:3000/users?id=${user.value.id}&password=${currentPassword}`
      );
      const verifiedUsers = await verifyResponse.json();

      if (verifiedUsers.length === 0) {
        return { success: false, error: 'Incorrect current password' };
      }

      // 2. Update to new password
      const updateResponse = await fetch(`http://localhost:3000/users/${user.value.id}`, {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ password: newPassword }),
      });

      if (updateResponse.ok) {
        return { success: true };
      } else {
        return { success: false, error: 'Failed to update password' };
      }
    } catch (error) {
      console.error('Change password error:', error);
      return { success: false, error: error.message };
    }
  };

  // Logout
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
    updateProfile,
    changePassword,
    logout
  };
});