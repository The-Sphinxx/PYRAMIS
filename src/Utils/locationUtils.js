// utils/locationUtils.js
/**
 * حساب المسافة بين نقطتين على الأرض باستخدام Haversine formula
 * @param {number} lat1 - خط العرض للنقطة الأولى
 * @param {number} lon1 - خط الطول للنقطة الأولى
 * @param {number} lat2 - خط العرض للنقطة الثانية
 * @param {number} lon2 - خط الطول للنقطة الثانية
 * @returns {number} - المسافة بالكيلومترات
 */
export function calculateDistance(lat1, lon1, lat2, lon2) {
  const R = 6371; // نصف قطر الأرض بالكيلومترات
  const dLat = toRad(lat2 - lat1);
  const dLon = toRad(lon2 - lon1);
  
  const a =
    Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    Math.cos(toRad(lat1)) * Math.cos(toRad(lat2)) *
    Math.sin(dLon / 2) * Math.sin(dLon / 2);
  
  const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
  const distance = R * c;
  
  return Math.round(distance * 10) / 10; // تقريب لرقم عشري واحد
}

/**
 * تحويل الدرجات إلى راديان
 */
function toRad(degrees) {
  return degrees * (Math.PI / 180);
}

/**
 * تنسيق المسافة للعرض
 * @param {number} distance - المسافة بالكيلومترات
 * @returns {string} - المسافة منسقة (متر أو كيلومتر)
 */
export function formatDistance(distance) {
  if (distance < 1) {
    return `${Math.round(distance * 1000)} m`;
  }
  return `${distance} km`;
}

/**
 * إيجاد أقرب الأماكن من موقع معين
 * @param {object} currentLocation - الموقع الحالي {latitude, longitude}
 * @param {array} allLocations - كل المواقع المتاحة
 * @param {number} limit - عدد النتائج المطلوبة
 * @returns {array} - أقرب المواقع مع المسافة
 */
export function findNearbyLocations(currentLocation, allLocations, limit = 3) {
  const locationsWithDistance = allLocations
    .filter(loc => loc.id !== currentLocation.id) // استبعاد الموقع الحالي
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
    .sort((a, b) => a.distance - b.distance) // ترتيب حسب الأقرب
    .slice(0, limit); // أخذ أول 3 نتائج

  return locationsWithDistance;
}

/**
 * تحديد أيقونة الموقع حسب النوع
 * @param {string} type - نوع الموقع
 * @returns {string} - اسم الأيقونة
 */
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

/**
 * تحديد الفئات المناسبة للـ Nearby Landmarks
 * @param {string} currentType - نوع الموقع الحالي
 * @returns {array} - الفئات المناسبة
 */
export function getSuitableCategories(currentType) {
  const categoryMap = {
    attraction: ['MUSEUMS', 'HISTORICAL', 'CULTURE'],
    hotel: ['attraction', 'restaurant', 'shopping'],
    trip: ['attraction', 'hotel', 'restaurant'],
    car: ['attraction', 'hotel', 'parking']
  };
  
  return categoryMap[currentType] || [];
}