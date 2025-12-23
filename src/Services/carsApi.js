import api from './api';

export const getAllCars = async () => {
  try {
    const response = await api.get('/Cars');
    return response.data;
  } catch (error) {
    console.error("Error fetching cars:", error);
    return [];
  }
};
