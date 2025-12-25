import api from './api';

const prefix = '/Upload';

export const uploadApi = {
  uploadPhoto: async (file) => {
    const formData = new FormData();
    formData.append('file', file);
    
    // Increase timeout to 5 minutes for large file uploads
    const { data } = await api.post(`${prefix}/photo`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
      timeout: 300000  // 5 minutes
    });
    return data;
  },
  
  deletePhoto: async (publicId) => {
    await api.post(`${prefix}/delete`, { publicId });
    return true;
  }
};

export default uploadApi;
