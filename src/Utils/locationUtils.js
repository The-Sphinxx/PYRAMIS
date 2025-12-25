// utils/locationUtils.js
export function calculateDistance(lat1, lon1, lat2, lon2) {
  const R = 6371;
  const dLat = toRad(lat2 - lat1);
  const dLon = toRad(lon2 - lon1);
  
  const a =
    Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    Math.cos(toRad(lat1)) * Math.cos(toRad(lat2)) *
    Math.sin(dLon / 2) * Math.sin(dLon / 2);
  
  const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
  const distance = R * c;
  
  return Math.round(distance * 10) / 10;
}

function toRad(degrees) {
  return degrees * (Math.PI / 180);
}


export function formatDistance(distance) {
  if (distance < 1) {
    return `${Math.round(distance * 1000)} m`;
  }
  return `${distance} km`;
}


export function findNearbyLocations(currentLocation, allLocations, limit = 3) {
  const locationsWithDistance = allLocations
    .filter(loc => loc.id !== currentLocation.id)
    .map(location => {
      const distance = calculateDistance(
        currentLocation.latitude,
        currentLocation.longitude,
        location.latitude,
        location.longitude
      );
      
      return {
        ...location,
        distance,
        formattedDistance: formatDistance(distance)
      };
    })
    .sort((a, b) => a.distance - b.distance)
    .slice(0, limit); 

  return locationsWithDistance;
}

export function getLocationIcon(type) {
  const icons = {
    attraction: 'landmark',
    hotel: 'building',
    trip: 'navigation',
    car: 'car',
    restaurant: 'utensils',
    museum: 'building-columns'
  };
  
  return icons[type] || 'map-pin';
}

export function getSuitableCategories(currentType) {
  const categoryMap = {
    attraction: ['MUSEUMS', 'HISTORICAL', 'CULTURE'],
    hotel: ['attraction', 'restaurant', 'shopping'],
    trip: ['attraction', 'hotel', 'restaurant'],
    car: ['attraction', 'hotel', 'parking']
  };
  
  return categoryMap[currentType] || [];
}