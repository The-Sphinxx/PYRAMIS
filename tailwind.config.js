import daisyui from "daisyui";

export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],

  theme: {
    extend: {
      fontFamily: {
        cairo: ['Cairo', 'sans-serif'],
      },
      // إضافة Spacing مخصص للـ Page Margins
      spacing: {
        'page-x': '120px',      // Desktop
        'page-x-lg': '80px',    // Large tablets
        'page-x-md': '60px',    // Medium tablets
        'page-x-sm': '24px',    // Mobile
        'page-x-xs': '16px',    // Small mobile
      },
      backdropBlur: {
        glass: '20px',
      },
      colors: {
        'glass-bg': 'rgba(255, 255, 255, 0.1)',
      },
      borderColor: {
        'glass-border': 'rgba(255, 255, 255, 0.3)',
      },
      boxShadow: {
        'glass-shadow': '0 4px 30px rgba(0, 0, 0, 0.1)',
      },
      // تعديل الـ borderRadius
      borderRadius: {
        'none': '0',
        'sm': '4px',           // للعناصر الصغيرة جداً
        'DEFAULT': '8px',      // القيمة الافتراضية - للـ cards
        'md': '6px',           // 6px - للـ buttons و inputs
        'lg': '8px',           // 8px - للـ cards
        'xl': '8px',           // 8px - للـ cards
        '2xl': '8px',          // 8px - للـ cards
        '3xl': '8px',          // 8px - للـ cards
        'full': '9999px',      // للدوائر والعناصر المستديرة
        'glass-radius': '8px', // للـ glass effect

        // Custom values للاستخدام المباشر
        '6': '6px',            // للـ buttons و inputs
        '8': '8px',            // للـ cards و containers
      },
    },
  },

  plugins: [
    daisyui,
    function ({ addUtilities, addComponents }) {
      addUtilities({
        '.glass': {
          'backdrop-filter': 'blur(20px)',
          '-webkit-backdrop-filter': 'blur(20px)',
          'background-color': 'rgba(255, 255, 255, 0.1)',
          'border': '1px solid rgba(255, 255, 255, 0.3)',
          'border-radius': '8px',
          'box-shadow': '0 4px 30px rgba(0, 0, 0, 0.1)',
        },
        // Page Container Utility
        '.page-container': {
          'width': '100%',
          'padding-left': '120px',
          'padding-right': '120px',
        },
      });

      // Override DaisyUI button and input border radius
      addComponents({
        '.btn': {
          'border-radius': '6px',
        },
        '.input': {
          'border-radius': '6px',
        },
        '.select': {
          'border-radius': '6px',
        },
        '.textarea': {
          'border-radius': '6px',
        },
      });
    },
    // Plugin للـ Responsive Variants
    function ({ addComponents, theme }) {
      addComponents({
        '.page-container': {
          width: '100%',
          paddingLeft: theme('spacing.page-x'),
          paddingRight: theme('spacing.page-x'),
          '@media (max-width: 1280px)': {
            paddingLeft: theme('spacing.page-x-lg'),
            paddingRight: theme('spacing.page-x-lg'),
          },
          '@media (max-width: 1024px)': {
            paddingLeft: theme('spacing.page-x-md'),
            paddingRight: theme('spacing.page-x-md'),
          },
          '@media (max-width: 768px)': {
            paddingLeft: theme('spacing.page-x-sm'),
            paddingRight: theme('spacing.page-x-sm'),
          },
          '@media (max-width: 640px)': {
            paddingLeft: theme('spacing.page-x-xs'),
            paddingRight: theme('spacing.page-x-xs'),
          },
        },
      });
    },
  ],

  daisyui: {
    themes: [
      {
        nileheritage: {
          "primary": "#C86A41",
          "primary-focus": "#B85F3A",
          "primary-content": "#ffffff",
          "secondary": "#2A6F7A",
          "secondary-focus": "#245F68",
          "secondary-content": "#ffffff",
          "accent": "#D5B77A",
          "accent-focus": "#C3A56C",
          "accent-content": "#3A2D17",
          "neutral": "#3F3F41",
          "neutral-content": "#ffffff",
          "base-100": "#F7F3EA",
          "base-200": "#EFE8DB",
          "base-300": "#E5DBC7",
          "base-content": "#2F2F2F",
          "info": "#3ABFF8",
          "success": "#36B37E",
          "warning": "#F2A540",
          "error": "#E45858",

          // DaisyUI Border Radius Override
          "--rounded-box": "8px",      // للـ cards
          "--rounded-btn": "6px",      // للـ buttons
          "--rounded-badge": "6px",    // للـ badges
        },
      },
      {
        nileheritageDark: {
          "primary": "#C86A41",
          "primary-focus": "#A55733",
          "primary-content": "#ffffff",
          "secondary": "#1E4F56",
          "secondary-focus": "#184046",
          "secondary-content": "#ffffff",
          "accent": "#D5B77A",
          "accent-focus": "#C3A56C",
          "accent-content": "#1E1A12",
          "neutral": "#2F2F30",
          "neutral-content": "#EDEDED",
          "base-100": "#1A1A1A",
          "base-200": "#222222",
          "base-300": "#2A2A2A",
          "base-content": "#EDEDED",
          "info": "#3ABFF8",
          "success": "#36B37E",
          "warning": "#F2A540",
          "error": "#E45858",

          // DaisyUI Border Radius Override
          "--rounded-box": "8px",      // للـ cards
          "--rounded-btn": "6px",      // للـ buttons
          "--rounded-badge": "6px",    // للـ badges
        },
      },
    ],
    darkTheme: "nileheritageDark",
  },
};