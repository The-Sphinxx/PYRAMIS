// src/utils/validation.js

/**
 * Validation Rules and Helpers
 */

// Email validation
export const validateEmail = (email) => {
  if (!email) {
    return { valid: false, message: 'Email is required' };
  }
  
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  
  if (!emailRegex.test(email)) {
    return { valid: false, message: 'Please enter a valid email address' };
  }
  
  // Check for common typos
  const commonDomains = ['gmail.com', 'yahoo.com', 'hotmail.com', 'outlook.com'];
  const domain = email.split('@')[1];
  
  if (domain && domain.length < 4) {
    return { valid: false, message: 'Email domain seems too short' };
  }
  
  return { valid: true, message: '' };
};

// Password validation
export const validatePassword = (password) => {
  if (!password) {
    return { valid: false, message: 'Password is required' };
  }
  
  if (password.length < 8) {
    return { 
      valid: false, 
      message: 'Password must be at least 8 characters long' 
    };
  }
  
  if (password.length > 128) {
    return { 
      valid: false, 
      message: 'Password is too long (max 128 characters)' 
    };
  }
  
  // Check for at least one lowercase letter
  if (!/[a-z]/.test(password)) {
    return { 
      valid: false, 
      message: 'Password must contain at least one lowercase letter' 
    };
  }
  
  // Check for at least one uppercase letter
  if (!/[A-Z]/.test(password)) {
    return { 
      valid: false, 
      message: 'Password must contain at least one uppercase letter' 
    };
  }
  
  // Check for at least one number
  if (!/[0-9]/.test(password)) {
    return { 
      valid: false, 
      message: 'Password must contain at least one number' 
    };
  }
  
  // Check for at least one special character
  if (!/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) {
    return { 
      valid: false, 
      message: 'Password must contain at least one special character' 
    };
  }
  
  // Check for common passwords
  const commonPasswords = [
    'password', '12345678', 'qwerty', 'abc123', 
    'password123', '123456789', 'password1'
  ];
  
  if (commonPasswords.includes(password.toLowerCase())) {
    return { 
      valid: false, 
      message: 'This password is too common. Please choose a stronger one' 
    };
  }
  
  return { valid: true, message: '' };
};

// Password strength checker
export const getPasswordStrength = (password) => {
  let strength = 0;
  
  if (password.length >= 8) strength++;
  if (password.length >= 12) strength++;
  if (/[a-z]/.test(password)) strength++;
  if (/[A-Z]/.test(password)) strength++;
  if (/[0-9]/.test(password)) strength++;
  if (/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) strength++;
  
  if (strength <= 2) return { level: 'weak', color: 'error', percentage: 33 };
  if (strength <= 4) return { level: 'medium', color: 'warning', percentage: 66 };
  return { level: 'strong', color: 'success', percentage: 100 };
};

// Confirm password validation
export const validateConfirmPassword = (password, confirmPassword) => {
  if (!confirmPassword) {
    return { valid: false, message: 'Please confirm your password' };
  }
  
  if (password !== confirmPassword) {
    return { valid: false, message: 'Passwords do not match' };
  }
  
  return { valid: true, message: '' };
};

