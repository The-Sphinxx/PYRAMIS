<template>
  <div class="space-y-4">
    <div class="flex justify-between items-center mb-4">
      <label class="label">
        <span class="label-text font-semibold text-base-content">Trip Itinerary</span>
      </label>
      <button 
        type="button"
        @click="addDay" 
        class="btn btn-primary btn-sm gap-2"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        Add Day
      </button>
    </div>

    <div v-if="days.length === 0" class="text-center py-8 text-base-content/60 border-2 border-dashed border-base-300 rounded-lg">
      <p>No days added yet. Click "Add Day" to start building your itinerary.</p>
    </div>

    <div v-for="(day, dayIndex) in days" :key="dayIndex" class="card bg-base-100 border border-base-300 shadow-sm">
      <div class="card-body p-4">
        <!-- Day Header -->
        <div class="flex justify-between items-start mb-3">
          <div class="flex-1 grid grid-cols-2 gap-3">
            <div class="form-control">
              <label class="label py-1">
                <span class="label-text text-xs">Day Number</span>
              </label>
              <input 
                type="number" 
                v-model.number="day.day" 
                class="input input-bordered input-sm"
                min="1"
                required
              />
            </div>
            <div class="form-control">
              <label class="label py-1">
                <span class="label-text text-xs">Day Title</span>
              </label>
              <input 
                type="text" 
                v-model="day.title" 
                class="input input-bordered input-sm"
                placeholder="e.g., Arrival & City Tour"
                required
              />
            </div>
          </div>
          <button 
            type="button"
            @click="removeDay(dayIndex)" 
            class="btn btn-ghost btn-sm btn-circle text-error ml-2"
            title="Remove Day"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Activities Section -->
        <div class="divider my-2">Activities</div>
        
        <div class="space-y-2">
          <div v-for="(activity, actIndex) in day.activities" :key="actIndex" class="bg-base-200/50 p-3 rounded-lg">
            <div class="flex gap-2 mb-2">
              <div class="form-control flex-1">
                <input 
                  type="text" 
                  v-model="activity.time" 
                  class="input input-bordered input-sm"
                  placeholder="Time (e.g., 09:00 AM)"
                />
              </div>
              <div class="form-control flex-[2]">
                <input 
                  type="text" 
                  v-model="activity.title" 
                  class="input input-bordered input-sm"
                  placeholder="Activity Title"
                />
              </div>
              <button 
                type="button"
                @click="removeActivity(dayIndex, actIndex)" 
                class="btn btn-ghost btn-sm btn-circle text-error"
                title="Remove Activity"
              >
                <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
              </button>
            </div>
            <textarea 
              v-model="activity.desc" 
              class="textarea textarea-bordered textarea-sm w-full"
              placeholder="Activity description..."
              rows="2"
            ></textarea>
          </div>
        </div>

        <button 
          type="button"
          @click="addActivity(dayIndex)" 
          class="btn btn-outline btn-sm gap-2 mt-2"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          Add Activity
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';

const props = defineProps({
  modelValue: {
    type: Array,
    default: () => []
  }
});

const emit = defineEmits(['update:modelValue']);

const days = ref([]);

// Initialize from modelValue
if (props.modelValue && props.modelValue.length > 0) {
  days.value = JSON.parse(JSON.stringify(props.modelValue));
} else {
  days.value = [];
}

// Watch for changes and emit
watch(days, (newDays) => {
  emit('update:modelValue', JSON.parse(JSON.stringify(newDays)));
}, { deep: true });

const addDay = () => {
  const newDayNumber = days.value.length > 0 
    ? Math.max(...days.value.map(d => d.day)) + 1 
    : 1;
  
  days.value.push({
    day: newDayNumber,
    title: '',
    activities: []
  });
};

const removeDay = (index) => {
  days.value.splice(index, 1);
};

const addActivity = (dayIndex) => {
  days.value[dayIndex].activities.push({
    time: '',
    title: '',
    desc: ''
  });
};

const removeActivity = (dayIndex, activityIndex) => {
  days.value[dayIndex].activities.splice(activityIndex, 1);
};
</script>
