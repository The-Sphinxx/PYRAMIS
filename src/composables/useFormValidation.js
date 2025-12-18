// src/composables/useFormValidation.js

import { ref, computed } from 'vue';

export function useFormValidation() {
  const errors = ref({});
  const touched = ref({});
  
  const hasErrors = computed(() => {
    return Object.keys(errors.value).length > 0;
  });
  
  // Set error for a specific field
  const setError = (field, message) => {
    errors.value[field] = message;
  };
  
  // Clear error for a specific field
  const clearError = (field) => {
    delete errors.value[field];
  };
  
  // Clear all errors
  const clearErrors = () => {
    errors.value = {};
  };
  
  // Mark field as touched
  const touchField = (field) => {
    touched.value[field] = true;
  };
  
  // Check if field is touched
  const isFieldTouched = (field) => {
    return touched.value[field] || false;
  };
  
  // Get error for a specific field
  const getError = (field) => {
    return errors.value[field] || '';
  };
  
  // Check if field has error
  const hasError = (field) => {
    return !!errors.value[field];
  };
  
  // Validate single field
  const validateField = (field, value, validator) => {
    const result = validator(value);
    
    if (!result.valid) {
      setError(field, result.message);
    } else {
      clearError(field);
    }
    
    return result.valid;
  };
  
  // Validate multiple fields
  const validateFields = (fieldsData) => {
    let isValid = true;
    
    Object.keys(fieldsData).forEach(field => {
      const { value, validator } = fieldsData[field];
      const result = validator(value);
      
      if (!result.valid) {
        setError(field, result.message);
        isValid = false;
      } else {
        clearError(field);
      }
    });
    
    return isValid;
  };
  
  return {
    errors,
    touched,
    hasErrors,
    setError,
    clearError,
    clearErrors,
    touchField,
    isFieldTouched,
    getError,
    hasError,
    validateField,
    validateFields
  };
}