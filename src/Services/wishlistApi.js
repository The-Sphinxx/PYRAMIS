import api from './api';

const prefix = '/Wishlist';

// WishlistItemType enum mapping
const WishlistItemType = {
  Trip: 0,
  Hotel: 1,
  Car: 2,
  Attraction: 3
};

export const wishlistApi = {
  getMine: async () => {
    const { data } = await api.get(prefix);
    return data;
  },
  add: async (payload) => {
    // Convert to Pascal case and enum value for backend
    const requestPayload = {
      UserId: '', // Backend will override this from token
      ItemId: payload.itemId,
      ItemType: WishlistItemType[payload.itemType] ?? 0,
      Title: payload.title,
      ImageUrl: payload.imageUrl,
      Location: payload.location,
      Rating: payload.rating
    };
    const { data } = await api.post(prefix, requestPayload);
    return data;
  },
  remove: async (itemType, itemId) => {
    // itemType should be the string name (Trip, Hotel, Car, Attraction)
    await api.delete(`${prefix}/${itemType}/${itemId}`);
    return true;
  }
};

export default wishlistApi;
