<template>
  <div class="space-y-8 font-cairo bg-base-100 p-4 card shadow-xl">
    <!-- Guest Information Card -->
    <div class="card bg-base-100 shadow-xl border border-base-200">
      <div class="card-body p-8">
        <h2 class="text-2xl font-bold text-base-content/80 mb-6">Guest Information</h2>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- First Name -->
          <div class="form-control">
            <label class="label pl-0">
              <span class="label-text font-bold text-base-content/70">First Name</span>
            </label>
            <input
              v-model="localData.firstName"
              type="text"
              placeholder="Enter First name"
              class="input input-bordered w-full focus:input-primary bg-base-100"
              :class="{ 'input-error': errors.firstName }"
              @input="clearError('firstName')"
            />
            <label v-if="errors.firstName" class="label pl-0">
              <span class="label-text-alt text-error">{{ errors.firstName }}</span>
            </label>
          </div>

          <!-- Last Name -->
          <div class="form-control">
            <label class="label pl-0">
              <span class="label-text font-bold text-base-content/70">Last Name</span>
            </label>
            <input
              v-model="localData.lastName"
              type="text"
              placeholder="Enter Last name"
              class="input input-bordered w-full focus:input-primary bg-base-100"
              :class="{ 'input-error': errors.lastName }"
              @input="clearError('lastName')"
            />
            <label v-if="errors.lastName" class="label pl-0">
              <span class="label-text-alt text-error">{{ errors.lastName }}</span>
            </label>
          </div>

          <!-- Email -->
          <div class="form-control">
            <label class="label pl-0">
              <span class="label-text font-bold text-base-content/70 uppercase text-xs tracking-wide">EMAIL ADDRESS</span>
            </label>
            <input
              v-model="localData.email"
              type="email"
              placeholder="Your Email"
              class="input input-bordered w-full focus:input-primary bg-base-100"
              :class="{ 'input-error': errors.email }"
              @input="clearError('email')"
            />
            <label v-if="errors.email" class="label pl-0">
              <span class="label-text-alt text-error">{{ errors.email }}</span>
            </label>
          </div>

          <!-- Phone -->
          <div class="form-control">
            <label class="label pl-0">
              <span class="label-text font-bold text-base-content/70 uppercase text-xs tracking-wide">PHONE NUMBER</span>
            </label>
            <input
              v-model="localData.phone"
              type="tel"
              placeholder="Phone number"
              class="input input-bordered w-full focus:input-primary bg-base-100"
              :class="{ 'input-error': errors.phone }"
              @input="clearError('phone')"
            />
            <label v-if="errors.phone" class="label pl-0">
              <span class="label-text-alt text-error">{{ errors.phone }}</span>
            </label>
          </div>
        </div>

        <!-- Special Requests (Optional) -->
        <div class="form-control mt-6">
          <label class="label pl-0">
            <span class="label-text font-bold text-base-content/70 uppercase text-xs tracking-wide">SPECIAL REQUESTS (OPTIONAL)</span>
          </label>
          <textarea
            v-model="localData.specialRequests"
            placeholder="Any special requests or notes..."
            class="textarea textarea-bordered h-24 focus:textarea-primary bg-base-100"
          ></textarea>
        </div>
      </div>
    </div>

    <!-- Payment Method Card -->
    <div class="card bg-base-100 shadow-xl border border-base-200">
      <div class="card-body p-8">
        <h2 class="text-2xl font-bold text-base-content/80 mb-6">Payment method</h2>

        <!-- Payment Method Selection -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
          <label 
            class="flex items-center gap-4 p-4 border rounded-xl cursor-pointer transition-all h-20"
            :class="localData.paymentMethod === 'card' ? 'border-primary ring-1 ring-primary' : 'border-base-300 hover:border-base-content/30'"
          >
            <input
              v-model="localData.paymentMethod"
              type="radio"
              value="card"
              class="radio radio-primary"
            />
            <div class="flex items-center justify-between flex-1">
              <span class="font-medium text-lg text-base-content/80">Pay by Credit Card</span>
              <div class="flex gap-1">
                 <!-- Simple card icon representation -->
                 <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-base-content/50" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                   <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                 </svg>
              </div>
            </div>
          </label>

          <label 
            class="flex items-center gap-4 p-4 border rounded-xl cursor-pointer transition-all h-20"
            :class="localData.paymentMethod === 'arrival' ? 'border-primary ring-1 ring-primary' : 'border-base-300 hover:border-base-content/30'"
          >
            <input
              v-model="localData.paymentMethod"
              type="radio"
              value="arrival"
              class="radio radio-primary"
            />
            <span class="font-medium text-lg text-base-content/80">Pay On Arrival</span>
          </label>
        </div>

        <!-- Card Payment Details (Only show if card is selected) -->
        <div v-if="localData.paymentMethod === 'card'" class="space-y-6 animate-fade-in-down">
          <slot v-if="useStripeElements" name="payment-element" />

          <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Card Number -->
            <div class="form-control">
              <label class="label pl-0">
                <span class="label-text font-bold text-base-content/70 uppercase text-xs tracking-wide">CARD NUMBER</span>
              </label>
              <input
                v-model="localData.cardNumber"
                type="text"
                placeholder="1234 1234 1234"
                maxlength="19"
                class="input input-bordered w-full focus:input-primary bg-base-100"
                :class="{ 'input-error': errors.cardNumber }"
                @input="formatCardNumber"
              />
              <label v-if="errors.cardNumber" class="label pl-0">
                <span class="label-text-alt text-error">{{ errors.cardNumber }}</span>
              </label>
            </div>

            <!-- Name on Card -->
            <div class="form-control">
              <label class="label pl-0">
                <span class="label-text font-bold text-base-content/70 uppercase text-xs tracking-wide">NAME ON CARD</span>
              </label>
              <input
                v-model="localData.cardName"
                type="text"
                placeholder="Name on card"
                class="input input-bordered w-full focus:input-primary bg-base-100"
                :class="{ 'input-error': errors.cardName }"
                @input="clearError('cardName')"
              />
              <label v-if="errors.cardName" class="label pl-0">
                <span class="label-text-alt text-error">{{ errors.cardName }}</span>
              </label>
            </div>

            <!-- Expiration Date -->
            <div class="form-control">
              <label class="label pl-0">
                <span class="label-text font-bold text-base-content/70 uppercase text-xs tracking-wide">EXPIRATION DATE</span>
              </label>
              <input
                v-model="localData.expiryDate"
                type="text"
                placeholder="MM/YY"
                maxlength="5"
                class="input input-bordered w-full focus:input-primary bg-base-100"
                :class="{ 'input-error': errors.expiryDate }"
                @input="formatExpiryDate"
              />
              <label v-if="errors.expiryDate" class="label pl-0">
                <span class="label-text-alt text-error">{{ errors.expiryDate }}</span>
              </label>
            </div>

            <!-- CVC -->
            <div class="form-control">
              <label class="label pl-0">
                <span class="label-text font-bold text-base-content/70 uppercase text-xs tracking-wide">CVC</span>
              </label>
              <input
                v-model="localData.cvc"
                type="text"
                placeholder="CVC code"
                maxlength="4"
                class="input input-bordered w-full focus:input-primary bg-base-100"
                :class="{ 'input-error': errors.cvc }"
                @input="clearError('cvc')"
              />
              <label v-if="errors.cvc" class="label pl-0">
                <span class="label-text-alt text-error">{{ errors.cvc }}</span>
              </label>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- Place Order Button -->
    <button
      @click="handlePlaceOrder"
      :disabled="submitting"
      type="button"
      class="btn btn-primary btn-lg w-full mt-6 text-white font-bold"
      :class="{ 'loading': submitting }"
    >
      <span v-if="!submitting">Confirm Booking</span>
      <span v-else>Processing...</span>
    </button>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue';
