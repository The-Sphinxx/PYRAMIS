import { defineStore } from 'pinia';
import bookingService from '../Services/bookingService';
import { calculateBookingCosts, validateBookingData, extractPrice } from '../Utils/bookingCalculator';

export const useBookingStore = defineStore('booking', {
    state: () => ({
        bookings: [],
        currentBooking: JSON.parse(localStorage.getItem('currentBooking')) || null,
        bookingInProgress: JSON.parse(localStorage.getItem('bookingInProgress')) || null,
        loading: false,
        error: null,
        validationErrors: []
    }),

    getters: {
        // Get bookings by status
        getBookingsByStatus: (state) => (status) => {
            return state.bookings.filter(booking => booking.status === status);
        },

        // Get bookings by type
        getBookingsByType: (state) => (type) => {
            return state.bookings.filter(booking => booking.type === type);
        },

        // Get pending bookings
        pendingBookings: (state) => {
            return state.bookings.filter(booking => booking.status === 'pending');
        },

        // Get confirmed bookings
        confirmedBookings: (state) => {
            return state.bookings.filter(booking => booking.status === 'confirmed');
        },

        // Get cancelled bookings
        cancelledBookings: (state) => {
            return state.bookings.filter(booking => booking.status === 'cancelled');
        },

        // Calculate costs for booking in progress
        bookingCosts: (state) => {
            if (!state.bookingInProgress) return null;

            return calculateBookingCosts(
                state.bookingInProgress.type,
                state.bookingInProgress.basePrice,
                state.bookingInProgress.bookingData
            );
        },

        // Check if booking data is valid
        isBookingValid: (state) => {
            if (!state.bookingInProgress) return false;

            const validation = validateBookingData(
                state.bookingInProgress.type,
                state.bookingInProgress.bookingData
            );

            return validation.isValid;
        }
    },

    actions: {
        /**
         * Initialize a new booking
         */
        initializeBooking(type, itemId, itemData) {
            this.bookingInProgress = {
                type,
                itemId,
                itemName: itemData.name || itemData.title,
                basePrice: extractPrice(itemData.price),
                itemData,
                bookingData: this.getDefaultBookingData(type)
            };
            this.validationErrors = [];
            this.persistState();
        },

        /**
         * Get default booking data structure based on type
         */
        getDefaultBookingData(type) {
            const baseData = {
                date: null
            };

            switch (type) {
                case 'attraction':
                    return {
                        ...baseData,
                        guests: 2
                    };

                case 'hotel':
                    return {
                        checkIn: null,
                        checkOut: null,
                        rooms: 1,
                        guests: 2,
                        nights: 0
                    };

                case 'car':
                    return {
                        pickupDate: null,
                        returnDate: null,
                        pickupTime: null,
                        passengers: 2,
                        days: 0
                    };

                case 'trip':
                    return {
                        ...baseData,
                        guests: { adults: 2, children: 0 }
                    };

                default:
                    return baseData;
            }
        },

        /**
         * Update booking data
         */
        updateBookingData(field, value) {
            if (!this.bookingInProgress) return;

            this.bookingInProgress.bookingData[field] = value;

            // Auto-calculate nights/days if date fields change
            if (this.bookingInProgress.type === 'hotel') {
                const { checkIn, checkOut } = this.bookingInProgress.bookingData;
                if (checkIn && checkOut) {
                    const nights = Math.ceil(
                        (new Date(checkOut) - new Date(checkIn)) / (1000 * 60 * 60 * 24)
                    );
                    this.bookingInProgress.bookingData.nights = Math.max(0, nights);
                }
            }

            if (this.bookingInProgress.type === 'car') {
                const { pickupDate, returnDate } = this.bookingInProgress.bookingData;
                if (pickupDate && returnDate) {
                    const days = Math.ceil(
                        (new Date(returnDate) - new Date(pickupDate)) / (1000 * 60 * 60 * 24)
                    );
                    this.bookingInProgress.bookingData.days = Math.max(0, days);
                }
            }
            this.persistState();
        },

        /**
         * Increment guest/passenger count
         */
        incrementCount(field) {
            if (!this.bookingInProgress) return;

            const currentValue = this.bookingInProgress.bookingData[field] || 0;
            this.bookingInProgress.bookingData[field] = currentValue + 1;
            this.persistState();
        },

        /**
         * Decrement guest/passenger count
         */
        decrementCount(field) {
            if (!this.bookingInProgress) return;

            const currentValue = this.bookingInProgress.bookingData[field] || 1;
            if (currentValue > 1) {
                this.bookingInProgress.bookingData[field] = currentValue - 1;
            }
            this.persistState();
        },

        /**
         * Validate current booking
         */
        validateCurrentBooking() {
            if (!this.bookingInProgress) {
                this.validationErrors = ['No booking in progress'];
                return false;
            }

            const validation = validateBookingData(
                this.bookingInProgress.type,
                this.bookingInProgress.bookingData
            );

            this.validationErrors = validation.errors;
            return validation.isValid;
        },

        /**
         * Submit booking
         */
        async submitBooking(userId = null, checkoutData = {}) {
            if (!this.validateCurrentBooking()) {
                throw new Error('Invalid booking data');
            }

            this.loading = true;
            this.error = null;

            try {
                const costs = this.bookingCosts;

                // Structure guest info from flat checkout data if needed
                const guestInfo = {
                    firstName: checkoutData.firstName,
                    lastName: checkoutData.lastName,
                    email: checkoutData.email,
                    phone: checkoutData.phone,
                    specialRequests: checkoutData.specialRequests
                };

                const bookingPayload = {
                    userId,
                    type: this.bookingInProgress.type,
                    itemId: this.bookingInProgress.itemId,
                    itemName: this.bookingInProgress.itemName,
                    itemData: this.bookingInProgress.itemData,
                    bookingData: this.bookingInProgress.bookingData,
                    pricing: costs,
                    status: 'pending',
                    guestInfo,
                    paymentInfo: {
                        method: checkoutData.paymentMethod,
                        cardLastFour: checkoutData.cardNumber ? checkoutData.cardNumber.replace(/\s/g, '').slice(-4) : null
                    },
                    paymentStatus: checkoutData.paymentMethod === 'card' ? 'paid' : 'pending' // Added for TripConfirmation logic
                };

                const result = await bookingService.createBooking(bookingPayload);

                this.bookings.push(result);
                this.currentBooking = result;
                this.persistState();

                return result;
            } catch (error) {
                this.error = error.message || 'Failed to submit booking';
                console.error('Error submitting booking:', error);
                throw error;
            } finally {
                this.loading = false;
            }
        },

        /**
         * Clear booking in progress
         */
        clearBookingInProgress() {
            this.bookingInProgress = null;
            this.validationErrors = [];
        },

        /**
         * Fetch all bookings
         */
        async fetchBookings() {
            this.loading = true;
            this.error = null;

            try {
                const data = await bookingService.getAllBookings();
                this.bookings = data;
            } catch (error) {
                this.error = error.message || 'Failed to fetch bookings';
                console.error('Error fetching bookings:', error);
            } finally {
                this.loading = false;
            }
        },

        /**
         * Fetch booking by ID
         */
        async fetchBookingById(id) {
            this.loading = true;
            this.error = null;

            try {
                const data = await bookingService.getBookingById(id);
                this.currentBooking = data;
                return data;
            } catch (error) {
                this.error = error.message || 'Failed to fetch booking';
                console.error('Error fetching booking:', error);
                throw error;
            } finally {
                this.loading = false;
            }
        },

        /**
         * Cancel booking
         */
        async cancelBooking(bookingId) {
            this.loading = true;
            this.error = null;

            try {
                const result = await bookingService.cancelBooking(bookingId);

                // Update local state
                const index = this.bookings.findIndex(b => b.id === bookingId);
                if (index !== -1) {
                    this.bookings[index] = result;
                }

                return result;
            } catch (error) {
                this.error = error.message || 'Failed to cancel booking';
                console.error('Error cancelling booking:', error);
                throw error;
            } finally {
                this.loading = false;
            }
        },

        /**
         * Confirm payment
         */
        async confirmPayment(bookingId, paymentData) {
            this.loading = true;
            this.error = null;

            try {
                const result = await bookingService.confirmPayment(bookingId, paymentData);

                // Update local state
                const index = this.bookings.findIndex(b => b.id === bookingId);
                if (index !== -1) {
                    this.bookings[index] = result;
                }

                return result;
            } catch (error) {
                this.error = error.message || 'Failed to confirm payment';
                console.error('Error confirming payment:', error);
                throw error;
            } finally {
                this.loading = false;
            }
        },

        /**
         * Enrich booking in progress with guest info and pricing for confirmation page
         */
        enrichBookingWithDetails(guestInfo, paymentInfo) {
            if (!this.bookingInProgress) return;

            const costs = this.bookingCosts;

            // Add booking ID
            if (!this.bookingInProgress.id) {
                this.bookingInProgress.id = `booking_${Date.now()}`;
            }

            // Add guest information
            this.bookingInProgress.guestInfo = {
                firstName: guestInfo.firstName || '',
                lastName: guestInfo.lastName || '',
                email: guestInfo.email || '',
                phone: guestInfo.phone || '',
                specialRequests: guestInfo.specialRequests || ''
            };

            // Add pricing breakdown
            this.bookingInProgress.pricing = {
                basePrice: costs?.basePrice || this.bookingInProgress.basePrice || 0,
                subtotal: costs?.subtotal || 0,
                serviceFee: costs?.serviceFee || 0,
                taxes: costs?.taxes || 0,
                total: costs?.total || 0
            };

            // Add payment information
            this.bookingInProgress.paymentInfo = {
                method: paymentInfo.method || 'card',
                cardLastFour: paymentInfo.cardLastFour || null
            };

            // Set payment status
            this.bookingInProgress.paymentStatus = paymentInfo.status || 'paid';

            this.persistState();
        },

        /**
         * Persist state to localStorage
         */
        persistState() {
            localStorage.setItem('bookingInProgress', JSON.stringify(this.bookingInProgress));
            localStorage.setItem('currentBooking', JSON.stringify(this.currentBooking));
        }
    }
});