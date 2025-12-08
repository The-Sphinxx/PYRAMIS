<template>
  <div class="form-control w-full">
    <label v-if="label" class="label">
      <span class="label-text font-cairo">{{ label }}</span>
    </label>
    <div class="relative">
      <input
        type="text"
        :value="displayTime"
        @click="togglePicker"
        readonly
        :placeholder="placeholder"
        class="input input-bordered w-full font-cairo cursor-pointer"
        :class="inputClass"
      />
      
      <div
        v-if="showPicker"
        class="absolute z-50 mt-2 bg-white dark:bg-gray-800 rounded-xl shadow-[0_10px_40px_rgba(0,0,0,0.15)] dark:shadow-[0_10px_40px_rgba(0,0,0,0.5)] p-4 w-[240px]"
        :class="pickerPosition"
      >
        <div class="text-center mb-3">
          <p class="font-cairo font-bold text-base text-primary">Select Time</p>
        </div>

        <div class="flex items-center justify-center gap-3 mb-3">
          <!-- Hours -->
          <div class="flex flex-col items-center">
            <button
              @click="incrementHours"
              class="btn btn-ghost btn-xs btn-circle"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 15l7-7 7 7" />
              </svg>
            </button>
            <input
              v-model.number="hours"
              type="number"
              min="1"
              max="12"
              class="input input-bordered w-16 text-center text-xl font-cairo my-1"
              @blur="validateHours"
            />
            <button
              @click="decrementHours"
              class="btn btn-ghost btn-xs btn-circle"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
              </svg>
            </button>
            <span class="text-[10px] font-cairo mt-1 font-semibold text-base-content">Hours</span>
          </div>

          <div class="text-2xl font-bold">:</div>

          <!-- Minutes -->
          <div class="flex flex-col items-center">
            <button
              @click="incrementMinutes"
              class="btn btn-ghost btn-xs btn-circle"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 15l7-7 7 7" />
              </svg>
            </button>
            <input
              v-model.number="minutes"
              type="number"
              min="0"
              max="59"
              class="input input-bordered w-16 text-center text-xl font-cairo my-1"
              @blur="validateMinutes"
            />
            <button
              @click="decrementMinutes"
              class="btn btn-ghost btn-xs btn-circle"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
              </svg>
            </button>
            <span class="text-[10px] font-cairo mt-1 font-semibold text-base-content">Minutes</span>
          </div>

          <!-- AM/PM Toggle -->
          <div class="flex flex-col items-center">
            <button
              @click="togglePeriod"
              class="btn btn-primary btn-xs mb-2"
            >
              {{ period }}
            </button>
            <span class="text-[10px] font-cairo font-semibold text-base-content">Period</span>
          </div>
        </div>

        <div class="flex justify-end gap-2">
          <button @click="clearTime" class="btn btn-ghost btn-xs font-cairo">
            Clear
          </button>
          <button @click="confirmTime" class="btn btn-primary btn-xs font-cairo">
            Confirm
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted, onBeforeUnmount } from 'vue';
import dayjs from 'dayjs';

const props = defineProps({
  modelValue: {
    type: String,
    default: null
  },
  label: {
    type: String,
    default: ''
  },
  placeholder: {
    type: String,
    default: 'Select time'
  },
  format: {
    type: String,
    default: 'HH:mm'
  },
  is24Hour: {
    type: Boolean,
    default: false
  },
  inputClass: {
    type: String,
    default: ''
  },
  pickerPosition: {
    type: String,
    default: 'left-0'
  }
});

const emit = defineEmits(['update:modelValue']);

const showPicker = ref(false);
const hours = ref(12);
const minutes = ref(0);
const period = ref('AM');

const quickTimes = [
  { label: 'Now', hours: dayjs().hour(), minutes: dayjs().minute() },
  { label: '9:00 AM', hours: 9, minutes: 0 },
  { label: '12:00 PM', hours: 12, minutes: 0 },
  { label: '3:00 PM', hours: 15, minutes: 0 },
  { label: '6:00 PM', hours: 18, minutes: 0 }
];

const displayTime = computed(() => {
  if (!props.modelValue) return '';
  return dayjs(`2000-01-01 ${props.modelValue}`).format('hh:mm A');
});

const togglePicker = () => {
  showPicker.value = !showPicker.value;
};

const incrementHours = () => {
  hours.value = hours.value >= 12 ? 1 : hours.value + 1;
};

const decrementHours = () => {
  hours.value = hours.value <= 1 ? 12 : hours.value - 1;
};

const incrementMinutes = () => {
  minutes.value = minutes.value >= 59 ? 0 : minutes.value + 1;
};

const decrementMinutes = () => {
  minutes.value = minutes.value <= 0 ? 59 : minutes.value - 1;
};

const togglePeriod = () => {
  period.value = period.value === 'AM' ? 'PM' : 'AM';
};

const validateHours = () => {
  if (hours.value < 1) hours.value = 1;
  if (hours.value > 12) hours.value = 12;
};

const validateMinutes = () => {
  if (minutes.value < 0) minutes.value = 0;
  if (minutes.value > 59) minutes.value = 59;
};

const setQuickTime = (quick) => {
  if (quick.hours === 0) {
    hours.value = 12;
    period.value = 'AM';
  } else if (quick.hours === 12) {
    hours.value = 12;
    period.value = 'PM';
  } else if (quick.hours > 12) {
    hours.value = quick.hours - 12;
    period.value = 'PM';
  } else {
    hours.value = quick.hours;
    period.value = 'AM';
  }
  minutes.value = quick.minutes;
};

const getFormattedTime = () => {
  let h = hours.value;
  
  if (period.value === 'PM' && h !== 12) {
    h = h + 12;
  } else if (period.value === 'AM' && h === 12) {
    h = 0;
  }
  
  return `${String(h).padStart(2, '0')}:${String(minutes.value).padStart(2, '0')}`;
};

const confirmTime = () => {
  const timeString = getFormattedTime();
  emit('update:modelValue', timeString);
  showPicker.value = false;
};

const clearTime = () => {
  emit('update:modelValue', null);
  showPicker.value = false;
};

const handleClickOutside = (event) => {
  const picker = event.target.closest('.relative');
  if (!picker && showPicker.value) {
    showPicker.value = false;
  }
};

watch(() => props.modelValue, (newVal) => {
  if (newVal) {
    const time = dayjs(`2000-01-01 ${newVal}`);
    const h = time.hour();
    const m = time.minute();
    
    if (h === 0) {
      hours.value = 12;
      period.value = 'AM';
    } else if (h === 12) {
      hours.value = 12;
      period.value = 'PM';
    } else if (h > 12) {
      hours.value = h - 12;
      period.value = 'PM';
    } else {
      hours.value = h;
      period.value = 'AM';
    }
    
    minutes.value = m;
  }
}, { immediate: true });

onMounted(() => {
  document.addEventListener('click', handleClickOutside);
});

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside);
});
</script>