import { useFormValidation } from '@/composables/useFormValidation';
import { useAuthStore } from '@/stores/authStore';
import { useToast } from '@/composables/useToast';
import {
  validateRequired,
  validateEmail,
  validatePhone,
  validateCreditCard,
  validateCVV,
  validateExpiryDate as validateExpiryDateUtil
} from '@/Utils/validators';

const props = defineProps({
  modelValue: {
    type: Object,
    default: () => ({
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      specialRequests: '',
      paymentMethod: 'card',
      cardNumber: '',
      cardName: '',
      expiryDate: '',
      cvc: ''
    })
  },
  submitting: {
    type: Boolean,
    default: false
  },
  useStripeElements: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['update:modelValue', 'validate', 'submit']);
const authStore = useAuthStore();
const { toast } = useToast();

const localData = ref({ ...props.modelValue });

// Pre-fill guest information on mount
onMounted(() => {
  // Try to get saved guest info from localStorage
  const savedGuestInfo = localStorage.getItem('guestInfo');
  
  if (savedGuestInfo) {
    try {
      const parsed = JSON.parse(savedGuestInfo);
      localData.value.firstName = parsed.firstName || localData.value.firstName;
      localData.value.lastName = parsed.lastName || localData.value.lastName;
      localData.value.email = parsed.email || localData.value.email;
      localData.value.phone = parsed.phone || localData.value.phone;
    } catch (e) {
      console.error('Failed to parse saved guest info', e);
    }
  }
  
  // If user is logged in, use their information (overrides localStorage)
  if (authStore.isAuthenticated && authStore.user) {
    localData.value.firstName = authStore.user.firstName || localData.value.firstName;
    localData.value.lastName = authStore.user.lastName || localData.value.lastName;
    localData.value.email = authStore.user.email || localData.value.email;
  }
  
  // Emit the pre-filled data
  emit('update:modelValue', { ...localData.value });
});

const { 
  errors, 
  validateFields, 
  clearError 
} = useFormValidation();

// Wrapper for expiry date validation to handle MM/YY string
const validateExpiryDateString = (value) => {
  if (!value) return { valid: false, message: 'Expiry date is required' };
  const [month, year] = value.split('/').map(v => v.trim());
  // Assuming strict MM/YY format, so year needs to be 20YY or just YY depending on validator.
  // The validator compares against current year (4 digits). 
  // Let's assume input is YY and convert to 20YY for the validator.
  if (!month || !year || year.length !== 2) return { valid: false, message: 'Invalid format (MM/YY)' };
  
  const fullYear = '20' + year;
  return validateExpiryDateUtil(month, fullYear);
};

// Watch for changes and emit
watch(localData, (newVal) => {
  emit('update:modelValue', { ...newVal });
  
  // Save guest info to localStorage (excluding payment details)
  const guestInfoToSave = {
    firstName: newVal.firstName,
    lastName: newVal.lastName,
    email: newVal.email,
    phone: newVal.phone
  };
  localStorage.setItem('guestInfo', JSON.stringify(guestInfoToSave));
}, { deep: true });

// Format card number (add spaces)
const formatCardNumber = () => {
  let value = localData.value.cardNumber.replace(/\s/g, '');
  value = value.replace(/(\d{4})/g, '$1 ').trim();
  localData.value.cardNumber = value;
  clearError('cardNumber');
};

// Format expiry date (add slash)
const formatExpiryDate = () => {
  let value = localData.value.expiryDate.replace(/\D/g, '');
  if (value.length >= 2) {
    value = value.slice(0, 2) + '/' + value.slice(2, 4);
  }
  localData.value.expiryDate = value;
  clearError('expiryDate');
};

// Validate form
const validate = () => {
  const fieldsToValidate = {
    firstName: { value: localData.value.firstName, validator: (v) => validateRequired(v, 'First name') },
    lastName: { value: localData.value.lastName, validator: (v) => validateRequired(v, 'Last name') },
    email: { value: localData.value.email, validator: validateEmail },
    phone: { value: localData.value.phone, validator: validatePhone },
  };

  if (localData.value.paymentMethod === 'card' && !props.useStripeElements) {
    fieldsToValidate.cardNumber = { value: localData.value.cardNumber, validator: validateCreditCard };
    fieldsToValidate.cardName = { value: localData.value.cardName, validator: (v) => validateRequired(v, 'Name on card') };
    fieldsToValidate.expiryDate = { value: localData.value.expiryDate, validator: validateExpiryDateString };
    fieldsToValidate.cvc = { value: localData.value.cvc, validator: validateCVV };
  }

  const isValid = validateFields(fieldsToValidate);
  emit('validate', isValid);
  return isValid;
};

const handlePlaceOrder = () => {
  if (validate()) {
    emit('submit');
  } else {
    // Show toast notification for validation errors
    const errorMessages = Object.values(errors.value);
    if (errorMessages.length > 0) {
      toast.error(errorMessages[0] || 'Please fill in all required fields correctly');
    }
  }
};

// Expose validate method to parent
defineExpose({ validate });
</script>