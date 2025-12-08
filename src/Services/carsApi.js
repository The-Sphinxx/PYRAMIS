import axios from "axios";

const API_URL = "http://localhost:3001/cars";

export const getAllCars = async () => {
  try {
    const response = await axios.get(API_URL);
    return response.data;
  } catch (error) {
    console.error("Error fetching cars:", error);
    return [];
  }
};
