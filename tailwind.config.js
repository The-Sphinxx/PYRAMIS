import daisyui from "daisyui";

export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],

  darkMode: 'class',

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
      // داخل قسم addUtilities في الـ plugins
addUtilities({
  '.glass-morphism': {
    'background': 'rgba(255, 255, 255, 0.05)', // خلفية بيضاء شفافة جداً
    'backdrop-filter': 'blur(12px) saturate(180%)', // التغبيش مع زيادة تشبع الألوان خلفه
    '-webkit-backdrop-filter': 'blur(12px) saturate(180%)',
    'border': '1px solid rgba(255, 255, 255, 0.125)', // حدود رفيعة تحاكي انعكاس الضوء
    'border-radius': '12px', // الصورة توضح حواف ناعمة (أكثر من 8px)
    'box-shadow': '0 8px 32px 0 rgba(0, 0, 0, 0.05)', // ظل خفيف جداً غير مشتت
  },
  '.glass-morphism-dark': {
    'background': 'rgba(15, 23, 42, 0.3)', // خلفية داكنة للـ Dark Mode
    'backdrop-filter': 'blur(12px) saturate(160%)',
    '-webkit-backdrop-filter': 'blur(12px) saturate(160%)',
    'border': '1px solid rgba(255, 255, 255, 0.08)',
  }
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
          marginLeft: 'auto',
          marginRight: 'auto',
          maxWidth: '1400px', // Prevent 4K stretching
          paddingLeft: theme('spacing.page-x-xs'), // Mobile default (16px)
          paddingRight: theme('spacing.page-x-xs'),

          // Mobile-first breakpoints (min-width)
          '@media (min-width: 640px)': { // sm
            paddingLeft: theme('spacing.page-x-sm'), // 24px
            paddingRight: theme('spacing.page-x-sm'),
          },
          '@media (min-width: 768px)': { // md
            paddingLeft: theme('spacing.page-x-md'), // 60px
            paddingRight: theme('spacing.page-x-md'),
          },
          '@media (min-width: 1024px)': { // lg
            paddingLeft: theme('spacing.page-x-lg'), // 80px
            paddingRight: theme('spacing.page-x-lg'),
          },
          '@media (min-width: 1280px)': { // xl
            paddingLeft: theme('spacing.page-x'), // 120px
            paddingRight: theme('spacing.page-x'),
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
          // اللون الأساسي (النحاسي) يبقى كما هو لتمييز الهوية
          "primary": "#C86A41",
          "primary-focus": "#E07A4D",
          "primary-content": "#ffffff",

          // الكحلي الداكن جداً للخلفية الأساسية
          "base-100": "#0F172A", // Deep Navy
          "base-200": "#1E293B", // Lighter Navy for cards
          "base-300": "#334155", // For borders/hovers
          "base-content": "#F1F5F9", // Off-white text

          // لون ثانوي يتماشى مع الكحلي (درجة Teal عميقة)
          "secondary": "#38BDF8", // Sky blue لمسة إشراق
          "secondary-focus": "#0EA5E9",
          "secondary-content": "#0F172A",

          // الألوان الأخرى
          "accent": "#FDE047", // Yellow gold للتباين
          "neutral": "#1E293B",
          "neutral-content": "#94A3B8",

          "info": "#0CA5E9",
          "success": "#2DD4BF",
          "warning": "#F59E0B",
          "error": "#FB7185",

          "--rounded-box": "8px",
          "--rounded-btn": "6px",
          "--rounded-badge": "6px",
        },
      },
    ],
    darkTheme: "nileheritageDark",
  },
};