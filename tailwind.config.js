// tailwind.config.js

import daisyui from "daisyui"; 

/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {},
  },
  
  plugins: [daisyui], 

  daisyui: {
    themes: [
      "light",
      "dark",
      {
        papyrus: { 
          "primary": "#d4af37",
          "primary-focus": "#c19e35",
          "primary-content": "#ffffff",
          "secondary": "#bfa76f",
          "secondary-focus": "#ab9561",
          "secondary-content": "#ffffff",
          "accent": "#a1802d",
          "accent-content": "#ffffff",
          "neutral": "#3d4451",
          "neutral-content": "#ffffff",
          "base-100": "#f8f5f0",        
          "base-200": "#f0ece4",        
          "base-300": "#e8e0d4",
          "base-content": "#3a3a3a",
          "info": "#3abff8",
          "success": "#36d399",
          "warning": "#fbbd23",
          "error": "#f87272",
        },
      },
    ],
  },
}