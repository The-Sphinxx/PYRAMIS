<template>
  <div class="relative w-full h-screen overflow-hidden">

    <!-- Background Image -->
    <div
      class="absolute inset-0 w-full h-full bg-cover bg-right"
      :style="{ backgroundImage: `url(${heroImage})` }"
    >
      <!-- Gradient -->
      <div class="absolute inset-0 bg-gradient-to-b from-black/70 via-black/50 to-black/80"></div>
    </div>

    <!-- Content -->
    <div class="relative z-10 flex flex-col items-center justify-start w-full h-full pt-8 px-3 md:px-8">

      <!-- Hero Text -->
      <div class="text-center mb-6 md:mb-10 w-full max-w-4xl mt-20 md:mt-32">
        <h1
          class="text-3xl sm:text-4xl md:text-5xl lg:text-6xl font-bold leading-tight"
          style="line-height: 1;"
        >
          Find Your
          <span
            class="relative inline-block text-white"
            style="
              background:linear-gradient(to bottom, white 56%, transparent 85%);
              -webkit-background-clip:text;
              -webkit-text-fill-color:transparent;
            "
          >
            Perfect Ride
          </span>
        </h1>
<br> <br>
        <p class="text-sm sm:text-base md:text-lg text-gray-200 leading-relaxed mt-4">
          Discover premium vehicles for every journey. Luxury, comfort,<br />
          and reliability at your fingertips
        </p>
      </div>

      <!-- Search Fields -->
      <div class="flex flex-col lg:flex-row items-center gap-3 w-full max-w-7xl mt-6">

        <!-- Inputs Container -->
        <div class="flex flex-col sm:flex-row flex-1 border border-gray-300 rounded-2xl overflow-hidden bg-transparent w-full max-w-full">

          <!-- Pick-up Location -->
          <div class="flex-1 flex flex-col justify-center p-3 border-b sm:border-b-0 sm:border-r border-gray-300">
            <label class="text-xs text-gray-200 mb-1 text-center">Pick-up Location</label>
            <div class="flex items-center justify-center">
              <MapPinIcon class="w-5 h-5 text-gray-200" />
              <input
                type="text"
                v-model="location"
                class="w-full text-sm text-white bg-transparent border-none outline-none ml-2 text-center"
              />
            </div>
          </div>

          <!-- Pick-up Date -->
          <div class="flex-1 flex flex-col justify-center p-3 border-b sm:border-b-0 sm:border-r border-gray-300 relative">
            <label class="text-xs text-gray-200 mb-1 text-center">Pick-up Date</label>
            <div class="flex items-center justify-center relative">
              <CalendarIcon
                class="w-5 h-5 text-gray-200 absolute left-2 top-1/2 -translate-y-1/2 cursor-pointer"
                @click="openPickupCalendar"
              />
              <input
                ref="pickupInput"
                type="date"
                v-model="pickupDate"
                class="w-full text-sm text-white bg-transparent border-none outline-none pl-8 text-center"
              />
            </div>
          </div>

          <!-- Pick-up Time -->
          <div class="flex-1 flex flex-col justify-center p-3 border-b sm:border-b-0 sm:border-r border-gray-300">
            <label class="text-xs text-gray-200 mb-1 text-center">Time</label>
            <div class="flex items-center justify-center">
              <CalendarIcon class="w-5 h-5 text-gray-200" />
              <input
                type="text"
                placeholder="HH:MM AM/PM"
                class="w-full text-sm text-white bg-transparent border-none outline-none ml-2 text-center"
                v-model="timeField1"
                @input="handleTimeInput($event, timeField1)"
              />
            </div>
          </div>

          <!-- Drop-off Date -->
          <div class="flex-1 flex flex-col justify-center p-3 border-b sm:border-b-0 sm:border-r border-gray-300 relative">
            <label class="text-xs text-gray-200 mb-1 text-center">Drop-off Date</label>
            <div class="flex items-center justify-center relative">
              <CalendarIcon
                class="w-5 h-5 text-gray-200 absolute left-2 top-1/2 -translate-y-1/2 cursor-pointer"
                @click="openReturnCalendar"
              />
              <input
                ref="returnInput"
                type="date"
                v-model="returnDate"
                class="w-full text-sm text-white bg-transparent border-none outline-none pl-8 text-center"
              />
            </div>
          </div>

          <!-- Drop-off Time -->
          <div class="flex-1 flex flex-col justify-center p-3">
            <label class="text-xs text-gray-200 mb-1 text-center">Time</label>
            <div class="flex items-center justify-center">
              <CalendarIcon class="w-5 h-5 text-gray-200" />
              <input
                type="text"
                placeholder="HH:MM AM/PM"
                class="w-full text-sm text-white bg-transparent border-none outline-none ml-2 text-center"
                v-model="timeField2"
                @input="handleTimeInput($event, timeField2)"
              />
            </div>
          </div>

        </div>

        <!-- Search Button -->
        <button
          class="min-w-[140px] flex items-center justify-center gap-2 bg-gradient-to-r from-orange-500 to-orange-600 text-white px-6 py-3 rounded-2xl font-semibold text-base hover:from-orange-600 hover:to-orange-700 transition-all duration-200 shadow-xl hover:shadow-2xl transform hover:scale-105 whitespace-nowrap"
        >
          <MagnifyingGlassIcon class="w-6 h-6" />
          Search
        </button>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { CalendarIcon, MapPinIcon, MagnifyingGlassIcon } from "@heroicons/vue/24/outline";
import heroImage from "@/assets/image/CarHeroSection.jpg";

const location = ref("");
const pickupDate = ref("");
const returnDate = ref("");
const timeField1 = ref("");
const timeField2 = ref("");

const pickupInput = ref(null);
const returnInput = ref(null);

function openPickupCalendar() {
  pickupInput.value?.showPicker?.();
}

function openReturnCalendar() {
  returnInput.value?.showPicker?.();
}

function handleTimeInput(e, fieldRef) {
  let val = e.target.value.replace(/[^0-9]/g, "");
  if (val.length > 4) val = val.slice(0, 4);
  if (val.length >= 3) val = val.slice(0, 2) + ":" + val.slice(2);
  if (val.length === 5) val += " AM";
  fieldRef.value = val;
}
</script>

<style>
html,
body,
#app {
  overflow-x: hidden;
  background: black;
}
</style>
