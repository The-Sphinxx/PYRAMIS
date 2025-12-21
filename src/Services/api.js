import axios from 'axios';

// إنشاء instance من axios
export const api = axios.create({
    baseURL: 'http://localhost:3000',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Interceptor للـ requests - إضافة token
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Interceptor للـ responses - معالجة الأخطاء
api.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        if (error.response?.status === 401) {
            // إزالة token منتهي الصلاحية
            localStorage.removeItem('token');
            localStorage.removeItem('user');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

// دوال مساعدة للـ Authentication
export const authApi = {
    // تسجيل دخول
    login: async (email, password) => {
        try {
            // البحث عن المستخدم
            const usersResponse = await api.get('/users', {
                params: { email }
            });

            const user = usersResponse.data[0];

            if (!user) {
                return {
                    success: false,
                    message: 'Email or Password is incorrect'
                };
            }

            // التحقق من كلمة المرور (في الإنتاج استخدم bcrypt)
            if (user.password !== password) {
                return {
                    success: false,
                    message: 'Email or Password is incorrect'
                };
            }

            // إنشاء token بسيط (في الإنتاج استخدم JWT)
            const token = btoa(`${user.id}:${Date.now()}`);

            // إرجاع بيانات المستخدم بدون كلمة المرور
            const { password: _, ...userWithoutPassword } = user;

            return {
                success: true,
                token,
                user: userWithoutPassword
            };
        } catch (error) {
            return {
                success: false,
                message: 'Error occurred during login'
            };
        }
    },

    // إنشاء حساب جديد
    signup: async (userData) => {
        try {
            // التحقق من وجود المستخدم
            const existingUsers = await api.get('/users', {
                params: { email: userData.email }
            });

            if (existingUsers.data.length > 0) {
                return {
                    success: false,
                    message: 'Email is already registered'
                };
            }

            // إنشاء المستخدم الجديد
            const newUser = {
                ...userData,
                id: Date.now().toString(),
                isVerified: false,
                resetToken: null,
                resetTokenExpiry: null,
                createdAt: new Date().toISOString(),
                updatedAt: new Date().toISOString()
            };

            const response = await api.post('/users', newUser);

            // إنشاء token
            const token = btoa(`${response.data.id}:${Date.now()}`);

            // إرجاع بيانات المستخدم بدون كلمة المرور
            const { password: _, ...userWithoutPassword } = response.data;

            return {
                success: true,
                token,
                user: userWithoutPassword
            };
        } catch (error) {
            return {
                success: false,
                message: 'Error occurred during signup'
            };
        }
    },

    // نسيت كلمة المرور
    forgotPassword: async (email) => {
        try {
            const usersResponse = await api.get('/users', {
                params: { email }
            });

            const user = usersResponse.data[0];

            if (!user) {
                return {
                    success: false,
                    message: 'Email is not registered'
                };
            }

            // إنشاء رمز التحقق
            const resetToken = Math.floor(100000 + Math.random() * 900000).toString();
            const resetTokenExpiry = new Date(Date.now() + 3600000).toISOString(); // ساعة واحدة

            // تحديث المستخدم
            await api.patch(`/users/${user.id}`, {
                resetToken,
                resetTokenExpiry,
                updatedAt: new Date().toISOString()
            });

            // في الإنتاج، أرسل الرمز عبر البريد الإلكتروني
            console.log('Reset Token:', resetToken);

            return {
                success: true,
                message: 'Reset Token sent to your email',
                resetToken // في الإنتاج لا ترجع الرمز
            };
        } catch (error) {
            return {
                success: false,
                message: 'Error occurred during forgot password'
            };
        }
    },

    // إعادة تعيين كلمة المرور
    resetPassword: async (email, resetToken, newPassword) => {
        try {
            const usersResponse = await api.get('/users', {
                params: { email }
            });

            const user = usersResponse.data[0];

            if (!user) {
                return {
                    success: false,
                    message: 'Email is not registered'
                };
            }

            // التحقق من الرمز وصلاحيته
            if (user.resetToken !== resetToken) {
                return {
                    success: false,
                    message: 'Reset Token is incorrect'
                };
            }

            if (new Date(user.resetTokenExpiry) < new Date()) {
                return {
                    success: false,
                    message: 'Reset Token has expired'
                };
            }

            // تحديث كلمة المرور
            await api.patch(`/users/${user.id}`, {
                password: newPassword,
                resetToken: null,
                resetTokenExpiry: null,
                updatedAt: new Date().toISOString()
            });

            return {
                success: true,
                message: 'Password reset successfully'
            };
        } catch (error) {
            return {
                success: false,
                message: 'Error occurred during reset password'
            };
        }
    }
};

export default api;