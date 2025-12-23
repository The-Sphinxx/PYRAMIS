import { ref, watch } from 'vue';

// Initialize isDark from localStorage or system preference immediately
const getInitialTheme = () => {
    const savedTheme = localStorage.getItem('theme');
    
    if (savedTheme) {
        // Use saved preference
        return savedTheme === 'dark';
    } else {
        // Check system preference
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        // Save the detected system preference to localStorage
        localStorage.setItem('theme', prefersDark ? 'dark' : 'light');
        return prefersDark;
    }
};

export function useTheme() {
    const isDark = ref(getInitialTheme());

    // Apply theme to DOM synchronously using DaisyUI data-theme
    const applyTheme = () => {
        const html = document.documentElement;
        if (isDark.value) {
            html.setAttribute('data-theme', 'nileheritageDark');
            html.classList.add('dark');
        } else {
            html.setAttribute('data-theme', 'nileheritage');
            html.classList.remove('dark');
        }
    };

    // Initialization Logic
    const initializeTheme = () => {
        // Re-check localStorage in case it was changed externally
        const savedTheme = localStorage.getItem('theme');
        
        if (savedTheme) {
            isDark.value = savedTheme === 'dark';
        }
        
        applyTheme();
    };

    // Toggle Logic
    const toggleTheme = () => {
        isDark.value = !isDark.value;
        localStorage.setItem('theme', isDark.value ? 'dark' : 'light');
    };

    // Watch for isDark changes and apply theme
    watch(
        () => isDark.value,
        () => {
            applyTheme();
        }
    );

    return { isDark, toggleTheme, initializeTheme };
}