// Full name validation
export const validateFullName = (name) => {
  if (!name) {
    return { valid: false, message: 'Full name is required' };
  }
  
  if (name.trim().length < 2) {
    return { 
      valid: false, 
      message: 'Name must be at least 2 characters long' 
    };
  }
  
  if (name.trim().length > 50) {
    return { 
      valid: false, 
      message: 'Name is too long (max 50 characters)' 
    };
  }
  
  // Check if name contains at least first and last name
  const nameParts = name.trim().split(/\s+/);
  if (nameParts.length < 2) {
    return { 
      valid: false, 
      message: 'Please enter your first and last name' 
    };
  }
  
  // Check for invalid characters
  if (!/^[a-zA-Z\s'-]+$/.test(name)) {
    return { 
      valid: false, 
      message: 'Name can only contain letters, spaces, hyphens, and apostrophes' 
    };
  }
  
  return { valid: true, message: '' };
};

// Phone number validation (Egyptian format)
export const validatePhone = (phone) => {
  if (!phone) {
    return { valid: false, message: 'Phone number is required' };
  }
  
  // Remove spaces and dashes
  const cleanPhone = phone.replace(/[\s-]/g, '');
  
  // Egyptian phone format: 01xxxxxxxxx (11 digits starting with 01)
  const egyptianPhoneRegex = /^(01)[0-9]{9}$/;
  
  if (!egyptianPhoneRegex.test(cleanPhone)) {
    return { 
      valid: false, 
      message: 'Please enter a valid Egyptian phone number (01xxxxxxxxx)' 
    };
  }
  
  return { valid: true, message: '' };
};

// Date validation (for booking dates)
export const validateDate = (date, minDate = null, maxDate = null) => {
  if (!date) {
    return { valid: false, message: 'Date is required' };
  }
  
  const selectedDate = new Date(date);
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  
  if (selectedDate < today) {
    return { 
      valid: false, 
      message: 'Date cannot be in the past' 
    };
  }
  
  if (minDate) {
    const min = new Date(minDate);
    if (selectedDate < min) {
      return { 
        valid: false, 
        message: `Date must be after ${min.toLocaleDateString()}` 
      };
    }
  }
  
  if (maxDate) {
    const max = new Date(maxDate);
    if (selectedDate > max) {
      return { 
        valid: false, 
        message: `Date must be before ${max.toLocaleDateString()}` 
      };
    }
  }
  
  return { valid: true, message: '' };
};

// Number of guests validation
export const validateGuestCount = (count, min = 1, max = 50) => {
  if (!count) {
    return { valid: false, message: 'Number of guests is required' };
  }
  
  const numCount = Number(count);
  
  if (isNaN(numCount)) {
    return { valid: false, message: 'Please enter a valid number' };
  }
  
  if (numCount < min) {
    return { 
      valid: false, 
      message: `Minimum number of guests is ${min}` 
    };
  }
  
  if (numCount > max) {
    return { 
      valid: false, 
      message: `Maximum number of guests is ${max}` 
    };
  }
  
  return { valid: true, message: '' };
};

// Credit card validation (basic)
export const validateCreditCard = (cardNumber) => {
  if (!cardNumber) {
    return { valid: false, message: 'Card number is required' };
  }
  
  // Remove spaces and dashes
  const cleanCard = cardNumber.replace(/[\s-]/g, '');
  
  // Check if it's a valid number
  if (!/^\d+$/.test(cleanCard)) {
    return { 
      valid: false, 
      message: 'Card number must contain only digits' 
    };
  }
  
  // Check length (most cards are 13-19 digits)
  if (cleanCard.length < 13 || cleanCard.length > 19) {
    return { 
      valid: false, 
      message: 'Invalid card number length' 
    };
  }
  
  // Luhn algorithm
  let sum = 0;
  let isEven = false;
  
  for (let i = cleanCard.length - 1; i >= 0; i--) {
    let digit = parseInt(cleanCard.charAt(i), 10);
    
    if (isEven) {
      digit *= 2;
      if (digit > 9) {
        digit -= 9;
      }
    }
    
    sum += digit;
    isEven = !isEven;
  }
  
  if (sum % 10 !== 0) {
    return { valid: false, message: 'Invalid card number' };
  }
  
  return { valid: true, message: '' };
};

// CVV validation
export const validateCVV = (cvv) => {
  if (!cvv) {
    return { valid: false, message: 'CVV is required' };
  }
  
  if (!/^\d{3,4}$/.test(cvv)) {
    return { 
      valid: false, 
      message: 'CVV must be 3 or 4 digits' 
    };
  }
  
  return { valid: true, message: '' };
};

// Expiry date validation
export const validateExpiryDate = (month, year) => {
  if (!month || !year) {
    return { valid: false, message: 'Expiry date is required' };
  }
  
  const numMonth = Number(month);
  const numYear = Number(year);
  
  if (numMonth < 1 || numMonth > 12) {
    return { valid: false, message: 'Invalid month' };
  }
  
  const today = new Date();
  const currentYear = today.getFullYear();
  const currentMonth = today.getMonth() + 1;
  
  if (numYear < currentYear || (numYear === currentYear && numMonth < currentMonth)) {
    return { valid: false, message: 'Card has expired' };
  }
  
  return { valid: true, message: '' };
};

// Generic required field validation
export const validateRequired = (value, fieldName = 'This field') => {
  if (!value || (typeof value === 'string' && value.trim() === '')) {
    return { valid: false, message: `${fieldName} is required` };
  }
  
  return { valid: true, message: '' };
};

// URL validation
export const validateURL = (url) => {
  if (!url) {
    return { valid: false, message: 'URL is required' };
  }
  
  try {
    new URL(url);
    return { valid: true, message: '' };
  } catch {
    return { valid: false, message: 'Please enter a valid URL' };
  }
};

// Message/Comment validation
export const validateMessage = (message, minLength = 10, maxLength = 500) => {
  if (!message) {
    return { valid: false, message: 'Message is required' };
  }
  
  if (message.trim().length < minLength) {
    return { 
      valid: false, 
      message: `Message must be at least ${minLength} characters long` 
    };
  }
  
  if (message.trim().length > maxLength) {
    return { 
      valid: false, 
      message: `Message is too long (max ${maxLength} characters)` 
    };
  }
  
  return { valid: true, message: '' };
};

// Sanitize input (prevent XSS)
export const sanitizeInput = (input) => {
  if (typeof input !== 'string') return input;
  
  return input
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#x27;')
    .replace(/\//g, '&#x2F;');
};

// Validate entire form
export const validateForm = (formData, rules) => {
  const errors = {};
  let isValid = true;
  
  Object.keys(rules).forEach(field => {
    const rule = rules[field];
    const value = formData[field];
    
    if (rule.required) {
      const result = validateRequired(value, rule.label || field);
      if (!result.valid) {
        errors[field] = result.message;
        isValid = false;
        return;
      }
    }
    
    if (rule.validator && value) {
      const result = rule.validator(value, formData);
      if (!result.valid) {
        errors[field] = result.message;
        isValid = false;
      }
    }
  });
  
  return { isValid, errors };
};