import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import wishlistApi from '@/Services/wishlistApi';

export const useWishlistStore = defineStore('wishlist', () => {
  const wishlistItems = ref([]);
  const isLoading = ref(false);

  // Create a set of wishlisted item keys for fast lookup
  const wishlistedKeys = computed(() => {
    const keys = new Set();
    wishlistItems.value.forEach(item => {
      const itemId = item.itemId ?? item.id;
      const itemType = item.itemType;
      keys.add(`${itemType}-${itemId}`);
    });
    return keys;
  });

  // Check if an item is wishlisted
  const isWishlisted = (itemType, itemId) => {
    // Convert string type to enum number if needed
    const typeMap = { 'Trip': 0, 'Hotel': 1, 'Car': 2, 'Attraction': 3 };
    const typeNum = typeof itemType === 'string' ? typeMap[itemType] : itemType;
    return wishlistedKeys.value.has(`${typeNum}-${itemId}`);
  };

  // Load user's wishlist
  const loadWishlist = async () => {
    isLoading.value = true;
    try {
      const data = await wishlistApi.getMine();
      wishlistItems.value = data;
    } catch (error) {
      console.error('Failed to load wishlist:', error);
      wishlistItems.value = [];
    } finally {
      isLoading.value = false;
    }
  };

  // Add item to wishlist
  const addToWishlist = async (payload) => {
    try {
      const result = await wishlistApi.add(payload);
      // Add to local store
      wishlistItems.value.push({
        id: result.id,
        itemId: payload.itemId,
        itemType: typeof payload.itemType === 'string' 
          ? { 'Trip': 0, 'Hotel': 1, 'Car': 2, 'Attraction': 3 }[payload.itemType]
          : payload.itemType,
        title: payload.title,
        imageUrl: payload.imageUrl,
        location: payload.location,
        rating: payload.rating
      });
      return result;
    } catch (error) {
      console.error('Failed to add to wishlist:', error);
      throw error;
    }
  };

  // Remove item from wishlist
  const removeFromWishlist = async (itemType, itemId) => {
    try {
      await wishlistApi.remove(itemType, itemId);
      // Remove from local store
      const typeMap = { 'Trip': 0, 'Hotel': 1, 'Car': 2, 'Attraction': 3 };
      const typeNum = typeof itemType === 'string' ? typeMap[itemType] : itemType;
      wishlistItems.value = wishlistItems.value.filter(
        item => !((item.itemId ?? item.id) === itemId && item.itemType === typeNum)
      );
    } catch (error) {
      console.error('Failed to remove from wishlist:', error);
      throw error;
    }
  };

  // Clear wishlist (for logout)
  const clearWishlist = () => {
    wishlistItems.value = [];
  };

  return {
    wishlistItems,
    isLoading,
    isWishlisted,
    loadWishlist,
    addToWishlist,
    removeFromWishlist,
    clearWishlist
  };
});
