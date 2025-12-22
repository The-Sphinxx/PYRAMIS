import axios from 'axios';

const API_BASE = import.meta.env.VITE_API_BASE_URL || '/api';

export async function getBackgrounds(pageName) {
  try {
    const url = `${API_BASE}/System/backgrounds/${encodeURIComponent(pageName)}`;
    console.log('Fetching backgrounds from:', url);
    const { data } = await axios.get(url);
    console.log('Raw API response:', data);
    // Normalize to array of URLs
    const result = Array.isArray(data)
      ? data.map((x) => ({
          url: x.imageUrl || x.ImageUrl || x.url || '',
          publicId: x.publicId || x.PublicId || '',
          group: x.group || x.Group || null,
          displayOrder: x.displayOrder || x.DisplayOrder || null,
        }))
      : [];
    console.log('Normalized backgrounds:', result);
    return result;
  } catch (error) {
    console.error('Failed to fetch backgrounds:', error);
    return [];
  }
}
