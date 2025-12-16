// utils/bookingCalculator.js

/**
 * Extract numeric price from string (e.g., "472 $" -> 472)
 */
export const extractPrice = (priceString) => {
  if (typeof priceString === 'number') return priceString;
  if (!priceString) return 0;
  
  const numericPrice = priceString.toString().replace(/[^0-9.]/g, '');
  return parseFloat(numericPrice) || 0;
};

/**
 * Calculate booking costs based on type and data
 */
export const calculateBookingCosts = (bookingType, basePrice, bookingData) => {
  const price = extractPrice(basePrice);
  let subtotal = 0;
  let duration = 1;
  
  switch (bookingType) {
    case 'attraction':
      // Price per person
      subtotal = price * (bookingData.guests || 1);
      duration = 1;
      break;
      
    case 'hotel':
      // Price per night × nights × rooms
      duration = bookingData.nights || 1;
      subtotal = price * duration * (bookingData.rooms || 1);
      break;
      
    case 'car':
      // Price per day × days
      duration = bookingData.days || 1;
      subtotal = price * duration;
      break;
      
    case 'trip':
      // Price per person
      subtotal = price * (bookingData.travelers || 1);
      duration = 1;
      break;
      
    default:
      subtotal = price;
  }
  
  // Calculate fees
  const serviceFee = subtotal * 0.05; // 5% service fee
  const taxes = subtotal * 0.14; // 14% VAT
  const total = subtotal + serviceFee + taxes;
  
  return {
    basePrice: price,
    subtotal: parseFloat(subtotal.toFixed(2)),
    serviceFee: parseFloat(serviceFee.toFixed(2)),
    taxes: parseFloat(taxes.toFixed(2)),
    total: parseFloat(total.toFixed(2)),
    duration
  };
};

/**
 * Calculate number of nights between two dates
 */
export const calculateNights = (checkIn, checkOut) => {
  if (!checkIn || !checkOut) return 0;
  
  const start = new Date(checkIn);
  const end = new Date(checkOut);
  const diffTime = Math.abs(end - start);
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  
  return diffDays;
};

/**
 * Calculate number of days for car rental
 */
export const calculateDays = (pickupDate, returnDate) => {
  return calculateNights(pickupDate, returnDate);
};

/**
 * Format price with currency
 */
export const formatPrice = (price, currency = '$') => {
  if (typeof price !== 'number') return `0${currency}`;
  return `${price.toFixed(2)}${currency}`;
};

/**
 * Validate booking data based on type
 */
export const validateBookingData = (bookingType, bookingData) => {
  const errors = [];
  
  // Common validations
  if (!bookingData.date) {
    errors.push('Please select a date');
  }
  
  switch (bookingType) {
    case 'attraction':
      if (!bookingData.guests || bookingData.guests < 1) {
        errors.push('Please select at least 1 guest');
      }
      break;
      
    case 'hotel':
      if (!bookingData.checkIn) {
        errors.push('Please select check-in date');
      }
      if (!bookingData.checkOut) {
        errors.push('Please select check-out date');
      }
      if (bookingData.checkIn && bookingData.checkOut) {
        if (new Date(bookingData.checkOut) <= new Date(bookingData.checkIn)) {
          errors.push('Check-out date must be after check-in date');
        }
      }
      if (!bookingData.rooms || bookingData.rooms < 1) {
        errors.push('Please select at least 1 room');
      }
      if (!bookingData.guests || bookingData.guests < 1) {
        errors.push('Please select at least 1 guest');
      }
      break;
      
    case 'car':
      if (!bookingData.pickupDate) {
        errors.push('Please select pickup date');
      }
      if (!bookingData.returnDate) {
        errors.push('Please select return date');
      }
      if (!bookingData.pickupTime) {
        errors.push('Please select pickup time');
      }
      if (bookingData.pickupDate && bookingData.returnDate) {
        if (new Date(bookingData.returnDate) <= new Date(bookingData.pickupDate)) {
          errors.push('Return date must be after pickup date');
        }
      }
      if (!bookingData.passengers || bookingData.passengers < 1) {
        errors.push('Please select at least 1 passenger');
      }
      break;
      
    case 'trip':
      if (!bookingData.travelers || bookingData.travelers < 1) {
        errors.push('Please select at least 1 traveler');
      }
      break;
  }
  
  return {
    isValid: errors.length === 0,
    errors
  };
};