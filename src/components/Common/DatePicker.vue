<template>
  <div class="form-control w-full">
    <label v-if="label" class="label">
      <span class="label-text font-cairo">{{ label }}</span>
    </label>
    <div class="relative">
      <input
        type="text"
        :value="displayValue"
        @click="togglePicker"
        readonly
        :placeholder="placeholder"
        class="input input-bordered w-full font-cairo cursor-pointer"
        :class="inputClass"
      />
      <div
        v-if="showPicker"
        class="absolute z-50 mt-2 bg-white dark:bg-gray-800 rounded-xl shadow-[0_10px_40px_rgba(0,0,0,0.15)] dark:shadow-[0_10px_40px_rgba(0,0,0,0.5)] p-4 min-w-[320px]"
        :class="pickerPosition"
      >
        <div class="flex items-center justify-between mb-3">
          <button
            @click="previousMonth"
            class="btn btn-ghost btn-xs btn-circle"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <div class="text-center font-cairo font-semibold text-sm">
            <select v-model="currentMonth" class="select select-bordered select-xs mx-1">
              <option v-for="(month, idx) in months" :key="idx" :value="idx">
                {{ month }}
              </option>
            </select>
            <select v-model="currentYear" class="select select-bordered select-xs mx-1">
              <option v-for="year in years" :key="year" :value="year">
                {{ year }}
              </option>
            </select>
          </div>
          <button
            @click="nextMonth"
            class="btn btn-ghost btn-xs btn-circle"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
            </svg>
          </button>
        </div>

        <div class="grid grid-cols-7 gap-2 mb-2">
          <div
            v-for="day in weekDays"
            :key="day"
            class="text-center text-xs font-cairo font-bold text-primary uppercase py-1"
          >
            {{ day }}
          </div>
        </div>

        <div class="grid grid-cols-7 gap-2">
          <button
            v-for="(day, idx) in calendarDays"
            :key="idx"
            @click="selectDate(day)"
            :disabled="!day.isCurrentMonth"
            class="btn btn-sm font-cairo"
            :class="{
              'btn-primary': day.isSelected,
              'btn-ghost': day.isCurrentMonth && !day.isSelected,
              'bg-transparent text-base-content/40 hover:bg-transparent border-transparent': !day.isCurrentMonth,
              'ring-2 ring-accent': day.isToday && !day.isSelected
            }"
          >
            {{ day.day }}
          </button>
        </div>

        <div class="flex justify-end gap-2 mt-4">
          <button @click="clearDate" class="btn btn-ghost btn-sm font-cairo">
            Clear
          </button>
          <button @click="closePicker" class="btn btn-primary btn-sm font-cairo">
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
    type: [String, Date, null],
    default: null
  },
  label: {
    type: String,
    default: ''
  },
  placeholder: {
    type: String,
    default: 'Select date'
  },
  format: {
    type: String,
    default: 'YYYY-MM-DD'
  },
  displayFormat: {
    type: String,
    default: 'DD/MM/YYYY'
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
const currentMonth = ref(dayjs().month());
const currentYear = ref(dayjs().year());
const selectedDate = ref(null);

const months = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December'
];

const weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

const years = computed(() => {
  const currentY = dayjs().year();
  const yearsList = [];
  for (let i = currentY - 50; i <= currentY + 50; i++) {
    yearsList.push(i);
  }
  return yearsList;
});

const displayValue = computed(() => {
  if (!selectedDate.value) return '';
  return dayjs(selectedDate.value).format(props.displayFormat);
});

const calendarDays = computed(() => {
  const firstDay = dayjs().year(currentYear.value).month(currentMonth.value).startOf('month');
  const lastDay = firstDay.endOf('month');
  const startDate = firstDay.startOf('week');
  const endDate = lastDay.endOf('week');
  
  const days = [];
  let current = startDate;
  
  while (current.isBefore(endDate) || current.isSame(endDate, 'day')) {
    days.push({
      day: current.date(),
      date: current.toDate(),
      isCurrentMonth: current.month() === currentMonth.value,
      isToday: current.isSame(dayjs(), 'day'),
      isSelected: selectedDate.value && current.isSame(dayjs(selectedDate.value), 'day')
    });
    current = current.add(1, 'day');
  }
  
  return days;
});

const togglePicker = () => {
  showPicker.value = !showPicker.value;
};

const closePicker = () => {
  showPicker.value = false;
};

const selectDate = (day) => {
  if (!day.isCurrentMonth) return;
  
  selectedDate.value = day.date;
  emit('update:modelValue', dayjs(day.date).format(props.format));
};

const clearDate = () => {
  selectedDate.value = null;
  emit('update:modelValue', null);
  closePicker();
};

const previousMonth = () => {
  if (currentMonth.value === 0) {
    currentMonth.value = 11;
    currentYear.value--;
  } else {
    currentMonth.value--;
  }
};

const nextMonth = () => {
  if (currentMonth.value === 11) {
    currentMonth.value = 0;
    currentYear.value++;
  } else {
    currentMonth.value++;
  }
};

const handleClickOutside = (event) => {
  const picker = event.target.closest('.relative');
  if (!picker && showPicker.value) {
    closePicker();
  }
};

watch(() => props.modelValue, (newVal) => {
  if (newVal) {
    selectedDate.value = dayjs(newVal).toDate();
    currentMonth.value = dayjs(newVal).month();
    currentYear.value = dayjs(newVal).year();
  }
}, { immediate: true });

onMounted(() => {
  document.addEventListener('click', handleClickOutside);
});

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside);
});
</script>