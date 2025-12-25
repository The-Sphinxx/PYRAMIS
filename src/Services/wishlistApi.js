import api from './api';

const prefix = '/Wishlist';

export const wishlistApi = {
  getMine: async () => {
    const { data } = await api.get(prefix);
    return data;
  },
  add: async (payload) => {
    const { data } = await api.post(prefix, payload);
    return data;
  },
  remove: async (itemType, itemId) => {
    await api.delete(`${prefix}/${itemType}/${itemId}`);
    return true;
  }
};

export default wishlistApi;
