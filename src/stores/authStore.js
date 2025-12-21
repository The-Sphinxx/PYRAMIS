import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const user = ref(null);
  const token = ref(null);

  // Check if user is logged in
  const isAuthenticated = computed(() => !!user.value);

  // Initialize from localStorage
  const initAuth = () => {
    const savedUser = localStorage.getItem('user');
    const savedToken = localStorage.getItem('token');
    
    if (savedUser && savedToken) {
      user.value = JSON.parse(savedUser);
      token.value = savedToken;
    }
  };

  // Register new user
  const register = async (userData) => {
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
      const newUser = {
        id: Date.now().toString(),
        fullName: userData.fullName,
        email: userData.email,
        password: userData.password, // في الواقع لازم يتعمل hash
        createdAt: new Date().toISOString()
      };

      const response = await fetch('http://localhost:3000/users', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(newUser),
      });

      if (response.ok) {
        const createdUser = await response.json();
        
        // Remove password before storing
        const userWithoutPassword = { ...createdUser };
        delete userWithoutPassword.password;

        user.value = userWithoutPassword;
        token.value = `token_${createdUser.id}`;

        // Save to localStorage
        localStorage.setItem('user', JSON.stringify(userWithoutPassword));
        localStorage.setItem('token', token.value);

        return { success: true };
      } else {
        return {
          success: false,
          error: 'Registration failed'
        };
      }
    } catch (error) {
      console.error('Registration error:', error);
      return {
        success: false,
        error: 'Network error. Please try again.'
      };
    }
  };

  // Login user
  const login = async (credentials) => {
    try {
      const response = await fetch(
        `http://localhost:3000/users?email=${credentials.email}&password=${credentials.password}`
      );
      const users = await response.json();

      if (users.length > 0) {
        const foundUser = users[0];
        
        // Remove password before storing
        const userWithoutPassword = { ...foundUser };
        delete userWithoutPassword.password;

        user.value = userWithoutPassword;
        token.value = `token_${foundUser.id}`;

        // Save to localStorage if "Remember Me" is checked
        if (credentials.rememberMe) {
          localStorage.setItem('user', JSON.stringify(userWithoutPassword));
          localStorage.setItem('token', token.value);
        }

        return { success: true };
      } else {
        return {
          success: false,
          error: 'Invalid email or password'
        };
      }
    } catch (error) {
      console.error('Login error:', error);
      return {
        success: false,
        error: 'Network error. Please try again.'
      };
    }
  };

  // Google Sign In (Mock implementation)
  const loginWithGoogle = async () => {
    try {
      // هنا المفروض تستخدمي Firebase أو Google OAuth
      // دي mock implementation للتجربة
      const mockGoogleUser = {
        id: `google_${Date.now()}`,
        fullName: 'Google User',
        email: 'user@gmail.com',
        provider: 'google',
        createdAt: new Date().toISOString()
      };

      // Check if user exists
      const checkResponse = await fetch(`http://localhost:3000/users?email=${mockGoogleUser.email}`);
      const existingUsers = await checkResponse.json();

      let finalUser;

      if (existingUsers.length > 0) {
        finalUser = existingUsers[0];
      } else {
        // Create new user
        const response = await fetch('http://localhost:3000/users', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(mockGoogleUser),
        });
        finalUser = await response.json();
      }

      user.value = finalUser;
      token.value = `token_${finalUser.id}`;

      localStorage.setItem('user', JSON.stringify(finalUser));
      localStorage.setItem('token', token.value);

      return { success: true };
    } catch (error) {
      console.error('Google login error:', error);
      return {
        success: false,
        error: 'Google sign in failed'
      };
    }
  };

  // Logout
  const logout = () => {
    user.value = null;
    token.value = null;
    localStorage.removeItem('user');
    localStorage.removeItem('token');
  };

  return {
    user,
    token,
    isAuthenticated,
    initAuth,
    register,
    login,
    loginWithGoogle,
    logout
  };
});