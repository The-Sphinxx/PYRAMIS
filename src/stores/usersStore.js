import { defineStore } from 'pinia';
import { ref } from 'vue';
import { api } from '@/Services/api';

export const useUsersStore = defineStore('users', () => {
    // State
    const users = ref([]);
    const isLoading = ref(false);
    const error = ref(null);

    // Actions

    // جلب جميع المستخدمين
    const fetchUsers = async () => {
        isLoading.value = true;
        error.value = null;

        try {
            const response = await api.get('/users');
            users.value = response.data;
            return { success: true, data: response.data };
        } catch (err) {
            error.value = err.response?.data?.message || 'Failed to fetch users';
            return { success: false, message: error.value };
        } finally {
            isLoading.value = false;
        }
    };

    // جلب مستخدم محدد
    const fetchUserById = async (userId) => {
        isLoading.value = true;
        error.value = null;

        try {
            const response = await api.get(`/users/${userId}`);
            return { success: true, data: response.data };
        } catch (err) {
            error.value = err.response?.data?.message || 'Failed to fetch user data';
            return { success: false, message: error.value };
        } finally {
            isLoading.value = false;
        }
    };

    // تحديث بيانات مستخدم
    const updateUser = async (userId, updates) => {
        isLoading.value = true;
        error.value = null;

        try {
            const response = await api.patch(`/users/${userId}`, {
                ...updates,
                updatedAt: new Date().toISOString()
            });

            // تحديث المستخدم في القائمة
            const index = users.value.findIndex(u => u.id === userId);
            if (index !== -1) {
                users.value[index] = response.data;
            }

            return { success: true, data: response.data };
        } catch (err) {
            error.value = err.response?.data?.message || 'Failed to update user';
            return { success: false, message: error.value };
        } finally {
            isLoading.value = false;
        }
    };

    // حذف مستخدم
    const deleteUser = async (userId) => {
        isLoading.value = true;
        error.value = null;

        try {
            await api.delete(`/users/${userId}`);

            // إزالة المستخدم من القائمة
            users.value = users.value.filter(u => u.id !== userId);

            return { success: true };
        } catch (err) {
            error.value = err.response?.data?.message || 'Failed to delete user';
            return { success: false, message: error.value };
        } finally {
            isLoading.value = false;
        }
    };

    // البحث عن مستخدمين
    const searchUsers = async (query) => {
        isLoading.value = true;
        error.value = null;

        try {
            const response = await api.get('/users', {
                params: { q: query }
            });
            return { success: true, data: response.data };
        } catch (err) {
            error.value = err.response?.data?.message || 'Failed to search users';
            return { success: false, message: error.value };
        } finally {
            isLoading.value = false;
        }
    };

    // التحقق من وجود بريد إلكتروني
    const checkEmailExists = async (email) => {
        try {
            const response = await api.get('/users', {
                params: { email }
            });
            return response.data.length > 0;
        } catch (err) {
            return false;
        }
    };

    // تحديث صورة البروفايل
    const updateProfileImage = async (userId, imageUrl) => {
        return await updateUser(userId, { profileImage: imageUrl });
    };

    // تفعيل حساب المستخدم
    const verifyUser = async (userId) => {
        return await updateUser(userId, { isVerified: true });
    };

    // مسح الأخطاء
    const clearError = () => {
        error.value = null;
    };

    return {
        // State
        users,
        isLoading,
        error,

        // Actions
        fetchUsers,
        fetchUserById,
        updateUser,
        deleteUser,
        searchUsers,
        checkEmailExists,
        updateProfileImage,
        verifyUser,
        clearError
    };
});