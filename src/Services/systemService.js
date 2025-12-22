import axios from 'axios';

const API_BASE = import.meta.env.VITE_API_BASE_URL || '/api';

export async function getBackgrounds(pageName) {
  const url = `${API_BASE}/System/backgrounds/${encodeURIComponent(pageName)}`;
  const { data } = await axios.get(url);
  // Normalize to array of URLs
  return Array.isArray(data)
    ? data.map((x) => ({
        url: x.imageUrl || x.ImageUrl || '',
        publicId: x.publicId || x.PublicId || '',
        group: x.group || x.Group || null,
        displayOrder: x.displayOrder || x.DisplayOrder || null,
      }))
    : [];
}
