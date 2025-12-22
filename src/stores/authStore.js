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
      // Check if user already exists
      const checkResponse = await fetch(`http://localhost:3000/users?email=${userData.email}`);
      const existingUsers = await checkResponse.json();

      if (existingUsers.length > 0) {
        return {
          success: false,
          error: 'Email already exists'
        };
      }

      // Create new user
      // Helper for default images
      const defaultImages = [
        '/images/users/user_m_1.jpg',
        '/images/users/user_f_2.jpg',
        '/images/users/user_m_3.jpg',
        '/images/users/user_f_4.jpg'
      ];
      const randomImage = defaultImages[Math.floor(Math.random() * defaultImages.length)];

      const nameParts = userData.fullName.trim().split(' ');
      const firstName = nameParts[0];
      const lastName = nameParts.length > 1 ? nameParts.slice(1).join(' ') : '';

      // Create new user with complete profile data
      const newUser = {
        id: Date.now().toString(),
        fullName: userData.fullName,
        firstName: firstName,
        lastName: lastName,
        email: userData.email,
        password: userData.password, 
        phone: '',
        dateOfBirth: null,
        nationality: '',
        gender: '',
        profileImage: randomImage,
        isVerified: false,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      };

      const response = await fetch('http://localhost:3000/users', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(newUser),
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
    logout
  };
});