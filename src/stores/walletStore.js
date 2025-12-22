import { defineStore } from 'pinia';
import axios from 'axios';

export const useWalletStore = defineStore('wallet', {
    state: () => ({
        wallet: null,
        loading: false,
        error: null
    }),

    actions: {
        async fetchWallet(userId) {
            this.loading = true;
            this.error = null;
            try {
                const response = await axios.get(`http://localhost:3000/wallet?userId=${userId}`);
                if (response.data && response.data.length > 0) {
                     // Ensure secure match even if API returns others
                    const userWallet = response.data.find(w => w.userId == userId);
                    this.wallet = userWallet || null;
                } else {
                    this.wallet = null;
                }
            } catch (err) {
                this.error = 'Failed to load wallet';
                console.error(err);
            } finally {
                this.loading = false;
            }
        },

        async createWallet(userId) {
            this.loading = true;
            this.error = null;
            try {
                const newWallet = {
                    userId: userId,
                    balance: 0,
                    currency: 'EGP',
                    points: 0,
                    transactions: []
                };
                const response = await axios.post('http://localhost:3000/wallet', newWallet);
                this.wallet = response.data;
                return { success: true };
            } catch (err) {
                this.error = 'Failed to create wallet';
                return { success: false, error: err.message };
            } finally {
                this.loading = false;
            }
        },

        async addFunds(amount, description = 'Deposit') {
             if (!this.wallet) return { success: false, error: 'No wallet found' };

             this.loading = true;
             try {
                // Calculate new state
                const newBalance = this.wallet.balance + amount;
                const newTransaction = {
                    id: `txn_${Date.now()}`,
                    type: 'deposit',
                    date: new Date().toLocaleDateString(),
                    description: description,
                    amount: amount
                };

                const updatedWallet = {
                    ...this.wallet,
                    balance: newBalance,
                    transactions: [newTransaction, ...this.wallet.transactions]
                };

                // Patch Update
                const response = await axios.put(`http://localhost:3000/wallet/${this.wallet.id}`, updatedWallet);
                this.wallet = response.data;
                return { success: true };
             } catch (err) {
                return { success: false, error: 'Failed to add funds' };
             } finally {
                this.loading = false;
             }
        }
    }
});
